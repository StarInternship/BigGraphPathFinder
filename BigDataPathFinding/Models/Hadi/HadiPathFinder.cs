using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataPathFinding.Models.Hadi
{
    public class HadiPathFinder : PathFinder
    {
        private ISearchData searchData;

        public HadiPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed) : base(metadata, sourceId, targetId, directed)
        {
        }

        public override void FindPath()
        {
            searchData = new SearchData(new NodeData(SourceId, 0));


            while (!searchData.IsEmpty())
            {
                var node = searchData.PopBestCurrentNode();

                if (node.Explored)
                    continue;
                node.Explored = true;


                foreach (var adjacent in Metadata.GetOutputAdjacents(node.Id))
                {
                    UpdateInAdjacent(node, adjacent);
                }

                if (!Directed)
                {
                    foreach (var adjacent in Metadata.GetInputAdjacents(node.Id))
                    {
                        UpdateInAdjacent(node, adjacent);
                    }
                }
            }
        }




        private void UpdateInAdjacent(NodeData node, Adjacent adjacent)
        {
            var outAdjacent = GetNode(adjacent.Id);
            if (!PossiblePath(node, adjacent)) return;

            if (outAdjacent == null)
            {
                outAdjacent = AddToNodeSet(adjacent);
            }


            if (node.Distance + adjacent.Weight < outAdjacent.Distance)
            {
                outAdjacent.ClearAdjacentsAndUpdateDistance(adjacent, node.Distance + adjacent.Weight);
            }


            else if (node.Distance + adjacent.Weight == outAdjacent.Distance)
            {
                outAdjacent.addAdjacent(adjacent);
            }


        }

        private NodeData AddToNodeSet(Adjacent adjacent)
        {
            searchData.AddToNodeSet(new NodeData(adjacent.Id, Double.MaxValue));
            return GetNode(adjacent.Id);
        }

        private bool PossiblePath(NodeData node, Adjacent adjacent)
        {
            return GetNode(TargetId) == null || node.Distance + adjacent.Weight < GetNode(TargetId).Distance;
        }

        private NodeData GetNode(Guid node) => searchData.GetNode(node);

        public override Dictionary<Guid, NodeData> GetResultNodeSet()
        {
            throw new NotImplementedException();
        }
    }
}
