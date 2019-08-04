using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models.FileGraph
{
    public class FileNode : Node
    {
        public FileNode(Guid id, string name) : base(id, name)
        {
        }

        public List<Adjacent> OutputAdjucents { get; } = new List<Adjacent>();
        public List<Adjacent> InputAdjucents { get; } = new List<Adjacent>();

        public void AddInput(Guid inputId, double weight)
        {
            InputAdjucents.Add(new Adjacent(inputId, weight));
        }

        public void AddOutput(Guid outputId, double weight)
        {
            OutputAdjucents.Add(new Adjacent(outputId, weight));
        }
    }
}