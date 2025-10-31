using TurnBasedStrategy.Core.Map;

namespace TurnBasedStrategy.AI.Strategic
{
    /// <summary>
    /// Represents a tactical waypoint for strategic positioning
    /// </summary>
    public enum WaypointType
    {
        Attack,    // Offensive position to push enemy
        Defense,   // Defensive position to hold
        Resource,  // Resource gathering position
        Rally      // Regrouping point
    }

    public class TacticalWaypoint
    {
        public HexCoordinates Position { get; set; }
        public WaypointType Type { get; set; }
        public int Priority { get; set; } // Higher = more important
        public bool IsActive { get; set; }

        public TacticalWaypoint(HexCoordinates position, WaypointType type, int priority = 5)
        {
            Position = position;
            Type = type;
            Priority = priority;
            IsActive = true;
        }
    }
}
