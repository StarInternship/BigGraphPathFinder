using System;

namespace BigDataPathFinding.Models
{
    public abstract class PathFinder
    {
        protected readonly IDatabase Database;
        protected readonly IMetadata Metadata;
        protected readonly Guid SourceId;
        protected readonly Guid TargetId;
        protected readonly bool Directed;

        protected PathFinder(IDatabase database, IMetadata metadata, Guid sourceId, Guid targetId, bool directed)
        {
            Database = database;
            Metadata = metadata;
            SourceId = sourceId;
            TargetId = targetId;
            Directed = directed;
        }

        public Graph Result { get; } = new Graph();

        public abstract void FindPath();
    }
}