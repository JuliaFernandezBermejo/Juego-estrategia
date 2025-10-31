using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Core.Map;
using TurnBasedStrategy.Core.Units;

namespace TurnBasedStrategy.Core.Resources
{
    /// <summary>
    /// Manages resources for all players
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        private Dictionary<int, PlayerResources> playerResources = new Dictionary<int, PlayerResources>();

        public static ResourceManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void InitializePlayer(int playerId, int startingWood = 100, int startingGold = 100, int startingFood = 100)
        {
            playerResources[playerId] = new PlayerResources
            {
                Wood = startingWood,
                Gold = startingGold,
                Food = startingFood
            };
        }

        public PlayerResources GetResources(int playerId)
        {
            if (!playerResources.ContainsKey(playerId))
                InitializePlayer(playerId);
            return playerResources[playerId];
        }

        public bool CanAfford(int playerId, UnitData unitData)
        {
            PlayerResources resources = GetResources(playerId);
            return resources.Wood >= unitData.woodCost &&
                   resources.Gold >= unitData.goldCost &&
                   resources.Food >= unitData.foodCost;
        }

        public bool CanAffordStructure(int playerId, StructureType structureType)
        {
            PlayerResources resources = GetResources(playerId);
            var cost = GetStructureCost(structureType);
            return resources.Wood >= cost.wood &&
                   resources.Gold >= cost.gold &&
                   resources.Food >= cost.food;
        }

        public void PayForUnit(int playerId, UnitData unitData)
        {
            PlayerResources resources = GetResources(playerId);
            resources.Wood -= unitData.woodCost;
            resources.Gold -= unitData.goldCost;
            resources.Food -= unitData.foodCost;
        }

        public void PayForStructure(int playerId, StructureType structureType)
        {
            PlayerResources resources = GetResources(playerId);
            var cost = GetStructureCost(structureType);
            resources.Wood -= cost.wood;
            resources.Gold -= cost.gold;
            resources.Food -= cost.food;
        }

        public void CollectResourcesForPlayer(int playerId)
        {
            PlayerResources resources = GetResources(playerId);

            // Collect from owned cells with resources
            foreach (Cell cell in MapManager.Instance.GetAllCells())
            {
                if (cell.Owner == playerId && cell.HasResource)
                {
                    switch (cell.ResourceOnCell.Value)
                    {
                        case ResourceType.Wood:
                            resources.Wood += 5;
                            break;
                        case ResourceType.Gold:
                            resources.Gold += 5;
                            break;
                        case ResourceType.Food:
                            resources.Food += 5;
                            break;
                    }
                }
            }
        }

        private (int wood, int gold, int food) GetStructureCost(StructureType type)
        {
            return type switch
            {
                StructureType.Barracks => (50, 30, 20),
                StructureType.Factory => (80, 60, 30),
                _ => (0, 0, 0)
            };
        }

        public void AddResources(int playerId, int wood, int gold, int food)
        {
            PlayerResources resources = GetResources(playerId);
            resources.Wood += wood;
            resources.Gold += gold;
            resources.Food += food;
        }
    }

    [System.Serializable]
    public class PlayerResources
    {
        public int Wood;
        public int Gold;
        public int Food;
    }
}
