using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Enums;
using EntityObjects;
using EntityObjects.Cards.Abstraction;
using EntityObjects.Cards.Colored;
using EntityObjects.Cards.Wild;
using FakeItEasy;
using GameProcessingService.CardEffectProcessors;
using GameProcessingService.CoreManagers.GameManagers;
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
            _game.LastCardPlayed = new LastCardPlayed(CardColor.Blue,CardValue.BlackHole,"","",true);
            var player = new Player(new User("123", "john"));
            player.Cards = new List<ICard>() { new Charity(), new BlackHole(), new Blackjack() };
            _game.Players = new List<Player>() { player };
            _game.Deck = A.Fake<Deck>();
            _moveParams = new MoveParams(player, player.Cards.First(), player, CardColor.Blue, _game.DiscardedPile.First(), new List<int>() { 1, 2, 3 }, new List<ICard>() { new Charity() }, 10, new List<int>() { 0, 1, 2 }, new BlackHole(), "odd");

        }

        [Test]
        public void EnsureAllCardEffectProcessorsCanBeInstantiatedAndPlayed()
        {
            var cardEffectProcessors = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(ICardEffectProcessor).IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface).ToList();

            var constructor = new object[] { _gameManager };

            foreach (var cardEffectProcessor in cardEffectProcessors)
            {
                var instance = Activator.CreateInstance(cardEffectProcessor, constructor, null);
                MethodInfo method = cardEffectProcessor.GetMethod("ProcessCardEffect");
                method.Invoke(instance, new object[] { _game, _moveParams });
            }
        }


        [Test]
        public void EnsureAllCardsAreHaveAccordingEffectProcessor()
        {
            var cardEffectProcessors = typeof(ICardEffectProcessor).Assembly.GetTypes()
                .Where(p => typeof(ICardEffectProcessor).IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface).ToList();

            var allCards = typeof(Number).Assembly.GetTypes().Where(t => String.Equals(t.Namespace, typeof(Number).Namespace, StringComparison.Ordinal) || String.Equals(t.Namespace, typeof(BlackHole).Namespace, StringComparison.Ordinal)).ToList();

            foreach (var card in allCards)
            {
                var targetedCardEffectProcessor = card.Name.ToLower() + "effectprocessor";
                Assert.IsNotNull(cardEffectProcessors.FirstOrDefault(x => x.Name.ToLower().Equals(targetedCardEffectProcessor)));
            }

            allCards = typeof(ICard).Assembly.GetTypes()
                .Where(p => typeof(ICard).IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface).ToList();

            foreach (var card in allCards)
            {
                var targetedCardEffectProcessor = card.Name.ToLower() + "effectprocessor";
                Assert.IsNotNull(cardEffectProcessors.FirstOrDefault(x => x.Name.ToLower().Equals(targetedCardEffectProcessor)));
            }

        }
    }
}