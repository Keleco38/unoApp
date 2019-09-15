﻿using System.Collections.Generic;
using Common.Enums;

namespace DomainObjects
{
    public class TournamentSetupDto
    {
        public string Password { get; set; }
        public List<CardValue> BannedCards { get; set; }
        public int RoundsToWin { get; set; }
        public GameType GameType { get; set; }
        public bool DrawFourDrawTwoShouldSkipTurn { get; set; }
        public bool ReverseShouldSkipTurnInTwoPlayers { get; set; }
        public bool MatchingCardStealsTurn { get; set; }
        public bool WildCardCanBePlayedOnlyIfNoOtherOptions { get; set; }
        public int NumberOfPlayers { get; set; }
    }
}