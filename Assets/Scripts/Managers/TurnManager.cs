using UnityEngine;
using TurnBasedStrategy.Core.Map;

namespace TurnBasedStrategy.Managers
{
    /// <summary>
    /// Manages turn rotation and game phases
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        public int numberOfPlayers = 2;
        public int currentPlayer = 0;
        public int turnNumber = 1;

        public static TurnManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void StartGame()
        {
            currentPlayer = 0;
            turnNumber = 1;
            BeginPlayerTurn();
        }

        private void BeginPlayerTurn()
        {
            Debug.Log($"Turn {turnNumber} - Player {currentPlayer}'s turn");

            // Reset units for this player
            UnitManager.Instance.ResetAllUnitsForNewTurn(currentPlayer);

            // Collect resources
            Core.Resources.ResourceManager.Instance.CollectResourcesForPlayer(currentPlayer);

            // Check victory condition
            if (CheckVictory(currentPlayer))
            {
                Debug.Log($"Player {currentPlayer} wins!");
                return;
            }
        }

        public void EndTurn()
        {
            // Clean up dead units
            UnitManager.Instance.CleanupDeadUnits();

            // Move to next player
            currentPlayer = (currentPlayer + 1) % numberOfPlayers;

            // If back to player 0, increment turn number
            if (currentPlayer == 0)
                turnNumber++;

            BeginPlayerTurn();
        }

        private bool CheckVictory(int playerId)
        {
            // Victory condition: Control 60% of the map
            int totalCells = MapManager.Instance.GetTotalCells();
            int controlledCells = 0;

            foreach (var cell in MapManager.Instance.GetAllCells())
            {
                if (cell.Owner == playerId)
                    controlledCells++;
            }

            float controlPercentage = (float)controlledCells / totalCells;
            return controlPercentage >= 0.6f;
        }

        public bool IsPlayerTurn(int playerId)
        {
            return currentPlayer == playerId;
        }

        public int GetCurrentPlayer()
        {
            return currentPlayer;
        }
    }
}
