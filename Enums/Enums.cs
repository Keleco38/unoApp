namespace Uno.Enums
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
        KeepMyHand= 31

    }

    public enum Direction
    {
        Right = 1,
        Left = 2
    }

    public enum TypeOfMessage
    {
        Chat = 1,
        Server = 2,
        Spectators = 3
    }
    public enum GameMode
    {
        Normal = 1,
        SpecialCards = 2,
        SpecialCardsAndAvalonCards = 3
    }

    public enum ChatMessageIntention
    {
        Normal = 1,
        Buzz = 2
    }

}