using System;
using System.Collections.Generic;
using System.Linq;

namespace BigDataPathFinding.Models.Hadi
{
    public class SearchData : ISearchData
    {
        private readonly Dictionary<Guid, NodeData> _nodeSet = new Dictionary<Guid, NodeData>();
        private readonly SortedSet<NodeData> _queue = new SortedSet<NodeData>();

        public SearchData(NodeData source)
        {
            _queue.Add(source);
            _nodeSet[source.Id] = source;
        }

        public void AddToNodeSet(NodeData node)
        {
            _nodeSet[node.Id] = node;
        }

        public void AddToQueue(NodeData node)
        {
            _queue.Add(node);
        }

        public NodeData GetNode(Guid id)
        {
            return _nodeSet?[id];
        }

        public bool IsEmpty()
        {
            return _queue.Count == 0;
        }

        public NodeData PopBestCurrentNode()
        {
            var first = _queue.First();
            _queue.Remove(first);
            return first;
        }
    }
}