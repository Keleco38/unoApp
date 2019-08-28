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