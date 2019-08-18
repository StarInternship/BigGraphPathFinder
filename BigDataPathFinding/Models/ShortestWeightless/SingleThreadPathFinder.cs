using System;
using System.Collections.Generic;
using BigDataPathFinding.Models.Elastic;
using BigDataPathFinding.Models.Interfaces;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    public class SingleThreadPathFinder : AbstractPathFinder
    {
        private readonly SearchData _searchData = new SearchData();
        private int _backwardDepth;

        private int _forwardDepth;
        private bool _reachedToTarget;

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
                {
                    return;
                }

                ExpandForward();

                if (_reachedToTarget)
                {
                    break;
                }

                ExpandBackward();
            }

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

            var listOfEdges = Directed
                ? Metadata.GetInputEdges(_searchData.CurrentBackwardNodes)
                : Metadata.GetAllEdges(_searchData.CurrentBackwardNodes);

            if (Directed)
            {
                foreach (var edges in listOfEdges)
                {
                    foreach (var edge in edges)
                    {
                        VisitBackwardEdge(newLayer, edge);
                    }
                }
            }
            else
            {
                foreach (var edges in listOfEdges)
                {
                    foreach (var edge in edges)
                    {
                        if (_searchData.CurrentBackwardNodes.Contains(edge.TargetId))
                        {
                            VisitBackwardEdge(newLayer, edge);
                        }

                        if (_searchData.CurrentBackwardNodes.Contains(edge.SourceId))
                        {
                            VisitBackwardEdge(newLayer, edge.Reversed());
                        }
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

            var listOfEdges = Directed
                ? Metadata.GetOutputEdges(_searchData.CurrentForwardNodes)
                : Metadata.GetAllEdges(_searchData.CurrentForwardNodes);

            if (Directed)
            {
                foreach (var edges in listOfEdges)
                {
                    foreach (var edge in edges)
                    {
                        VisitForwardEdge(newLayer, edge);
                    }
                }
            }
            else
            {
                foreach (var edges in listOfEdges)
                {
                    foreach (var edge in edges)
                    {
                        if (_searchData.CurrentForwardNodes.Contains(edge.TargetId))
                        {
                            VisitForwardEdge(newLayer, edge.Reversed());
                        }

                        if (_searchData.CurrentForwardNodes.Contains(edge.SourceId))
                        {
                            VisitForwardEdge(newLayer, edge);
                        }
                    }
                }
            }

            _searchData.UpdateCurrentForwardNodes(newLayer);
        }

        private void VisitBackwardEdge(ISet<Guid> nextLayerNodes, Edge edge)
        {
            var node = _searchData.GetNode(edge.SourceId);
            if (node == null)
            {
                _searchData.AddToNodeSet(new NodeData(edge.SourceId, _backwardDepth, Seen.Backward));
                nextLayerNodes.Add(edge.SourceId);
                node = _searchData.GetNode(edge.SourceId);
            }

            if (Math.Abs(node.Distance - _backwardDepth) < 0.01)
            {
                node.AddForwardAdjacent(edge);
            }

            if (node.Seen == Seen.Backward)
            {
                return;
            }

            _searchData.Joints.Add(edge.SourceId);
            node.AddForwardAdjacent(edge);
            _reachedToTarget = true;
        }

        private void VisitForwardEdge(ISet<Guid> nextLayerNodes, Edge edge)
        {
            var node = _searchData.GetNode(edge.TargetId);
            if (node == null)
            {
                _searchData.AddToNodeSet(new NodeData(edge.TargetId, _forwardDepth, Seen.Forward));
                nextLayerNodes.Add(edge.TargetId);
                node = _searchData.GetNode(edge.TargetId);
            }

            if (Math.Abs(node.Distance - _forwardDepth) < 0.01)
            {
                node.AddBackwardAdjacent(edge);
            }

            if (node.Seen == Seen.Forward)
            {
                return;
            }

            _searchData.AddJoint(edge.TargetId);
            node.AddBackwardAdjacent(edge);
            _reachedToTarget = true;
        }

        public override ISearchData GetSearchData()
        {
            return _searchData;
        }
    }
}