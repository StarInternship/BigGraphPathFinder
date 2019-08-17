using System;
using System.Diagnostics;
using System.Threading;
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
                    _database = new ElasticDatabase("permutation10_node_set");
                    _metadata = new ElasticMetadata("permutation10_connections");
                    break;
                case Source.File:
                    _database = new FileGraph(TestFilesPath + "hosein2.txt");
                    _metadata = new FileMetadata((FileGraph)_database);
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
                        sourceId = ((FileGraph)_database).GetId(Console.ReadLine());
                        break;
                }

                Console.Write("target: ");
                switch (Source)
                {
                    case Source.Elastic:
                        targetId = new Guid(Console.ReadLine()?.Trim() ?? throw new InvalidOperationException());
                        break;
                    case Source.File:
                        targetId = ((FileGraph)_database).GetId(Console.ReadLine());
                        break;
                }

                Console.Write("directed(1 or 0): ");
                var directed = Console.ReadLine() != "0";
                Console.Write("max distance: ");
                var maxDistance = int.Parse(Console.ReadLine());

                long totalTime = 0;
                long sumOfSquares = 0;
                int pathDistance = 0;
                int edgesCount = 0;
                const double searchCount = 10;
                for (int i = 0; i < searchCount; i++)
                {
                    Console.Write(".");
                    StopWatch.Restart();
                    var pathFinder = new SingleThreadPathFinder(_metadata, sourceId, targetId, directed,maxDistance, 0);
                    (long t, int c, int d) = FindPath(pathFinder);
                    pathDistance = d;
                    edgesCount = c;
                    totalTime += t;
                    sumOfSquares += t * t;

                    Thread.Sleep(5000);
                }
                var average = totalTime / searchCount;
                Console.WriteLine();
                Console.WriteLine("edges count : " + edgesCount);
                Console.WriteLine("path distance : " + pathDistance);
                Console.WriteLine("Average time: " + average + " ms.");
                Console.WriteLine("Standard deviation of time: " + Math.Sqrt(((sumOfSquares / searchCount) - average * average)));
                Console.WriteLine();
            }
        }

        private static (long, int, int) FindPath(AbstractPathFinder pathFinder)
        {
            if (Source == Source.Elastic)
                ((ElasticMetadata)_metadata).NumberOfRequests = 0;

            pathFinder.FindPath();
            StopWatch.Stop();
            long time = StopWatch.ElapsedMilliseconds;
#if DEBUG
            Console.WriteLine("Finding Path Finished In " + stopWatch.ElapsedMilliseconds + "ms.");
#endif
            StopWatch.Reset();
            StopWatch.Start();
            var resultBuilder = new ResultBuilder(_database, pathFinder.GetSearchData());
            var edges = resultBuilder.Build().Edges;
            StopWatch.Stop();
#if DEBUG
            Console.WriteLine("Generating Graph Finished In " + stopWatch.ElapsedMilliseconds + "ms.");
            Console.WriteLine("path distance: " + pathFinder.GetSearchData().GetPathDistance());

            if (Source == Source.Elastic)
                Console.WriteLine("number of requests: " + ((ElasticMetadata)metadata).NumberOfRequests);
            Console.WriteLine("number of edges: " + edges.Count);
            foreach (var edge in edges)
                Console.WriteLine(database.GetNode(edge.SourceId).Data.MakeString() + "," + database.GetNode(edge.TargetId).Data.MakeString() + "," + edge.Weight);
#endif
            return (time, edges.Count, pathFinder.GetSearchData().GetPathDistance());
        }
    }

    internal enum Source
    {
        Elastic,
        File
    }
}