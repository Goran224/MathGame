using Hangfire;
using MathGame_Api.Middleware;
using MathGame_Domain;
using MathGame_Service;
using MathGame_Service.Interfaces;
using MathGame_Service.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var dbConnection = builder.Configuration["ConnectionStrings:DatabaseConnectionString"];

builder.Services.AddHangfire(x => x.UseSqlServerStorage(dbConnection));
builder.Services.AddHangfireServer(); 
DIServiceFactory.RegisterServices(builder.Services);


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
         builder.WithOrigins("https://localhost:4200")
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials());
});

builder.Services.AddControllers();

builder.Services.AddSignalR()
    .AddJsonProtocol(options => {
        options.PayloadSerializerOptions.PropertyNamingPolicy = null;
        options.PayloadSerializerOptions.MaxDepth = 32;
        options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "MathGame API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
//
//Auth Config for JWT Token
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddCookie(x => x.Cookie.Name = builder.Configuration["Jwt:Name"])
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true
        };
        o.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies[builder.Configuration["Jwt:Name"]];
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddAuthorization();


//DB Connection
builder.Services.AddDbContext<MathGameDbContext>(options => options
                .UseSqlServer(dbConnection));

builder.Services.AddHttpContextAccessor();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = Convert.ToInt32(builder.Configuration["RateLimiter:PermitLimit"]);
        options.Window = TimeSpan.FromSeconds(Convert.ToInt32(builder.Configuration["RateLimiter:PermitedAttempsInTimeSpan"]));
    });

});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["ConnectionStrings:Redis"];
});

//Serilog Config
Log.Logger = new LoggerConfiguration()
            .WriteTo.File("logs/log.txt",
                          rollingInterval: RollingInterval.Day,
                          restrictedToMinimumLevel: LogEventLevel.Error)
                          .MinimumLevel.Error()
            .CreateLogger();

var app = builder.Build();

app.UseCors();
app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints => {
    endpoints.MapHub<GameHub>("/gamehub");
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapControllers();
app.MapHangfireDashboard("/dashboard");
app.UseRateLimiter();
app.Services.GetService<IHostApplicationLifetime>().ApplicationStarted.Register(() =>
{
    var hangfireJobService = app.Services.GetRequiredService<IHangfireJobService>();
    hangfireJobService.ScheduleGameExpressionGenerationJob();
});
app.Run();