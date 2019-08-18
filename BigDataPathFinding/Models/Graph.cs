using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public class Graph
    {
        private readonly HashSet<Guid> _exploredNodes = new HashSet<Guid>();
        private Dictionary<Guid, NodeInfo> Nodes { get; } = new Dictionary<Guid, NodeInfo>();
        public HashSet<Edge> Edges { get; } = new HashSet<Edge>();

        public void AddEdge(Guid source, Guid target, double weight)
        {
            AddEdge(GetNode(source), GetNode(target), weight);
        }

        private NodeInfo GetNode(Guid id)
        {
            return !ContainsNode(id) ? null : Nodes[id];
        }

        public bool ContainsNode(Guid id)
        {
            return Nodes.ContainsKey(id);
        }

        public void AddNode(NodeInfo nodeInfo)
        {
            Nodes[nodeInfo.Id] = nodeInfo;
        }

        private void AddEdge(NodeInfo source, NodeInfo target, double weight)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Edges.Add(new Edge(source.Id, target.Id, weight));
        }

        public bool Explored(Guid id)
        {
            return ContainsNode(id) && _exploredNodes.Contains(id);
        }

        public void Explore(Guid id)
        {
            _exploredNodes.Add(id);
        }

        public override bool Equals(object obj)
        {
            return this == obj || obj is Graph graph && Edges.SetEquals(graph.Edges);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}