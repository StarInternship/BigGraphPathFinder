using System;
using System.Collections.Generic;
using System.Linq;

namespace BigDataPathFinding.Models.Hadi
{
    public class SearchData : ISearchData
    {
        public Dictionary<Guid, NodeData> NodeSet { get; } = new Dictionary<Guid, NodeData>();
        private readonly SortedSet<NodeData> _queue = new SortedSet<NodeData>();

        public SearchData(NodeData source , NodeData target)
        {
            _queue.Add(source);
            NodeSet[source.Id] = source;
            NodeSet[target.Id] = target;
        }

        public SearchData(NodeData source)
        {
            _queue.Add(source);
            NodeSet[source.Id] = source;
        }


        public void AddToNodeSet(NodeData node) => NodeSet[node.Id] = node;

        public void AddToQueue(NodeData node) => _queue.Add(node);

        public NodeData GetNode(Guid id) => NodeSet?[id];

        public bool IsEmpty() => _queue.Count == 0;

        public NodeData PopBestCurrentNode()
        {
            var first = _queue.First();
            _queue.Remove(first);
            return first;
        }
    }
}
