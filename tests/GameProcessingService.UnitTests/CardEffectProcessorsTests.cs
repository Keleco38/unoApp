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
using GameProcessingService.CardEffectProcessors.AutomaticallyTriggered;
using GameProcessingService.CardEffectProcessors.Played;
using GameProcessingService.CoreManagers;
using GameProcessingService.Models;
using NUnit.Framework;
using Repository;

namespace GameProcessingService.UnitTests
{
    public class CardEffectProcessorsTests
    {
        private IGameManager _gameManager;
        private Game _game;
        private MoveParams _moveParams;
        private GameSetup _gameSetup;
        private IHallOfFameRepository _hallOfFameRepository;

        [SetUp]
        public void Setup()
        {
            _hallOfFameRepository = A.Fake<IHallOfFameRepository>();
            _gameManager = new GameManager(_hallOfFameRepository);
            _gameSetup = new GameSetup() { BannedCards = new List<CardValue>() };
            _game = new Game(_gameSetup);
            _game.Direction = Direction.Left;
            _game.Deck = new Deck(_gameSetup);
            _game.DiscardedPile = new List<ICard>() { new Charity(), new BlackHole(), new Blackjack() };
            _game.LastCardPlayed = new LastCardPlayed(CardColor.Blue, CardValue.Five, "", "", false);
            var player = new Player(new User("123", "john"));
            var player2 = new Player(new User("456", "andrew"));
            player.Cards = new List<ICard>() { new Charity(), new BlackHole(), new Blackjack(), new Charity(), new BlackHole(), new Blackjack() };
            player2.Cards = new List<ICard>() { new Charity(), new BlackHole(), new Blackjack(), new Charity(), new BlackHole(), new Blackjack() };
            _game.Players = new List<Player>() { player, player2 };
            _game.PlayerToPlay = player;
            _moveParams = new MoveParams(player, player.Cards.First(), player, CardColor.Blue, _game.DiscardedPile.First(), new List<int>() { 1, 2, 3 }, new List<ICard>() { new Charity() }, 10, new List<int>() { 0, 1 }, new BlackHole(), "odd");

        }

        [Test]
        public void EnsureAllCardEffectProcessorsCanBeInstantiatedAndPlayed()
        {

            List<Type> playedCardEffectProcessorsTypes = typeof(IPlayedCardEffectProcessor).Assembly.GetTypes().Where(p => typeof(IPlayedCardEffectProcessor).IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface).ToList();
            List<Type> automaticallyTriggeredEffectProcessorsTypes = typeof(IAutomaticallyTriggeredCardEffectProcessor).Assembly.GetTypes().Where(p => typeof(IAutomaticallyTriggeredCardEffectProcessor).IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface).ToList();

            List<IAutomaticallyTriggeredCardEffectProcessor> allAutomaticallyTriggeredCardEffectProcessors = new List<IAutomaticallyTriggeredCardEffectProcessor>();

            var constructorSimple = new object[] { _gameManager };

            foreach (var automaticallyPlayedCardEffectProcessor in automaticallyTriggeredEffectProcessorsTypes)
            {
                allAutomaticallyTriggeredCardEffectProcessors.Add((IAutomaticallyTriggeredCardEffectProcessor)Activator.CreateInstance(automaticallyPlayedCardEffectProcessor, constructorSimple, null));
            }

            var constructorComplex = new object[] { _gameManager, allAutomaticallyTriggeredCardEffectProcessors };


            foreach (var cardEffeccardEffectProcessorType in playedCardEffectProcessorsTypes)
            {

                var ctor = cardEffeccardEffectProcessorType.GetConstructors().First();
                var isComplex = ctor.GetParameters().Length > 1;

                var instance = (IPlayedCardEffectProcessor)Activator.CreateInstance(cardEffeccardEffectProcessorType, isComplex ? constructorComplex : constructorSimple, null);

                instance.ProcessCardEffect(_game, _moveParams);
                Setup();
            }



        }


        [Test]
        public void EnsureAllCardsAreHaveAccordingEffectProcessor()
        {
            List<Type> playedCardEffectProcessorsTypes = typeof(IPlayedCardEffectProcessor).Assembly.GetTypes().Where(p => typeof(IPlayedCardEffectProcessor).IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface).ToList();
            List<Type> automaticallyTriggeredEffectProcessorsTypes = typeof(IAutomaticallyTriggeredCardEffectProcessor).Assembly.GetTypes().Where(p => typeof(IAutomaticallyTriggeredCardEffectProcessor).IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface).ToList();

            List<IPlayedCardEffectProcessor> allPlayedCardEffectProcessors = new List<IPlayedCardEffectProcessor>();
            List<IAutomaticallyTriggeredCardEffectProcessor> allAutomaticallyTriggeredCardEffectProcessors = new List<IAutomaticallyTriggeredCardEffectProcessor>();

            var constructorSimple = new object[] { _gameManager };

            foreach (var automaticallyPlayedCardEffectProcessor in automaticallyTriggeredEffectProcessorsTypes)
            {
                allAutomaticallyTriggeredCardEffectProcessors.Add((IAutomaticallyTriggeredCardEffectProcessor)Activator.CreateInstance(automaticallyPlayedCardEffectProcessor, constructorSimple, null));
            }

            var constructorComplex = new object[] { _gameManager, allAutomaticallyTriggeredCardEffectProcessors };

            foreach (var cardEffectProcessor in playedCardEffectProcessorsTypes)
            {
                var ctor = cardEffectProcessor.GetConstructors().First();
                var isComplex = ctor.GetParameters().Length > 1;

                allPlayedCardEffectProcessors.Add((IPlayedCardEffectProcessor)Activator.CreateInstance(cardEffectProcessor, isComplex ? constructorComplex : constructorSimple, null));
            }

            var allCards = Enum.GetValues(typeof(CardValue));
            foreach (CardValue card in allCards)
            {
                Assert.IsNotNull(allPlayedCardEffectProcessors.FirstOrDefault(x => x.CardAffected == card));
            }
        }
    }
}