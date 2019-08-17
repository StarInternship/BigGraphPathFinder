using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BigDataPathFinding.Models.Interfaces;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    public class PathFinderMultiThread : AbstractPathFinder
    {
        private bool _reachToTarget = false;

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

        private readonly object _targetLock = new object();
        private readonly object _distanceLock = new object();

        private int _forwardLayer = 0;
        private int _backwardLayer = 0;

        private readonly SearchData
            _searchData = new SearchData();

        public PathFinderMultiThread(IMetadata metadata, Guid sourceId, Guid targetId, bool directed,
            int maxDistance, int minDistance)
            : base(metadata, sourceId, targetId, directed, maxDistance, minDistance)
        {
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

                foreach (var edges in Metadata.GetInputAdjacent(_searchData.CurrentBackwardNodes))
                {
                    foreach (var edge in edges)
                    {
                        VisitBackwardEdge(_backwardLayer, nextLayerNodes, edge.SourceId, edge.TargetId, edge.Weight);
                    }
                }

                if (!Directed)
                {
                    foreach (var edges in Metadata.GetOutputAdjacent(_searchData.CurrentBackwardNodes)
                    )
                    {
                        foreach (var edge in edges)
                        {
                            VisitBackwardEdge(_backwardLayer, nextLayerNodes, edge.TargetId, edge.SourceId,
                                edge.Weight);
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

                foreach (var edges in Metadata.GetOutputAdjacent(_searchData.CurrentForwardNodes))
                {
                    foreach (var edge in edges)
                    {
                        VisitForwardEdge(nextLayerNodes, edge.SourceId, edge.TargetId, edge.Weight);
                    }
                }

                if (!Directed)
                {
                    foreach (var edges in Metadata.GetInputAdjacent(_searchData.CurrentForwardNodes))
                    {
                        foreach (var edge in edges)
                        {
                            VisitForwardEdge(nextLayerNodes, edge.TargetId, edge.SourceId, edge.Weight);
                        }
                    }
                }

                _searchData.UpdateCurrentForwardNodes(nextLayerNodes);
            }
        }

        private void VisitBackwardEdge(int backwardLayer, HashSet<Guid> nextLayerNodes, Guid sourceId, Guid targetId,
            double weight)
        {
            lock (_searchData)
            {
                if (_searchData.GetNode(sourceId) == null)
                {
                    VisitBackwardNode(backwardLayer, nextLayerNodes, sourceId);
                }
            }

            if (_searchData.GetNode(sourceId).Distance == backwardLayer &&
                _searchData.GetNode(targetId).Seen == Seen.Backward)
            {
                _searchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
            }

            if (_searchData.GetNode(sourceId).Seen == Seen.Forward &&
                (PathDistance == _searchData.GetNode(sourceId).Distance +
                 _searchData.GetNode(targetId).Distance + 1 || PathDistance == 0))
            {
                _searchData.Joints.Add(sourceId);
                _searchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
                ReachedToTarget = true;
                PathDistance = (int) _searchData.GetNode(sourceId).Distance +
                               (int) _searchData.GetNode(targetId).Distance + 1;
            }
        }

        private void VisitBackwardNode(int backwardLayer, HashSet<Guid> nextLayerNodes, Guid sourceId)
        {
            if (nextLayerNodes == null) throw new ArgumentNullException(nameof(nextLayerNodes));
            _searchData.AddToNodeSet(new NodeData(sourceId, backwardLayer, Seen.Backward));
            nextLayerNodes.Add(sourceId);
        }

        private void VisitForwardEdge(HashSet<Guid> nextLayerNodes, Guid sourceId, Guid targetId, double weight)
        {
            lock (_searchData)
            {
                if (_searchData.GetNode(targetId) == null)
                {
                    VisitForwardNode(_forwardLayer, nextLayerNodes, targetId);
                }
            }

            if (_searchData.GetNode(targetId).Distance == _forwardLayer &&
                _searchData.GetNode(targetId).Seen == Seen.Forward)
            {
                _searchData.GetNode(targetId).AddForwardAdjacent(new Adjacent(sourceId, weight));
            }

            if (_searchData.GetNode(targetId).Seen == Seen.Backward &&
                (PathDistance == _searchData.GetNode(sourceId).Distance +
                 _searchData.GetNode(targetId).Distance + 1 || PathDistance == 0))
            {
                _searchData.Joints.Add(targetId);
                _searchData.GetNode(targetId).AddForwardAdjacent(new Adjacent(sourceId, weight));
                ReachedToTarget = true;
                PathDistance = (int) _searchData.GetNode(sourceId).Distance +
                               (int) _searchData.GetNode(targetId).Distance + 1;
            }
        }

        private void VisitForwardNode(int layer, HashSet<Guid> nextLayerNodes, Guid targetId)
        {
            if (nextLayerNodes == null) throw new ArgumentNullException(nameof(nextLayerNodes));
            _searchData.AddToNodeSet(new NodeData(targetId, layer, Seen.Forward));
            nextLayerNodes.Add(targetId);
        }

        public override ISearchData GetSearchData() => _searchData;
    }
}