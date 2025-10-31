using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Core.Map;
using TurnBasedStrategy.Managers;

namespace TurnBasedStrategy.AI.Strategic
{
    /// <summary>
    /// Influence map for strategic decision making
    /// Tracks friendly and enemy strength across the map
    /// </summary>
    public class InfluenceMap
    {
        private Dictionary<HexCoordinates, float> friendlyInfluence = new Dictionary<HexCoordinates, float>();
        private Dictionary<HexCoordinates, float> enemyInfluence = new Dictionary<HexCoordinates, float>();

        public void Calculate(int playerId)
        {
            friendlyInfluence.Clear();
            enemyInfluence.Clear();

            // Initialize all cells to zero influence
            foreach (var cell in MapManager.Instance.GetAllCells())
            {
                friendlyInfluence[cell.Coordinates] = 0f;
                enemyInfluence[cell.Coordinates] = 0f;
            }

            // Add influence from all units
            foreach (var unit in UnitManager.Instance.GetAllUnits())
            {
                if (!unit.IsAlive()) continue;

                bool isFriendly = unit.Owner == playerId;
                float baseInfluence = CalculateUnitInfluence(unit);

                // Propagate influence to nearby cells
                PropagateInfluence(unit.Position, baseInfluence, isFriendly, unit.Data.attackRange + 2);
            }

            // Add influence from production buildings
            foreach (var cell in MapManager.Instance.GetAllCells())
            {
                if (cell.HasStructure)
                {
                    bool isFriendly = cell.Structure.Owner == playerId;
                    float buildingInfluence = 5f;
                    PropagateInfluence(cell.Coordinates, buildingInfluence, isFriendly, 3);
                }
            }
        }

        private float CalculateUnitInfluence(Core.Units.Unit unit)
        {
            // Influence based on unit strength
            return (unit.Data.attackPoints + unit.Data.defensePoints) * 0.5f;
        }

        private void PropagateInfluence(HexCoordinates source, float strength, bool isFriendly, int range)
        {
            Queue<(HexCoordinates pos, int distance)> queue = new Queue<(HexCoordinates, int)>();
            HashSet<HexCoordinates> visited = new HashSet<HexCoordinates>();

            queue.Enqueue((source, 0));
            visited.Add(source);

            while (queue.Count > 0)
            {
                var (current, distance) = queue.Dequeue();

                // Calculate influence decay with distance
                float decay = 1f - (distance / (float)range);
                if (decay <= 0) continue;

                float influence = strength * decay;

                // Add to appropriate influence map
                if (isFriendly)
                    friendlyInfluence[current] = Mathf.Max(friendlyInfluence.GetValueOrDefault(current), influence);
                else
                    enemyInfluence[current] = Mathf.Max(enemyInfluence.GetValueOrDefault(current), influence);

                // Propagate to neighbors
                if (distance < range)
                {
                    foreach (var neighbor in MapManager.Instance.GetNeighbors(current))
                    {
                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            queue.Enqueue((neighbor, distance + 1));
                        }
                    }
                }
            }
        }

        public float GetFriendlyInfluence(HexCoordinates position)
        {
            return friendlyInfluence.GetValueOrDefault(position, 0f);
        }

        public float GetEnemyInfluence(HexCoordinates position)
        {
            return enemyInfluence.GetValueOrDefault(position, 0f);
        }

        public float GetSecurity(HexCoordinates position)
        {
            float friendly = GetFriendlyInfluence(position);
            float enemy = GetEnemyInfluence(position);
            return friendly - enemy; // Positive = safe, Negative = dangerous
        }

        public HexCoordinates FindSafestPosition()
        {
            HexCoordinates safest = new HexCoordinates(0, 0);
            float maxSecurity = float.MinValue;

            foreach (var cell in MapManager.Instance.GetAllCells())
            {
                float security = GetSecurity(cell.Coordinates);
                if (security > maxSecurity)
                {
                    maxSecurity = security;
                    safest = cell.Coordinates;
                }
            }

            return safest;
        }

        public HexCoordinates FindWeakestEnemyPosition()
        {
            HexCoordinates weakest = new HexCoordinates(0, 0);
            float minSecurity = float.MaxValue;

            foreach (var cell in MapManager.Instance.GetAllCells())
            {
                float security = GetSecurity(cell.Coordinates);
                float enemyInf = GetEnemyInfluence(cell.Coordinates);

                // Find positions with enemy presence but low security (for them)
                if (enemyInf > 0 && security < minSecurity)
                {
                    minSecurity = security;
                    weakest = cell.Coordinates;
                }
            }

            return weakest;
        }
    }
}
