﻿using System;
using System.Collections.Generic;
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
            var database = new FileGraph(testFilesPath + "hosein2.txt");
            var metadata = new FileMetadata(database);
            var sourceId = database.GetId("1");
            var targetId = database.GetId("2");

            var stopWatch = new Stopwatch();

            stopWatch.Start();
            PathFinder pathFinder = new MahdiPathFinder(metadata, sourceId, targetId, false);
            pathFinder.FindPath();
            stopWatch.Stop();
            Console.WriteLine("Finding Path Finished In " + stopWatch.ElapsedMilliseconds + "ms.");
            stopWatch.Reset();
            stopWatch.Start();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            var actual = resultBuilder.Build(targetId).Edges;
            Console.WriteLine("Generating Graph Finished In " + stopWatch.ElapsedMilliseconds + "ms.");
            foreach (var edge in actual)
            {
                Console.WriteLine(edge);
            }

            Console.WriteLine("******* Mahdi *********\n\n");

            stopWatch.Reset();
            stopWatch.Start();
            pathFinder = new HadiPathFinder(metadata, sourceId, targetId, false);
            pathFinder.FindPath();
            stopWatch.Stop();
            Console.WriteLine("Finding Path Finished In " + stopWatch.ElapsedMilliseconds + "ms.");
            stopWatch.Reset();
            stopWatch.Start();
            resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            actual = resultBuilder.Build(targetId).Edges;
            Console.WriteLine("Generating Graph Finished In " + stopWatch.ElapsedMilliseconds + "ms.");
            foreach (var edge in actual)
            {
                Console.WriteLine(edge);
            }
            Console.WriteLine("******** Hadi ********");
            Console.ReadKey();
        }
    }
}