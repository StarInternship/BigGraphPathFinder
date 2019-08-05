using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models.Hadi
{
    public class HadiPathFinder : PathFinder
    {
        private SearchData _searchData;

        public HadiPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed)
            : base(metadata, sourceId, targetId, directed)
        {
        }

        public override void FindPath()
        {
            _searchData = new SearchData(new NodeData(SourceId, 0));


            while (!_searchData.IsEmpty())
            {
                var node = _searchData.PopBestCurrentNode();

                if (node.Explored)
                    continue;
                node.Explored = true;


                foreach (var adjacent in Metadata.GetOutputAdjacents(node.Id)) UpdateAdjacent(node, adjacent);

                if (!Directed)
                    foreach (var adjacent in Metadata.GetInputAdjacents(node.Id))
                        UpdateAdjacent(node, adjacent);
            }
        }


        private void UpdateAdjacent(NodeData node, Adjacent adjacent)
        {
            if (!PossiblePath(node, adjacent)) return;

            var outAdjacent = GetNode(adjacent.Id) ?? AddToNodeSet(adjacent);

            if (node.Distance + adjacent.Weight < outAdjacent.Distance)
            {
                outAdjacent.ClearAdjacentsAndUpdateDistance(new Adjacent(node.Id, adjacent.Weight),
                    node.Distance + adjacent.Weight);
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

        private bool PossiblePath(NodeData node, Adjacent adjacent)
        {
            return GetNode(TargetId) == null || node.Distance + adjacent.Weight < GetNode(TargetId).Distance;
        }

        private NodeData GetNode(Guid node)
        {
            return _searchData.GetNode(node);
        }

        public override Dictionary<Guid, NodeData> GetResultNodeSet()
        {
            return _searchData.NodeSet;
        }
    }
}