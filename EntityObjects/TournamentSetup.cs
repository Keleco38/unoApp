using System.Collections.Generic;
using Common.Enums;

namespace EntityObjects
{
    public class TournamentSetup
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public List<CardValue> BannedCards { get; set; }
        public int RoundsToWin { get; set; }
        public GameType GameType { get; set; }
        public bool DrawFourDrawTwoShouldSkipTurn { get; set; }
        public bool ReverseShouldSkipTurnInTwoPlayers { get; set; }
        public bool MatchingCardStealsTurn { get; set; }
        public bool WildCardCanBePlayedOnlyIfNoOtherOptions { get; set; }
        public int NumberOfPlayers { get; set; }
        public bool DrawAutoPlay { get; set; }
        public bool SpectatorsCanViewHands { get; set; }
    }
}