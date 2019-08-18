using System;
using System.Diagnostics;
using BigDataPathFinding.Models;
using BigDataPathFinding.Models.ElasticGraph;
using BigDataPathFinding.Models.FileGraph;
using BigDataPathFinding.Models.Interfaces;
using BigDataPathFinding.Models.ShortestWeightless;

namespace BigDataPathFinding
{
    internal static class Program
    {
        private const Source Source = BigDataPathFinding.Source.Elastic;
        private const string TestFilesPath = @"../../../TestFiles/";
        private static readonly Stopwatch StopWatch = new Stopwatch();
        private static IDatabase _database;
        private static IMetadata _metadata;

        private static void Main()
        {
            StopWatch.Start();

            switch (Source)
            {
                case Source.Elastic:
                    _database = new ElasticDatabase("hard1_node_set");
                    _metadata = new ElasticMetadata("hard1_connections");
                    break;
                case Source.File:
                    _database = new FileGraph(TestFilesPath + "Hard1.txt");
                    _metadata = new FileMetadata((FileGraph) _database);
                    break;
            }

            StopWatch.Stop();

            Console.WriteLine("Get Ready After " + StopWatch.ElapsedMilliseconds + " ms.");

            while (true)
            {
                Console.Write("source: ");
                Guid sourceId, targetId;
                switch (Source)
                {
                    case Source.Elastic:
                        sourceId = new Guid(Console.ReadLine()?.Trim() ?? throw new InvalidOperationException());
                        break;
                    case Source.File:
                        sourceId = ((FileGraph) _database).GetId(Console.ReadLine());
                        break;
                }

                Console.Write("target: ");
                switch (Source)
                {
                    case Source.Elastic:
                        targetId = new Guid(Console.ReadLine()?.Trim() ?? throw new InvalidOperationException());
                        break;
                    case Source.File:
                        targetId = ((FileGraph) _database).GetId(Console.ReadLine());
                        break;
                }

                Console.Write("directed(1 or 0): ");
                var directed = Console.ReadLine() != "0";
                Console.Write("max distance: ");
                var maxDistance = int.Parse(Console.ReadLine());

                StopWatch.Reset();
                StopWatch.Start();

                AbstractPathFinder pathFinder =
                    new SingleThreadPathFinder(_metadata, sourceId, targetId, directed, maxDistance, 0);
                if (Source == Source.Elastic)
                {
                    ((ElasticMetadata) _metadata).NumberOfRequests = 0;
                }

                pathFinder.FindPath();

                StopWatch.Stop();
                Console.WriteLine("Finding Path Finished In " + StopWatch.ElapsedMilliseconds + "ms.");
                StopWatch.Reset();
                StopWatch.Start();

                var resultBuilder = new ResultBuilder(_database, pathFinder.GetSearchData());
                var edges = resultBuilder.Build().Edges;

                StopWatch.Stop();
                Console.WriteLine("Generating Graph Finished In " + StopWatch.ElapsedMilliseconds + "ms.");

                Console.WriteLine("path distance: " + pathFinder.GetSearchData().GetPathDistance());

                if (Source == Source.Elastic)
                {
                    Console.WriteLine("number of requests: " + ((ElasticMetadata) _metadata).NumberOfRequests);
                }

                Console.WriteLine("number of edges: " + edges.Count);
                /*foreach (var edge in edges)
                {
                    Console.WriteLine(_database.GetNode(edge.SourceId).Data.MakeString() + "," +
                                      _database.GetNode(edge.TargetId).Data.MakeString() + "," + edge.Weight);
                }*/

                Console.WriteLine();
            }
        }
    }

    internal enum Source
    {
        Elastic,
        File
    }
}