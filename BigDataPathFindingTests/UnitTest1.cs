using System;
using System.Collections.Generic;
using BigDataPathFinding.Models;
using BigDataPathFinding.Models.FileGraph;
using BigDataPathFinding.Models.Interfaces;
using BigDataPathFinding.Models.ShortestWeightless;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BigDataPathFindingTests
{
    [TestClass]
    public class FileShortestWeightlessPathFinderSingleThread
    {
        private const string TestFilesPath = @"../../../TestFiles/";
        private static IDatabase _database;
        private static IMetadata _metadata;

        [TestMethod]
        public void DirectedTest1()
        {
            _database = new FileGraph(TestFilesPath + "NewTest1.test");
            _metadata = new FileMetadata((FileGraph) _database);

            var sourceId = ((FileGraph) _database).GetId("A");


            var targetId = ((FileGraph) _database).GetId("H");

            const bool directed = false;
            const int maxDistance = 3;


            var pathFinder = new SingleThreadPathFinder(_metadata, sourceId, targetId, directed, maxDistance, 0);


            var resultGraph = pathFinder.FindPath(_database);
            var edges = resultGraph.Edges;

            Assert.IsTrue(edges.Count == 4);
            var results = new HashSet<string>();
            foreach (var edge in edges)
            {
                results.Add(_database.GetNode(edge.SourceId).Data.MakeString() + "," +
                            _database.GetNode(edge.TargetId).Data.MakeString() + "," + edge.Weight);
            }

            Assert.IsTrue(results.Contains("{[A] },{[D] },2"));
            Assert.IsTrue(results.Contains("{[D] },{[H] },1"));
            Assert.IsTrue(results.Contains("{[A] },{[I] },5"));
            Assert.IsTrue(results.Contains("{[I] },{[H] },1"));
        }

        [TestMethod]
        public void InDirectedTest1()
        {
            var sourceId = ((FileGraph) _database).GetId("A");


            var targetId = ((FileGraph) _database).GetId("H");

            const bool directed = true;
            const int maxDistance = 6;


            var pathFinder = new SingleThreadPathFinder(_metadata, sourceId, targetId, directed, maxDistance, 0);

            var resultBuilder = pathFinder.FindPath(_database);
            var edges = resultBuilder.Edges;

            Assert.IsTrue(edges.Count == 5);
            var results = new HashSet<string>();
            foreach (var edge in edges)
            {
                results.Add(_database.GetNode(edge.SourceId).Data.MakeString() + "," +
                            _database.GetNode(edge.TargetId).Data.MakeString() + "," + edge.Weight);
            }

            Assert.IsTrue(results.Contains("{[C] },{[E] },1"));
            Assert.IsTrue(results.Contains("{[E] },{[D] },1"));
            Assert.IsTrue(results.Contains("{[B] },{[C] },1"));
            Assert.IsTrue(results.Contains("{[A] },{[B] },1"));
            Assert.IsTrue(results.Contains("{[D] },{[H] },1"));
        }

        [TestMethod]
        public void InDirectedTest2()
        {
            var sourceId = ((FileGraph) _database).GetId("A");


            var targetId = ((FileGraph) _database).GetId("H");

            const bool directed = true;
            const int maxDistance = 4;


            var pathFinder = new SingleThreadPathFinder(_metadata, sourceId, targetId, directed, maxDistance, 0);

            var resultBuilder = pathFinder.FindPath(_database);
            var edges = resultBuilder.Edges;

            Assert.IsTrue(edges.Count == 0);
        }
    }
}