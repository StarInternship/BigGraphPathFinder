using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataPathFinding.Models
{
    public abstract class PathFinder
    {
        protected readonly IMetadata metadata;
        protected readonly Guid sourceId;
        protected readonly Guid targetId;
        protected readonly bool directed;
        public Graph Result { get; } = new Graph();

        protected PathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed)
        {
            this.metadata = metadata;
            this.sourceId = sourceId;
            this.targetId = targetId;
            this.directed = directed;
        }

        public abstract void FindPath();
    }
}
