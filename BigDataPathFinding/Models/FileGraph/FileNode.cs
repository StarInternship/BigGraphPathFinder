﻿using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models.FileGraph
{
    public class FileNode : Node
    {
        public List<Adjacent> OutputAdjucents { get; } = new List<Adjacent>();
        public List<Adjacent> InputAdjucents { get; } = new List<Adjacent>();

        public FileNode(Guid id, string name) : base(id, name)
        {
        }

        internal void AddInput(Guid inputId, double weight) => InputAdjucents.Add(new Adjacent(inputId, weight));

        internal void AddOutput(Guid outputId, double weight) => OutputAdjucents.Add(new Adjacent(outputId, weight));
    }
}