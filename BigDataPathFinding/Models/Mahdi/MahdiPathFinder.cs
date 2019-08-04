using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models.Mahdi
{
    internal class MahdiPathFinder : PathFinder
    {
        private readonly SearchData _searchData = new SearchData();
        private double _checkingDistance;
        private double _minDistance = int.MaxValue;
        private bool _shouldContinue = true;

        public MahdiPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed) : base(metadata,
            sourceId, targetId, directed)
        {
        }

        private void Go()
        {
            if (!_searchData.HasCandidate() || _checkingDistance > _minDistance)
            {
                _shouldContinue = false;
                return;
            }

            var bestCandidateData = _searchData.GetBestCandidateData();
            var bestCandidate = _searchData.GetBestCandidate();
            _searchData.MoveToDiscovery(bestCandidate, bestCandidateData);
            _checkingDistance = bestCandidateData.Distance;
            if (bestCandidate == TargetId)
                if (Math.Abs(_minDistance - int.MaxValue) < 1)
                    _minDistance = bestCandidateData.Distance;

            foreach (var adjacent in Metadata.GetOutputAdjacents(bestCandidate))
            {
                var newNodeData = new NodeData(bestCandidate, adjacent.Weight + bestCandidateData.Distance);
                if (_searchData.ContainsDiscovery(adjacent.Id))
                {
                    var currentData = _searchData.GetDiscoveryData(adjacent.Id);
                    if (Math.Abs(currentData.Distance - newNodeData.Distance) < 0.01)
                        currentData.PreviousAdjacents.Add(new Adjacent(bestCandidate, adjacent.Weight));
                    continue;
                }

                if (_searchData.ContainsCandidate(adjacent.Id))
                {
                    var currentData = _searchData.GetCandidateData(adjacent.Id);
                    if (Math.Abs(currentData.Distance - newNodeData.Distance) < 0.01)
                        currentData.PreviousAdjacents.Add(new Adjacent(bestCandidate, adjacent.Weight));
                    else if (currentData.Distance > newNodeData.Distance)
                        _searchData.UpdateCandidate(adjacent.Id, newNodeData);
                    continue;
                }

                _searchData.AddCandidate(adjacent.Id, newNodeData);
            }
        }

        public override void FindPath()
        {
            while (_shouldContinue) Go();
        }

        public override Dictionary<Guid, NodeData> GetResultNodeSet()
        {
            return _searchData.GetDiscoveries();
        }
    }
}