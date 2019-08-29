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

        [SetUp]
        public void Setup()
        {
            _gameManager = new GameManager();
            _game = new Game();
            _game.Direction = Direction.Left;
            _game.Deck = new Deck(new List<CardValue>());
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
            var cardEffectProcessorsTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => typeof(ICardEffectProcessor).IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface).ToList();

            var constructor = new object[] { _gameManager };
            foreach (var cardEffectProcessorType in cardEffectProcessorsTypes)
            {
                var instance = (ICardEffectProcessor)Activator.CreateInstance(cardEffectProcessorType, constructor, null);
                instance.ProcessCardEffect(_game, _moveParams);
            }
        }


        [Test]
        public void EnsureAllCardsAreHaveAccordingEffectProcessor()
        {
            List<Type> cardEffectProcessorsTypes = typeof(ICardEffectProcessor).Assembly.GetTypes().Where(p => typeof(ICardEffectProcessor).IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface).ToList();
            List<ICardEffectProcessor> allCardEffectProcessors = new List<ICardEffectProcessor>();

            var constructor = new object[] { _gameManager };
            foreach (var cardEffectProcessor in cardEffectProcessorsTypes)
            {
                allCardEffectProcessors.Add((ICardEffectProcessor)Activator.CreateInstance(cardEffectProcessor, constructor, null));
            }

            var allCards = Enum.GetValues(typeof(CardValue));
            foreach (CardValue card in allCards)
            {
                Assert.IsNotNull(allCardEffectProcessors.FirstOrDefault(x => x.CardAffected == card));
            }
        }
    }
}