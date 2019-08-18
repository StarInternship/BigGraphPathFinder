using System;

namespace BigDataPathFinding.Models
{
    public class InputAdjacent : Adjacent
    {
        public InputAdjacent(Guid sourceId, Guid targetId, double weight) : base(sourceId, weight)
        {
            SourceId = sourceId;
            TargetId = targetId;
        }

        public Guid SourceId { get; }
        public Guid TargetId { get; }
    }
}