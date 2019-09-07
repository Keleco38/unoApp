using System.Collections.Generic;
using EntityObjects;
using GameProcessingService.Models;

namespace GameProcessingService.CoreManagers
{
    public interface IGameManager
    {
        void UpdateGameAndRoundStatus(Game game);
        void StartNewGame(Game game);
        void DrawCard(Game game, Player player, int count, bool normalDraw);
        Player GetNextPlayer(Game game, Player player, List<Player> listOfPlayers);
    }
}