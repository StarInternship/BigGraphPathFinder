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

        protected AbstractPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed)
        {
            Metadata = metadata;
            SourceId = sourceId;
            TargetId = targetId;
            Directed = directed;
        }

        public abstract void FindPath();

        public abstract Dictionary<Guid, NodeData> GetResultNodeSet();
    }
}