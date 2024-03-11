using MathGame_Domain.EntityModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace MathGame_Domain
{
    public class MathGameDbContext : DbContext
    {
        public MathGameDbContext(DbContextOptions<MathGameDbContext> options) : base(options) { }
        public DbSet<Player> Players { get; set; }
        public DbSet<LoggedPlayerInfo> LoggedPlayersInfo { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }
        public DbSet<GameExpression> GameExpressions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            ConfigureUser(modelBuilder);
            ConfigureLoggedPlayersInfo(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }


        private static void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                        .HasKey(u => u.Id);

            modelBuilder.Entity<Player>()
                .Property(u => u.FirstName)
                .IsRequired();

            modelBuilder.Entity<Player>()
                .Property(u => u.LastName)
                .IsRequired();

            modelBuilder.Entity<Player>()
                .Property(u => u.Email)
                .IsRequired();

            modelBuilder.Entity<Player>()
                .Property(u => u.Password)
                .IsRequired();

            modelBuilder.Entity<Player>()
          .HasOne(p => p.GameSession)
          .WithMany(gs => gs.Players)
          .HasForeignKey(p => p.GameSessionId)
          .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameExpression>()
                .HasOne(e => e.GameSession)
                .WithMany(gs => gs.GameExpressions)
                .HasForeignKey(e => e.GameSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        }
        private static void ConfigureLoggedPlayersInfo(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoggedPlayerInfo>()
                        .HasKey(u => u.Id);

            modelBuilder.Entity<LoggedPlayerInfo>()
                    .Property(u => u.LastLogin)
                    .IsRequired(false);

        }
    }
}