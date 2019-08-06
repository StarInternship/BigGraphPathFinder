using System;
using System.Linq;
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

        public Node GetNode(Guid id)
        {
            var search = _client.Search<Node>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(node => node.Id)
                        .Query(id.ToString())
                    )
                )
                .Size(1)
            );
            return search.Total == 0 ? null : search.Documents.First();
        }
    }
}
