using System;
using System.Collections.Generic;
using System.Linq;
namespace BigDataPathFinding.Models.ShortestWeightless
{
    public class WeightlessPathFinder : AbstractPathFinder
    {

        private bool reachedToTarget = false;

        private int forwardLeyer = 0;
        private int backwardLeyer = 0;

        private readonly SearchData searchData = new SearchData();

        public WeightlessPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed, int maxDistance)
            : base(metadata, sourceId, targetId, directed, maxDistance)
        {
            searchData.MaxForwardDistance = maxDistance / 2;
            searchData.MaxBackwardDistance = (maxDistance + 1) / 2;
        }

        public override void FindPath()
        {
            searchData.AddToNodeSet(new WeightlessNodeData(SourceId, forwardLeyer, Seen.forward));
            searchData.AddToCurrentForwardNodes(SourceId);

            searchData.AddToNodeSet(new WeightlessNodeData(TargetId, backwardLeyer, Seen.backward));
            searchData.AddToCurrentBackwardNodes(TargetId);

            while (!reachedToTarget)
            {
                if (searchData.CurrentBackwardNodes.Count == 0 && searchData.CurrentForwardNodes.Count == 0)
                    return;

                GoForward();

                if (reachedToTarget)
                    break;

                GoBackward();

            }

            searchData.PathDistance = forwardLeyer + backwardLeyer;
        }

        private void GoBackward()
        {
            backwardLeyer++;

            if (searchData.CurrentBackwardNodes.Count == 0 || backwardLeyer > searchData.MaxBackwardDistance)
            {
                searchData.ClearCurrentBackwardNodes();
                return;
            }

            var nextLeyerNodes = new HashSet<Guid>();

            foreach (IEnumerable<Edge> edges in Metadata.GetInputAdjacent(searchData.CurrentBackwardNodes))
            {
                foreach (Edge edge in edges)
                {
                    VisiteBackwardEdge(backwardLeyer, nextLeyerNodes, edge.SourceId, edge.TargetId, edge.Weight);
                }
            }


            if (!Directed)
            {
                foreach (IEnumerable<Edge> edges in Metadata.GetOutputAdjacent(searchData.CurrentBackwardNodes))
                {
                    foreach (Edge edge in edges)
                    {
                        VisiteBackwardEdge(backwardLeyer, nextLeyerNodes, edge.TargetId, edge.SourceId, edge.Weight);
                    }
                }
            }

            searchData.UpdateCurrentBackwardNodes(nextLeyerNodes);
        }

        private void GoForward()
        {
            forwardLeyer++;
            if (searchData.CurrentForwardNodes.Count == 0 || forwardLeyer > searchData.MaxForwardDistance)
            {
                searchData.ClearCurrentForwardNodes();
                return;
            }

            var nextLeyerNodes = new HashSet<Guid>();

            foreach (IEnumerable<Edge> edges in Metadata.GetOutputAdjacent(searchData.CurrentForwardNodes))
            {
                foreach (Edge edge in edges)
                {
                    VisiteForwardEdge(forwardLeyer, nextLeyerNodes, edge.SourceId, edge.TargetId, edge.Weight);
                }
            }


            if (!Directed)
            {
                foreach (IEnumerable<Edge> edges in Metadata.GetInputAdjacent(searchData.CurrentForwardNodes))
                {
                    foreach (Edge edge in edges)
                    {
                        VisiteForwardEdge(forwardLeyer, nextLeyerNodes, edge.TargetId, edge.SourceId, edge.Weight);
                    }
                }
            }

            searchData.UpdateCurrentForwardNodes(nextLeyerNodes);
        }

        private void VisiteBackwardEdge(int leyer, HashSet<Guid> nextLeyerNodes, Guid sourceId, Guid targetId, double weight)
        {

            if (searchData.GetNode(sourceId) == null)
            {
                var newNode = new WeightlessNodeData(sourceId, leyer, Seen.backward);
                searchData.AddToNodeSet(newNode);
                nextLeyerNodes.Add(newNode.Id);
            }

            if (searchData.GetNode(sourceId).Distance == leyer)
            {
                searchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
            }

            if (searchData.GetNode(sourceId).Seen == Seen.forward)
            {
                searchData.Joints.Add(sourceId);
                searchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(sourceId, weight));
                reachedToTarget = true;
            }

        }

        private void VisiteForwardEdge(int leyer, HashSet<Guid> nextLeyerNodes, Guid sourceId, Guid targetId, double weight)
        {
            if (searchData.GetNode(targetId) == null)
            {
                VisiteNewNode(leyer, nextLeyerNodes, sourceId, targetId, weight);
            }

            if (searchData.GetNode(targetId).Distance == leyer)
            {
                searchData.GetNode(targetId).AddAdjacent(new Adjacent(sourceId, weight));
            }

            if (searchData.GetNode(targetId).Seen == Seen.backward)
            {
                searchData.Joints.Add(targetId);
                searchData.GetNode(targetId).AddAdjacent(new Adjacent(sourceId, weight));
                reachedToTarget = true;
            }
        }

        private void VisiteNewNode(int leyer, HashSet<Guid> nextLeyerNodes, Guid sourceId, Guid targetId, double weight)
        {
            var newNode = new WeightlessNodeData(targetId, leyer, Seen.forward);
            searchData.AddToNodeSet(newNode);
            nextLeyerNodes.Add(newNode.Id);
        }

        public override Dictionary<Guid, NodeData> GetResultNodeSet() => searchData.NodeSet;

        public override ISearchData GetSearchData() => searchData;
    }
}
