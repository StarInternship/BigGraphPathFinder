using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    public class WeightlessPathFinder : AbstractPathFinder
    {

        private bool reachedToTarget = false;

        private int forwardLeyer = 0;
        private int backwardLeyer = 0;

        private readonly SearchData searchData = new SearchData();

        public WeightlessPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed, int maxDistance, int minDistance)
            : base(metadata, sourceId, targetId, directed, maxDistance, minDistance)
        {
            searchData.MaxForwardDistance = (maxDistance + 1) / 2;
            searchData.MaxBackwardDistance = (maxDistance) / 2;
        }

        public override void FindPath()
        {
            searchData.AddToNodeSet(new NodeData(SourceId, forwardLeyer, Seen.Forward));
            searchData.AddToCurrentForwardNodes(SourceId);

            searchData.AddToNodeSet(new NodeData(TargetId, backwardLeyer, Seen.Backward));
            searchData.AddToCurrentBackwardNodes(TargetId);

            while (!reachedToTarget)
            {
                if (searchData.CurrentBackwardNodes.Count == 0 && searchData.CurrentForwardNodes.Count == 0)
                    return;

                ExpandForward();

                if (reachedToTarget)
                    break;

                ExpandBackward();

            }
            CalculatePathDistance();
        }

        private void CalculatePathDistance()
        {
            backwardLeyer = Math.Min(backwardLeyer, searchData.MaxBackwardDistance);
            forwardLeyer = Math.Min(forwardLeyer, searchData.MaxForwardDistance);
            searchData.PathDistance = forwardLeyer + backwardLeyer;
        }

        private void ExpandBackward()
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

        private void ExpandForward()
        {
            forwardLeyer++;
            if (searchData.CurrentForwardNodes.Count == 0 || forwardLeyer > searchData.MaxForwardDistance)
            {
                searchData.ClearCurrentForwardNodes();
                return;
            }

            var nextLeyerNodes = new HashSet<Guid>();

            foreach (var edges in Metadata.GetOutputAdjacent(searchData.CurrentForwardNodes))
            {
                foreach (var edge in edges)
                {
                    VisiteForwardEdge(nextLeyerNodes, edge.SourceId, edge.TargetId, edge.Weight);
                }
            }


            if (!Directed)
            {
                foreach (IEnumerable<Edge> edges in Metadata.GetInputAdjacent(searchData.CurrentForwardNodes))
                {
                    foreach (Edge edge in edges)
                    {
                        VisiteForwardEdge(nextLeyerNodes, edge.TargetId, edge.SourceId, edge.Weight);
                    }
                }
            }

            searchData.UpdateCurrentForwardNodes(nextLeyerNodes);
        }

        private void VisiteBackwardEdge(int backwardLeyer, HashSet<Guid> nextLeyerNodes, Guid sourceId, Guid targetId, double weight)
        {

            if (searchData.GetNode(sourceId) == null)
            {
                VisitBackwardNode(backwardLeyer, nextLeyerNodes, sourceId);
            }

            if (searchData.GetNode(sourceId).Distance == backwardLeyer)
            {
                searchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
            }

            if (searchData.GetNode(sourceId).Seen == Seen.Forward)
            {
                searchData.Joints.Add(sourceId);
                searchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
                reachedToTarget = true;
            }

        }

        private void VisitBackwardNode(int backwardLeyer, HashSet<Guid> nextLeyerNodes, Guid sourceId)
        {
            searchData.AddToNodeSet(new NodeData(sourceId, backwardLeyer, Seen.Backward));
            nextLeyerNodes.Add(sourceId);
        }

        private void VisiteForwardEdge(HashSet<Guid> nextLeyerNodes, Guid sourceId, Guid targetId, double weight)
        {
            if (searchData.GetNode(targetId) == null)
            {
                VisiteNewNode(forwardLeyer, nextLeyerNodes, targetId);
            }

            if (searchData.GetNode(targetId).Distance == forwardLeyer)
            {
                searchData.GetNode(targetId).AddForwardAdjacent(new Adjacent(sourceId, weight));
            }

            if (searchData.GetNode(targetId).Seen == Seen.Backward)
            {
                searchData.AddJoint(targetId);
                searchData.GetNode(targetId).AddForwardAdjacent(new Adjacent(sourceId, weight));
                reachedToTarget = true;
            }
        }

        private void VisiteNewNode(int leyer, HashSet<Guid> nextLeyerNodes, Guid targetId)
        {
            searchData.AddToNodeSet(new NodeData(targetId, leyer, Seen.Forward));
            nextLeyerNodes.Add(targetId);
        }


        public override ISearchData GetSearchData() => searchData;
    }
}
