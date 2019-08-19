using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BigDataPathFinding.Models.Elastic;
using BigDataPathFinding.Models.Interfaces;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    //has bugs
    public class MultiThreadPathFinder : AbstractPathFinder
    {
        private readonly object _distanceLock = new object();
        private readonly object _targetLock = new object();

        private readonly SearchData _searchData = new SearchData();
        private int _backwardDepth;
        private int _forwardDepth;
        private bool _reachedToTarget;

        public MultiThreadPathFinder(IMetadata metadata, Guid sourceId, Guid targetId, bool directed,
            int maxDistance, int minDistance)
            : base(metadata, sourceId, targetId, directed, maxDistance, minDistance)
        {
        }

        private bool ReachedToTarget
        {
            get
            {
                lock (_targetLock)
                {
                    return _reachedToTarget;
                }
            }
            set
            {
                lock (_targetLock)
                {
                    _reachedToTarget = value;
                }
            }
        }

        private int PathDistance
        {
            get
            {
                lock (_distanceLock)
                {
                    return _searchData.PathDistance;
                }
            }
            set
            {
                lock (_distanceLock)
                {
                    _searchData.PathDistance = value;
                }
            }
        }

        public override void FindPath()
        {
            _searchData.AddToNodeSet(new NodeData(SourceId, _forwardDepth, Seen.Forward));
            _searchData.AddToCurrentForwardNodes(SourceId);

            _searchData.AddToNodeSet(new NodeData(TargetId, _backwardDepth, Seen.Backward));
            _searchData.AddToCurrentBackwardNodes(TargetId);

            var forwardTask = new Task(ExpandForward);
            var backwardTask = new Task(ExpandBackward);

            forwardTask.Start();
            backwardTask.Start();

            Task.WaitAll(forwardTask, backwardTask);
        }

        private void ExpandBackward()
        {
            while (!ReachedToTarget && _searchData.CurrentBackwardNodes.Count > 0 && _backwardDepth + _forwardDepth < MaxDistance)
            {
                _backwardDepth++;
                var nextLayerNodes = new HashSet<Guid>();

                var listOfEdges = Directed
                    ? Metadata.GetInputEdges(_searchData.CurrentBackwardNodes)
                    : Metadata.GetAllEdges(_searchData.CurrentBackwardNodes);

                if (Directed)
                {
                    foreach (var edges in listOfEdges)
                    {
                        foreach (var edge in edges)
                        {
                            VisitBackwardEdge(nextLayerNodes, edge);
                        }
                    }
                }
                else
                {
                    foreach (var edges in listOfEdges)
                    {
                        foreach (var edge in edges)
                        {
                            if (_searchData.CurrentBackwardNodes.Contains(edge.TargetId))
                            {
                                VisitBackwardEdge(nextLayerNodes, edge);
                            }

                            if (_searchData.CurrentBackwardNodes.Contains(edge.SourceId))
                            {
                                VisitBackwardEdge(nextLayerNodes, edge.Reversed());
                            }
                        }
                    }
                }

                _searchData.UpdateCurrentBackwardNodes(nextLayerNodes);
            }
            _searchData.ClearCurrentBackwardNodes();
        }

        private void ExpandForward()
        {
            while (!ReachedToTarget && _searchData.CurrentForwardNodes.Count > 0 && _backwardDepth + _forwardDepth < MaxDistance)
            {
                _forwardDepth++;
                var nextLayerNodes = new HashSet<Guid>();

                var listOfEdges = Directed
                    ? Metadata.GetOutputEdges(_searchData.CurrentBackwardNodes)
                    : Metadata.GetAllEdges(_searchData.CurrentBackwardNodes);

                if (Directed)
                {
                    foreach (var edges in listOfEdges)
                    {
                        foreach (var edge in edges)
                        {
                            VisitForwardEdge(nextLayerNodes, edge);
                        }
                    }
                }
                else
                {
                    foreach (var edges in listOfEdges)
                    {
                        foreach (var edge in edges)
                        {
                            if (_searchData.CurrentBackwardNodes.Contains(edge.TargetId))
                            {
                                VisitForwardEdge(nextLayerNodes, edge.Reversed());
                            }

                            if (_searchData.CurrentBackwardNodes.Contains(edge.SourceId))
                            {
                                VisitForwardEdge(nextLayerNodes, edge);
                            }
                        }
                    }
                }
                _searchData.UpdateCurrentBackwardNodes(nextLayerNodes);
            }
            _searchData.ClearCurrentBackwardNodes();
        }

        private void VisitBackwardEdge(ISet<Guid> nextLayerNodes, Edge edge)
        {
            NodeData sourceNode,targetNode;
            lock (_searchData)
            {
                sourceNode = _searchData.GetNode(edge.SourceId);
                targetNode= _searchData.GetNode(edge.TargetId);
                if (sourceNode == null)
                {
                    _searchData.AddToNodeSet(new NodeData(edge.SourceId, _backwardDepth, Seen.Backward));
                    nextLayerNodes.Add(edge.SourceId);
                    sourceNode = _searchData.GetNode(edge.SourceId);
                }
            }

            if (Math.Abs(sourceNode.Distance - _backwardDepth) < 0.01 &&
                targetNode.Seen == Seen.Backward)
            {
                sourceNode.AddForwardAdjacent(edge);
            }

            if (sourceNode.Seen == Seen.Forward &&
                (Math.Abs(PathDistance - (sourceNode.Distance + targetNode.Distance + 1)) < 0.01 || PathDistance == 0))
            {
                _searchData.Joints.Add(edge.SourceId);
                sourceNode.AddForwardAdjacent(edge);
                ReachedToTarget = true;
                PathDistance = (int) sourceNode.Distance +
                               (int) targetNode.Distance + 1;
                MaxDistance = PathDistance;
            }
        }

        private void VisitForwardEdge(ISet<Guid> nextLayerNodes, Edge edge)
        {
            NodeData sourceNode, targetNode;
            lock (_searchData)
            {
                sourceNode = _searchData.GetNode(edge.SourceId);
                targetNode = _searchData.GetNode(edge.TargetId);
                if (targetNode == null)
                {
                    _searchData.AddToNodeSet(new NodeData(edge.TargetId, _forwardDepth, Seen.Forward));
                    nextLayerNodes.Add(edge.TargetId);
                    targetNode = _searchData.GetNode(edge.TargetId);
                }
            }

            if (Math.Abs(targetNode.Distance - _forwardDepth) < 0.01 &&
                sourceNode.Seen == Seen.Forward)
            {
                targetNode.AddBackwardAdjacent(edge);
            }

            if (targetNode.Seen == Seen.Backward &&
                (Math.Abs(PathDistance - (sourceNode.Distance + targetNode.Distance + 1)) < 0.01 || PathDistance == 0))
            {
                _searchData.Joints.Add(edge.TargetId);
                targetNode.AddBackwardAdjacent(edge);
                ReachedToTarget = true;
                PathDistance = (int) sourceNode.Distance +
                               (int) targetNode.Distance + 1;
                MaxDistance = PathDistance;
            }
        }

        public override ISearchData GetSearchData()
        {
            return _searchData;
        }
    }
}