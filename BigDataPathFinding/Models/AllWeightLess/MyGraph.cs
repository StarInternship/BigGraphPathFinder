using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BigDataPathFinding.Models.Elastic;
using BigDataPathFinding.Models.Interfaces;

namespace BigDataPathFinding.Models.AllWeightLess
{
    public class MyGraph
    {
        private readonly Dictionary<Guid, MyNode> _nodes = new Dictionary<Guid, MyNode>();

        public void AddEdge(Guid firstId, Guid secondId, double weight)
        {
            if (!_nodes.ContainsKey(firstId))
                _nodes.Add(firstId, new MyNode(firstId));
            if (!_nodes.ContainsKey(secondId))
                _nodes.Add(secondId, new MyNode(secondId));
            var firstNode = _nodes[firstId];
            var secondNode = _nodes[secondId];
            firstNode.AddOutput(secondNode, weight);
            secondNode.AddInput(firstNode, weight);
        }

        public MyNode GetNode(Guid id)
        {
            return _nodes[id];
        }

        public static MyGraph ReadGraph(Guid firstId, Guid secondId, int depth, IMetadata metadata, bool isDirected)
        {
            var graph = new MyGraph();
            var currentLevelNodes = new HashSet<Guid> {firstId};
            for (var i = 1; i <= depth / 2; i++)
            {
                var nextLevelNodes = new HashSet<Guid>();
                if (isDirected)
                {
                    foreach (var edges in metadata.GetOutputEdges(currentLevelNodes))
                    {
                        foreach (var edge in edges)
                        {
                            graph.AddEdge(edge.SourceId, edge.TargetId, edge.Weight);
                            if (!currentLevelNodes.Contains(edge.TargetId))
                                nextLevelNodes.Add(edge.TargetId);
                        }
                    }
                }
                else
                {
                    foreach (var edges in metadata.GetAllEdges(currentLevelNodes))
                    {
                        foreach (var edge in edges)
                        {
                            graph.AddEdge(edge.SourceId, edge.TargetId, edge.Weight);
                            if (!currentLevelNodes.Contains(edge.TargetId))
                            {
                                nextLevelNodes.Add(edge.TargetId);
                            }

                            if (!currentLevelNodes.Contains(edge.SourceId))
                            {
                                nextLevelNodes.Add(edge.SourceId);
                            }
                        }
                    }
                }

                currentLevelNodes.Clear();
                currentLevelNodes = nextLevelNodes;
            }

            currentLevelNodes.Clear();
            currentLevelNodes.Add(secondId);

            for (var i = 1; i <= depth - depth / 2; i++)
            {
                var nextLevelNodes = new HashSet<Guid>();
                if (isDirected)
                {
                    foreach (var edges in metadata.GetInputEdges(currentLevelNodes))
                    {
                        foreach (var edge in edges)
                        {
                            graph.AddEdge(edge.SourceId, edge.TargetId, edge.Weight);
                            if (!currentLevelNodes.Contains(edge.SourceId))
                                nextLevelNodes.Add(edge.SourceId);
                        }
                    }
                }
                else
                {
                    foreach (var edges in metadata.GetAllEdges(currentLevelNodes))
                    {
                        foreach (var edge in edges)
                        {
                            graph.AddEdge(edge.SourceId, edge.TargetId, edge.Weight);
                            if (!currentLevelNodes.Contains(edge.TargetId))
                            {
                                nextLevelNodes.Add(edge.TargetId);
                            }

                            if (!currentLevelNodes.Contains(edge.SourceId))
                            {
                                nextLevelNodes.Add(edge.SourceId);
                            }
                        }
                    }
                }

                currentLevelNodes.Clear();
                currentLevelNodes = nextLevelNodes;
            }

            return graph;
        }
    }
}