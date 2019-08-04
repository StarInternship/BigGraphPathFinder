using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataPathFinding.Models
{
    public class Graph
    {
        private readonly HashSet<Node> nodes = new HashSet<Node>();
        private readonly HashSet<Edge> edges = new HashSet<Edge>();

        public void AddEdge(Node source, Node target, double weight)
        {
            nodes.Add(source);
            nodes.Add(target);
            edges.Add(new Edge(source.Id, target.Id, weight));
        }
    }
}
