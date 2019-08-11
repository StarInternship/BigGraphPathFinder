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
                        if (searchData.GetNode(edge.TargetId) == null)
                        {
                            var newNode = new NodeData(edge.TargetId, distance);
                            searchData.AddToNodeSet(newNode);
                            newNode.AddAdjacent(new Adjacent(edge.SourceId, 1));
                            guids.Add(newNode.Id);
                        }
                        else if (searchData.GetNode(edge.TargetId).Distance == distance)
                        {
                            searchData.GetNode(edge.TargetId).AddAdjacent(new Adjacent(edge.SourceId, 1));
                        }

                        if (TargetId == edge.TargetId)
                            reachedToTarget = true;
                    }
                }
                if (!Directed)
                {
                    foreach (IEnumerable<Edge> edges in Metadata.GetInputAdjacent(searchData.currentNodes))
                    {
                        foreach (Edge edge in edges)
                        {
                            if (searchData.GetNode(edge.SourceId) == null)
                            {
                                var newNode = new NodeData(edge.SourceId, distance);
                                searchData.AddToNodeSet(newNode);
                                newNode.AddAdjacent(new Adjacent(edge.TargetId, 1));
                                guids.Add(newNode.Id);
                            }
                            else if (searchData.GetNode(edge.SourceId).Distance == distance)
                            {
                                searchData.GetNode(edge.SourceId).AddAdjacent(new Adjacent(edge.TargetId, 1));
                            }

                            if (TargetId == edge.TargetId)
                                reachedToTarget = true;
                        }
                    }
                }

                searchData.ClearCurrentNodes(guids);

                if (reachedToTarget)
                    break;
            }
        }

        public override Dictionary<Guid, NodeData> GetResultNodeSet()
        {
            throw new NotImplementedException();
        }
    }
}
