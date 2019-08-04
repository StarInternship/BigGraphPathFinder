using System;
using System.Collections.Generic;
using System.IO;

namespace BigDataPathFinding.Models.Mahdi
{
    internal class MahdiPathFinder : PathFinder
    {
        public MahdiPathFinder(IDatabase database, IMetadata metadata, Guid sourceId, Guid targetId, bool directed) :
            base(database, metadata, sourceId, targetId, directed)
        {
        }

        
        private readonly HashSet<Guid> _commonNodes = new HashSet<Guid>();
        private double _checkingDistance;
        private bool _shouldContinue = true;
        private int _minDistance = int.MaxValue;

        private void GoBackward()
        {
            lock (this)
            {
                if (_candidates.IsFree() || _forwardCheckingDistance + _checkingDistance > _minDistance)
                {
                    _shouldContinue = false;
                    return;
                }
            }

            var firstBestAccesses = _candidates.GetFirstBestAccesses();
            var firstVertex = _candidates.GetFirstVertex();
            _candidates.RemoveFirst();
            lock (this)
            {
                _checkingDistance = firstBestAccesses.Distance;
                _discoveries.Add(firstVertex, firstBestAccesses);
                if (_forwardDiscoveries.ContainsKey(firstVertex))
                {
                    var currentDistance = firstBestAccesses.Distance + _forwardDiscoveries[firstVertex].Distance;
                    if (_minDistance == int.MaxValue)
                        _minDistance = currentDistance;
                    if (_minDistance == currentDistance)
                        _commonNodes.Add(firstVertex);
                }
            }

            foreach (var adjacent in firstVertex.Inputs.Keys)
            {
                var newAccesses =
                    new BestAccesses(firstVertex, firstVertex.Inputs[adjacent] + firstBestAccesses.Distance);
                lock (this)
                {
                    if (_forwardDiscoveries.ContainsKey(firstVertex) && _forwardDiscoveries.ContainsKey(adjacent) &&
                        _forwardDiscoveries[firstVertex].PreviousVertices.Contains(adjacent)) continue;
                    if (_forwardDiscoveries.ContainsKey(adjacent)) continue;
                    if (_discoveries.ContainsKey(adjacent))
                    {
                        var currentAccesses = _discoveries[adjacent];
                        if (currentAccesses.Distance == newAccesses.Distance)
                            currentAccesses.PreviousVertices.Add(firstVertex);
                        continue;
                    }
                }

                if (_candidates.ContainsVertex(adjacent))
                {
                    var currentAccesses = _candidates.GetBestAccesses(adjacent);
                    if (currentAccesses.Distance == newAccesses.Distance)
                        currentAccesses.PreviousVertices.Add(firstVertex);
                    else if (currentAccesses.Distance > newAccesses.Distance)
                        _candidates.Update(adjacent, newAccesses);
                    continue;
                }

                _candidates.Add(adjacent, newAccesses);
            }
        }

        public override void FindPath()
        {
            while (_shouldContinue)
            {
                GoBackward();
            }
            HashSet<>
        }

        public override Dictionary<Guid, NodeData> GetResultNodeSet()
        {
            throw new NotImplementedException();
        }
    }
}