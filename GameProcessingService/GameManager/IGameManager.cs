﻿using System.Collections.Generic;
using Common.Enums;
using EntityObjects;
using GameProcessingService.Models;

namespace GameProcessingService.GameManager
{
    public interface IGameManager
    {
        void StartNewGame(Game game);
        void DrawCard(Game game, Player player, int count, bool normalDraw);
        Player GetNextPlayer(Game game, Player player, List<Player> listOfPlayers);
        void UpdateGameAndRoundStatus(Game game, MoveResult moveResult);
    }
}