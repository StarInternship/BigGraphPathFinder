﻿using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models.FileGraph
{
    public class FileNode : Node
    {
        public FileNode(Guid id, string name) : base(id, new Dictionary<string, object> {["name"] = name })
        {
        }

        public List<Adjacent> OutputAdjacent { get; } = new List<Adjacent>();
        public List<Adjacent> InputAdjacent { get; } = new List<Adjacent>();

        public void AddInput(Guid inputId, double weight) => InputAdjacent.Add(new Adjacent(inputId, weight));

        public void AddOutput(Guid outputId, double weight) => OutputAdjacent.Add(new Adjacent(outputId, weight));
    }
}