using System;
using System.Collections.Generic;
using System.Linq;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    public class WeightlessPathFinder : AbstractPathFinder
    {

        private bool reachedToTarget = false;

        private int forwardLeyer = 0;
        private int backwardLeyer = 2000;

        private HashSet<Guid> joints = new HashSet<Guid>();

        private readonly SearchData searchData = new SearchData();

        public WeightlessPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed, int maxDistance)
            : base(metadata, sourceId, targetId, directed, maxDistance)
        {
        }

        public override void FindPath()
        {
            searchData.AddToNodeSet(new WeightlessNodeData(SourceId, forwardLeyer, Seen.forward));
            searchData.AddToCurrentForwardNodes(SourceId);

            searchData.AddToNodeSet(new WeightlessNodeData(TargetId, backwardLeyer, Seen.backward));
            searchData.AddToCurrentBackwardNodes(TargetId);

            while (!reachedToTarget)
            {
                if (searchData.CurrentBackwardNodes.Count == 0 && searchData.CurrentForwardNodes.Count == 0)
                    return;

                GoForward();

                if (joints.Count > 0)
                    break;

                GoBackward();

                if (joints.Count > 0)
                    break;
            }
            AddPreviousAdjacentToBackwardNodes();
        }

        private void AddPreviousAdjacentToBackwardNodes()
        {
            forwardLeyer++;
            var gap = 2000 - forwardLeyer;

            while (joints.Count > 0)
            {
                var node = searchData.GetNode(joints.First());
                joints.Remove(node.Id);

                if (node.Explored)
                    continue;
                node.Explored = true;

                foreach (Adjacent adjacent in node.ForwardAdjacents)
                {
                    searchData.GetNode(adjacent.Id).AddAdjacent(new Adjacent(node.Id, adjacent.Weight));
                    joints.Add(adjacent.Id);
                }
            }
        }

        private void GoBackward()
        {
            backwardLeyer--;

            if (searchData.CurrentBackwardNodes.Count == 0)
                return;

            var nextLeyerNodes = new HashSet<Guid>();

            foreach (IEnumerable<Edge> edges in Metadata.GetInputAdjacent(searchData.CurrentBackwardNodes))
            {
                foreach (Edge edge in edges)
                {
                    VisiteBackwardEdge(backwardLeyer, nextLeyerNodes, edge.SourceId, edge.TargetId, edge.Weight);
                }
            }


            if (!Directed)
            {
                foreach (IEnumerable<Edge> edges in Metadata.GetOutputAdjacent(searchData.CurrentBackwardNodes))
                {
                    foreach (Edge edge in edges)
                    {
                        VisiteBackwardEdge(backwardLeyer, nextLeyerNodes, edge.TargetId, edge.SourceId, edge.Weight);
                    }
                }
            }

            searchData.ClearCurrentBackwardNodes(nextLeyerNodes);
        }

        private void GoForward()
        {
            forwardLeyer++;
            if (searchData.CurrentForwardNodes.Count == 0)
                return;
            var nextLeyerNodes = new HashSet<Guid>();

            foreach (IEnumerable<Edge> edges in Metadata.GetOutputAdjacent(searchData.CurrentForwardNodes))
            {
                foreach (Edge edge in edges)
                {
                    VisiteForwardEdge(forwardLeyer, nextLeyerNodes, edge.SourceId, edge.TargetId, edge.Weight);
                }
            }


            if (!Directed)
            {
                foreach (IEnumerable<Edge> edges in Metadata.GetInputAdjacent(searchData.CurrentForwardNodes))
                {
                    foreach (Edge edge in edges)
                    {
                        VisiteForwardEdge(forwardLeyer, nextLeyerNodes, edge.TargetId, edge.SourceId, edge.Weight);
                    }
                }
            }

            searchData.ClearCurrentForwardNodes(nextLeyerNodes);
        }

        private void VisiteBackwardEdge(int leyer, HashSet<Guid> nextLeyerNodes, Guid sourceId, Guid targetId, double weight)
        {

            if (searchData.GetNode(sourceId) == null)
            {
                var newNode = new WeightlessNodeData(sourceId, leyer, Seen.backward);
                searchData.AddToNodeSet(newNode);
                nextLeyerNodes.Add(newNode.Id);
            }

            if (searchData.GetNode(sourceId).Distance == leyer)
            {
                searchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
            }

            if (searchData.GetNode(sourceId).Seen == Seen.forward)
            {
                joints.Add(sourceId);
                searchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(sourceId, weight));

            }

        }

        private void VisiteForwardEdge(int leyer, HashSet<Guid> nextLeyerNodes, Guid sourceId, Guid targetId, double weight)
        {
            if (searchData.GetNode(targetId) == null)
            {
                VisiteNewNode(leyer, nextLeyerNodes, sourceId, targetId, weight);
            }

            if (searchData.GetNode(targetId).Distance == leyer)
            {
                searchData.GetNode(targetId).AddAdjacent(new Adjacent(sourceId, weight));
            }

            if (searchData.GetNode(targetId).Seen == Seen.backward)
            {
                joints.Add(targetId);
                searchData.GetNode(targetId).AddAdjacent(new Adjacent(sourceId, weight));

            }
        }

        private void VisiteNewNode(int leyer, HashSet<Guid> nextLeyerNodes, Guid sourceId, Guid targetId, double weight)
        {
            var newNode = new WeightlessNodeData(targetId, leyer, Seen.forward);
            searchData.AddToNodeSet(newNode);
            nextLeyerNodes.Add(newNode.Id);
        }

        public override Dictionary<Guid, NodeData> GetResultNodeSet() => searchData.NodeSet;
    }
}
