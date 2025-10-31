using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.AI.Strategic;
using TurnBasedStrategy.AI.Tactical;
using TurnBasedStrategy.Core.Units;
using TurnBasedStrategy.Managers;

namespace TurnBasedStrategy.AI
{
    /// <summary>
    /// Main AI Controller - Integrates all 3 AI levels
    /// Strategic -> Tactical -> Operational hierarchy
    /// </summary>
    public class AIController : MonoBehaviour
    {
        private int playerId;
        private StrategicAI strategicAI;
        private TacticalAI tacticalAI;

        public void Initialize(int playerId)
        {
            this.playerId = playerId;
            this.strategicAI = new StrategicAI(playerId);
            this.tacticalAI = new TacticalAI();
        }

        /// <summary>
        /// Execute AI turn - full 3-level hierarchy
        /// </summary>
        public void ExecuteTurn()
        {
            Debug.Log($"AI Player {playerId} executing turn...");

            // LEVEL 1: STRATEGIC AI
            // Makes high-level decisions: production, waypoints, global strategy
            strategicAI.MakeStrategicDecisions();

            // LEVEL 2 & 3: TACTICAL AI + OPERATIONAL AI
            // For each unit, Tactical AI decides action, Operational AI executes movement
            List<Unit> myUnits = UnitManager.Instance.GetUnitsForPlayer(playerId);

            foreach (Unit unit in myUnits)
            {
                if (!unit.IsAlive())
                    continue;

                // Strategic AI provides objective
                Core.Map.HexCoordinates? objective = strategicAI.GetObjectiveForUnit(unit);

                // Tactical AI decides what to do with the objective
                // (Behavior tree internally calls Operational AI for pathfinding)
                tacticalAI.ExecuteUnitDecision(unit, objective);

                // Claim cells we move to
                Core.Map.Cell currentCell = Core.Map.MapManager.Instance.GetCell(unit.Position);
                if (currentCell != null && currentCell.Owner != playerId)
                {
                    currentCell.Owner = playerId;
                }
            }

            Debug.Log($"AI Player {playerId} turn complete.");
        }

        public StrategicAI GetStrategicAI()
        {
            return strategicAI;
        }
    }
}
