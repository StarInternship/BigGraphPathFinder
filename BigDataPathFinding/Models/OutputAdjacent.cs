using System;

namespace BigDataPathFinding.Models
{
    internal class OutputAdjacent : Adjacent
    {
        public OutputAdjacent(Guid sourceId, Guid targetId, double weight) : base(targetId, weight)
        {
            SourceId = sourceId;
            TargetId = targetId;
        }

        public Guid SourceId { get; }
        public Guid TargetId { get; }
    }
}