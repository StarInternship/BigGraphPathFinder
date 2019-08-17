using System;
using System.Collections.Generic;
using BigDataPathFinding.Models.Interfaces;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    public class ShortestWeightlessPathFinder : AbstractPathFinder
    {
        private bool _reachedToTarget = false;

        private int _forwardLayer = 0;
        private int _backwardLayer = 0;

        private readonly ShortestWeightlessSearchData
            _shortestWeightlessSearchData = new ShortestWeightlessSearchData();

        public ShortestWeightlessPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed,
            int maxDistance, int minDistance)
            : base(metadata, sourceId, targetId, directed, maxDistance, minDistance)
        {
            _shortestWeightlessSearchData.MaxForwardDistance = (maxDistance + 1) / 2;
            _shortestWeightlessSearchData.MaxBackwardDistance = (maxDistance) / 2;
        }

        public override void FindPath()
        {
            _shortestWeightlessSearchData.AddToNodeSet(new NodeData(SourceId, _forwardLayer, Seen.Forward));
            _shortestWeightlessSearchData.AddToCurrentForwardNodes(SourceId);

            _shortestWeightlessSearchData.AddToNodeSet(new NodeData(TargetId, _backwardLayer, Seen.Backward));
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
            _backwardLayer = Math.Min(_backwardLayer, _shortestWeightlessSearchData.MaxBackwardDistance);
            _forwardLayer = Math.Min(_forwardLayer, _shortestWeightlessSearchData.MaxForwardDistance);
            _shortestWeightlessSearchData.PathDistance = _forwardLayer + _backwardLayer;
        }

        private void ExpandBackward()
        {
            _backwardLayer++;

            if (_shortestWeightlessSearchData.CurrentBackwardNodes.Count == 0 ||
                _backwardLayer > _shortestWeightlessSearchData.MaxBackwardDistance)
            {
                _shortestWeightlessSearchData.ClearCurrentBackwardNodes();
                return;
            }

            var layerNodes = new HashSet<Guid>();

            foreach (var edges in Metadata.GetInputAdjacent(_shortestWeightlessSearchData.CurrentBackwardNodes))
            {
                foreach (var edge in edges)
                {
                    VisitBackwardEdge(_backwardLayer, layerNodes, edge.SourceId, edge.TargetId, edge.Weight);
                }
            }


            if (!Directed)
            {
                foreach (var edges in Metadata.GetOutputAdjacent(_shortestWeightlessSearchData.CurrentBackwardNodes))
                {
                    foreach (var edge in edges)
                    {
                        VisitBackwardEdge(_backwardLayer, layerNodes, edge.TargetId, edge.SourceId, edge.Weight);
                    }
                }
            }

            _shortestWeightlessSearchData.UpdateCurrentBackwardNodes(layerNodes);
        }

        private void ExpandForward()
        {
            _forwardLayer++;
            if (_shortestWeightlessSearchData.CurrentForwardNodes.Count == 0 ||
                _forwardLayer > _shortestWeightlessSearchData.MaxForwardDistance)
            {
                _shortestWeightlessSearchData.ClearCurrentForwardNodes();
                return;
            }

            var layerNodes = new HashSet<Guid>();

            foreach (var edges in Metadata.GetOutputAdjacent(_shortestWeightlessSearchData.CurrentForwardNodes))
            {
                foreach (var edge in edges)
                {
                    VisitForwardEdge(layerNodes, edge.SourceId, edge.TargetId, edge.Weight);
                }
            }


            if (!Directed)
            {
                foreach (var edges in Metadata.GetInputAdjacent(_shortestWeightlessSearchData.CurrentForwardNodes))
                {
                    foreach (var edge in edges)
                    {
                        VisitForwardEdge(layerNodes, edge.TargetId, edge.SourceId, edge.Weight);
                    }
                }
            }

            _shortestWeightlessSearchData.UpdateCurrentForwardNodes(layerNodes);
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
                VisitNewNode(_forwardLayer, nextLayerNodes, targetId);
            }

            if (Math.Abs(_shortestWeightlessSearchData.GetNode(targetId).Distance - _forwardLayer) < 0.01)
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