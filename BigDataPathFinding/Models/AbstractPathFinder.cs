using System;
using BigDataPathFinding.Models.Interfaces;

namespace BigDataPathFinding.Models
{
    public abstract class AbstractPathFinder
    {
        protected readonly bool Directed;
        internal int MaxDistance;
        protected readonly IMetadata Metadata;
        internal int MinDistance;
        protected readonly Guid SourceId;
        protected readonly Guid TargetId;

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

        public abstract void FindPath();

        public abstract ISearchData GetSearchData();
    }
}