using UnityEngine;
using TurnBasedStrategy.Core.Units;
using TurnBasedStrategy.Core.Map;
using TurnBasedStrategy.AI.Operational;
using TurnBasedStrategy.Managers;
using System.Collections.Generic;

namespace TurnBasedStrategy.AI.Tactical
{
    /// <summary>
    /// Concrete behavior actions for units
    /// </summary>

    // Condition Nodes
    public class IsHealthLowNode : BehaviorNode
    {
        private float threshold;

        public IsHealthLowNode(float threshold = 0.3f)
        {
            this.threshold = threshold;
        }

        public override NodeStatus Execute(Unit unit, TacticalContext context)
        {
            float healthPercent = (float)unit.CurrentHealth / unit.Data.health;
            context.IsHealthLow = healthPercent < threshold;
            return context.IsHealthLow ? NodeStatus.Success : NodeStatus.Failure;
        }
    }

    public class IsEnemyNearbyNode : BehaviorNode
    {
        private int range;

        public IsEnemyNearbyNode(int range = 3)
        {
            this.range = range;
        }

        public override NodeStatus Execute(Unit unit, TacticalContext context)
        {
            Unit nearest = FindNearestEnemy(unit);
            if (nearest != null)
            {
                int distance = HexCoordinates.Distance(unit.Position, nearest.Position);
                context.NearestEnemy = nearest;
                return distance <= range ? NodeStatus.Success : NodeStatus.Failure;
            }
            return NodeStatus.Failure;
        }

        private Unit FindNearestEnemy(Unit unit)
        {
            Unit nearest = null;
            int minDistance = int.MaxValue;

            foreach (var other in UnitManager.Instance.GetAllUnits())
            {
                if (other.Owner != unit.Owner && other.IsAlive())
                {
                    int distance = HexCoordinates.Distance(unit.Position, other.Position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = other;
                    }
                }
            }
            return nearest;
        }
    }

    public class CanAttackEnemyNode : BehaviorNode
    {
        public override NodeStatus Execute(Unit unit, TacticalContext context)
        {
            if (unit.HasAttacked || context.NearestEnemy == null)
                return NodeStatus.Failure;

            return unit.CanAttack(context.NearestEnemy.Position) ? NodeStatus.Success : NodeStatus.Failure;
        }
    }

    public class HasObjectiveNode : BehaviorNode
    {
        public override NodeStatus Execute(Unit unit, TacticalContext context)
        {
            return context.HasObjective ? NodeStatus.Success : NodeStatus.Failure;
        }
    }

    // Action Nodes
    public class AttackEnemyNode : BehaviorNode
    {
        public override NodeStatus Execute(Unit unit, TacticalContext context)
        {
            if (context.NearestEnemy == null || !unit.CanAttack(context.NearestEnemy.Position))
                return NodeStatus.Failure;

            Cell attackerCell = MapManager.Instance.GetCell(unit.Position);
            Cell defenderCell = MapManager.Instance.GetCell(context.NearestEnemy.Position);

            unit.Attack(context.NearestEnemy, attackerCell.Terrain, defenderCell.Terrain);
            Debug.Log($"Unit {unit.Data.unitType} attacked enemy {context.NearestEnemy.Data.unitType}");

            return NodeStatus.Success;
        }
    }

    public class MoveToObjectiveNode : BehaviorNode
    {
        public override NodeStatus Execute(Unit unit, TacticalContext context)
        {
            if (!context.HasObjective || !unit.CanMove())
                return NodeStatus.Failure;

            List<HexCoordinates> path = Pathfinding.FindPath(unit.Position, context.ObjectivePosition, unit, true);

            if (path != null && path.Count > 1)
            {
                // Move as far as possible along path
                int stepsTaken = 0;
                for (int i = 1; i < path.Count && unit.RemainingMovement > 0; i++)
                {
                    Cell nextCell = MapManager.Instance.GetCell(path[i]);
                    if (nextCell.IsOccupied)
                        break;

                    int moveCost = nextCell.GetMovementCost();
                    if (moveCost <= unit.RemainingMovement)
                    {
                        // Update old cell
                        Cell oldCell = MapManager.Instance.GetCell(unit.Position);
                        oldCell.Occupant = null;

                        // Move unit
                        unit.Move(path[i], moveCost);
                        unit.transform.position = path[i].ToWorldPosition(MapManager.Instance.hexSize) + Vector3.up * 0.5f;

                        // Update new cell
                        nextCell.Occupant = unit;
                        stepsTaken++;
                    }
                    else
                    {
                        break;
                    }
                }

                return stepsTaken > 0 ? NodeStatus.Success : NodeStatus.Failure;
            }

            return NodeStatus.Failure;
        }
    }

    public class RetreatNode : BehaviorNode
    {
        public override NodeStatus Execute(Unit unit, TacticalContext context)
        {
            if (!unit.CanMove() || context.NearestEnemy == null)
                return NodeStatus.Failure;

            // Find safest cell to retreat to
            HexCoordinates retreatPosition = FindSafestPosition(unit, context.NearestEnemy.Position);

            if (retreatPosition == unit.Position)
                return NodeStatus.Failure;

            List<HexCoordinates> path = Pathfinding.FindPath(unit.Position, retreatPosition, unit, true);

            if (path != null && path.Count > 1)
            {
                Cell nextCell = MapManager.Instance.GetCell(path[1]);
                if (!nextCell.IsOccupied)
                {
                    int moveCost = nextCell.GetMovementCost();
                    if (moveCost <= unit.RemainingMovement)
                    {
                        Cell oldCell = MapManager.Instance.GetCell(unit.Position);
                        oldCell.Occupant = null;

                        unit.Move(path[1], moveCost);
                        unit.transform.position = path[1].ToWorldPosition(MapManager.Instance.hexSize) + Vector3.up * 0.5f;

                        nextCell.Occupant = unit;
                        Debug.Log($"Unit {unit.Data.unitType} retreating");
                        return NodeStatus.Success;
                    }
                }
            }

            return NodeStatus.Failure;
        }

        private HexCoordinates FindSafestPosition(Unit unit, HexCoordinates threatPosition)
        {
            List<HexCoordinates> neighbors = MapManager.Instance.GetNeighbors(unit.Position);
            HexCoordinates safest = unit.Position;
            int maxDistance = HexCoordinates.Distance(unit.Position, threatPosition);

            foreach (var neighbor in neighbors)
            {
                Cell cell = MapManager.Instance.GetCell(neighbor);
                if (cell == null || cell.IsOccupied)
                    continue;

                int distance = HexCoordinates.Distance(neighbor, threatPosition);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    safest = neighbor;
                }
            }

            return safest;
        }
    }

    public class DefendPositionNode : BehaviorNode
    {
        public override NodeStatus Execute(Unit unit, TacticalContext context)
        {
            // Stay in place (defending is passive)
            Debug.Log($"Unit {unit.Data.unitType} defending position");
            return NodeStatus.Success;
        }
    }
}
