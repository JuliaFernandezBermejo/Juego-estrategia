using TurnBasedStrategy.Core.Resources;
using TurnBasedStrategy.Core.Units;

namespace TurnBasedStrategy.Core.Map
{
    /// <summary>
    /// Represents a single cell on the hexagonal grid
    /// </summary>
    public class Cell
    {
        public HexCoordinates Coordinates { get; private set; }
        public TerrainType Terrain { get; set; }
        public Unit Occupant { get; set; }
        public int Owner { get; set; } // -1 = neutral, 0+ = player ID
        public ResourceType? ResourceOnCell { get; set; } // null if no resource
        public Structure Structure { get; set; } // Building on this cell

        public Cell(HexCoordinates coordinates, TerrainType terrain)
        {
            Coordinates = coordinates;
            Terrain = terrain;
            Owner = -1; // Neutral by default
        }

        public bool IsOccupied => Occupant != null;
        public bool HasResource => ResourceOnCell.HasValue;
        public bool HasStructure => Structure != null;

        // Get movement cost for this terrain type
        public int GetMovementCost()
        {
            return Terrain switch
            {
                TerrainType.Plains => 1,
                TerrainType.Forest => 2,
                TerrainType.Mountain => 3,
                _ => 1
            };
        }

        // Get defense bonus for this terrain
        public float GetDefenseBonus()
        {
            return Terrain switch
            {
                TerrainType.Plains => 0f,
                TerrainType.Forest => 0.2f,   // 20% defense bonus
                TerrainType.Mountain => 0.4f, // 40% defense bonus
                _ => 0f
            };
        }
    }
}
