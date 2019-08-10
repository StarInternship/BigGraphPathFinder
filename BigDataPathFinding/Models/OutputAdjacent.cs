using System;

namespace BigDataPathFinding.Models
{
    class OutputAdjacent : Adjacent
    {
        public Guid SourceId { get; }
        public Guid TargetId { get; }

        public OutputAdjacent(Guid sourceId, Guid targetId, double weight) : base(targetId, weight)
        {
            SourceId = sourceId;
            TargetId = targetId;
        }
    }
}
