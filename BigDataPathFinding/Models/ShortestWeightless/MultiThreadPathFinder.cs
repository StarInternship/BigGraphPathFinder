using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BigDataPathFinding.Models.Interfaces;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    public class MultiThreadPathFinder : AbstractPathFinder
    {
        private readonly object _distanceLock = new object();

        private readonly SearchData
            _searchData = new SearchData();

        private readonly object _targetLock = new object();
        private int _backwardLayer;

        private int _forwardLayer;
        private bool _reachToTarget;

        public MultiThreadPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed,
            int maxDistance, int minDistance)
            : base(metadata, sourceId, targetId, directed, maxDistance, minDistance)
        {
        }

        private bool ReachedToTarget
        {
            get
            {
                lock (_targetLock)
                {
                    return _reachToTarget;
                }
            }
            set
            {
                lock (_targetLock)
                {
                    _reachToTarget = value;
                }
            }
        }

        private int PathDistance
        {
            get
            {
                lock (_distanceLock)
                {
                    return _searchData.PathDistance;
                }
            }
            set
            {
                lock (_distanceLock)
                {
                    _searchData.PathDistance = value;
                }
            }
        }

        public override void FindPath()
        {
            _searchData.AddToNodeSet(new NodeData(SourceId, _forwardLayer, Seen.Forward));
            _searchData.AddToCurrentForwardNodes(SourceId);

            _searchData.AddToNodeSet(new NodeData(TargetId, _backwardLayer, Seen.Backward));
            _searchData.AddToCurrentBackwardNodes(TargetId);

            var forwardTask = new Task(ExpandForward);
            var backwardTask = new Task(ExpandBackward);

            forwardTask.Start();
            backwardTask.Start();

            forwardTask.Wait();
            backwardTask.Wait();
        }

        private void ExpandBackward()
        {
            while (!ReachedToTarget)
            {
                if (_searchData.CurrentBackwardNodes.Count == 0 ||
                    _backwardLayer + _forwardLayer > MaxDistance)
                {
                    break;
                }

                _backwardLayer++;
                var nextLayerNodes = new HashSet<Guid>();

                foreach (var edges in Metadata.GetInputEdges(_searchData.CurrentBackwardNodes))
                {
                    foreach (var edge in edges)
                    {
                        VisitBackwardEdge(_backwardLayer, nextLayerNodes, edge);
                    }
                }

                if (!Directed)
                {
                    foreach (var edges in Metadata.GetOutputEdges(_searchData.CurrentBackwardNodes)
                    )
                    {
                        foreach (var edge in edges)
                        {
                            VisitBackwardEdge(_backwardLayer, nextLayerNodes, edge);
                        }
                    }
                }

                _searchData.UpdateCurrentBackwardNodes(nextLayerNodes);
            }
        }

        private void ExpandForward()
        {
            while (!ReachedToTarget)
            {
                _forwardLayer++;

                if (_searchData.CurrentForwardNodes.Count == 0 ||
                    _forwardLayer + _backwardLayer > MaxDistance)
                {
                    return;
                }

                var nextLayerNodes = new HashSet<Guid>();

                foreach (var edges in Metadata.GetOutputEdges(_searchData.CurrentForwardNodes))
                {
                    foreach (var edge in edges)
                    {
                        VisitForwardEdge(nextLayerNodes, edge);
                    }
                }

                if (!Directed)
                {
                    foreach (var edges in Metadata.GetInputEdges(_searchData.CurrentForwardNodes))
                    {
                        foreach (var edge in edges)
                        {
                            VisitForwardEdge(nextLayerNodes, edge);
                        }
                    }
                }

                _searchData.UpdateCurrentForwardNodes(nextLayerNodes);
            }
        }

        private void VisitBackwardEdge(int backwardLayer, HashSet<Guid> nextLayerNodes, Edge edge)
        {
            lock (_searchData)
            {
                if (_searchData.GetNode(edge.SourceId) == null)
                {
                    VisitBackwardNode(backwardLayer, nextLayerNodes, edge.SourceId);
                }
            }

            if (_searchData.GetNode(edge.SourceId).Distance == backwardLayer &&
                _searchData.GetNode(edge.TargetId).Seen == Seen.Backward)
            {
                _searchData.GetNode(edge.SourceId).AddForwardAdjacent(edge);
            }

            if (_searchData.GetNode(edge.SourceId).Seen == Seen.Forward &&
                (PathDistance == _searchData.GetNode(edge.SourceId).Distance +
                 _searchData.GetNode(edge.TargetId).Distance + 1 || PathDistance == 0))
            {
                _searchData.Joints.Add(edge.SourceId);
                _searchData.GetNode(edge.SourceId).AddForwardAdjacent(edge);
                ReachedToTarget = true;
                PathDistance = (int) _searchData.GetNode(edge.SourceId).Distance +
                               (int) _searchData.GetNode(edge.TargetId).Distance + 1;
            }
        }

        private void VisitBackwardNode(int backwardLayer, HashSet<Guid> nextLayerNodes, Guid sourceId)
        {
            if (nextLayerNodes == null)
            {
                throw new ArgumentNullException(nameof(nextLayerNodes));
            }

            _searchData.AddToNodeSet(new NodeData(sourceId, backwardLayer, Seen.Backward));
            nextLayerNodes.Add(sourceId);
        }

        private void VisitForwardEdge(HashSet<Guid> nextLayerNodes, Edge edge)
        {
            lock (_searchData)
            {
                if (_searchData.GetNode(edge.TargetId) == null)
                {
                    VisitForwardNode(_forwardLayer, nextLayerNodes, edge.TargetId);
                }
            }

            if (_searchData.GetNode(edge.TargetId).Distance == _forwardLayer &&
                _searchData.GetNode(edge.TargetId).Seen == Seen.Forward)
            {
                _searchData.GetNode(edge.TargetId).AddBackwardAdjacent(edge);
            }

            if (_searchData.GetNode(edge.TargetId).Seen == Seen.Backward &&
                (PathDistance == _searchData.GetNode(edge.SourceId).Distance +
                 _searchData.GetNode(edge.TargetId).Distance + 1 || PathDistance == 0))
            {
                _searchData.Joints.Add(edge.TargetId);
                _searchData.GetNode(edge.TargetId).AddBackwardAdjacent(edge);
                ReachedToTarget = true;
                PathDistance = (int) _searchData.GetNode(edge.SourceId).Distance +
                               (int) _searchData.GetNode(edge.TargetId).Distance + 1;
            }
        }

        private void VisitForwardNode(int layer, HashSet<Guid> nextLayerNodes, Guid targetId)
        {
            if (nextLayerNodes == null)
            {
                throw new ArgumentNullException(nameof(nextLayerNodes));
            }

            _searchData.AddToNodeSet(new NodeData(targetId, layer, Seen.Forward));
            nextLayerNodes.Add(targetId);
        }

        public override ISearchData GetSearchData()
        {
            return _searchData;
        }
    }
}