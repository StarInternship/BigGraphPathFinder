using System;
using System.Collections.Generic;
using System.Linq;
using Nest;

namespace BigDataPathFinding.Models
{
    public class ElasticMetadata : IMetadata
    {
        private static readonly Uri Uri = new Uri($"http://localhost:9200");
        private const int Size = 10000;
        private const string Scroll = "10s";
        private readonly ElasticClient _client;

        public ElasticMetadata(string connectionsIndex)
        {
            var settings = new ConnectionSettings(Uri).DefaultIndex(connectionsIndex);
            _client = new ElasticClient(settings);
        }

        public IEnumerable<Adjacent> GetOutputAdjacent(Guid id)
        {
            var search = _client.Search<Edge>(s => s
                .StoredFields(sf => sf
                    .Fields(
                        f => f.TargetId,
                        f => f.Weight
                    )
                )
                .Query(q => q
                    .Match(m => m
                        .Field(t => t.SourceId)
                        .Query(id.ToString())
                    )
                )
                .Size(Size)
                .Scroll(Scroll)
            );
            return search.Documents.Select(edge => new Adjacent(edge.TargetId, edge.Weight));
        }

        public IEnumerable<Adjacent> GetInputAdjacent(Guid id)
        {
            var search = _client.Search<Edge>(s => s
                .StoredFields(sf => sf
                    .Fields(
                        f => f.SourceId,
                        f => f.Weight
                    )
                )
                .Query(q => q
                    .Match(m => m
                        .Field(t => t.TargetId)
                        .Query(id.ToString())
                    )
                )
                .Size(Size)
                .Scroll(Scroll)
            );
            return search.Documents.Select(edge => new Adjacent(edge.SourceId, edge.Weight));
        }
    }
}
