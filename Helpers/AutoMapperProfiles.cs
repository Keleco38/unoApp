using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Uno.Models;
using Uno.Models.Dtos;
using unoApp.Models.Abstraction;

namespace Uno.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<ChatMessage, ChatMessageDto>();
            CreateMap<User, UserDto>();
            CreateMap<Spectator, SpectatorDto>();
            CreateMap<LastCardPlayed, LastCardPlayedDto>();
            CreateMap<ICard, CardDto>();

            CreateMap<Game, GameDto>();

            CreateMap<GameSetup, GameSetupDto>()
                .ForMember(dest => dest.IsPasswordProtected, opt =>
                {
                    opt.MapFrom(src => src.IsPasswordProtected);
                });

       

            CreateMap<Player, PlayerDto>()
                .ForMember(dest => dest.NumberOfCards, opt =>
               {
                   opt.MapFrom(src => src.Cards.Count);
               });

            CreateMap<Deck, DeckDto>()
                .ForMember(dest => dest.DeckSize, opt =>
                {
                    opt.MapFrom(src => src.Cards.Count);
                });

        }
    }
}