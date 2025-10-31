using TurnBasedStrategy.Core.Units;

namespace TurnBasedStrategy.AI.Tactical
{
    /// <summary>
    /// Simple Behavior Tree implementation for unit decision making
    /// Tactical AI Level
    /// </summary>

    public enum NodeStatus
    {
        Success,
        Failure,
        Running
    }

    public abstract class BehaviorNode
    {
        public abstract NodeStatus Execute(Unit unit, TacticalContext context);
    }

    // Composite Nodes
    public class SequenceNode : BehaviorNode
    {
        private BehaviorNode[] children;

        public SequenceNode(params BehaviorNode[] children)
        {
            this.children = children;
        }

        public override NodeStatus Execute(Unit unit, TacticalContext context)
        {
            foreach (var child in children)
            {
                NodeStatus status = child.Execute(unit, context);
                if (status != NodeStatus.Success)
                    return status;
            }
            return NodeStatus.Success;
        }
    }

    public class SelectorNode : BehaviorNode
    {
        private BehaviorNode[] children;

        public SelectorNode(params BehaviorNode[] children)
        {
            this.children = children;
        }

        public override NodeStatus Execute(Unit unit, TacticalContext context)
        {
            foreach (var child in children)
            {
                NodeStatus status = child.Execute(unit, context);
                if (status != NodeStatus.Failure)
                    return status;
            }
            return NodeStatus.Failure;
        }
    }

    // Decorator Node
    public class InverterNode : BehaviorNode
    {
        private BehaviorNode child;

        public InverterNode(BehaviorNode child)
        {
            this.child = child;
        }

        public override NodeStatus Execute(Unit unit, TacticalContext context)
        {
            NodeStatus status = child.Execute(unit, context);
            return status switch
            {
                NodeStatus.Success => NodeStatus.Failure,
                NodeStatus.Failure => NodeStatus.Success,
                _ => status
            };
        }
    }

    /// <summary>
    /// Context data passed through behavior tree execution
    /// </summary>
    public class TacticalContext
    {
        public Unit NearestEnemy;
        public float ThreatLevel;
        public bool IsHealthLow;
        public bool HasObjective;
        public Core.Map.HexCoordinates ObjectivePosition;
        public int PlayerId;
    }
}
