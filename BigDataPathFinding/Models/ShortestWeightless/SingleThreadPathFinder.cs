using System;
using System.Collections.Generic;
using BigDataPathFinding.Models.Interfaces;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    public class SingleThreadPathFinder : AbstractPathFinder
    {
        private bool _reachedToTarget = false;

        private int _forwardDepth = 0;
        private int _backwardDepth = 0;

        private readonly SearchData
            _searchData = new SearchData();

        public SingleThreadPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed,
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
                    VisitBackwardEdge(_backwardDepth, newLayer, edge);
                }
            }


            if (!Directed)
            {
                foreach (var edges in Metadata.GetOutputAdjacent(_searchData.CurrentBackwardNodes))
                {
                    foreach (var edge in edges)
                    {
                        VisitBackwardEdge(_backwardDepth, newLayer, edge);
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
                    VisitForwardEdge(newLayer, edge);
                }
            }


            if (!Directed)
            {
                foreach (var edges in Metadata.GetInputAdjacent(_searchData.CurrentForwardNodes))
                {
                    foreach (var edge in edges)
                    {
                        VisitForwardEdge(newLayer, edge);
                    }
                }
            }

            _searchData.UpdateCurrentForwardNodes(newLayer);
        }

        private void VisitBackwardEdge(int backwardLayer, HashSet<Guid> nextLayerNodes, Edge edge)
        {
            if (nextLayerNodes == null) throw new ArgumentNullException(nameof(nextLayerNodes));

            if (_searchData.GetNode(edge.SourceId) == null)
            {
                VisitBackwardNode(backwardLayer, nextLayerNodes, edge.SourceId);
            }

            if (Math.Abs(_searchData.GetNode(edge.SourceId).Distance - backwardLayer) < 0.01)
            {
                _searchData.GetNode(edge.SourceId).AddBackwardAdjacent(new Adjacent(edge.TargetId, edge.Weight));
            }

            if (_searchData.GetNode(edge.SourceId).Seen == Seen.Forward)
            {
                _searchData.Joints.Add(edge.SourceId);
                _searchData.GetNode(edge.SourceId).AddBackwardAdjacent(new Adjacent(edge.TargetId, edge.Weight));
                _reachedToTarget = true;
            }
        }

        private void VisitBackwardNode(int backwardLayer, ISet<Guid> nextLayerNodes, Guid sourceId)
        {
            _searchData.AddToNodeSet(new NodeData(sourceId, backwardLayer, Seen.Backward));
            nextLayerNodes.Add(sourceId);
        }

        private void VisitForwardEdge(ISet<Guid> nextLayerNodes, Edge edge)
        {
            if (_searchData.GetNode(edge.TargetId) == null)
            {
                VisitNewNode(_forwardDepth, nextLayerNodes, edge.TargetId);
            }

            if (Math.Abs(_searchData.GetNode(edge.TargetId).Distance - _forwardDepth) < 0.01)
            {
                _searchData.GetNode(edge.TargetId).AddForwardAdjacent(new Adjacent(edge.SourceId, edge.Weight));
            }

            if (_searchData.GetNode(edge.TargetId).Seen == Seen.Backward)
            {
                _searchData.AddJoint(edge.TargetId);
                _searchData.GetNode(edge.TargetId).AddForwardAdjacent(new Adjacent(edge.SourceId, edge.Weight));
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