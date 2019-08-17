using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BigDataPathFinding.Models.Interfaces;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    public class ShortestWeightlessPathFinderMultiThread : AbstractPathFinder
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
                    return _shortestWeightlessSearchData.PathDistance;
                }
            }
            set
            {
                lock (_distanceLock)
                {
                    _shortestWeightlessSearchData.PathDistance = value;
                }
            }
        }

        private readonly object _targetLock = new object();
        private readonly object _distanceLock = new object();

        private int _forwardLayer = 0;
        private int _backwardLayer = 0;

        private readonly ShortestWeightlessSearchData
            _shortestWeightlessSearchData = new ShortestWeightlessSearchData();

        public ShortestWeightlessPathFinderMultiThread(IMetadata metadata, Guid sourceId, Guid targetId, bool directed,
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
                _backwardLayer++;

                if (_shortestWeightlessSearchData.CurrentBackwardNodes.Count == 0 ||
                    _backwardLayer > _shortestWeightlessSearchData.MaxBackwardDistance)
                {
                    break;
                }

                var nextLayerNodes = new HashSet<Guid>();

                foreach (var edges in Metadata.GetInputAdjacent(_shortestWeightlessSearchData.CurrentBackwardNodes))
                {
                    foreach (var edge in edges)
                    {
                        VisitBackwardEdge(_backwardLayer, nextLayerNodes, edge.SourceId, edge.TargetId, edge.Weight);
                    }
                }

                if (!Directed)
                {
                    foreach (var edges in Metadata.GetOutputAdjacent(_shortestWeightlessSearchData.CurrentBackwardNodes)
                    )
                    {
                        foreach (var edge in edges)
                        {
                            VisitBackwardEdge(_backwardLayer, nextLayerNodes, edge.TargetId, edge.SourceId,
                                edge.Weight);
                        }
                    }
                }

                _shortestWeightlessSearchData.UpdateCurrentBackwardNodes(nextLayerNodes);
            }
        }

        private void ExpandForward()
        {
            while (!ReachedToTarget)
            {
                _forwardLayer++;

                if (_shortestWeightlessSearchData.CurrentForwardNodes.Count == 0 ||
                    _forwardLayer > _shortestWeightlessSearchData.MaxForwardDistance)
                {
                    return;
                }

                var nextLayerNodes = new HashSet<Guid>();

                foreach (var edges in Metadata.GetOutputAdjacent(_shortestWeightlessSearchData.CurrentForwardNodes))
                {
                    foreach (var edge in edges)
                    {
                        VisitForwardEdge(nextLayerNodes, edge.SourceId, edge.TargetId, edge.Weight);
                    }
                }

                if (!Directed)
                {
                    foreach (var edges in Metadata.GetInputAdjacent(_shortestWeightlessSearchData.CurrentForwardNodes))
                    {
                        foreach (var edge in edges)
                        {
                            VisitForwardEdge(nextLayerNodes, edge.TargetId, edge.SourceId, edge.Weight);
                        }
                    }
                }

                _shortestWeightlessSearchData.UpdateCurrentForwardNodes(nextLayerNodes);
            }
        }

        private void VisitBackwardEdge(int backwardLayer, HashSet<Guid> nextLayerNodes, Guid sourceId, Guid targetId,
            double weight)
        {
            lock (_shortestWeightlessSearchData)
            {
                if (_shortestWeightlessSearchData.GetNode(sourceId) == null)
                {
                    VisitBackwardNode(backwardLayer, nextLayerNodes, sourceId);
                }
            }

            if (_shortestWeightlessSearchData.GetNode(sourceId).Distance == backwardLayer &&
                _shortestWeightlessSearchData.GetNode(targetId).Seen == Seen.Backward)
            {
                _shortestWeightlessSearchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
            }

            if (_shortestWeightlessSearchData.GetNode(sourceId).Seen == Seen.Forward &&
                (PathDistance == _shortestWeightlessSearchData.GetNode(sourceId).Distance +
                 _shortestWeightlessSearchData.GetNode(targetId).Distance + 1 || PathDistance == 0))
            {
                _shortestWeightlessSearchData.Joints.Add(sourceId);
                _shortestWeightlessSearchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
                ReachedToTarget = true;
                PathDistance = (int) _shortestWeightlessSearchData.GetNode(sourceId).Distance +
                               (int) _shortestWeightlessSearchData.GetNode(targetId).Distance + 1;
            }
        }

        private void VisitBackwardNode(int backwardLayer, HashSet<Guid> nextLayerNodes, Guid sourceId)
        {
            if (nextLayerNodes == null) throw new ArgumentNullException(nameof(nextLayerNodes));
            _shortestWeightlessSearchData.AddToNodeSet(new NodeData(sourceId, backwardLayer, Seen.Backward));
            nextLayerNodes.Add(sourceId);
        }

        private void VisitForwardEdge(HashSet<Guid> nextLayerNodes, Guid sourceId, Guid targetId, double weight)
        {
            lock (_shortestWeightlessSearchData)
            {
                if (_shortestWeightlessSearchData.GetNode(targetId) == null)
                {
                    VisitForwardNode(_forwardLayer, nextLayerNodes, targetId);
                }
            }

            if (_shortestWeightlessSearchData.GetNode(targetId).Distance == _forwardLayer &&
                _shortestWeightlessSearchData.GetNode(targetId).Seen == Seen.Forward)
            {
                _shortestWeightlessSearchData.GetNode(targetId).AddForwardAdjacent(new Adjacent(sourceId, weight));
            }

            if (_shortestWeightlessSearchData.GetNode(targetId).Seen == Seen.Backward &&
                (PathDistance == _shortestWeightlessSearchData.GetNode(sourceId).Distance +
                 _shortestWeightlessSearchData.GetNode(targetId).Distance + 1 || PathDistance == 0))
            {
                _shortestWeightlessSearchData.Joints.Add(targetId);
                _shortestWeightlessSearchData.GetNode(targetId).AddForwardAdjacent(new Adjacent(sourceId, weight));
                ReachedToTarget = true;
                PathDistance = (int) _shortestWeightlessSearchData.GetNode(sourceId).Distance +
                               (int) _shortestWeightlessSearchData.GetNode(targetId).Distance + 1;
            }
        }

        private void VisitForwardNode(int layer, HashSet<Guid> nextLayerNodes, Guid targetId)
        {
            if (nextLayerNodes == null) throw new ArgumentNullException(nameof(nextLayerNodes));
            _shortestWeightlessSearchData.AddToNodeSet(new NodeData(targetId, layer, Seen.Forward));
            nextLayerNodes.Add(targetId);
        }

        public override ISearchData GetSearchData() => _shortestWeightlessSearchData;
    }
}