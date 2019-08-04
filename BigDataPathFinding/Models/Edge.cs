using System;

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

        //TODO: override methods
    }
}