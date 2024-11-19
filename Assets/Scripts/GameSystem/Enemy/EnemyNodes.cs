using System;
using System.Collections.Generic;

public class EnemyNodes
{
    public enum NodeStatus {Success, Failure, Running }
    public abstract class BaseNode { public abstract NodeStatus Execute(); }

    public class ActionNode : BaseNode
    {
        private Func<NodeStatus> _action;
        public ActionNode(Func<NodeStatus> action) => _action = action;
        public override NodeStatus Execute() => _action();
    }

    public class ConditionNode : BaseNode
    {
        private Func<bool> _condition;
        public ConditionNode(Func<bool> condition) => _condition = condition;
        public override NodeStatus Execute() => _condition()? NodeStatus.Success : NodeStatus.Failure;
    }

    public class SequenceNode : BaseNode
    {
        private List<BaseNode> _nodes = new List<BaseNode>();
        public void Add(BaseNode node) => _nodes.Add(node);
        public override NodeStatus Execute()
        {
            foreach (var node in _nodes)
            {
                var status = node.Execute();
                if (status == NodeStatus.Failure) return NodeStatus.Failure;
                else if (status == NodeStatus.Running) return NodeStatus.Running;
            }
            return NodeStatus.Success;
        }
    }

    public class SelectorNode : BaseNode
    {
        private List<BaseNode> _nodes = new List<BaseNode>();
        public void Add(BaseNode node) => _nodes.Add(node);

        public override NodeStatus Execute()
        {
            foreach (var node in _nodes)
            {
                var status = node.Execute();
                if (status == NodeStatus.Success) return NodeStatus.Success;
                else if (status == NodeStatus.Running) return NodeStatus.Running;
            }
            return NodeStatus.Failure;
        }
    }
}