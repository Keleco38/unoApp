using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using EntityObjects.Cards.Abstraction;

namespace GameProcessingService.Models
{
    public class AutomaticallyTriggeredParams
    {
        public AutomaticallyTriggeredDeflectParams DeflectParams { get; set; }
        public AutomaticallyTriggeredDoubleDrawParams DoubleDrawParams { get; set; }
        public AutomaticallyTriggeredKeepMyHandParams KeepMyHandParams { get; set; }
        public AutomaticallyTriggeredMagneticPolarityParams MagneticPolarityParams { get; set; }
        public AutomaticallyTriggeredPromiseKeeperParams PromiseKeeperParams { get; set; }
        public AutomaticallyTriggeredTheLastStandParams TheLastStandParams { get; set; }
    }

    public class AutomaticallyTriggeredDoubleDrawParams
    {
        public AutomaticallyTriggeredDoubleDrawParams(Player playerAffected, int numberOfCardsToDraw, CardColor targetedCardColor)
        {
            PlayerAffected = playerAffected;
            NumberOfCardsToDraw = numberOfCardsToDraw;
            TargetedCardColor = targetedCardColor;
        }

        public Player PlayerAffected { get; set; }
        public int NumberOfCardsToDraw { get; set; }
        public CardColor TargetedCardColor { get; set; }
    }

    public class AutomaticallyTriggeredDeflectParams
    {
        public AutomaticallyTriggeredDeflectParams(Player playerPlayed, Player playerTargeted, int numberOfCardsToDraw, ICard cardPlayed, CardColor targetedCardColor)
        {
            PlayerPlayed = playerPlayed;
            PlayerTargeted = playerTargeted;
            NumberOfCardsToDraw = numberOfCardsToDraw;
            CardPlayed = cardPlayed;
            TargetedCardColor = targetedCardColor;
        }

        public Player PlayerPlayed { get; set; }
        public Player PlayerTargeted { get; set; }
        public int NumberOfCardsToDraw { get; set; }
        public ICard CardPlayed { get; set; }
        public CardColor TargetedCardColor { get; set; }
    }

    public class AutomaticallyTriggeredKeepMyHandParams
    {
        public AutomaticallyTriggeredKeepMyHandParams(List<Player> playersAffected, CardColor targetedCardColor)
        {
            PlayersAffected = playersAffected;
            TargetedCardColor = targetedCardColor;
        }

        public List<Player> PlayersAffected { get; set; }
        public CardColor TargetedCardColor { get; set; }
    }

    public class AutomaticallyTriggeredMagneticPolarityParams
    {
        public AutomaticallyTriggeredMagneticPolarityParams(CardColor targetedCardColor, Player playerPlayed, Player playerTargeted)
        {
            TargetedCardColor = targetedCardColor;
            PlayerPlayed = playerPlayed;
            PlayerTargeted = playerTargeted;
        }

        public CardColor TargetedCardColor { get; set; }
        public Player PlayerPlayed { get; set; }
        public Player PlayerTargeted { get; set; }
    }

    public class AutomaticallyTriggeredPromiseKeeperParams
    {
        public AutomaticallyTriggeredPromiseKeeperParams(Player playerPlayed, ICard cardPlayed)
        {
            PlayerPlayed = playerPlayed;
            CardPlayed = cardPlayed;
        }

        public Player PlayerPlayed { get; set; }
        public ICard CardPlayed { get; set; }
    }

    public class AutomaticallyTriggeredTheLastStandParams
    {
    }


}