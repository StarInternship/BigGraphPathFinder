using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public class Graph
    {
        public Dictionary<Guid, ResultNode> Nodes { get; } = new Dictionary<Guid, ResultNode>();
        public HashSet<Edge> Edges { get; } = new HashSet<Edge>();

        public void AddEdge(Guid source, Guid target, double weight) => AddEdge(GetNode(source), GetNode(target), weight);

        private ResultNode GetNode(Guid id) => Nodes?[id];

        public bool ContainsNode(Guid id) => Nodes.ContainsKey(id);

        public void AddNode(ResultNode node) => Nodes[node.Id] = node;

        public void AddEdge(ResultNode source, ResultNode target, double weight) => Edges.Add(new Edge(source.Id, target.Id, weight));

        public bool Explored(Guid id) => ContainsNode(id) && Nodes[id].Explored;

        public void Explore(Guid id) => GetNode(id).Explored = true;
    }
}