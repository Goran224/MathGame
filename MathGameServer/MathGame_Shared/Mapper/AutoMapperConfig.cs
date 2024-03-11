using AutoMapper;
using MathGame_Domain.DtoModels;
using MathGame_Domain.EntityModels;
using System.Numerics;

namespace MathGame_Shared.Mapper
{
    public class AutoMapperConfig
    {
        public static IMapper Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {

                cfg.CreateMap<Player, PlayerDto>()
                .ForMember(dest => dest.ConfirmPassword, opt => opt.Ignore());

                cfg.CreateMap<PlayerDto, Player>();

            });

            return config.CreateMapper();
        }
    }
}
