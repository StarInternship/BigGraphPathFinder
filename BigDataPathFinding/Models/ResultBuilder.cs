using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public class ResultBuilder
    {
        private readonly IDatabase _database;
        private readonly Dictionary<Guid, NodeData> _nodeSet = new Dictionary<Guid, NodeData>();
        private readonly Graph _result = new Graph();

        public ResultBuilder(IDatabase database, Dictionary<Guid, NodeData> nodeSet)
        {
            _database = database;
            _nodeSet = nodeSet;
        }

        public Graph Build()
        {
            return _result;
        }
    }
}