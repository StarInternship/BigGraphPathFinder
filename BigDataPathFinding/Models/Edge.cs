﻿using System;

namespace BigDataPathFinding.Models
{
    public class Edge
    {
        public Edge(Guid sourceId, Guid targetId, double weight)
        {
            SourceId = sourceId;
            TargetId = targetId;
            Weight = weight;
        }

        public Guid SourceId { get; }
        public Guid TargetId { get; }
        public double Weight { get; }

        public override bool Equals(object obj)
        {
            return this == obj ||
                   obj is Edge edge && SourceId.Equals(edge.SourceId) && TargetId.Equals(edge.TargetId) &&
                   Math.Abs(Weight - edge.Weight) < 0.01;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return SourceId + "," + TargetId + "," + Weight;
        }
    }
}