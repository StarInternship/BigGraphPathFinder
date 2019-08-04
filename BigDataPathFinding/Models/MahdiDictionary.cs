using System;
using System.Collections.Generic;
using System.Linq;

namespace BigDataPathFinding.Models
{
    internal class MahdiDictionary
    {
        private readonly Dictionary<Guid, NodeData> _dictionary1 = new Dictionary<Guid, NodeData>();

        private readonly SortedDictionary<NodeData, Guid> _dictionary2 = new SortedDictionary<NodeData, Guid>();

        public void Add(Guid node, NodeData nodeData)
        {
            _dictionary1.Add(node, nodeData);
            _dictionary2.Add(nodeData, node);
        }

        public bool ContainsNode(Guid node)
        {
            return _dictionary1.ContainsKey(node);
        }

        public Guid GetFirstNode()
        {
            return _dictionary2.First().Value;
        }

        public NodeData GetFirstNodeData()
        {
            return _dictionary2.First().Key;
        }

        public NodeData GetNodeData(Guid node)
        {
            return _dictionary1[node];
        }

        public void RemoveFirst()
        {
            var vertex = _dictionary2.First().Value;
            var bestAccesses = _dictionary2.First().Key;
            _dictionary1.Remove(vertex);
            _dictionary2.Remove(bestAccesses);
        }

        public bool IsFree()
        {
            return _dictionary1.Count == 0;
        }

        public double GetDistance(Guid node)
        {
            return _dictionary1[node].Distance;
        }

        public void Update(Guid node, NodeData data)
        {
            var preData = _dictionary1[node];
            _dictionary1[node] = data;
            _dictionary2.Remove(preData);
            _dictionary2.Add(data, node);
        }
    }
}