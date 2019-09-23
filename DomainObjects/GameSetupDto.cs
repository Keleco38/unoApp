using System.Collections.Generic;
using Common.Enums;

namespace DomainObjects
{
    public class GameSetupDto
    {
        public string Password { get; set; }
        public List<CardValue> BannedCards { get; set; }
        public int RoundsToWin { get; set; }
        public GameType GameType { get; set; }
        public PlayersSetup PlayersSetup { get; set; }
        public bool DrawFourDrawTwoShouldSkipTurn { get; set; }
        public bool ReverseShouldSkipTurnInTwoPlayers { get; set; }
        public bool MatchingCardStealsTurn { get; set; }
        public bool WildCardCanBePlayedOnlyIfNoOtherOptions { get; set; }
        public int MaxNumberOfPlayers { get; set; }
        public bool CanSeeTeammatesHandInTeamGame { get; set; }
    }
}