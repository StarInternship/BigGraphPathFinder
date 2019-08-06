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
            var searchResponse = _client.Search<Node>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(t => t.Id)
                        .Query(id.ToString())
                    )
                )
            );
            return searchResponse.Total == 0 ? null : searchResponse.Documents.First();
        }
    }
}
