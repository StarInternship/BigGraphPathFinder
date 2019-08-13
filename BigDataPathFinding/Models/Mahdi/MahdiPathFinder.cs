using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models.Mahdi
{
    public class MahdiPathFinder : AbstractPathFinder
    {
        private readonly SearchData _searchData = new SearchData();
        private double _checkingDistance;
        private double _minDistance = int.MaxValue;
        private bool _shouldContinue = true;

        public MahdiPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed, int maxDistance) : base(metadata,
            sourceId, targetId, directed, maxDistance)
        {
            _searchData.AddCandidate(sourceId, new NodeData(sourceId, 0));
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
                if (Math.Abs(_minDistance - int.MaxValue) < 0.01) //if minDistance hasn't been set! 
                    _minDistance = bestCandidateData.Distance;

            foreach (var adjacentList in Metadata.GetOutputAdjacent(bestCandidate))
            {
                foreach (var adjacent in adjacentList)
                {
                    CheckAdjacent(adjacent, bestCandidateData, bestCandidate);
                }
            }

            if (Directed)
                return;
            foreach (var adjacentList in Metadata.GetInputAdjacent(bestCandidate))
            {
                foreach (var adjacent in adjacentList)
                {
                    CheckAdjacent(adjacent, bestCandidateData, bestCandidate);
                }
            }
        }

        private void CheckAdjacent(Adjacent adjacent, NodeData bestCandidateData, Guid bestCandidate)
        {
            var newNodeData = new NodeData(adjacent.Id, adjacent.Weight + bestCandidateData.Distance);
            if (newNodeData.Distance > _minDistance)
                return;
            newNodeData.AddAdjacent(new Adjacent(bestCandidate, adjacent.Weight));
            if (_searchData.ContainsDiscovery(adjacent.Id))
            {
                var currentData = _searchData.GetDiscoveryData(adjacent.Id);
                if (Math.Abs(currentData.Distance - newNodeData.Distance) < 0.01)
                    currentData.AddAdjacent(new Adjacent(bestCandidate, adjacent.Weight));
                return;
            }

            if (_searchData.ContainsCandidate(adjacent.Id))
            {
                var currentData = _searchData.GetCandidateData(adjacent.Id);
                if (Math.Abs(currentData.Distance - newNodeData.Distance) < 0.01)
                    currentData.AddAdjacent(new Adjacent(bestCandidate, adjacent.Weight));
                else if (currentData.Distance > newNodeData.Distance)
                    _searchData.UpdateCandidate(adjacent.Id, newNodeData);
                return;
            }

            _searchData.AddCandidate(adjacent.Id, newNodeData);
        }

        public override void FindPath()
        {
            while (_shouldContinue) Go();
        }

        public override Dictionary<Guid, NodeData> GetResultNodeSet() => _searchData.GetDiscoveries();
    }
}