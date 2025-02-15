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
        private int _activeChild = 0;
        private List<BaseNode> _nodes = new List<BaseNode>();
        public void Add(BaseNode node) => _nodes.Add(node);
        public override NodeStatus Execute()
        {
            var childState = _nodes[_activeChild].Execute();
            switch (childState)
            {
                case NodeStatus.Success:
                    _activeChild++;
                    if (_activeChild == _nodes.Count)
                    {
                        _activeChild = 0;
                        return NodeStatus.Success;
                    }
                    else
                    {
                        return NodeStatus.Running;
                    }
                case NodeStatus.Failure:
                    _activeChild = 0;
                    return NodeStatus.Failure;
                case NodeStatus.Running:
                    return NodeStatus.Running;
            }
            throw new Exception("想定していない状態が返されました。");
        }
    }

    public class SelectorNode : BaseNode
    {
        private int _activeChild = 0;
        private List<BaseNode> _nodes = new List<BaseNode>();
        public void Add(BaseNode node) => _nodes.Add(node);

        public override NodeStatus Execute()
        {
            var childState = _nodes[_activeChild].Execute();
            switch (childState)
            {
                case NodeStatus.Success:
                    _activeChild = 0;
                    return NodeStatus.Success;
                case NodeStatus.Failure:
                    _activeChild++;
                    if (_activeChild == _nodes.Count)
                    {
                        _activeChild = 0;
                        return NodeStatus.Failure;
                    }
                    else
                    {
                        return NodeStatus.Running;
                    }
                case NodeStatus.Running:
                    return NodeStatus.Running;
            }
            throw new Exception("想定していない状態が返されました。");
        }
    }
}