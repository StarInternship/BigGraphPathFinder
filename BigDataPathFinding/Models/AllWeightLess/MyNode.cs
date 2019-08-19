using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BigDataPathFinding.Models.AllWeightLess
{
    [DebuggerDisplay("{" + nameof(Id) + "}")]
    public class MyNode
    {
        internal MyNode(Guid id)
        {
            Id = id;
        }

        internal Guid Id { get; }

        internal Dictionary<MyNode, double> Inputs { get; } = new Dictionary<MyNode, double>();
        internal Dictionary<MyNode, double> Outputs { get; } = new Dictionary<MyNode, double>();

        public void AddOutput(MyNode vertex, double weight)
        {
            Outputs[vertex] = weight;
        }

        public void AddInput(MyNode vertex, double weight)
        {
            Inputs[vertex] = weight;
        }
    }
}