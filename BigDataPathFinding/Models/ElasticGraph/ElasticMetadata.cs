using System;
using System.Collections.Generic;
using System.Diagnostics;
using BigDataPathFinding.Models.Elastic;
using BigDataPathFinding.Models.Interfaces;
using Nest;

namespace BigDataPathFinding.Models.ElasticGraph
{
    public class ElasticMetadata : IMetadata
    {
        private const int Size = 10000;
        private const string Scroll = "500s";
        private static readonly Uri Uri = new Uri("http://localhost:9200");
        private readonly ElasticClient _client;
        private readonly Stopwatch _sw = new Stopwatch();

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
            ).Validate();
            var remaining = search.Total - search.Hits.Count;

            yield return search.Documents;

            while (remaining > 0)
            {
                search = _client.Scroll<OutputAdjacent>(Scroll, search.ScrollId).Validate();
                remaining -= search.Hits.Count;
                yield return search.Documents;
            }

            _client.ClearScroll(c => c.ScrollId(search.ScrollId)).Validate();
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
            ).Validate();
            var remaining = search.Total - search.Hits.Count;

            yield return search.Documents;

            while (remaining > 0)
            {
                search = _client.Scroll<InputAdjacent>(Scroll, search.ScrollId).Validate();
                remaining -= search.Hits.Count;
                yield return search.Documents;
            }

            _client.ClearScroll(c => c.ScrollId(search.ScrollId)).Validate();
        }

        public IEnumerable<IEnumerable<Adjacent>> GetAllAdjacent(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEnumerable<Edge>> GetOutputEdges(IEnumerable<Guid> ids)
        {
#if DEBUG
            Console.WriteLine("output source count: " + ids.Count());
#endif
            NumberOfRequests++;
            _sw.Restart();
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
            ).Validate();
            var remaining = search.Total - search.Hits.Count;
#if DEBUG
            Console.WriteLine("result count: " + search.Total);
            Console.WriteLine("search time: " + _sw.ElapsedMilliseconds + " ms.");
#endif
            yield return search.Documents;

            while (remaining > 0)
            {
                _sw.Restart();
                search = _client.Scroll<Edge>(Scroll, search.ScrollId).Validate();
                remaining -= search.Hits.Count;
#if DEBUG
                Console.WriteLine("scroll time: " + _sw.ElapsedMilliseconds + " ms.");
#endif
                yield return search.Documents;
            }
#if DEBUG
            Console.WriteLine();
#endif

            _client.ClearScroll(c => c.ScrollId(search.ScrollId)).Validate();
        }

        public IEnumerable<IEnumerable<Edge>> GetInputEdges(IEnumerable<Guid> ids)
        {
#if DEBUG
            Console.WriteLine("input source count: " + ids.Count());
#endif
            NumberOfRequests++;
            _sw.Restart();
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
            ).Validate();

            var remaining = search.Total - search.Hits.Count;
#if DEBUG
            Console.WriteLine("result count: " + search.Total);
            Console.WriteLine("search time: " + _sw.ElapsedMilliseconds + " ms.");
#endif
            yield return search.Documents;

            while (remaining > 0)
            {
                _sw.Restart();
                search = _client.Scroll<Edge>(Scroll, search.ScrollId).Validate();
                remaining -= search.Hits.Count;
#if DEBUG
                Console.WriteLine("scroll time: " + _sw.ElapsedMilliseconds + " ms.");
#endif
                yield return search.Documents;
            }
#if DEBUG
            Console.WriteLine();
#endif

            _client.ClearScroll(c => c.ScrollId(search.ScrollId)).Validate();
        }

        public IEnumerable<IEnumerable<Edge>> GetAllEdges(IEnumerable<Guid> ids)
        {
#if DEBUG
            Console.WriteLine("input-output source count: " + ids.Count());
#endif
            NumberOfRequests++;
            _sw.Restart();
            var search = _client.Search<Edge>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Should(
                            sh => sh.Terms(t => t.Field(edge => edge.TargetId).Terms(ids)),
                            sh => sh.Terms(t => t.Field(edge => edge.SourceId).Terms(ids))
                        )
                    )
                )
                .Size(Size)
                .Scroll(Scroll)
            ).Validate();

            var remaining = search.Total - search.Hits.Count;
#if DEBUG
            Console.WriteLine("result count: " + search.Total);
            Console.WriteLine("search time: " + _sw.ElapsedMilliseconds + " ms.");
#endif
            yield return search.Documents;

            while (remaining > 0)
            {
                _sw.Restart();
                search = _client.Scroll<Edge>(Scroll, search.ScrollId).Validate();
                remaining -= search.Hits.Count;
#if DEBUG
                Console.WriteLine("scroll time: " + _sw.ElapsedMilliseconds + " ms.");
#endif
                yield return search.Documents;
            }
#if DEBUG
            Console.WriteLine();
#endif

            _client.ClearScroll(c => c.ScrollId(search.ScrollId)).Validate();
        }
    }
}