using System;
using System.Collections.Generic;
using System.Linq;
using BigDataPathFinding.Models.Interfaces;

namespace BigDataPathFinding.Models.AllWeightLess
{
    internal class SingleThreadDFS : AbstractPathFinder
    {
        private readonly MyNode _destination;
        private readonly MyNode _origin;
        private readonly LinkedList<MyNode> _path = new LinkedList<MyNode>();
        private readonly HashSet<MyNode> _usedVertices = new HashSet<MyNode>();
        private readonly Graph _resultGraph = new Graph();

        public SingleThreadDFS(IMetadata metadata, Guid sourceId, Guid targetId, bool directed, int maxDistance,
            int minDistance) : base(metadata, sourceId, targetId, directed, maxDistance, minDistance)
        {
            var inputGraph = MyGraph.ReadGraph(sourceId, targetId, maxDistance, metadata, directed);
            _destination = inputGraph.GetNode(targetId);
            _origin = inputGraph.GetNode(sourceId);
        }

        public override Graph FindPath(IDatabase database)
        {
            GoThrough(_origin, MaxDistance, database);
            return _resultGraph;
        }

        private void GoThrough(MyNode next, int availableDepth, IDatabase database)
        {
            _path.AddLast(next);
            _usedVertices.Add(next);
            if (next == _destination)
            {
                AddResult(database);
            }
            else if (availableDepth != 0)
            {
                availableDepth--;
                foreach (var vertex in _path.Last().Outputs.Keys)
                {
                    if (!_usedVertices.Contains(vertex))
                    {
                        GoThrough(vertex, availableDepth, database);
                    }
                }
            }

            _path.RemoveLast();
            _usedVertices.Remove(next);
        }

        private void AddResult(IDatabase database)
        {
            var list = _path.ToList();
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (!_resultGraph.ContainsNode(list[i].Id))
                {
                    _resultGraph.AddNode(database.GetNode(list[i].Id));
                }
                if (!_resultGraph.ContainsNode(list[i+1].Id))
                {
                    _resultGraph.AddNode(database.GetNode(list[i+1].Id));
                }
                _resultGraph.AddEdge(list[i].Id, list[i+1].Id, 1);
            }
            /*foreach (var node in _path)
            {
                Console.Write(database.GetNode(node.Id).Data.MakeString() + " - ");
            }
            Console.WriteLine("-----");*/
        }
    }
}