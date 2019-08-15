using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public abstract class AbstractPathFinder
    {
        protected readonly bool Directed;
        protected readonly IMetadata Metadata;
        protected readonly Guid SourceId;
        protected readonly Guid TargetId;
        protected readonly int MaxDistance;
        protected readonly int MinDistance;

        protected AbstractPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed , int maxDistance , int minDistance)
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