using AutoMapper;
using Uno.Models;
using Uno.Models.Dtos;

namespace Uno.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Player, PlayerDto>();

            CreateMap<Game, GameDto>()
                .ForMember(dest => dest.DeckSize, opt =>
                {
                    opt.MapFrom(src => src.Deck.Cards.Count);
                });

            CreateMap<GameSetup, GameSetupDto>().ForMember(dest => dest.IsPasswordProtected, opt =>
            {
                opt.MapFrom(src => src.Password.Length > 0);
            });
        }
    }
}