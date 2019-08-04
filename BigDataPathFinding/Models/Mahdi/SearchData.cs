using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataPathFinding.Models.Mahdi
{
    class SearchData
    {
        private readonly Dictionary<Guid, NodeData> _candidatesDictionary1 = new Dictionary<Guid, NodeData>();

        private readonly SortedDictionary<NodeData, Guid> _candidatesDictionary2 =
            new SortedDictionary<NodeData, Guid>();

        private readonly Dictionary<Guid, NodeData> _discoveries = new Dictionary<Guid, NodeData>();

        public void MoveToDiscovery(Guid node)
        {
            _discoveries.Add(node, _candidatesDictionary1[node]);
        }

        public void AddCandidate(Guid node, NodeData nodeData)
        {
            _candidatesDictionary1.Add(node, nodeData);
            _candidatesDictionary2.Add(nodeData, node);
        }

        public bool ContainsDiscovery(Guid node)
        {
            return _discoveries.ContainsKey(node);
        }

        public bool ContainsCandidate(Guid node)
        {
            return _candidatesDictionary1.ContainsKey(node);
        }

        public Guid GetBestCandidate()
        {
            return _candidatesDictionary2.First().Value;
        }

        public NodeData GetBestCandidateData()
        {
            return _candidatesDictionary2.First().Key;
        }

        public NodeData GetDiscoveryData(Guid node)
        {
            return _discoveries[node];
        }

        public NodeData GetCandidateData(Guid node)
        {
            return _candidatesDictionary1[node];
        }

        public void RemoveBestCandidate()
        {
            var vertex = _candidatesDictionary2.First().Value;
            var bestAccesses = _candidatesDictionary2.First().Key;
            _candidatesDictionary1.Remove(vertex);
            _candidatesDictionary2.Remove(bestAccesses);
        }

        public bool HasCandidate()
        {
            return _candidatesDictionary1.Count == 0;
        }

        public void UpdateCandidate(Guid node, NodeData data)
        {
            var preData = _candidatesDictionary1[node];
            _candidatesDictionary1[node] = data;
            _candidatesDictionary2.Remove(preData);
            _candidatesDictionary2.Add(data, node);
        }

        public Dictionary<Guid, NodeData> GetDiscoveries()
        {
            return _discoveries;
        }
    }
}