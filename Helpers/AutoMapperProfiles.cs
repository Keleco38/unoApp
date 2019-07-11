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
            CreateMap<Card, CardDto>();
            CreateMap<Game, GameDto>();

            CreateMap<GameSetup, GameSetupDto>()
                .ForMember(dest => dest.IsPasswordProtected, opt =>
                {
                    opt.MapFrom(src => src.IsPasswordProtected);
                });

            CreateMap<List<Card>, MyHandDto>()
                .ForMember(dest => dest.Cards, opt =>
                {
                    opt.MapFrom(src => src);
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