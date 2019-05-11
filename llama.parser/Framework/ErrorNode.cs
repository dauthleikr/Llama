using System;
using System.Collections.Generic;
using System.Text;

namespace Llama.Parser.Framework
{
    class ErrorNode
    {
        public ErrorNode Parent { get; }
        public int Depth { get; }

        private readonly List<ErrorNode> _children = new List<ErrorNode>();
        private readonly IErrorWithConfidence _error;

        public ErrorNode(IErrorWithConfidence error)
        {
            _error = error;
        }

        private ErrorNode(ErrorNode parent, IErrorWithConfidence error, int depth) : this(error)
        {
            Parent = parent;
            Depth = depth;
        }

        public ErrorNode AddChild(IErrorWithConfidence error) => AddChild(error, Depth + 1);

        public ErrorNode AddChild(IErrorWithConfidence error, int depth)
        {
            if (depth <= Depth)
                throw new InvalidOperationException($"Cannot add error with depth {depth} as child of node with depth {Depth}");
            var newNode = new ErrorNode(this, error, depth);
            _children.Add(newNode);
            return newNode;

        }
        public void KillChildren() => _children.Clear();

        public IErrorWithConfidence GetHighest() => GetHighest(this)._error;

        private ErrorNode GetHighest(ErrorNode current)
        {
            if (_children.Count == 0)
            {
                if (Depth > current.Depth || Depth == current.Depth && _error.ConfidenceMetric > current._error.ConfidenceMetric)
                    return this;
                return current;
            }

            foreach (var errorNode in _children)
                current = errorNode.GetHighest(current);
            return current;
        }
    }
}
