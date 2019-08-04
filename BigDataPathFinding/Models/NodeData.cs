using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public class NodeData : IComparable<NodeData>
    {
        public Guid Id { get; }
        public bool Explored { get; set; }
        public double Distance { get; set; }
        public HashSet<Adjacent> PreviousAdjacents { get; } = new HashSet<Adjacent>();

        public NodeData(Guid id) => Id = id;

        public int CompareTo(NodeData other)
        {
            if (Math.Abs(Distance - other.Distance) < 0.01)
            {
                return Id.CompareTo(other.Id);
            }

            return Distance - other.Distance > 0 ? 1 : -1;
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            return obj is NodeData nodeData && Id.Equals(nodeData.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}