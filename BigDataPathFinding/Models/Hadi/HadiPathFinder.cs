using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataPathFinding.Models.Hadi
{
    public class HadiPathFinder : PathFinder
    {

        public HadiPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed) : base(metadata, sourceId, targetId, directed)
        {
        }

        public override void FindPath()
        {
        }

        public override Dictionary<Guid, NodeData> GetResultNodeSet()
        {
            throw new NotImplementedException();
        }
    }
}
