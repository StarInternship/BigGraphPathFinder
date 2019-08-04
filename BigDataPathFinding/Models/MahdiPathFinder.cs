using System;

namespace BigDataPathFinding.Models
{
    internal class MahdiPathFinder : PathFinder
    {
        public MahdiPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed) :
            base(metadata, sourceId, targetId, directed)
        {
        }

        public override void FindPath()
        {
            throw new NotImplementedException();
        }
    }
}