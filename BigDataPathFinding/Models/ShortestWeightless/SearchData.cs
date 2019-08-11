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
        public HashSet<Guid> currentNodes { get; private set; } = new HashSet<Guid>();



        public void AddToNodeSet(NodeData node) => NodeSet[node.Id] = node;

        public NodeData GetNode(Guid id) => !NodeSet.ContainsKey(id) ? null : NodeSet[id];

        public void AddToCurrentNodes(Guid id)
        {
            currentNodes.Add(id);
        }

        public void ClearCurrentNodes(HashSet<Guid> edges)
        {
            currentNodes = new HashSet<Guid>(edges);
        }

    }
}
