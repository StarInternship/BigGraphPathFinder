using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    public class WeightlessPathFinder : AbstractPathFinder
    {

        private bool reachedToTarget = false;

        private int forwardLeyer = 0;
        private int backwardLeyer = 0;

        private readonly SearchData searchData = new SearchData();


        public WeightlessPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed, int maxDistance)
            : base(metadata, sourceId, targetId, directed, maxDistance)
        {
            searchData.MaxForwardDistance = maxDistance ;
            searchData.MaxBackwardDistance = (maxDistance + 1) / 2;
        }

        public override void FindPath()
        {
            searchData.AddToNodeSet(new WeightlessNodeData(SourceId, forwardLeyer, Seen.forward));
            searchData.AddToCurrentForwardNodes(SourceId);

            searchData.AddToNodeSet(new WeightlessNodeData(TargetId, backwardLeyer, Seen.backward));
            searchData.AddToCurrentBackwardNodes(TargetId);

            var forwardTask = new Task(ExpandForward);
//var backwardTask = new Task(ExpandBackward);
            forwardTask.Start();
            //backwardTask.Start();

            forwardTask.Wait();
          //  backwardTask.Wait();

            searchData.PathDistance = forwardLeyer + backwardLeyer;
        }




        private void ExpandBackward()
        {
            while (!reachedToTarget)
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
            while (!reachedToTarget)
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


            if (searchData.GetNode(sourceId).Distance == backwardLeyer)
            {
                searchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(targetId, weight));
            }


            lock (searchData)
            {
                if (searchData.GetNode(sourceId).Seen == Seen.forward)
                {
                    searchData.Joints.Add(sourceId);
                    searchData.GetNode(sourceId).AddBackwardAdjacent(new Adjacent(sourceId, weight));
                    reachedToTarget = true;
                }
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

            if (searchData.GetNode(targetId).Distance == forwardLeyer)
            {
                searchData.GetNode(targetId).AddAdjacent(new Adjacent(sourceId, weight));
            }
            lock (searchData)
            {
                if (searchData.GetNode(targetId).Seen == Seen.backward)
                {
                    searchData.Joints.Add(targetId);
                    searchData.GetNode(targetId).AddAdjacent(new Adjacent(sourceId, weight));
                    reachedToTarget = true;
                }
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
