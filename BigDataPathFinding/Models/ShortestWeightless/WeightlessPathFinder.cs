using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    public class WeightlessPathFinder : AbstractPathFinder
    {

        private bool reachedToTarget = false;

        private int forwardLeyer = 0;
        private int backwardLeyer = 2000;

        private HashSet<Guid> joints = new HashSet<Guid>();

        private readonly SearchData searchData = new SearchData();

        public WeightlessPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed, int maxDistance)
            : base(metadata, sourceId, targetId, directed, maxDistance)
        {
        }

        public override void FindPath()
        {
            searchData.AddToNodeSet(new NodeData(SourceId, forwardLeyer));
            searchData.AddToCurrentForwardNodes(SourceId);

            searchData.AddToNodeSet(new NodeData(TargetId, backwardLeyer));
            searchData.AddToCurrentBackwardNodes(TargetId);

            while (!reachedToTarget)
            {
                GoForward();
                GoBackward();
            }
            AddPreviousAdjacentToBackwardNodes();
        }

        private void AddPreviousAdjacentToBackwardNodes()
        {
            throw new NotImplementedException();
        }

        private void GoBackward()
        {
            throw new NotImplementedException();
        }

        private void GoForward()
        {
            var nextLeyerNodes = new HashSet<Guid>();

            foreach (IEnumerable<Edge> edges in Metadata.GetOutputAdjacent(searchData.CurrentForwardNodes))
            {
                foreach (Edge edge in edges)
                {
                    if (VisiteForwardEdge(forwardLeyer, nextLeyerNodes, edge.SourceId, edge.TargetId, edge.Weight))
                    {
                        reachedToTarget = true;
                    }
                }
            }


            if (!Directed)
            {
                foreach (IEnumerable<Edge> edges in Metadata.GetInputAdjacent(searchData.CurrentForwardNodes))
                {
                    foreach (Edge edge in edges)
                    {
                        if (VisiteForwardEdge(forwardLeyer, nextLeyerNodes, edge.TargetId, edge.SourceId, edge.Weight))
                        {
                            reachedToTarget = true;
                        }
                    }
                }
            }

        }

        private bool VisiteForwardEdge(int leyer, HashSet<Guid> nextLeyerNodes, Guid sourceId, Guid targetId, double weight)
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

        public override Dictionary<Guid, NodeData> GetResultNodeSet() => searchData.NodeSet;
    }
}
