using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    public class WeightlessPathFinder : AbstractPathFinder
    {

        private readonly SearchData searchData = new SearchData();

        public WeightlessPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed)
            : base(metadata, sourceId, targetId, directed)
        {
        }

        public override void FindPath()
        {
            int leyer = 0;
            searchData.AddToNodeSet(new NodeData(SourceId, leyer));
            searchData.AddToCurrentNodes(SourceId);
            bool reachedToTarget = false;

            while (!reachedToTarget)
            {
                HashSet<Guid> nextLeyerNodes = new HashSet<Guid>();
                leyer++;

                foreach (IEnumerable<Edge> edges in Metadata.GetOutputAdjacent(searchData.CurrentNodes))
                {
                    foreach (Edge edge in edges)
                    {
                        if (VisiteEdge(leyer, nextLeyerNodes, edge.SourceId, edge.TargetId, edge.Weight))
                        {
                            reachedToTarget = true;
                        }
                    }
                }

                if (!Directed)
                {
                    foreach (IEnumerable<Edge> edges in Metadata.GetInputAdjacent(searchData.CurrentNodes))
                    {
                        foreach (Edge edge in edges)
                        {
                            if (VisiteEdge(leyer, nextLeyerNodes, edge.TargetId, edge.SourceId, edge.Weight))
                            {
                                reachedToTarget = true;
                            }
                        }
                    }
                }
                
                searchData.ClearCurrentNodes(nextLeyerNodes);
            }
        }

        private bool VisiteEdge(int leyer, HashSet<Guid> nextLeyerNodes, Guid sourceId, Guid targetId, double weight)
        {
            if (searchData.GetNode(targetId) == null)
            {
                VisiteNewNode(leyer, nextLeyerNodes, sourceId, targetId, weight);
            }

            else if (searchData.GetNode(targetId).Distance == leyer)
            {
                searchData.GetNode(targetId).AddAdjacent(new Adjacent(sourceId, weight));
            }

            return TargetId == targetId;
        }

        private void VisiteNewNode(int leyer, HashSet<Guid> nextLeyerNodes, Guid sourceId, Guid targetId, double weight)
        {
            var newNode = new NodeData(targetId, leyer);
            searchData.AddToNodeSet(newNode);
            newNode.AddAdjacent(new Adjacent(sourceId, weight));
            nextLeyerNodes.Add(newNode.Id);
        }

        public override Dictionary<Guid, NodeData> GetResultNodeSet()
        {
            return searchData.NodeSet;
        }
    }
}
