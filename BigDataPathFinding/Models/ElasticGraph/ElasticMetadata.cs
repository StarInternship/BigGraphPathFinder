using System;
using System.Collections.Generic;
using System.Diagnostics;
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
#if DEBUG
            settings.DisableDirectStreaming();
#endif
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

        private Stopwatch sw = new Stopwatch();

        public IEnumerable<IEnumerable<Edge>> GetOutputAdjacent(IEnumerable<Guid> ids)
        {
            Console.WriteLine("forward source count: " + ids.Count());
            NumberOfRequests++;
            sw.Restart();
            var search = _client.Search<Edge>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Should(sh => sh
                            .Terms(t => t.Field(edge => edge.SourceId).Terms(ids))
                        )
                    )
                )
                .Size(Size)
                .Scroll(Scroll)
            );
            var remaining = search.Total - search.Hits.Count;

            Console.WriteLine("result count: " + search.Total);
            Console.WriteLine("search time: " + sw.ElapsedMilliseconds + " ms.");
            yield return search.Documents;

            while (remaining > 0)
            {
                sw.Restart();
                search = _client.Scroll<Edge>(Scroll, search.ScrollId);
                remaining -= search.Hits.Count;
                Console.WriteLine("scroll time: " + sw.ElapsedMilliseconds + " ms.");
                yield return search.Documents;
            }
            Console.WriteLine();

            _client.ClearScroll(c => c.ScrollId(search.ScrollId));
        }

        public IEnumerable<IEnumerable<Edge>> GetInputAdjacent(IEnumerable<Guid> ids)
        {
            Console.WriteLine("backward source count: " + ids.Count());
            NumberOfRequests++;
            sw.Restart();
            var search = _client.Search<Edge>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Should(sh => sh
                            .Terms(t => t.Field(edge => edge.TargetId).Terms(ids))
                        )
                    )
                )
                .Size(Size)
                .Scroll(Scroll)
            );

            var remaining = search.Total - search.Hits.Count;

            Console.WriteLine("result count: " + search.Total);
            Console.WriteLine("search time: " + sw.ElapsedMilliseconds + " ms.");
            yield return search.Documents;

            while (remaining > 0)
            {
                sw.Restart();
                search = _client.Scroll<Edge>(Scroll, search.ScrollId);
                remaining -= search.Hits.Count;
                Console.WriteLine("scroll time: " + sw.ElapsedMilliseconds + " ms.");
                yield return search.Documents;
            }
            Console.WriteLine();

            _client.ClearScroll(c => c.ScrollId(search.ScrollId));
        }
    }
}
