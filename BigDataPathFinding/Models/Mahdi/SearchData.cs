using System;
using System.Collections.Generic;
using System.Linq;

namespace BigDataPathFinding.Models.Mahdi
{
    internal class SearchData
    {
        private readonly Dictionary<Guid, NodeData> _candidatesDictionary1 = new Dictionary<Guid, NodeData>();

        private readonly SortedDictionary<NodeData, Guid> _candidatesDictionary2 =
            new SortedDictionary<NodeData, Guid>();

        private readonly Dictionary<Guid, NodeData> _discoveries = new Dictionary<Guid, NodeData>();

        public void MoveToDiscovery(Guid node, NodeData data)
        {
            _candidatesDictionary1.Remove(node);
            _candidatesDictionary2.Remove(data);
            _discoveries.Add(node, data);
        }

        public void AddCandidate(Guid node, NodeData nodeData)
        {
            _candidatesDictionary1.Add(node, nodeData);
            _candidatesDictionary2.Add(nodeData, node);
        }

        public bool ContainsDiscovery(Guid node) => _discoveries.ContainsKey(node);

        public bool ContainsCandidate(Guid node) => _candidatesDictionary1.ContainsKey(node);

        public Guid GetBestCandidate() => _candidatesDictionary2.First().Value;

        public NodeData GetBestCandidateData() => _candidatesDictionary2.First().Key;

        public NodeData GetDiscoveryData(Guid node) => _discoveries[node];

        public NodeData GetCandidateData(Guid node) => _candidatesDictionary1[node];

        public bool HasCandidate() => _candidatesDictionary1.Count > 0;

        public void UpdateCandidate(Guid node, NodeData data)
        {
            var preData = _candidatesDictionary1[node];
            _candidatesDictionary1[node] = data;
            _candidatesDictionary2.Remove(preData);
            _candidatesDictionary2.Add(data, node);
        }

        public Dictionary<Guid, NodeData> GetDiscoveries() => _discoveries;
    }
}