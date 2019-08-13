using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models.Hadi
{
    public class HadiPathFinder : AbstractPathFinder
    {
        private SearchData _searchData;
        private double _checkingDistance;
        public HadiPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed, int maxDistance)
            : base(metadata, sourceId, targetId, directed, maxDistance)
        {
        }

        public override void FindPath()
        {
            _searchData = new SearchData(new NodeData(SourceId, 0));

            while (!_searchData.IsEmpty())
            {
                var node = _searchData.PopBestCurrentNode();

                _checkingDistance = node.Distance;

                if (GetNode(TargetId) != null && _checkingDistance >= GetNode(TargetId).Distance)
                {
                    break;
                }

                if (node.Explored) continue;
                node.Explored = true;

                foreach (var adjacentList in Metadata.GetOutputAdjacent(node.Id))
                {
                    foreach (var adjacent in adjacentList)
                    {
                        UpdateAdjacent(node, adjacent);
                    }

                }

                if (!Directed)
                    foreach (var adjacentList in Metadata.GetInputAdjacent(node.Id))
                    {
                        foreach (var adjacent in adjacentList)
                        {
                            UpdateAdjacent(node, adjacent);
                        }
                    }
            }

            if (_searchData.GetNode(TargetId) != null)
                _searchData.GetJoints().Add(TargetId);
        }

        private void UpdateAdjacent(NodeData node, Adjacent adjacent)
        {
            if (!PossiblePath(node, adjacent)) return;
            var outAdjacent = GetNode(adjacent.Id) ?? AddToNodeSet(adjacent);

            if (node.Distance + adjacent.Weight < outAdjacent.Distance)
            {
                //_searchData.RemoveFromQueue(outAdjacent);
                outAdjacent.ClearAdjacentAndUpdateDistance(
                    new Adjacent(node.Id, adjacent.Weight), node.Distance + adjacent.Weight
                );
                _searchData.AddToQueue(outAdjacent);
            }

            else if (Math.Abs(node.Distance + adjacent.Weight - outAdjacent.Distance) < 0.01)
            {
                outAdjacent.AddAdjacent(new Adjacent(node.Id, adjacent.Weight));
            }
        }

        private NodeData AddToNodeSet(Adjacent adjacent)
        {
            _searchData.AddToNodeSet(new NodeData(adjacent.Id, double.MaxValue));
            return GetNode(adjacent.Id);
        }

        private bool PossiblePath(NodeData node, Adjacent adjacent) =>
            GetNode(TargetId) == null || node.Distance + adjacent.Weight <= GetNode(TargetId).Distance;

        private NodeData GetNode(Guid node) => _searchData.GetNode(node);


        public override ISearchData GetSearchData() => _searchData;
    }
}