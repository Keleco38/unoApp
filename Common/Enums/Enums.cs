namespace Common.Enums
{
    public enum CardColor
    {
        Blue = 1,
        Green = 2,
        Red = 3,
        Yellow = 4,
        Wild = 5
    }

    public enum CardValue
    {
        Zero = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Reverse = 10,
        Skip = 11,
        DrawTwo = 12,
        DrawFour = 13,
        ChangeColor = 14,
        BlackHole = 15,
        DiscardWildCards = 16,
        StealTurn = 17,
        SwapHands = 18,
        DoubleEdge = 19,
        DiscardColor = 20,
        HandOfGod = 21,
        Judgement = 22,
        UnitedWeFall = 23,
        ParadigmShift = 24,
        Deflect = 25,
        InspectHand = 26,
        GraveDigger = 27,
        RussianRoulette = 28,
        Roulette = 29,
        Duel = 30,
        KeepMyHand = 31,
        Charity = 32,
        Blackjack = 33,
        FairPlay = 34,
        TricksOfTheTrade = 35,
        DiscardNumber = 36,
        TheLastStand = 37,
        MagneticPolarity = 38,
        FortuneTeller = 39,
        DoubleDraw = 40,
        Poison = 41,
        RandomColor = 42,
        PromiseKeeper = 43,
        Gambling = 44,
        CopyCat = 45,
        RobinHood= 46,
        Handcuff=47,
        Silence=48,
        Assassinate=49,
        SummonWildcard=50,
        DevilsDeal=51,
        KingsDecree=52,
        QueensDecree=53,
        Surprise=54

    }

    public enum Direction
    {
        Right = 1,
        Left = 2
    }
    public enum GameType
    {
        Normal = 1,
        SpecialWildCards = 2
    }
    public enum PlayersSetup
    {
        Individual = 1,
        Teams = 2
    }

    public enum TypeOfMessage
    {
        Chat = 1,
        Server = 2,
        Spectators = 3
    }
    public enum ChatDestination
    {
        All = 1,
        Tournament = 2,
        Game = 3
    }

    public enum ChatMessageIntention
    {
        Normal = 1,
        Buzz = 2
    }

}