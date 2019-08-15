using System;
using System.Diagnostics;
using BigDataPathFinding.Models;
using BigDataPathFinding.Models.ElasticGraph;
using BigDataPathFinding.Models.FileGraph;
using BigDataPathFinding.Models.ShortestWeightless;

namespace BigDataPathFinding
{
    internal static class Program
    {
        private const Source Source = BigDataPathFinding.Source.Elastic;
        private const string TestFilesPath = @"../../../TestFiles/";
        private static readonly Stopwatch stopWatch = new Stopwatch();
        private static IDatabase database;
        private static IMetadata metadata;

        private static void Main()
        {
            stopWatch.Start();

            switch (Source)
            {
                case Source.Elastic:
                    database = new ElasticDatabase("permutation10_node_set");
                    metadata = new ElasticMetadata("permutation10_connections");
                    break;
                case Source.File:
                    database = new FileGraph(TestFilesPath + "hosein2.txt");
                    metadata = new FileMetadata((FileGraph)database);
                    break;
            }

            stopWatch.Stop();

            Console.WriteLine("Get Ready After " + stopWatch.ElapsedMilliseconds + " ms.");

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
                        sourceId = ((FileGraph)database).GetId(Console.ReadLine());
                        break;
                }

                Console.Write("target: ");
                switch (Source)
                {
                    case Source.Elastic:
                        targetId = new Guid(Console.ReadLine()?.Trim() ?? throw new InvalidOperationException());
                        break;
                    case Source.File:
                        targetId = ((FileGraph)database).GetId(Console.ReadLine());
                        break;
                }

                Console.Write("directed(1 or 0): ");
                var directed = Console.ReadLine() != "0";
                Console.Write("max distance: ");
                var maxDistance = int.Parse(Console.ReadLine());

                stopWatch.Restart();
                var pathFinder = new WeightlessPathFinder(metadata, sourceId, targetId, directed, maxDistance);
                FindPath(pathFinder);
                Console.WriteLine("******* Weightless *********\n\n");
            }
        }

        private static void FindPath(AbstractPathFinder pathFinder)
        {
            if (Source == Source.Elastic)
                ((ElasticMetadata)metadata).NumberOfRequests = 0;

            pathFinder.FindPath();
            Console.WriteLine("Finding Path Finished In " + stopWatch.ElapsedMilliseconds + "ms.");

            stopWatch.Restart();
            var edges = new ResultBuilder(database, pathFinder.GetSearchData()).Build().Edges;
            Console.WriteLine("Generating Graph Finished In " + stopWatch.ElapsedMilliseconds + "ms.");
            Console.WriteLine("path distance: " + pathFinder.GetSearchData().GetPathDistance());

            if (Source == Source.Elastic)
                Console.WriteLine("number of requests: " + ((ElasticMetadata)metadata).NumberOfRequests);
            Console.WriteLine("number of edges: " + edges.Count);

            if (edges.Count > 100) return;

            foreach (var edge in edges)
                Console.WriteLine(database.GetNode(edge.SourceId).Data.MakeString() + "," + database.GetNode(edge.TargetId).Data.MakeString() + "," + edge.Weight);
        }
    }

    internal enum Source
    {
        Elastic,
        File
    }
}