using System.Linq;
using AutoMapper;
using DomainObjects;
using EntityObjects;
using EntityObjects.Cards.Abstraction;

namespace Web.Helpers
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
            CreateMap<GameSetup, GameSetupDto>();

            CreateMap<Game, GameListDto>()
                .ForMember(dest => dest.GameStarted, opt =>
               {
                   opt.MapFrom(src => src.GameStarted);
               })
                .ForMember(dest => dest.Host, opt =>
                {
                    opt.MapFrom(src => src.Players.Any()? src.Players[0].User.Name:"No players.");
                })
                .ForMember(dest => dest.IsPasswordProtected, opt =>
               {
                   opt.MapFrom(src => src.GameSetup.IsPasswordProtected);
               })
                .ForMember(dest => dest.NumberOfPlayers, opt =>
               {
                   opt.MapFrom(src => src.Players.Count);
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