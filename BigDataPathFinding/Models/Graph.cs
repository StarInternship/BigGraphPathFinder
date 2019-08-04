using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public class Graph
    {
        public HashSet<Node> nodes { get; } = new HashSet<Node>();
        public HashSet<Edge> edges { get; } = new HashSet<Edge>();

        public void AddEdge(Node source, Node target, double weight)
        {
            nodes.Add(source);
            nodes.Add(target);
            edges.Add(new Edge(source.Id, target.Id, weight));
        }
    }
}