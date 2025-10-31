using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Core.Map;
using TurnBasedStrategy.Core.Units;

namespace TurnBasedStrategy.AI.Operational
{
    /// <summary>
    /// A* Pathfinding with tactical considerations
    /// Operational AI Level - Movement execution
    /// </summary>
    public class Pathfinding
    {
        private class PathNode
        {
            public HexCoordinates Position;
            public PathNode Parent;
            public float GCost; // Distance from start
            public float HCost; // Heuristic distance to goal
            public float FCost => GCost + HCost;
        }

        /// <summary>
        /// Find path using A* with tactical weights
        /// </summary>
        public static List<HexCoordinates> FindPath(HexCoordinates start, HexCoordinates goal, Unit unit, bool avoidEnemies = true)
        {
            Dictionary<HexCoordinates, PathNode> openSet = new Dictionary<HexCoordinates, PathNode>();
            HashSet<HexCoordinates> closedSet = new HashSet<HexCoordinates>();

            PathNode startNode = new PathNode
            {
                Position = start,
                Parent = null,
                GCost = 0,
                HCost = HexCoordinates.Distance(start, goal)
            };

            openSet[start] = startNode;

            while (openSet.Count > 0)
            {
                // Find node with lowest F cost
                PathNode current = null;
                float lowestF = float.MaxValue;

                foreach (var node in openSet.Values)
                {
                    if (node.FCost < lowestF)
                    {
                        lowestF = node.FCost;
                        current = node;
                    }
                }

                if (current == null) break;

                // Reached goal
                if (current.Position == goal)
                    return ReconstructPath(current);

                openSet.Remove(current.Position);
                closedSet.Add(current.Position);

                // Check neighbors
                foreach (var neighborCoord in MapManager.Instance.GetNeighbors(current.Position))
                {
                    if (closedSet.Contains(neighborCoord))
                        continue;

                    Cell neighborCell = MapManager.Instance.GetCell(neighborCoord);
                    if (neighborCell == null)
                        continue;

                    // Can't move through occupied cells (unless it's the goal)
                    if (neighborCell.IsOccupied && neighborCoord != goal)
                        continue;

                    float movementCost = CalculateTacticalCost(neighborCell, unit, avoidEnemies);
                    float newGCost = current.GCost + movementCost;

                    if (!openSet.ContainsKey(neighborCoord) || newGCost < openSet[neighborCoord].GCost)
                    {
                        PathNode neighborNode = new PathNode
                        {
                            Position = neighborCoord,
                            Parent = current,
                            GCost = newGCost,
                            HCost = HexCoordinates.Distance(neighborCoord, goal)
                        };

                        openSet[neighborCoord] = neighborNode;
                    }
                }
            }

            return null; // No path found
        }

        /// <summary>
        /// Calculate tactical cost considering terrain, enemy proximity, and unit preferences
        /// </summary>
        private static float CalculateTacticalCost(Cell cell, Unit unit, bool avoidEnemies)
        {
            float baseCost = cell.GetMovementCost();

            // Terrain modifier for unit type
            float terrainModifier = unit.Data.GetTerrainModifier(cell.Terrain);
            float cost = baseCost / terrainModifier; // Preferred terrain = lower cost

            // Enemy proximity penalty (tactical pathfinding)
            if (avoidEnemies)
            {
                float threatLevel = CalculateThreatLevel(cell.Coordinates, unit.Owner);
                cost += threatLevel * 2f; // Add threat as extra cost
            }

            return cost;
        }

        /// <summary>
        /// Calculate threat level from nearby enemy units
        /// </summary>
        private static float CalculateThreatLevel(HexCoordinates position, int friendlyPlayerId)
        {
            float threat = 0f;

            foreach (var enemyUnit in UnitManager.Instance.GetAllUnits())
            {
                if (enemyUnit.Owner == friendlyPlayerId || !enemyUnit.IsAlive())
                    continue;

                int distance = HexCoordinates.Distance(position, enemyUnit.Position);

                // Enemy units within attack range are threatening
                if (distance <= enemyUnit.Data.attackRange)
                {
                    threat += 1.0f / Mathf.Max(distance, 1);
                }
            }

            return threat;
        }

        private static List<HexCoordinates> ReconstructPath(PathNode endNode)
        {
            List<HexCoordinates> path = new List<HexCoordinates>();
            PathNode current = endNode;

            while (current != null)
            {
                path.Add(current.Position);
                current = current.Parent;
            }

            path.Reverse();
            return path;
        }

        /// <summary>
        /// Get all cells within movement range (for UI display)
        /// </summary>
        public static List<HexCoordinates> GetReachableCells(HexCoordinates start, int movementPoints, Unit unit)
        {
            List<HexCoordinates> reachable = new List<HexCoordinates>();
            Dictionary<HexCoordinates, float> visited = new Dictionary<HexCoordinates, float>();
            Queue<(HexCoordinates pos, float cost)> queue = new Queue<(HexCoordinates, float)>();

            queue.Enqueue((start, 0));
            visited[start] = 0;

            while (queue.Count > 0)
            {
                var (current, currentCost) = queue.Dequeue();

                foreach (var neighbor in MapManager.Instance.GetNeighbors(current))
                {
                    Cell neighborCell = MapManager.Instance.GetCell(neighbor);
                    if (neighborCell == null || neighborCell.IsOccupied)
                        continue;

                    float moveCost = CalculateTacticalCost(neighborCell, unit, false);
                    float newCost = currentCost + moveCost;

                    if (newCost <= movementPoints && (!visited.ContainsKey(neighbor) || newCost < visited[neighbor]))
                    {
                        visited[neighbor] = newCost;
                        reachable.Add(neighbor);
                        queue.Enqueue((neighbor, newCost));
                    }
                }
            }

            return reachable;
        }
    }
}
