﻿using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public class NodeData : IComparable<NodeData>
    {
        public NodeData(Guid id, double distance)
        {
            Id = id;
            Distance = distance;
        }

        public Guid Id { get; }
        public bool Explored { get; set; } = false;
        public double Distance { get; set; }
        public HashSet<Adjacent> PreviousAdjacent { get; } = new HashSet<Adjacent>();
        public HashSet<Adjacent> ForwardAdjacents { get; } = new HashSet<Adjacent>();

        public int CompareTo(NodeData other)
        {
            if (Math.Abs(Distance - other.Distance) < 0.01) return Id.CompareTo(other.Id);
            return Distance - other.Distance > 0 ? 1 : -1;
        }

        public void ClearAdjacentAndUpdateDistance(Adjacent adjacent, double distance)
        {
            PreviousAdjacent.Clear();
            PreviousAdjacent.Add(adjacent);
            Distance = distance;
        }

        public void AddAdjacent(Adjacent adjacent) => PreviousAdjacent.Add(adjacent);

        public override bool Equals(object obj) => this == obj || (obj is NodeData nodeData && Id.Equals(nodeData.Id));

        public override int GetHashCode() => Id.GetHashCode();
    }
}