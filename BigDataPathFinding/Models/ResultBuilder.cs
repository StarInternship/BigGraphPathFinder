using System;
using System.Collections.Generic;
using System.Linq;

namespace BigDataPathFinding.Models
{
    public class ResultBuilder
    {
        private readonly IDatabase _database;
        private readonly Dictionary<Guid, NodeData> _nodeSet;
        private readonly Graph _result = new Graph();

        public ResultBuilder(IDatabase database, Dictionary<Guid, NodeData> nodeSet)
        {
            _database = database;
            _nodeSet = nodeSet;
        }

        public Graph Build(Guid targetId)
        {
            if (!_nodeSet.ContainsKey(targetId))
            {
                return _result;
            }

            _result.AddNode(new ResultNode(_database.GetNode(targetId)));
            var currentNodes = new HashSet<NodeData> { _nodeSet[targetId] };

            while (currentNodes.Count > 0)
            {
                var node = currentNodes.First();
                currentNodes.Remove(node);

                if (_result.Explored(node.Id)) continue;
                _result.Explore(node.Id);

                AddAdjacents(node, currentNodes);
            }

            return _result;
        }

        private void AddAdjacents(NodeData node, HashSet<NodeData> currentNodes)
        {
            foreach (var adjacent in node.PreviousAdjacents)
            {
                if (!_result.ContainsNode(adjacent.Id))
                {
                    _result.AddNode(new ResultNode(_database.GetNode(adjacent.Id)));
                }

                _result.AddEdge(node.Id, adjacent.Id, adjacent.Weight);
                currentNodes.Add(_nodeSet[adjacent.Id]);
            }
        }
    }
}
