using UnityEngine;
using TurnBasedStrategy.Core.Map;
using TurnBasedStrategy.Core.Resources;
using TurnBasedStrategy.Core.Units;
using TurnBasedStrategy.AI;

namespace TurnBasedStrategy.Managers
{
    /// <summary>
    /// Main Game Manager - Controls game initialization and flow
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        public bool playerIsHuman = false; // false = AI vs AI
        public int numberOfPlayers = 2;

        [Header("Initial Setup")]
        public int startingUnitsPerPlayer = 2;

        private AIController[] aiControllers;

        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            Debug.Log("Initializing Turn-Based Strategy Game...");

            // Generate map
            MapManager.Instance.GenerateMap();

            // Initialize resources for each player
            for (int i = 0; i < numberOfPlayers; i++)
            {
                ResourceManager.Instance.InitializePlayer(i, 150, 150, 150);
            }

            // Spawn initial units for each player
            SpawnInitialUnits();

            // Create production buildings for each player
            CreateInitialStructures();

            // Initialize AI controllers
            aiControllers = new AIController[numberOfPlayers];
            for (int i = 0; i < numberOfPlayers; i++)
            {
                if (i == 0 && playerIsHuman)
                    continue; // Player 0 is human

                GameObject aiObj = new GameObject($"AI_Player_{i}");
                aiObj.transform.SetParent(transform);
                AIController ai = aiObj.AddComponent<AIController>();
                ai.Initialize(i);
                aiControllers[i] = ai;
            }

            // Start the game
            TurnManager.Instance.StartGame();
        }

        private void SpawnInitialUnits()
        {
            // Get spawn positions for each player (opposite sides of map)
            HexCoordinates[] spawnPositions = GetPlayerSpawnPositions();

            for (int playerId = 0; playerId < numberOfPlayers; playerId++)
            {
                HexCoordinates spawnCenter = spawnPositions[playerId];

                // Spawn starting units around spawn position
                int unitsSpawned = 0;
                foreach (var neighbor in HexCoordinates.GetNeighbors(spawnCenter))
                {
                    if (unitsSpawned >= startingUnitsPerPlayer)
                        break;

                    Cell cell = MapManager.Instance.GetCell(neighbor);
                    if (cell != null && !cell.IsOccupied)
                    {
                        UnitType unitType = unitsSpawned == 0 ? UnitType.Infantry : UnitType.Cavalry;
                        UnitManager.Instance.SpawnUnit(unitType, playerId, neighbor);
                        cell.Owner = playerId;
                        unitsSpawned++;
                    }
                }
            }
        }

        private void CreateInitialStructures()
        {
            HexCoordinates[] spawnPositions = GetPlayerSpawnPositions();

            for (int playerId = 0; playerId < numberOfPlayers; playerId++)
            {
                HexCoordinates spawnCenter = spawnPositions[playerId];
                Cell centerCell = MapManager.Instance.GetCell(spawnCenter);

                if (centerCell != null)
                {
                    centerCell.Structure = new Structure(StructureType.Barracks, playerId);
                    centerCell.Owner = playerId;
                }
            }
        }

        private HexCoordinates[] GetPlayerSpawnPositions()
        {
            // Simple: place players at opposite edges
            int radius = MapManager.Instance.mapRadius;

            return new HexCoordinates[]
            {
                new HexCoordinates(-radius + 2, 0),  // Player 0: Left side
                new HexCoordinates(radius - 2, 0)    // Player 1: Right side
            };
        }

        /// <summary>
        /// Called by UI or automatically to end turn
        /// </summary>
        public void OnEndTurnButtonClicked()
        {
            int currentPlayer = TurnManager.Instance.GetCurrentPlayer();

            // If current player is AI, execute AI turn
            if (aiControllers[currentPlayer] != null)
            {
                aiControllers[currentPlayer].ExecuteTurn();
            }

            TurnManager.Instance.EndTurn();

            // If next player is also AI, execute immediately
            int nextPlayer = TurnManager.Instance.GetCurrentPlayer();
            if (aiControllers[nextPlayer] != null)
            {
                Invoke(nameof(ExecuteAITurnDelayed), 0.5f); // Small delay for visualization
            }
        }

        private void ExecuteAITurnDelayed()
        {
            int currentPlayer = TurnManager.Instance.GetCurrentPlayer();
            if (aiControllers[currentPlayer] != null)
            {
                aiControllers[currentPlayer].ExecuteTurn();
                TurnManager.Instance.EndTurn();

                // Continue if next is also AI
                int nextPlayer = TurnManager.Instance.GetCurrentPlayer();
                if (aiControllers[nextPlayer] != null)
                {
                    Invoke(nameof(ExecuteAITurnDelayed), 0.5f);
                }
            }
        }

        private void Update()
        {
            // Auto-advance for AI vs AI
            if (!playerIsHuman && Input.GetKeyDown(KeyCode.Space))
            {
                OnEndTurnButtonClicked();
            }
        }
    }
}
