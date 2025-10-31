using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TurnBasedStrategy.Core.Map;
using TurnBasedStrategy.Core.Units;
using TurnBasedStrategy.Core.Resources;
using TurnBasedStrategy.Managers;

namespace TurnBasedStrategy.AI.Strategic
{
    /// <summary>
    /// Strategic AI - Global decision making
    /// Coordinates all units, manages waypoints, and makes high-level decisions
    /// </summary>
    public class StrategicAI
    {
        private int playerId;
        private InfluenceMap influenceMap;
        private List<TacticalWaypoint> waypoints;

        public StrategicAI(int playerId)
        {
            this.playerId = playerId;
            this.influenceMap = new InfluenceMap();
            this.waypoints = new List<TacticalWaypoint>();
        }

        /// <summary>
        /// Main strategic decision loop - called at start of AI turn
        /// </summary>
        public void MakeStrategicDecisions()
        {
            // Update influence map
            influenceMap.Calculate(playerId);

            // Update waypoints based on current situation
            UpdateWaypoints();

            // Make production decisions
            DecideProduction();
        }

        /// <summary>
        /// Get objective for a specific unit (called by Tactical AI)
        /// </summary>
        public HexCoordinates? GetObjectiveForUnit(Unit unit)
        {
            if (waypoints.Count == 0)
                return null;

            // Find best waypoint for this unit based on:
            // 1. Waypoint priority
            // 2. Distance to waypoint
            // 3. Current unit health

            TacticalWaypoint bestWaypoint = null;
            float bestScore = float.MinValue;

            foreach (var waypoint in waypoints.Where(w => w.IsActive))
            {
                float distance = HexCoordinates.Distance(unit.Position, waypoint.Position);
                float healthFactor = (float)unit.CurrentHealth / unit.Data.health;

                // Score calculation
                float score = waypoint.Priority * 10f - distance;

                // Low health units prefer defensive/rally waypoints
                if (healthFactor < 0.5f)
                {
                    if (waypoint.Type == WaypointType.Defense || waypoint.Type == WaypointType.Rally)
                        score += 20f;
                }
                else
                {
                    // Healthy units prefer attack/resource waypoints
                    if (waypoint.Type == WaypointType.Attack || waypoint.Type == WaypointType.Resource)
                        score += 15f;
                }

                if (score > bestScore)
                {
                    bestScore = score;
                    bestWaypoint = waypoint;
                }
            }

            return bestWaypoint?.Position;
        }

        private void UpdateWaypoints()
        {
            waypoints.Clear();

            // Create attack waypoint at weakest enemy position
            HexCoordinates attackPos = influenceMap.FindWeakestEnemyPosition();
            waypoints.Add(new TacticalWaypoint(attackPos, WaypointType.Attack, priority: 8));

            // Create defensive waypoint at safest friendly position
            HexCoordinates defensePos = influenceMap.FindSafestPosition();
            waypoints.Add(new TacticalWaypoint(defensePos, WaypointType.Defense, priority: 6));

            // Create resource waypoints at resource-rich neutral cells
            List<Cell> resourceCells = MapManager.Instance.GetAllCells()
                .Where(c => c.HasResource && c.Owner != playerId)
                .OrderBy(c => HexCoordinates.Distance(c.Coordinates, GetAverageUnitPosition()))
                .Take(2)
                .ToList();

            foreach (var cell in resourceCells)
            {
                waypoints.Add(new TacticalWaypoint(cell.Coordinates, WaypointType.Resource, priority: 7));
            }

            // Create rally point at center of friendly units
            HexCoordinates rallyPos = GetAverageUnitPosition();
            waypoints.Add(new TacticalWaypoint(rallyPos, WaypointType.Rally, priority: 5));
        }

        private HexCoordinates GetAverageUnitPosition()
        {
            List<Unit> myUnits = UnitManager.Instance.GetUnitsForPlayer(playerId);
            if (myUnits.Count == 0)
                return new HexCoordinates(0, 0);

            int avgQ = 0, avgR = 0;
            foreach (var unit in myUnits)
            {
                avgQ += unit.Position.q;
                avgR += unit.Position.r;
            }

            return new HexCoordinates(avgQ / myUnits.Count, avgR / myUnits.Count);
        }

        private void DecideProduction()
        {
            PlayerResources resources = ResourceManager.Instance.GetResources(playerId);
            List<Unit> myUnits = UnitManager.Instance.GetUnitsForPlayer(playerId);

            // Simple production strategy:
            // - If few units, produce Infantry (cheapest)
            // - If enemy detected, balance unit types
            // - Prefer Cavalry if we have resources

            if (myUnits.Count < 3)
            {
                // Build initial army
                TryProduceUnit(UnitType.Infantry);
            }
            else
            {
                // Balance army composition
                int infantryCount = myUnits.Count(u => u.Data.unitType == UnitType.Infantry);
                int cavalryCount = myUnits.Count(u => u.Data.unitType == UnitType.Cavalry);
                int artilleryCount = myUnits.Count(u => u.Data.unitType == UnitType.Artillery);

                // Produce based on what we lack
                if (cavalryCount < infantryCount / 2)
                    TryProduceUnit(UnitType.Cavalry);
                else if (artilleryCount < myUnits.Count / 3)
                    TryProduceUnit(UnitType.Artillery);
                else
                    TryProduceUnit(UnitType.Infantry);
            }
        }

        private void TryProduceUnit(UnitType unitType)
        {
            UnitData unitData = UnitManager.Instance.infantryData; // Default

            switch (unitType)
            {
                case UnitType.Infantry:
                    unitData = UnitManager.Instance.infantryData;
                    break;
                case UnitType.Cavalry:
                    unitData = UnitManager.Instance.cavalryData;
                    break;
                case UnitType.Artillery:
                    unitData = UnitManager.Instance.artilleryData;
                    break;
            }

            if (ResourceManager.Instance.CanAfford(playerId, unitData))
            {
                // Find production building
                Cell productionCell = FindProductionCell(unitType);
                if (productionCell != null)
                {
                    // Find empty neighbor cell to spawn unit
                    foreach (var neighbor in MapManager.Instance.GetNeighbors(productionCell.Coordinates))
                    {
                        Cell cell = MapManager.Instance.GetCell(neighbor);
                        if (cell != null && !cell.IsOccupied)
                        {
                            ResourceManager.Instance.PayForUnit(playerId, unitData);
                            UnitManager.Instance.SpawnUnit(unitType, playerId, neighbor);
                            cell.Owner = playerId; // Claim cell
                            Debug.Log($"Strategic AI produced {unitType}");
                            return;
                        }
                    }
                }
            }
        }

        private Cell FindProductionCell(UnitType unitType)
        {
            StructureType requiredStructure = unitType == UnitType.Artillery
                ? StructureType.Factory
                : StructureType.Barracks;

            foreach (var cell in MapManager.Instance.GetAllCells())
            {
                if (cell.HasStructure &&
                    cell.Structure.Type == requiredStructure &&
                    cell.Structure.Owner == playerId)
                {
                    return cell;
                }
            }

            return null;
        }

        public List<TacticalWaypoint> GetWaypoints()
        {
            return waypoints;
        }
    }
}
