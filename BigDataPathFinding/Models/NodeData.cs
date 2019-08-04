using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public class NodeData
    {
        public Guid Id { get; }
        public bool Explored { get; set; }
        public double Distance { get; set; }
        public HashSet<Adjacent> PreviousAdjacents { get; } = new HashSet<Adjacent>();
    }
}