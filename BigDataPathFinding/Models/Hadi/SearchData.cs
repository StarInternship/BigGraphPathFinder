using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BigDataPathFinding.Models.Hadi
{
    
    public class SearchData : ISearchData
    {

        private readonly SortedDictionary<double, HashSet<NodeData>> _queue = new SortedDictionary<double, HashSet<NodeData>>();
        public Dictionary<Guid, NodeData> NodeSet { get; } = new Dictionary<Guid, NodeData>();


        public SearchData(NodeData source)
        {
            AddToQueue(source);
            NodeSet[source.Id] = source;
        }

        public void AddToQueue(NodeData node)
        {
            if (_queue.ContainsKey(node.Distance))
            {
                _queue[node.Distance].Add(node);
            }
            else
            {
                _queue[node.Distance] = new HashSet<NodeData> { node };
            }
        }

        public NodeData PopBestCurrentNode()
        {
            var first = _queue.First();
            NodeData firstNode = first.Value.First();
            first.Value.Remove(firstNode);

            if (first.Value.Count == 0)
            {
                _queue.Remove(first.Key);
            }
            return firstNode;
        }




        public void AddToNodeSet(NodeData node)
        {
            NodeSet[node.Id] = node;
        }

        public NodeData GetNode(Guid id)
        {
            return !NodeSet.ContainsKey(id) ? null : NodeSet[id];
        }

        public bool IsEmpty() => _queue.Count == 0;
    }
}