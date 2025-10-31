using UnityEngine;
using TurnBasedStrategy.Core.Map;

namespace TurnBasedStrategy.Core.Units
{
    /// <summary>
    /// ScriptableObject defining unit statistics and properties
    /// </summary>
    [CreateAssetMenu(fileName = "UnitData", menuName = "TBS/Unit Data")]
    public class UnitData : ScriptableObject
    {
        [Header("Basic Info")]
        public UnitType unitType;
        public string unitName;

        [Header("Costs")]
        public int woodCost;
        public int goldCost;
        public int foodCost;

        [Header("Combat Stats")]
        public int attackPoints;
        public int defensePoints;
        public int health;

        [Header("Movement")]
        public int movementPoints;
        public int attackRange;

        [Header("Terrain Modifiers")]
        public TerrainType preferredTerrain;
        public TerrainType penalizedTerrain;

        // Check if unit has terrain advantage
        public float GetTerrainModifier(TerrainType terrain)
        {
            if (terrain == preferredTerrain)
                return 1.2f; // 20% bonus
            if (terrain == penalizedTerrain)
                return 0.8f; // 20% penalty
            return 1.0f;
        }
    }
}
