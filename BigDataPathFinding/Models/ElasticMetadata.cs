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
        private const string Scroll = "5s";
        private readonly ElasticClient _client;

        public ElasticMetadata(string connectionsIndex)
        {
            var settings = new ConnectionSettings(Uri).DefaultIndex(connectionsIndex);
            _client = new ElasticClient(settings);
        }
        public int NumberOfRequests { get; set; } = 0;

        public IEnumerable<IEnumerable<Adjacent>> GetOutputAdjacent(Guid id)
        {
            NumberOfRequests++;
            var search = _client.Search<OutputAdjacent>(s => s
                .Source(src => src
                    .Includes(i => i.Fields(
                            f => f.TargetId,
                            f => f.Weight
                        ))
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
            
            yield return search.Documents;

            var current = _client.Scroll<OutputAdjacent>(Scroll, search.ScrollId);
            
            while(current.Hits.Count > 0)
            {
                yield return current.Documents;
                current = _client.Scroll<OutputAdjacent>(Scroll, search.ScrollId);
            }

            _client.ClearScroll(c => c.ScrollId(search.ScrollId));
        }

        public IEnumerable<IEnumerable<Adjacent>> GetInputAdjacent(Guid id)
        {
            NumberOfRequests++;
            var search = _client.Search<InputAdjacent>(s => s
                .Source(src => src
                    .Includes(i => i.Fields(
                        f => f.SourceId,
                        f => f.Weight
                    ))
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

            yield return search.Documents;

            var current = _client.Scroll<InputAdjacent>(Scroll, search.ScrollId);

            while (current.Hits.Count > 0)
            {
                yield return current.Documents;
                current = _client.Scroll<InputAdjacent>(Scroll, search.ScrollId);
            }

            _client.ClearScroll(c => c.ScrollId(search.ScrollId));
        }
    }
}
