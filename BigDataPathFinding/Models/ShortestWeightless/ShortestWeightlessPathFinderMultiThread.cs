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
                lock (targetLock)
                {
                    return _reachToTarget;
                }
            }
            set
            {
                lock (targetLock)
                {
                    _reachToTarget = value;
                }
            }
        }

        private int PathDistance
        {
            get
            {
                lock (distanceLock)
                {
                    return _shortestWeightlessSearchData.PathDistance;
                }
            }
            set
            {
                lock (distanceLock)
                {
                    _shortestWeightlessSearchData.PathDistance = value;
                }
            }
        }

        private readonly object targetLock = new object();
        private readonly object distanceLock = new object();

        private int forwardLeyer = 0;
        private int backwardLeyer = 0;

        private readonly ShortestWeightlessSearchData _shortestWeightlessSearchData = new ShortestWeightlessSearchData();

        public ShortestWeightlessPathFinderMultiThread(IMetadata metadata, Guid sourceId, Guid targetId, bool directed, int maxDistance, int minDistance)
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

                backwardLeyer++;

                if (_shortestWeightlessSearchData.CurrentBackwardNodes.Count == 0 || backwardLeyer > _shortestWeightlessSearchData.MaxBackwardDistance)
                {
                    break;
                }

                var nextLeyerNodes = new HashSet<Guid>();

                foreach (var edges in Metadata.GetInputAdjacent(_shortestWeightlessSearchData.CurrentBackwardNodes))
                {
                    foreach (var edge in edges)
                    {
                        VisiteBackwardEdge(backwardLeyer, nextLeyerNodes, edge.SourceId, edge.TargetId, edge.Weight);
                    }
                }

                if (!Directed)
                {
                    foreach (var edges in Metadata.GetOutputAdjacent(_shortestWeightlessSearchData.CurrentBackwardNodes))
                    {
                        foreach (var edge in edges)
                        {
                            VisiteBackwardEdge(backwardLeyer, nextLeyerNodes, edge.TargetId, edge.SourceId, edge.Weight);
                        }
                    }
                }

                _shortestWeightlessSearchData.UpdateCurrentBackwardNodes(nextLeyerNodes);
            }

        }

        private void ExpandForward()
        {
            while (!ReachedToTarget)
            {
                forwardLeyer++;

                if (_shortestWeightlessSearchData.CurrentForwardNodes.Count == 0 || forwardLeyer > _shortestWeightlessSearchData.MaxForwardDistance)
                {
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
                    foreach (var edges in Metadata.GetInputAdjacent(_shortestWeightlessSearchData.CurrentForwardNodes))
                    {
                        foreach (var edge in edges)
                        {
                            VisiteForwardEdge(nextLeyerNodes, edge.TargetId, edge.SourceId, edge.Weight);
                        }
                    }
                }

                _shortestWeightlessSearchData.UpdateCurrentForwardNodes(nextLeyerNodes);
            }

        }

        private void VisiteBackwardEdge(int backwardLeyer, HashSet<Guid> nextLeyerNodes, Guid sourceId, Guid targetId, double weight)
        {
            lock (_shortestWeightlessSearchData)
            {
                if (_shortestWeightlessSearchData.GetNode(sourceId) == null)
                {
                    VisitBackwardNode(backwardLeyer, nextLeyerNodes, sourceId);
                }
            }

            if (_shortestWeightlessSearchData.GetNode(sourceId).Distance == backwardLeyer && _shortestWeightlessSearchData.GetNode(targetId).Seen == Seen.Backward)
            {
                _shortestWeightlessSearchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
            }

            if (_shortestWeightlessSearchData.GetNode(sourceId).Seen == Seen.Forward && (PathDistance == _shortestWeightlessSearchData.GetNode(sourceId).Distance + _shortestWeightlessSearchData.GetNode(targetId).Distance + 1 || PathDistance == 0))
            {
                _shortestWeightlessSearchData.Joints.Add(sourceId);
                _shortestWeightlessSearchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
                ReachedToTarget = true;
                PathDistance = (int)_shortestWeightlessSearchData.GetNode(sourceId).Distance + (int)_shortestWeightlessSearchData.GetNode(targetId).Distance + 1;
            }
        }

        private void VisitBackwardNode(int backwardLeyer, HashSet<Guid> nextLeyerNodes, Guid sourceId)
        {
            _shortestWeightlessSearchData.AddToNodeSet(new NodeData(sourceId, backwardLeyer, Seen.Backward));
            nextLeyerNodes.Add(sourceId);
        }

        private void VisiteForwardEdge(HashSet<Guid> nextLeyerNodes, Guid sourceId, Guid targetId, double weight)
        {
            lock (_shortestWeightlessSearchData)
            {
                if (_shortestWeightlessSearchData.GetNode(targetId) == null)
                {
                    VisiteForwardNode(forwardLeyer, nextLeyerNodes, targetId);
                }
            }

            if (_shortestWeightlessSearchData.GetNode(targetId).Distance == forwardLeyer && _shortestWeightlessSearchData.GetNode(targetId).Seen == Seen.Forward)
            {
                _shortestWeightlessSearchData.GetNode(targetId).AddForwardAdjacent(new Adjacent(sourceId, weight));
            }

            if (_shortestWeightlessSearchData.GetNode(targetId).Seen == Seen.Backward && (PathDistance == _shortestWeightlessSearchData.GetNode(sourceId).Distance + _shortestWeightlessSearchData.GetNode(targetId).Distance + 1 || PathDistance == 0))
            {
                _shortestWeightlessSearchData.Joints.Add(targetId);
                _shortestWeightlessSearchData.GetNode(targetId).AddForwardAdjacent(new Adjacent(sourceId, weight));
                ReachedToTarget = true;
                PathDistance = (int)_shortestWeightlessSearchData.GetNode(sourceId).Distance + (int)_shortestWeightlessSearchData.GetNode(targetId).Distance + 1;
            }
        }

        private void VisiteForwardNode(int leyer, HashSet<Guid> nextLeyerNodes, Guid targetId)
        {
            _shortestWeightlessSearchData.AddToNodeSet(new NodeData(targetId, leyer, Seen.Forward));
            nextLeyerNodes.Add(targetId);
        }

        public override ISearchData GetSearchData() => _shortestWeightlessSearchData;
    }
}
