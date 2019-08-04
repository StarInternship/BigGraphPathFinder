using System;

namespace BigDataPathFinding.Models.Mahdi
{
    internal class MahdiPathFinder : PathFinder
    {
        public MahdiPathFinder(IDatabase database, IMetadata metadata, Guid sourceId, Guid targetId, bool directed) : base(database, metadata, sourceId, targetId, directed)
        {
        }

        public override void FindPath()
        {
            throw new NotImplementedException();
        }
    }
}