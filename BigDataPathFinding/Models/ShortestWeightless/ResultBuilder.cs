using System.Collections.Generic;
using System.Linq;
using BigDataPathFinding.Models.Interfaces;

namespace BigDataPathFinding.Models
{
    public class ResultBuilder
    {
        private readonly IDatabase _database;
        private readonly Graph _result = new Graph();
        private readonly ISearchData _searchData;

        public ResultBuilder(IDatabase database, ISearchData searchData)
        {
            _database = database;
            _searchData = searchData;
        }

        public Graph Build()
        {
            if (_searchData.GetJoints().Count == 0)
            {
                return _result;
            }

            var currentNodes = new HashSet<NodeData>();

            foreach (var guid in _searchData.GetJoints())
            {
                _result.AddNode(new NodeInfo(_database.GetNode(guid)));
                currentNodes.Add(_searchData.GetResultNodeSet()[guid]);
            }


            while (currentNodes.Count > 0)
            {
                var node = currentNodes.First();
                currentNodes.Remove(node);

                if (_result.Explored(node.Id))
                {
                    continue;
                }

                _result.Explore(node.Id);

                AddAdjacent(node, currentNodes);
            }

            return _result;
        }

        private void AddAdjacent(NodeData node, ISet<NodeData> currentNodes)
        {
            foreach (var adjacent in node.PreviousAdjacent)
            {
                if (!_result.ContainsNode(adjacent.Id))
                {
                    _result.AddNode(new NodeInfo(_database.GetNode(adjacent.Id)));
                }

                _result.AddEdge(adjacent.Id, node.Id, adjacent.Weight);
                currentNodes.Add(_searchData.GetResultNodeSet()[adjacent.Id]);
            }

            foreach (var adjacent in node.ForwardAdjacent)
            {
                if (!_result.ContainsNode(adjacent.Id))
                {
                    _result.AddNode(new NodeInfo(_database.GetNode(adjacent.Id)));
                }

                _result.AddEdge(node.Id, adjacent.Id, adjacent.Weight);
                currentNodes.Add(_searchData.GetResultNodeSet()[adjacent.Id]);
            }
        }
    }
}