﻿using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public class NodeData : IComparable<NodeData>
    {
        public Seen Seen { get; }

        public NodeData(Guid id, double distance, Seen seen)
        {
            Id = id;
            Distance = distance;
            Seen = seen;
        }

        public Guid Id { get; }
        public double Distance { get; private set; }
        public HashSet<Adjacent> PreviousAdjacent { get; } = new HashSet<Adjacent>();
        public HashSet<Adjacent> ForwardAdjacent { get; } = new HashSet<Adjacent>();

        public int CompareTo(NodeData other)
        {
            if (Math.Abs(Distance - other.Distance) < 0.01) return Id.CompareTo(other.Id);
            return Distance - other.Distance > 0 ? 1 : -1;
        }

        public void AddBackwardAdjacent(Edge edge) => PreviousAdjacent.Add(new Adjacent(edge.SourceId, edge.Weight));

        public void AddForwardAdjacent(Edge edge) => ForwardAdjacent.Add(new Adjacent(edge.TargetId, edge.Weight));

        public override bool Equals(object obj) => this == obj || (obj is NodeData nodeData && Id.Equals(nodeData.Id));

        public override int GetHashCode() => Id.GetHashCode();
    }

    public enum Seen
    {
        Forward,
        Backward
    }
}