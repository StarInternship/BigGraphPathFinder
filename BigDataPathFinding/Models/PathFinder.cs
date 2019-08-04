using System;

namespace BigDataPathFinding.Models
{
    public abstract class PathFinder
    {
        protected readonly bool directed;
        protected readonly IMetadata metadata;
        protected readonly Guid sourceId;
        protected readonly Guid targetId;

        protected PathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed)
        {
            this.metadata = metadata;
            this.sourceId = sourceId;
            this.targetId = targetId;
            this.directed = directed;
        }

        public Graph Result { get; } = new Graph();

        public abstract void FindPath();
    }
}