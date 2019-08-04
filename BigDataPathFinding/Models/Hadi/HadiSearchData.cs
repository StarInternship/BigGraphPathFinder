using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataPathFinding.Models.Hadi
{
    public class HadiSearchData : ISearchData
    {
        private Dictionary<Guid, NodeData> _nodeSet = new Dictionary<Guid, NodeData>();
        private SortedSet<NodeData> _queue = new SortedSet<NodeData>();

        public HadiSearchData(NodeData source)
        {
            _queue.Add(source);
            _nodeSet[source.Id] = source;
        }

        public void AddToNodeSet(NodeData node)
        {
            throw new NotImplementedException();
        }

        public void AddToQueue(NodeData node)
        {
            throw new NotImplementedException();
        }

        public NodeData GetNode(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty()
        {
            throw new NotImplementedException();
        }

        public Guid PopBestCurrentNode()
        {
            throw new NotImplementedException();
        }
    }
}
