using System;

namespace BigDataPathFinding.Models
{
    public abstract class PathFinder
    {
        protected readonly bool Directed;
        protected readonly IMetadata Metadata;
        protected readonly Guid SourceId;
        protected readonly Guid TargetId;

        protected PathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed)
        {
            Metadata = metadata;
            SourceId = sourceId;
            TargetId = targetId;
            Directed = directed;
        }

        public Graph Result { get; } = new Graph();

        public abstract void FindPath();
    }
}