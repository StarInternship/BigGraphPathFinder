using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    public class WeightlessPathFinder : AbstractPathFinder
    {

        private readonly SearchData searchData = new SearchData();

        public WeightlessPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed)
            : base(metadata, sourceId, targetId, directed)
        {
        }

        public override void FindPath()
        {
            int distance = 0;
            searchData.AddToNodeSet(new NodeData(SourceId, distance));
            searchData.AddToCurrentNodes(SourceId);

            while (true)
            {
                HashSet<Guid> guids = new HashSet<Guid>();
                distance++;
                bool reachedToTarget = false;
                foreach (IEnumerable<Edge> edges in Metadata.GetOutputAdjacent(searchData.currentNodes))
                {
                    foreach (Edge edge in edges)
                    {
                        reachedToTarget = VisiteEdge(distance, guids, edge.SourceId, edge.TargetId, edge.Weight);
                    }
                }
                if (!Directed)
                {
                    foreach (IEnumerable<Edge> edges in Metadata.GetInputAdjacent(searchData.currentNodes))
                    {
                        foreach (Edge edge in edges)
                        {
                            reachedToTarget = VisiteEdge(distance, guids, edge.TargetId, edge.SourceId, edge.Weight);
                        }
                    }
                }

                searchData.ClearCurrentNodes(guids);

                if (reachedToTarget)
                    break;
            }
        }

        private bool VisiteEdge(int distance, HashSet<Guid> guids, Guid sourceId, Guid targetId, double weight)
        {
            if (searchData.GetNode(targetId) == null)
            {
                VisiteNewNode(distance, guids, sourceId, targetId, weight);
            }
            else if (searchData.GetNode(targetId).Distance == distance)
            {
                searchData.GetNode(targetId).AddAdjacent(new Adjacent(sourceId, weight));
            }

            return TargetId == targetId;
        }

        private void VisiteNewNode(int distance, HashSet<Guid> guids, Guid sourceId, Guid targetId, double weight)
        {
            var newNode = new NodeData(targetId, distance);
            searchData.AddToNodeSet(newNode);
            newNode.AddAdjacent(new Adjacent(sourceId, weight));
            guids.Add(newNode.Id);
        }

        public override Dictionary<Guid, NodeData> GetResultNodeSet()
        {
            return searchData.NodeSet;
        }
    }
}
