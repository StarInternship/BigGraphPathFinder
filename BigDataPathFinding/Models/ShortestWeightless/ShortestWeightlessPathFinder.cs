using System;
using System.Collections.Generic;
using BigDataPathFinding.Models.Interfaces;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    public class ShortestWeightlessPathFinder : AbstractPathFinder
    {
        private bool _reachedToTarget = false;

        private int _forwardDepth = 0;
        private int _backwardDepth = 0;

        private readonly ShortestWeightlessSearchData
            _shortestWeightlessSearchData = new ShortestWeightlessSearchData();

        public ShortestWeightlessPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed,
            int maxDistance, int minDistance)
            : base(metadata, sourceId, targetId, directed, maxDistance, minDistance)
        {
        }

        public override void FindPath()
        {
            _shortestWeightlessSearchData.AddToNodeSet(new NodeData(SourceId, _forwardDepth, Seen.Forward));
            _shortestWeightlessSearchData.AddToCurrentForwardNodes(SourceId);

            _shortestWeightlessSearchData.AddToNodeSet(new NodeData(TargetId, _backwardDepth, Seen.Backward));
            _shortestWeightlessSearchData.AddToCurrentBackwardNodes(TargetId);

            while (!_reachedToTarget)
            {
                if (_shortestWeightlessSearchData.CurrentBackwardNodes.Count == 0 &&
                    _shortestWeightlessSearchData.CurrentForwardNodes.Count == 0)
                    return;

                ExpandForward();

                if (_reachedToTarget)
                    break;

                ExpandBackward();
            }

            CalculatePathDistance();
        }

        private void CalculatePathDistance()
        {
            _shortestWeightlessSearchData.PathDistance = _forwardDepth + _backwardDepth;
        }

        private void ExpandBackward()
        {
            if (_shortestWeightlessSearchData.CurrentBackwardNodes.Count == 0 ||
                _backwardDepth + _forwardDepth >= MaxDistance)
            {
                _shortestWeightlessSearchData.ClearCurrentBackwardNodes();
                return;
            }

            _backwardDepth++;
            var newLayer = new HashSet<Guid>();

            foreach (var edges in Metadata.GetInputAdjacent(_shortestWeightlessSearchData.CurrentBackwardNodes))
            {
                foreach (var edge in edges)
                {
                    VisitBackwardEdge(_backwardDepth, newLayer, edge.SourceId, edge.TargetId, edge.Weight);
                }
            }


            if (!Directed)
            {
                foreach (var edges in Metadata.GetOutputAdjacent(_shortestWeightlessSearchData.CurrentBackwardNodes))
                {
                    foreach (var edge in edges)
                    {
                        VisitBackwardEdge(_backwardDepth, newLayer, edge.TargetId, edge.SourceId, edge.Weight);
                    }
                }
            }

            _shortestWeightlessSearchData.UpdateCurrentBackwardNodes(newLayer);
        }

        private void ExpandForward()
        {
            if (_shortestWeightlessSearchData.CurrentForwardNodes.Count == 0 ||
                _forwardDepth + _backwardDepth >= MaxDistance)
            {
                _shortestWeightlessSearchData.ClearCurrentForwardNodes();
                return;
            }

            _forwardDepth++;
            var newLayer = new HashSet<Guid>();

            foreach (var edges in Metadata.GetOutputAdjacent(_shortestWeightlessSearchData.CurrentForwardNodes))
            {
                foreach (var edge in edges)
                {
                    VisitForwardEdge(newLayer, edge.SourceId, edge.TargetId, edge.Weight);
                }
            }


            if (!Directed)
            {
                foreach (var edges in Metadata.GetInputAdjacent(_shortestWeightlessSearchData.CurrentForwardNodes))
                {
                    foreach (var edge in edges)
                    {
                        VisitForwardEdge(newLayer, edge.TargetId, edge.SourceId, edge.Weight);
                    }
                }
            }

            _shortestWeightlessSearchData.UpdateCurrentForwardNodes(newLayer);
        }

        private void VisitBackwardEdge(int backwardLayer, HashSet<Guid> nextLayerNodes, Guid sourceId, Guid targetId,
            double weight)
        {
            if (nextLayerNodes == null) throw new ArgumentNullException(nameof(nextLayerNodes));

            if (_shortestWeightlessSearchData.GetNode(sourceId) == null)
            {
                VisitBackwardNode(backwardLayer, nextLayerNodes, sourceId);
            }

            if (Math.Abs(_shortestWeightlessSearchData.GetNode(sourceId).Distance - backwardLayer) < 0.01)
            {
                _shortestWeightlessSearchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
            }

            if (_shortestWeightlessSearchData.GetNode(sourceId).Seen == Seen.Forward)
            {
                _shortestWeightlessSearchData.Joints.Add(sourceId);
                _shortestWeightlessSearchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
                _reachedToTarget = true;
            }
        }

        private void VisitBackwardNode(int backwardLayer, ISet<Guid> nextLayerNodes, Guid sourceId)
        {
            _shortestWeightlessSearchData.AddToNodeSet(new NodeData(sourceId, backwardLayer, Seen.Backward));
            nextLayerNodes.Add(sourceId);
        }

        private void VisitForwardEdge(ISet<Guid> nextLayerNodes, Guid sourceId, Guid targetId, double weight)
        {
            if (_shortestWeightlessSearchData.GetNode(targetId) == null)
            {
                VisitNewNode(_forwardDepth, nextLayerNodes, targetId);
            }

            if (Math.Abs(_shortestWeightlessSearchData.GetNode(targetId).Distance - _forwardDepth) < 0.01)
            {
                _shortestWeightlessSearchData.GetNode(targetId).AddForwardAdjacent(new Adjacent(sourceId, weight));
            }

            if (_shortestWeightlessSearchData.GetNode(targetId).Seen == Seen.Backward)
            {
                _shortestWeightlessSearchData.AddJoint(targetId);
                _shortestWeightlessSearchData.GetNode(targetId).AddForwardAdjacent(new Adjacent(sourceId, weight));
                _reachedToTarget = true;
            }
        }

        private void VisitNewNode(int layer, ISet<Guid> nextLayerNodes, Guid targetId)
        {
            _shortestWeightlessSearchData.AddToNodeSet(new NodeData(targetId, layer, Seen.Forward));
            nextLayerNodes.Add(targetId);
        }


        public override ISearchData GetSearchData() => _shortestWeightlessSearchData;
    }
}