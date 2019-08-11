using System;
using System.Collections.Generic;
using System.Linq;
using Nest;

namespace BigDataPathFinding.Models.ElasticGraph
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
        public int NumberOfRequests { get; set; }

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
            var remaining = search.Total - search.Hits.Count;

            yield return search.Documents;

            while (remaining > 0)
            {
                search = _client.Scroll<OutputAdjacent>(Scroll, search.ScrollId);
                remaining -= search.Hits.Count;
                yield return search.Documents;
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
            var remaining = search.Total - search.Hits.Count;

            yield return search.Documents;

            while (remaining > 0)
            {
                search = _client.Scroll<InputAdjacent>(Scroll, search.ScrollId);
                remaining -= search.Hits.Count;
                yield return search.Documents;
            }

            _client.ClearScroll(c => c.ScrollId(search.ScrollId));
        }

        public IEnumerable<IEnumerable<Edge>> GetOutputAdjacent(IEnumerable<Guid> ids)
        {
            if (!ids.Any()) yield break;
            NumberOfRequests++;
            var search = _client.Search<Edge>(s => s
                .Query(q => q
                   .Bool(b => new BoolQuery
                   {
                       Should = ids.Select(id => new QueryContainer(new MatchQuery { Field = new Field("sourceId"), Query = id.ToString() }))
                   })
                )
                .Size(Size)
                .Scroll(Scroll)
            );
            var remaining = search.Total - search.Hits.Count;

            yield return search.Documents;

            while (remaining > 0)
            {
                search = _client.Scroll<Edge>(Scroll, search.ScrollId);
                remaining -= search.Hits.Count;
                yield return search.Documents;
            }

            _client.ClearScroll(c => c.ScrollId(search.ScrollId));
        }

        public IEnumerable<IEnumerable<Edge>> GetInputAdjacent(IEnumerable<Guid> ids)
        {
            if (!ids.Any()) yield break;
            NumberOfRequests++;
            var search = _client.Search<Edge>(s => s
                .Query(q => q
                   .Bool(b => new BoolQuery
                   {
                       Should = ids.Select(id => new QueryContainer(new MatchQuery { Field = new Field("targetId"), Query = id.ToString() }))
                   })
                )
                .Size(Size)
                .Scroll(Scroll)
            );
            var remaining = search.Total - search.Hits.Count;

            yield return search.Documents;

            while (remaining > 0)
            {
                search = _client.Scroll<Edge>(Scroll, search.ScrollId);
                remaining -= search.Hits.Count;
                yield return search.Documents;
            }

            _client.ClearScroll(c => c.ScrollId(search.ScrollId));
        }
    }
}
