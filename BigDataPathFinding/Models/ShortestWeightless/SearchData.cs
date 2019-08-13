using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    class SearchData
    {
        public Dictionary<Guid, NodeData> NodeSet { get; } = new Dictionary<Guid, NodeData>();
        public HashSet<Guid> CurrentBackwardNodes { get; private set; } = new HashSet<Guid>();
        public HashSet<Guid> CurrentForwardNodes { get; private set; } = new HashSet<Guid>();

        public void AddToNodeSet(WeightlessNodeData node) => NodeSet[node.Id] = node;

        public WeightlessNodeData GetNode(Guid id) => !NodeSet.ContainsKey(id) ? null : (WeightlessNodeData)NodeSet[id];

        public void AddToCurrentForwardNodes(Guid id) => CurrentForwardNodes.Add(id);

        public void ClearCurrentForwardNodes(HashSet<Guid> edges) => CurrentForwardNodes = edges;


        public void AddToCurrentBackwardNodes(Guid id) => CurrentBackwardNodes.Add(id);

        public void ClearCurrentBackwardNodes(HashSet<Guid> edges) => CurrentBackwardNodes = edges;

    }
}
