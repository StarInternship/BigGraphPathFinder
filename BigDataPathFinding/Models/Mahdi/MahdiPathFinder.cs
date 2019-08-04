using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models.Mahdi
{
    internal class MahdiPathFinder : PathFinder
    {
        public MahdiPathFinder(IDatabase database, IMetadata metadata, Guid sourceId, Guid targetId, bool directed) : base(metadata, sourceId, targetId, directed)
        {
        }

        public override void FindPath()
        {
            throw new NotImplementedException();
        }

        public override Dictionary<Guid, NodeData> GetResultNodeSet()
        {
            throw new NotImplementedException();
        }
    }
}