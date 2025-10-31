using TurnBasedStrategy.Core.Units;
using TurnBasedStrategy.Core.Map;

namespace TurnBasedStrategy.AI.Tactical
{
    /// <summary>
    /// Tactical AI - Makes decisions for individual units using Behavior Trees
    /// Receives objectives from Strategic AI
    /// </summary>
    public class TacticalAI
    {
        private BehaviorNode rootNode;

        public TacticalAI()
        {
            BuildBehaviorTree();
        }

        private void BuildBehaviorTree()
        {
            // Behavior Tree Structure:
            // Selector (try each until one succeeds)
            //   - Sequence: If health low AND enemy nearby -> Retreat
            //   - Sequence: If can attack enemy -> Attack
            //   - Sequence: If has objective -> Move to objective
            //   - Default: Defend position

            rootNode = new SelectorNode(
                // Priority 1: Retreat if low health and enemy nearby
                new SequenceNode(
                    new IsHealthLowNode(0.3f),
                    new IsEnemyNearbyNode(3),
                    new RetreatNode()
                ),

                // Priority 2: Attack if enemy in range
                new SequenceNode(
                    new IsEnemyNearbyNode(5),
                    new CanAttackEnemyNode(),
                    new AttackEnemyNode()
                ),

                // Priority 3: Move towards objective
                new SequenceNode(
                    new HasObjectiveNode(),
                    new MoveToObjectiveNode()
                ),

                // Priority 4: Defend current position
                new DefendPositionNode()
            );
        }

        /// <summary>
        /// Execute behavior tree for a unit with given objective
        /// </summary>
        public void ExecuteUnitDecision(Unit unit, HexCoordinates? objective = null)
        {
            TacticalContext context = new TacticalContext
            {
                HasObjective = objective.HasValue,
                ObjectivePosition = objective ?? unit.Position,
                PlayerId = unit.Owner
            };

            rootNode.Execute(unit, context);
        }
    }
}
