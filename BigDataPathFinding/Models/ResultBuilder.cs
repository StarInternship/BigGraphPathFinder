using System;
using System.Collections.Generic;
using System.Linq;

namespace BigDataPathFinding.Models
{
    public class ResultBuilder
    {
        private readonly IDatabase _database;
        private readonly Dictionary<Guid, NodeData> nodeSet;
        private readonly Graph _result = new Graph();

        public ResultBuilder(IDatabase database, Dictionary<Guid, NodeData> nodeSet)
        {
            _database = database;
            this.nodeSet = nodeSet;
        }

        public Graph Build(Guid targetId)
        {
            if (!nodeSet.ContainsKey(targetId)) return _result;

            _result.AddNode(new ResultNode(_database.GetNode(targetId)));
            var currentNodes = new HashSet<NodeData> {nodeSet[targetId]};

            while (currentNodes.Count > 0)
            {
                var node = currentNodes.First();
                currentNodes.Remove(node);

                if (_result.Explored(node.Id)) continue;
                _result.Explore(node.Id);

                AddAdjacent(node, currentNodes);
            }

            return _result;
        }

        private void AddAdjacent(NodeData node, HashSet<NodeData> currentNodes)
        {
            foreach (var adjacent in node.PreviousAdjacent)
            {
                if (!_result.ContainsNode(adjacent.Id)) _result.AddNode(new ResultNode(_database.GetNode(adjacent.Id)));

                _result.AddEdge(adjacent.Id, node.Id, adjacent.Weight);
                currentNodes.Add(nodeSet[adjacent.Id]);
            }
        }
    }
}