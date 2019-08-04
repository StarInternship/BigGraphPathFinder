using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public class Graph
    {
        public Dictionary<Guid, ResultNode> nodes { get; } = new Dictionary<Guid, ResultNode>();
        public HashSet<Edge> edges { get; } = new HashSet<Edge>();

        public void AddEdge(Guid source, Guid target, double weight)
        {
            AddEdge(GetNode(source), GetNode(target), weight);
        }

        private ResultNode GetNode(Guid id)
        {
            return nodes?[id];
        }

        public bool ContainsNode(Guid id)
        {
            return nodes.ContainsKey(id);
        }

        public void AddNode(ResultNode node)
        {
            nodes[node.Id] = node;
        }

        public void AddEdge(ResultNode source, ResultNode target, double weight)
        {
            edges.Add(new Edge(source.Id, target.Id, weight));
        }

        public bool Explored(Guid id)
        {
            return ContainsNode(id) && nodes[id].Explored;
        }

        public void Explore(Guid id)
        {
            GetNode(id).Explored = true;
        }
    }
}