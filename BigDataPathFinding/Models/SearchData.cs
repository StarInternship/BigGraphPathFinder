using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    internal class SearchData : ISearchData
    {
        private Dictionary<Guid, NodeData> NodeSet { get; } = new Dictionary<Guid, Models.NodeData>();
        public HashSet<Guid> CurrentBackwardNodes { get; private set; } = new HashSet<Guid>();
        public HashSet<Guid> CurrentForwardNodes { get; private set; } = new HashSet<Guid>();
        public int PathDistance { get; set; }
        public HashSet<Guid> Joints { get; } = new HashSet<Guid>();
        public int MaxForwardDistance { get; set; }
        public int MaxBackwardDistance { get; set; }

        public void AddToNodeSet(NodeData node) => NodeSet[node.Id] = node;

        public NodeData GetNode(Guid id) => !NodeSet.ContainsKey(id) ? null : (NodeData)NodeSet[id];

        public void AddToCurrentForwardNodes(Guid id) => CurrentForwardNodes.Add(id);

        public void UpdateCurrentForwardNodes(HashSet<Guid> edges) => CurrentForwardNodes = edges;

        public void ClearCurrentForwardNodes() => CurrentForwardNodes = new HashSet<Guid>();

        public void ClearCurrentBackwardNodes() => CurrentBackwardNodes = new HashSet<Guid>();

        public void AddToCurrentBackwardNodes(Guid id) => CurrentBackwardNodes.Add(id);

        public void UpdateCurrentBackwardNodes(HashSet<Guid> edges) => CurrentBackwardNodes = edges;

        public Dictionary<Guid, Models.NodeData> GetResultNodeSet() => NodeSet;

        public HashSet<Guid> GetJoints() => Joints;

        public int GetPathDistance() => PathDistance;

        public void AddJoint(Guid id) => Joints.Add(id);
    }
}
