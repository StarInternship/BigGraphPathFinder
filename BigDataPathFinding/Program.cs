using System;
using System.Diagnostics;
using BigDataPathFinding.Models;
using BigDataPathFinding.Models.AllWeightLess;
using BigDataPathFinding.Models.ElasticGraph;
using BigDataPathFinding.Models.FileGraph;
using BigDataPathFinding.Models.Interfaces;

namespace BigDataPathFinding
{
    internal static class Program
    {
        private const Source Source = BigDataPathFinding.Source.File;
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
                    _database = new ElasticDatabase("hosein2_node_set");
                    _metadata = new ElasticMetadata("hosein2_connections");
                    break;
                case Source.File:
                    _database = new FileGraph(TestFilesPath + "hosein2.txt");
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
                    new SingleThreadDFS(_metadata, sourceId, targetId, directed, maxDistance, 0);
                if (Source == Source.Elastic)
                {
                    ((ElasticMetadata) _metadata).NumberOfRequests = 0;
                }


                var resultGraph = pathFinder.FindPath(_database);
                var edges = resultGraph.Edges;

                StopWatch.Stop();
                Console.WriteLine("Finding Result Graph Finished In " + StopWatch.ElapsedMilliseconds + "ms.");


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