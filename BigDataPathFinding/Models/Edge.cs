using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BigDataPathFinding.Models
{
    public class Edge
    {
        public Guid SourceId { get; }
        public Guid TargetId { get; }
        public double Weight { get; }

        public Edge(Guid sourceId, Guid targetId, double weight)
        {
            SourceId = sourceId;
            TargetId = targetId;
            Weight = weight;
        }

        //TODO: override methods
    }
}