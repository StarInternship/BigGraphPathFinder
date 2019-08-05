using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace BigDataPathFinding.Models
{
    public class ElasticDatabase : IDatabase
    {
        private static readonly Uri Uri = new Uri($"http://localhost:9200");
        private readonly ElasticClient _client;

        public ElasticDatabase(string nodeSetIndex)
        {
            var settings = new ConnectionSettings(Uri).DefaultIndex(nodeSetIndex);
            _client = new ElasticClient(settings);
        }

        public Node GetNode(Guid id) // I THINK IT'S NOT GOOD AT ALL.
        {
            var searchResponse = _client.Search<Node>(s => s.Query(q => q.Match(m => m.Field(t => t.Id).Query(id.ToString()))));
            return searchResponse.Documents.First();
        }
    }
}
