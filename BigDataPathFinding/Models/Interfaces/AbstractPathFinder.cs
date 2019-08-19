using System;

namespace BigDataPathFinding.Models.Interfaces
{
    public abstract class AbstractPathFinder
    {
        protected readonly bool Directed;
        protected readonly IMetadata Metadata;
        protected readonly Guid SourceId;
        protected readonly Guid TargetId;
        internal int MaxDistance;
        internal int MinDistance;

        protected AbstractPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed, int maxDistance,
            int minDistance)
        {
            Metadata = metadata;
            SourceId = sourceId;
            TargetId = targetId;
            Directed = directed;
            MaxDistance = maxDistance;
            MinDistance = minDistance;
        }

        public abstract Graph FindPath(IDatabase _database);
    }
}