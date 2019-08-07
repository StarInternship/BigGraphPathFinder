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
            var database=new ElasticDatabase("hosein2_node_set");
            var metadata=new ElasticMetadata("hosein2_connections");

            /*foreach (var adjacant in metadata.GetOutputAdjacent(new Guid("eaae729e-96a3-4010-8e2d-898e6822b14c")))
            {
                Console.WriteLine(adjacant.Id+"  "+adjacant.Weight);
            }
            Console.ReadKey();*/
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
                foreach (var edge in actual) Console.WriteLine(database.GetNode(edge.SourceId).Name+","+database.GetNode(edge.TargetId).Name+","+edge.Weight);
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
                foreach (var edge in actual) Console.WriteLine(database.GetNode(edge.SourceId).Name + "," + database.GetNode(edge.TargetId).Name + "," + edge.Weight);
                Console.WriteLine("******* Mahdi *********");

                stopWatch.Reset();
                stopWatch.Start();
            }
        }
    }
}