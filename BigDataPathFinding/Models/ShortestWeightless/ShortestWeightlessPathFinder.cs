using System;
using System.Collections.Generic;
using BigDataPathFinding.Models.Interfaces;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    public class ShortestWeightlessPathFinder : AbstractPathFinder
    {

        private bool reachedToTarget = false;

        private int forwardLeyer = 0;
        private int backwardLeyer = 0;

        private readonly ShortestWeightlessSearchData _shortestWeightlessSearchData = new ShortestWeightlessSearchData();

        public ShortestWeightlessPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed, int maxDistance, int minDistance)
            : base(metadata, sourceId, targetId, directed, maxDistance, minDistance)
        {
            _shortestWeightlessSearchData.MaxForwardDistance = (maxDistance + 1) / 2;
            _shortestWeightlessSearchData.MaxBackwardDistance = (maxDistance) / 2;
        }

        public override void FindPath()
        {
            _shortestWeightlessSearchData.AddToNodeSet(new NodeData(SourceId, forwardLeyer, Seen.Forward));
            _shortestWeightlessSearchData.AddToCurrentForwardNodes(SourceId);

            _shortestWeightlessSearchData.AddToNodeSet(new NodeData(TargetId, backwardLeyer, Seen.Backward));
            _shortestWeightlessSearchData.AddToCurrentBackwardNodes(TargetId);

            while (!reachedToTarget)
            {
                if (_shortestWeightlessSearchData.CurrentBackwardNodes.Count == 0 && _shortestWeightlessSearchData.CurrentForwardNodes.Count == 0)
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
            backwardLeyer = Math.Min(backwardLeyer, _shortestWeightlessSearchData.MaxBackwardDistance);
            forwardLeyer = Math.Min(forwardLeyer, _shortestWeightlessSearchData.MaxForwardDistance);
            _shortestWeightlessSearchData.PathDistance = forwardLeyer + backwardLeyer;
        }

        private void ExpandBackward()
        {
            backwardLeyer++;

            if (_shortestWeightlessSearchData.CurrentBackwardNodes.Count == 0 || backwardLeyer > _shortestWeightlessSearchData.MaxBackwardDistance)
            {
                _shortestWeightlessSearchData.ClearCurrentBackwardNodes();
                return;
            }

            var nextLeyerNodes = new HashSet<Guid>();

            foreach (IEnumerable<Edge> edges in Metadata.GetInputAdjacent(_shortestWeightlessSearchData.CurrentBackwardNodes))
            {
                foreach (Edge edge in edges)
                {
                    VisiteBackwardEdge(backwardLeyer, nextLeyerNodes, edge.SourceId, edge.TargetId, edge.Weight);
                }
            }


            if (!Directed)
            {
                foreach (IEnumerable<Edge> edges in Metadata.GetOutputAdjacent(_shortestWeightlessSearchData.CurrentBackwardNodes))
                {
                    foreach (Edge edge in edges)
                    {
                        VisiteBackwardEdge(backwardLeyer, nextLeyerNodes, edge.TargetId, edge.SourceId, edge.Weight);
                    }
                }
            }

            _shortestWeightlessSearchData.UpdateCurrentBackwardNodes(nextLeyerNodes);
        }

        private void ExpandForward()
        {
            forwardLeyer++;
            if (_shortestWeightlessSearchData.CurrentForwardNodes.Count == 0 || forwardLeyer > _shortestWeightlessSearchData.MaxForwardDistance)
            {
                _shortestWeightlessSearchData.ClearCurrentForwardNodes();
                return;
            }

            var nextLeyerNodes = new HashSet<Guid>();

            foreach (var edges in Metadata.GetOutputAdjacent(_shortestWeightlessSearchData.CurrentForwardNodes))
            {
                foreach (var edge in edges)
                {
                    VisiteForwardEdge(nextLeyerNodes, edge.SourceId, edge.TargetId, edge.Weight);
                }
            }


            if (!Directed)
            {
                foreach (IEnumerable<Edge> edges in Metadata.GetInputAdjacent(_shortestWeightlessSearchData.CurrentForwardNodes))
                {
                    foreach (Edge edge in edges)
                    {
                        VisiteForwardEdge(nextLeyerNodes, edge.TargetId, edge.SourceId, edge.Weight);
                    }
                }
            }

            _shortestWeightlessSearchData.UpdateCurrentForwardNodes(nextLeyerNodes);
        }

        private void VisiteBackwardEdge(int backwardLeyer, HashSet<Guid> nextLeyerNodes, Guid sourceId, Guid targetId, double weight)
        {

            if (_shortestWeightlessSearchData.GetNode(sourceId) == null)
            {
                VisitBackwardNode(backwardLeyer, nextLeyerNodes, sourceId);
            }

            if (_shortestWeightlessSearchData.GetNode(sourceId).Distance == backwardLeyer)
            {
                _shortestWeightlessSearchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
            }

            if (_shortestWeightlessSearchData.GetNode(sourceId).Seen == Seen.Forward)
            {
                _shortestWeightlessSearchData.Joints.Add(sourceId);
                _shortestWeightlessSearchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
                reachedToTarget = true;
            }

        }

        private void VisitBackwardNode(int backwardLeyer, HashSet<Guid> nextLeyerNodes, Guid sourceId)
        {
            _shortestWeightlessSearchData.AddToNodeSet(new NodeData(sourceId, backwardLeyer, Seen.Backward));
            nextLeyerNodes.Add(sourceId);
        }

        private void VisiteForwardEdge(HashSet<Guid> nextLeyerNodes, Guid sourceId, Guid targetId, double weight)
        {
            if (_shortestWeightlessSearchData.GetNode(targetId) == null)
            {
                VisiteNewNode(forwardLeyer, nextLeyerNodes, targetId);
            }

            if (_shortestWeightlessSearchData.GetNode(targetId).Distance == forwardLeyer)
            {
                _shortestWeightlessSearchData.GetNode(targetId).AddForwardAdjacent(new Adjacent(sourceId, weight));
            }

            if (_shortestWeightlessSearchData.GetNode(targetId).Seen == Seen.Backward)
            {
                _shortestWeightlessSearchData.AddJoint(targetId);
                _shortestWeightlessSearchData.GetNode(targetId).AddForwardAdjacent(new Adjacent(sourceId, weight));
                reachedToTarget = true;
            }
        }

        private void VisiteNewNode(int leyer, HashSet<Guid> nextLeyerNodes, Guid targetId)
        {
            _shortestWeightlessSearchData.AddToNodeSet(new NodeData(targetId, leyer, Seen.Forward));
            nextLeyerNodes.Add(targetId);
        }


        public override ISearchData GetSearchData() => _shortestWeightlessSearchData;
    }
}
