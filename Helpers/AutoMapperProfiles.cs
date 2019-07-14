using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Uno.Models;
using Uno.Models.Dtos;

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
            CreateMap<Card, CardDto>();

            CreateMap<CardDto, Card>()
                .ForCtorParam("cardColor", opt => opt.MapFrom(src => src.Color))
                .ForCtorParam("cardValue", opt => opt.MapFrom(src => src.Value));

            CreateMap<Game, GameDto>();

            CreateMap<GameSetup, GameSetupDto>()
                .ForMember(dest => dest.IsPasswordProtected, opt =>
                {
                    opt.MapFrom(src => src.IsPasswordProtected);
                });

            CreateMap<List<Card>, MyHandDto>()
                .ForMember(dest => dest.Cards, opt =>
                {
                    opt.MapFrom(src => src.OrderBy(y => y.Color).ThenBy(y => y.Value));
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