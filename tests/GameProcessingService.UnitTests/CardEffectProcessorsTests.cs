using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Components.DictionaryAdapter;
using Common.Enums;
using EntityObjects;
using EntityObjects.Cards.Abstraction;
using EntityObjects.Cards.Colored;
using EntityObjects.Cards.Wild;
using FakeItEasy;
using GameProcessingService.CardEffectProcessors;
using GameProcessingService.CardEffectProcessors.Played;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;
using NUnit.Framework;

namespace GameProcessingService.UnitTests
{
    public class CardEffectProcessorsTests
    {
        private IGameManager _gameManager;
        private Game _game;
        private MoveParams _moveParams;
        private GameSetup _gameSetup;

        [SetUp]
        public void Setup()
        {
            _gameManager = new GameManager();
            _gameSetup=new GameSetup();
            _game = new Game(_gameSetup);
            _game.Direction = Direction.Left;
            _game.Deck = new Deck(_gameSetup);
            _game.DiscardedPile = new List<ICard>() { new Charity(), new BlackHole(), new Blackjack() };
            _game.LastCardPlayed = new LastCardPlayed(CardColor.Blue, CardValue.BlackHole, "", "", true);
            var player = new Player(new User("123", "john"));
            player.Cards = new List<ICard>() { new Charity(), new BlackHole(), new Blackjack() };
            _game.Players = new List<Player>() { player };
            _game.Deck = A.Fake<Deck>();
            _moveParams = new MoveParams(player, player.Cards.First(), player, CardColor.Blue, _game.DiscardedPile.First(), new List<int>() { 1, 2, 3 }, new List<ICard>() { new Charity() }, 10, new List<int>() { 0, 1, 2 }, new BlackHole(), "odd");

        }

        [Test]
        public void EnsureAllCardEffectProcessorsCanBeInstantiatedAndPlayed()
        {
            var cardEffectProcessorsTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(IPlayedCardEffectProcessor).IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface).ToList();

            var constructor = new object[] { _gameManager };
            foreach (var cardEffectProcessorType in cardEffectProcessorsTypes)
            {
                var instance = (IPlayedCardEffectProcessor)Activator.CreateInstance(cardEffectProcessorType, constructor, null);
                instance.ProcessCardEffect(_game, _moveParams);
            }
        }


        [Test]
        public void EnsureAllCardsAreHaveAccordingEffectProcessor()
        {
            List<Type> cardEffectProcessorsTypes = typeof(IPlayedCardEffectProcessor).Assembly.GetTypes().Where(p => typeof(IPlayedCardEffectProcessor).IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface).ToList();
            List<IPlayedCardEffectProcessor> allCardEffectProcessors = new List<IPlayedCardEffectProcessor>();

            var constructor = new object[] { _gameManager };
            foreach (var cardEffectProcessor in cardEffectProcessorsTypes)
            {
                allCardEffectProcessors.Add((IPlayedCardEffectProcessor)Activator.CreateInstance(cardEffectProcessor, constructor, null));
            }

            var allCards = Enum.GetValues(typeof(CardValue));
            foreach (CardValue card in allCards)
            {
                Assert.IsNotNull(allCardEffectProcessors.FirstOrDefault(x => x.CardAffected == card));
            }
        }
    }
}