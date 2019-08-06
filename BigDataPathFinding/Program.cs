using System;
using System.Diagnostics;
using BigDataPathFinding.Models;
using BigDataPathFinding.Models.FileGraph;
using BigDataPathFinding.Models.Hadi;
using BigDataPathFinding.Models.Mahdi;

namespace BigDataPathFinding
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            const string testFilesPath = @"../../../TestFiles/";
            /*var database = new FileGraph(testFilesPath + "Test1.txt");
            var metadata = new FileMetadata(database);*/
            var database=new ElasticDatabase("test1_node_set");
            var metadata=new ElasticMetadata("test1_connections");

            while (true)
            {
                Console.Write("source: ");
                var sourceId = new Guid(Console.ReadLine().Trim());
                Console.Write("target: ");
                var targetId = new Guid(Console.ReadLine().Trim());
                Console.Write("directed(1 or 0): ");
                var directed = Console.ReadLine() != "0";

                var stopWatch = new Stopwatch();

                stopWatch.Start();

                PathFinder pathFinder = new HadiPathFinder(metadata, sourceId, targetId, directed);
                pathFinder.FindPath();
                stopWatch.Stop();
                Console.WriteLine("Finding Path Finished In " + stopWatch.ElapsedMilliseconds + "ms.");
                stopWatch.Reset();
                stopWatch.Start();
                var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
                var actual = resultBuilder.Build(targetId).Edges;
                Console.WriteLine("Generating Graph Finished In " + stopWatch.ElapsedMilliseconds + "ms.");
                foreach (var edge in actual) Console.WriteLine(edge);
                Console.WriteLine("******** Hadi ********\n\n");


                pathFinder = new MahdiPathFinder(metadata, sourceId, targetId, directed);
                pathFinder.FindPath();
                stopWatch.Stop();
                Console.WriteLine("Finding Path Finished In " + stopWatch.ElapsedMilliseconds + "ms.");
                stopWatch.Reset();
                stopWatch.Start();
                resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
                actual = resultBuilder.Build(targetId).Edges;
                Console.WriteLine("Generating Graph Finished In " + stopWatch.ElapsedMilliseconds + "ms.");
                foreach (var edge in actual) Console.WriteLine(edge);
                Console.WriteLine("******* Mahdi *********");

                stopWatch.Reset();
                stopWatch.Start();
            }
        }
    }
}