using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    public class WeightlessPathFinderMultiThread : AbstractPathFinder
    {
        private bool reachToTarget = false;

        private bool ReachedToTarget
        {
            get
            {
                lock (targetLock)
                {
                    return reachToTarget;
                }
            }
            set
            {
                lock (targetLock)
                {
                    reachToTarget = value;
                }
            }
        }

        private int PathDistance
        {
            get
            {
                lock (distanceLock)
                {
                    return searchData.PathDistance;
                }
            }
            set
            {
                lock (distanceLock)
                {
                    searchData.PathDistance = value;
                }
            }
        }

        private readonly object targetLock = new object();
        private readonly object distanceLock = new object();

        private int forwardLeyer = 0;
        private int backwardLeyer = 0;

        private readonly SearchData searchData = new SearchData();

        public WeightlessPathFinderMultiThread(IMetadata metadata, Guid sourceId, Guid targetId, bool directed, int maxDistance, int minDistance)
            : base(metadata, sourceId, targetId, directed, maxDistance, minDistance)
        {
            searchData.MaxForwardDistance = (maxDistance + 1) / 2;
            searchData.MaxBackwardDistance = (maxDistance) / 2;
        }

        public override void FindPath()
        {
            searchData.AddToNodeSet(new WeightlessNodeData(SourceId, forwardLeyer, Seen.forward));
            searchData.AddToCurrentForwardNodes(SourceId);

            searchData.AddToNodeSet(new WeightlessNodeData(TargetId, backwardLeyer, Seen.backward));
            searchData.AddToCurrentBackwardNodes(TargetId);

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

                if (searchData.CurrentBackwardNodes.Count == 0 || backwardLeyer > searchData.MaxBackwardDistance)
                {
                    break;
                }

                var nextLeyerNodes = new HashSet<Guid>();

                foreach (var edges in Metadata.GetInputAdjacent(searchData.CurrentBackwardNodes))
                {
                    foreach (var edge in edges)
                    {
                        VisiteBackwardEdge(backwardLeyer, nextLeyerNodes, edge.SourceId, edge.TargetId, edge.Weight);
                    }
                }

                if (!Directed)
                {
                    foreach (var edges in Metadata.GetOutputAdjacent(searchData.CurrentBackwardNodes))
                    {
                        foreach (var edge in edges)
                        {
                            VisiteBackwardEdge(backwardLeyer, nextLeyerNodes, edge.TargetId, edge.SourceId, edge.Weight);
                        }
                    }
                }

                searchData.UpdateCurrentBackwardNodes(nextLeyerNodes);
            }

        }

        private void ExpandForward()
        {
            while (!ReachedToTarget)
            {
                forwardLeyer++;

                if (searchData.CurrentForwardNodes.Count == 0 || forwardLeyer > searchData.MaxForwardDistance)
                {
                    return;
                }

                var nextLeyerNodes = new HashSet<Guid>();

                foreach (var edges in Metadata.GetOutputAdjacent(searchData.CurrentForwardNodes))
                {
                    foreach (var edge in edges)
                    {
                        VisiteForwardEdge(nextLeyerNodes, edge.SourceId, edge.TargetId, edge.Weight);
                    }
                }

                if (!Directed)
                {
                    foreach (var edges in Metadata.GetInputAdjacent(searchData.CurrentForwardNodes))
                    {
                        foreach (var edge in edges)
                        {
                            VisiteForwardEdge(nextLeyerNodes, edge.TargetId, edge.SourceId, edge.Weight);
                        }
                    }
                }

                searchData.UpdateCurrentForwardNodes(nextLeyerNodes);
            }

        }

        private void VisiteBackwardEdge(int backwardLeyer, HashSet<Guid> nextLeyerNodes, Guid sourceId, Guid targetId, double weight)
        {
            lock (searchData)
            {
                if (searchData.GetNode(sourceId) == null)
                {
                    VisitBackwardNode(backwardLeyer, nextLeyerNodes, sourceId);
                }
            }

            if (searchData.GetNode(sourceId).Distance == backwardLeyer && searchData.GetNode(targetId).Seen == Seen.backward)
            {
                searchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
            }

            if (searchData.GetNode(sourceId).Seen == Seen.forward && (PathDistance == searchData.GetNode(sourceId).Distance + searchData.GetNode(targetId).Distance + 1 || PathDistance == 0))
            {
                searchData.Joints.Add(sourceId);
                searchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
                ReachedToTarget = true;
                PathDistance = (int)searchData.GetNode(sourceId).Distance + (int)searchData.GetNode(targetId).Distance + 1;
            }
        }

        private void VisitBackwardNode(int backwardLeyer, HashSet<Guid> nextLeyerNodes, Guid sourceId)
        {
            searchData.AddToNodeSet(new WeightlessNodeData(sourceId, backwardLeyer, Seen.backward));
            nextLeyerNodes.Add(sourceId);
        }

        private void VisiteForwardEdge(HashSet<Guid> nextLeyerNodes, Guid sourceId, Guid targetId, double weight)
        {
            lock (searchData)
            {
                if (searchData.GetNode(targetId) == null)
                {
                    VisiteForwardNode(forwardLeyer, nextLeyerNodes, targetId);
                }
            }

            if (searchData.GetNode(targetId).Distance == forwardLeyer && searchData.GetNode(targetId).Seen == Seen.forward)
            {
                searchData.GetNode(targetId).AddAdjacent(new Adjacent(sourceId, weight));
            }

            if (searchData.GetNode(targetId).Seen == Seen.backward && (PathDistance == searchData.GetNode(sourceId).Distance + searchData.GetNode(targetId).Distance + 1 || PathDistance == 0))
            {
                searchData.Joints.Add(targetId);
                searchData.GetNode(targetId).AddAdjacent(new Adjacent(sourceId, weight));
                ReachedToTarget = true;
                PathDistance = (int)searchData.GetNode(sourceId).Distance + (int)searchData.GetNode(targetId).Distance + 1;
            }
        }

        private void VisiteForwardNode(int leyer, HashSet<Guid> nextLeyerNodes, Guid targetId)
        {
            searchData.AddToNodeSet(new WeightlessNodeData(targetId, leyer, Seen.forward));
            nextLeyerNodes.Add(targetId);
        }

        public override ISearchData GetSearchData() => searchData;
    }
}
