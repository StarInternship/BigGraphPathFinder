using System;
using System.Collections.Generic;
using BigDataPathFinding.Models.Interfaces;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    public class PathFinder : AbstractPathFinder
    {
        private bool _reachedToTarget = false;

        private int _forwardDepth = 0;
        private int _backwardDepth = 0;

        private readonly SearchData
            _searchData = new SearchData();

        public PathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed,
            int maxDistance, int minDistance)
            : base(metadata, sourceId, targetId, directed, maxDistance, minDistance)
        {
        }

        public override void FindPath()
        {
            _searchData.AddToNodeSet(new NodeData(SourceId, _forwardDepth, Seen.Forward));
            _searchData.AddToCurrentForwardNodes(SourceId);

            _searchData.AddToNodeSet(new NodeData(TargetId, _backwardDepth, Seen.Backward));
            _searchData.AddToCurrentBackwardNodes(TargetId);

            while (!_reachedToTarget)
            {
                if (_searchData.CurrentBackwardNodes.Count == 0 &&
                    _searchData.CurrentForwardNodes.Count == 0)
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
            _searchData.PathDistance = _forwardDepth + _backwardDepth;
        }

        private void ExpandBackward()
        {
            if (_searchData.CurrentBackwardNodes.Count == 0 ||
                _backwardDepth + _forwardDepth >= MaxDistance)
            {
                _searchData.ClearCurrentBackwardNodes();
                return;
            }

            _backwardDepth++;
            var newLayer = new HashSet<Guid>();

            foreach (var edges in Metadata.GetInputAdjacent(_searchData.CurrentBackwardNodes))
            {
                foreach (var edge in edges)
                {
                    VisitBackwardEdge(_backwardDepth, newLayer, edge.SourceId, edge.TargetId, edge.Weight);
                }
            }


            if (!Directed)
            {
                foreach (var edges in Metadata.GetOutputAdjacent(_searchData.CurrentBackwardNodes))
                {
                    foreach (var edge in edges)
                    {
                        VisitBackwardEdge(_backwardDepth, newLayer, edge.TargetId, edge.SourceId, edge.Weight);
                    }
                }
            }

            _searchData.UpdateCurrentBackwardNodes(newLayer);
        }

        private void ExpandForward()
        {
            if (_searchData.CurrentForwardNodes.Count == 0 ||
                _forwardDepth + _backwardDepth >= MaxDistance)
            {
                _searchData.ClearCurrentForwardNodes();
                return;
            }

            _forwardDepth++;
            var newLayer = new HashSet<Guid>();

            foreach (var edges in Metadata.GetOutputAdjacent(_searchData.CurrentForwardNodes))
            {
                foreach (var edge in edges)
                {
                    VisitForwardEdge(newLayer, edge.SourceId, edge.TargetId, edge.Weight);
                }
            }


            if (!Directed)
            {
                foreach (var edges in Metadata.GetInputAdjacent(_searchData.CurrentForwardNodes))
                {
                    foreach (var edge in edges)
                    {
                        VisitForwardEdge(newLayer, edge.TargetId, edge.SourceId, edge.Weight);
                    }
                }
            }

            _searchData.UpdateCurrentForwardNodes(newLayer);
        }

        private void VisitBackwardEdge(int backwardLayer, HashSet<Guid> nextLayerNodes, Guid sourceId, Guid targetId,
            double weight)
        {
            if (nextLayerNodes == null) throw new ArgumentNullException(nameof(nextLayerNodes));

            if (_searchData.GetNode(sourceId) == null)
            {
                VisitBackwardNode(backwardLayer, nextLayerNodes, sourceId);
            }

            if (Math.Abs(_searchData.GetNode(sourceId).Distance - backwardLayer) < 0.01)
            {
                _searchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
            }

            if (_searchData.GetNode(sourceId).Seen == Seen.Forward)
            {
                _searchData.Joints.Add(sourceId);
                _searchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
                _reachedToTarget = true;
            }
        }

        private void VisitBackwardNode(int backwardLayer, ISet<Guid> nextLayerNodes, Guid sourceId)
        {
            _searchData.AddToNodeSet(new NodeData(sourceId, backwardLayer, Seen.Backward));
            nextLayerNodes.Add(sourceId);
        }

        private void VisitForwardEdge(ISet<Guid> nextLayerNodes, Guid sourceId, Guid targetId, double weight)
        {
            if (_searchData.GetNode(targetId) == null)
            {
                VisitNewNode(_forwardDepth, nextLayerNodes, targetId);
            }

            if (Math.Abs(_searchData.GetNode(targetId).Distance - _forwardDepth) < 0.01)
            {
                _searchData.GetNode(targetId).AddForwardAdjacent(new Adjacent(sourceId, weight));
            }

            if (_searchData.GetNode(targetId).Seen == Seen.Backward)
            {
                _searchData.AddJoint(targetId);
                _searchData.GetNode(targetId).AddForwardAdjacent(new Adjacent(sourceId, weight));
                _reachedToTarget = true;
            }
        }

        private void VisitNewNode(int layer, ISet<Guid> nextLayerNodes, Guid targetId)
        {
            _searchData.AddToNodeSet(new NodeData(targetId, layer, Seen.Forward));
            nextLayerNodes.Add(targetId);
        }


        public override ISearchData GetSearchData() => _searchData;
    }
}