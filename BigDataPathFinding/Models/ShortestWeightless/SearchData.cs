using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    class SearchData : ISearchData
    {
        public Dictionary<Guid, NodeData> NodeSet { get; } = new Dictionary<Guid, NodeData>();
        public HashSet<Guid> CurrentBackwardNodes { get; private set; } = new HashSet<Guid>();
        public HashSet<Guid> CurrentForwardNodes { get; private set; } = new HashSet<Guid>();
        public int PathDistance { get; set; }
        public HashSet<Guid> Joints { get; } = new HashSet<Guid>();

        public int MaxForwardDistance { get; set; }
        public int MaxBackwardDistance { get; set; }
        public void AddToNodeSet(WeightlessNodeData node) => NodeSet[node.Id] = node;

        public WeightlessNodeData GetNode(Guid id) => !NodeSet.ContainsKey(id) ? null : (WeightlessNodeData)NodeSet[id];

        public void AddToCurrentForwardNodes(Guid id) => CurrentForwardNodes.Add(id);

        public void UpdateCurrentForwardNodes(HashSet<Guid> edges) => CurrentForwardNodes = edges;
        public void ClearCurrentForwardNodes() => CurrentForwardNodes = new HashSet<Guid>();
        public void ClearCurrentBackwardNodes() => CurrentForwardNodes = new HashSet<Guid>();


        public void AddToCurrentBackwardNodes(Guid id) => CurrentBackwardNodes.Add(id);

        public void UpdateCurrentBackwardNodes(HashSet<Guid> edges) => CurrentBackwardNodes = edges;


        public Dictionary<Guid, NodeData> GetResultNodeSet() => NodeSet;

        public HashSet<Guid> GetJoints() => Joints;

        public int GetPathDistance() => PathDistance;
    }
}
