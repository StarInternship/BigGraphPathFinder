using System;
using System.Collections.Generic;
using System.Linq;
using BigDataPathFinding.Models.Elastic;
using BigDataPathFinding.Models.Interfaces;
using Nest;

namespace BigDataPathFinding.Models.ElasticGraph
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

        public NodeInfo GetNode(Guid id)
        {
            var search = _client.Search<Dictionary<string, object>>(s => s
                .Query(q => q
                    .Ids(i => i
                        .Values(id)
                    )
                )
            ).Validate();
            return search.Total == 0 ? null : new NodeInfo(id, search.Documents.First());
        }
    }
}
