using System.Collections.Generic;
using BigDataPathFinding.Models;
using BigDataPathFinding.Models.FileGraph;
using BigDataPathFinding.Models.Hadi;
using BigDataPathFinding.Models.Mahdi;
using BigDataPathFinding.Models.ShortestWeightless;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BigDataPathFindingTests.Models
{
    [TestClass]
    public class Mahdi
    {
        private const string TestFilesPath = @"../../../TestFiles/";

        [TestMethod]
        public void EasyGraph()
        {
            var database = new FileGraph(TestFilesPath + "EasyGraph.csv");
            var metadata = new FileMetadata(database);
            var sourceId = database.GetId("0");
            var targetId = database.GetId("4");
            var pathFinder = new MahdiPathFinder(metadata, sourceId, targetId, true);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            var actual = resultBuilder.Build(targetId).Edges;
            var expected = new HashSet<Edge>
            {
                new Edge(database.GetId("0"), database.GetId("1"), 1),
                new Edge(database.GetId("1"), database.GetId("3"), 1),
                new Edge(database.GetId("0"), database.GetId("3"), 2),
                new Edge(database.GetId("3"), database.GetId("4"), 1)
            };
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod]
        public void BigGraph()
        {
            var database = new FileGraph(TestFilesPath + "BigGraph.csv");
            var metadata = new FileMetadata(database);
            var sourceId = database.GetId("0");
            var targetId = database.GetId("4");
            var pathFinder = new MahdiPathFinder(metadata, sourceId, targetId, true);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            var actual = resultBuilder.Build(targetId).Edges;
            var expected = new HashSet<Edge>
            {
                new Edge(database.GetId("0"), database.GetId("648"), 20),
                new Edge(database.GetId("648"), database.GetId("3"), 13),
                new Edge(database.GetId("3"), database.GetId("731"), 1),
                new Edge(database.GetId("731"), database.GetId("525"), 8),
                new Edge(database.GetId("525"), database.GetId("4"), 8),
            };
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod]
        public void ALittleComplicated()
        {
            var database = new FileGraph(TestFilesPath + "ALittleComplicated.csv");
            var metadata = new FileMetadata(database);
            var sourceId = database.GetId("0");
            var targetId = database.GetId("4");
            var pathFinder = new MahdiPathFinder(metadata, sourceId, targetId, false);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            var actual = resultBuilder.Build(targetId).Edges;
            var expected = new HashSet<Edge>
            {
                new Edge(database.GetId("0"), database.GetId("2"), 1),
                new Edge(database.GetId("0"), database.GetId("1"), 1),
                new Edge(database.GetId("0"), database.GetId("3"), 2),
                new Edge(database.GetId("1"), database.GetId("3"), 1),
                new Edge(database.GetId("2"), database.GetId("3"), 1),
                new Edge(database.GetId("3"), database.GetId("4"), 1),
            };
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod]
        public void MahdiGraph()
        {
            var database = new FileGraph(TestFilesPath + "mahdi.csv");
            var metadata = new FileMetadata(database);
            var sourceId = database.GetId("A");
            var targetId = database.GetId("F");
            var pathFinder = new MahdiPathFinder(metadata, sourceId, targetId, true);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            var actual = resultBuilder.Build(targetId).Edges;
            Assert.IsTrue(actual.Count == 6);
        }

        [TestMethod]
        public void VisitedGraph()
        {
            var database = new FileGraph(TestFilesPath + "VisitedGraph.csv");
            var metadata = new FileMetadata(database);
            var sourceId = database.GetId("0");
            var targetId = database.GetId("4");
            var pathFinder = new MahdiPathFinder(metadata, sourceId, targetId, true);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            var actual = resultBuilder.Build(targetId).Edges;
            var expected = new HashSet<Edge>
            {
                new Edge(database.GetId("0"), database.GetId("2"), 1),
                new Edge(database.GetId("2"), database.GetId("1"), 1),
                new Edge(database.GetId("1"), database.GetId("3"), 1),
                new Edge(database.GetId("3"), database.GetId("4"), 20),
            };
            Assert.IsTrue(expected.SetEquals(actual));
        }
    }

    [TestClass]
    public class Hadi
    {
        private const string TestFilesPath = @"../../../TestFiles/";

        [TestMethod]
        public void EasyGraph()
        {
            var database = new FileGraph(TestFilesPath + "EasyGraph.csv");
            var metadata = new FileMetadata(database);
            var sourceId = database.GetId("0");
            var targetId = database.GetId("4");
            var pathFinder = new HadiPathFinder(metadata, sourceId, targetId, true);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            var actual = resultBuilder.Build(targetId).Edges;
            var expected = new HashSet<Edge>
            {
                new Edge(database.GetId("0"), database.GetId("1"), 1),
                new Edge(database.GetId("1"), database.GetId("3"), 1),
                new Edge(database.GetId("0"), database.GetId("3"), 2),
                new Edge(database.GetId("3"), database.GetId("4"), 1)
            };
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod]
        public void ALittleComplicated()
        {
            var database = new FileGraph(TestFilesPath + "ALittleComplicated.csv");
            var metadata = new FileMetadata(database);
            var sourceId = database.GetId("0");
            var targetId = database.GetId("4");
            var pathFinder = new HadiPathFinder(metadata, sourceId, targetId, false);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            var actual = resultBuilder.Build(targetId).Edges;
            var expected = new HashSet<Edge>
            {
                new Edge(database.GetId("0"), database.GetId("2"), 1),
                new Edge(database.GetId("0"), database.GetId("1"), 1),
                new Edge(database.GetId("0"), database.GetId("3"), 2),
                new Edge(database.GetId("1"), database.GetId("3"), 1),
                new Edge(database.GetId("2"), database.GetId("3"), 1),
                new Edge(database.GetId("3"), database.GetId("4"), 1),
            };
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod]
        public void BigGraph()
        {
            var database = new FileGraph(TestFilesPath + "BigGraph.csv");
            var metadata = new FileMetadata(database);
            var sourceId = database.GetId("0");
            var targetId = database.GetId("4");
            var pathFinder = new HadiPathFinder(metadata, sourceId, targetId, true);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            var actual = resultBuilder.Build(targetId).Edges;
            var expected = new HashSet<Edge>
            {
                new Edge(database.GetId("0"), database.GetId("648"), 20),
                new Edge(database.GetId("648"), database.GetId("3"), 13),
                new Edge(database.GetId("3"), database.GetId("731"), 1),
                new Edge(database.GetId("731"), database.GetId("525"), 8),
                new Edge(database.GetId("525"), database.GetId("4"), 8),
            };
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod]
        public void MahdiGraph()
        {
            var database = new FileGraph(TestFilesPath + "mahdi.csv");
            var metadata = new FileMetadata(database);
            var sourceId = database.GetId("A");
            var targetId = database.GetId("F");
            var pathFinder = new HadiPathFinder(metadata, sourceId, targetId, true);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            var actual = resultBuilder.Build(targetId).Edges;
            Assert.IsTrue(actual.Count == 6);
        }

        [TestMethod]
        public void VisitedGraph()
        {
            var database = new FileGraph(TestFilesPath + "VisitedGraph.csv");
            var metadata = new FileMetadata(database);
            var sourceId = database.GetId("0");
            var targetId = database.GetId("4");
            var pathFinder = new HadiPathFinder(metadata, sourceId, targetId, true);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            var actual = resultBuilder.Build(targetId).Edges;
            var expected = new HashSet<Edge>
            {
                new Edge(database.GetId("0"), database.GetId("2"), 1),
                new Edge(database.GetId("2"), database.GetId("1"), 1),
                new Edge(database.GetId("1"), database.GetId("3"), 1),
                new Edge(database.GetId("3"), database.GetId("4"), 20),
            };
            Assert.IsTrue(expected.SetEquals(actual));
        }
    }

    [TestClass]
    public class FindShortestPassWeightLess
    {
        private const string TestFilesPath = @"../../../TestFiles/";

        [TestMethod]
        public void EasyTest()
        {
            var database = new FileGraph(TestFilesPath + "VisitedGraph.csv");
            var metadata = new FileMetadata(database);
            var sourceId = database.GetId("0");
            var targetId = database.GetId("4");
            var pathFinder = new WeightlessPathFinder(metadata, sourceId, targetId, true);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            var actual = resultBuilder.Build(targetId).Edges;
            var expected = new HashSet<Edge>
            {
                new Edge(database.GetId("0"), database.GetId("1"), 10),
                new Edge(database.GetId("1"), database.GetId("3"), 1),
                new Edge(database.GetId("3"), database.GetId("4"), 20),
            };
            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod]
        public void Undirected()
        {
            var database = new FileGraph(TestFilesPath + "undirected.csv");
            var metadata = new FileMetadata(database);
            var sourceId = database.GetId("0");
            var targetId = database.GetId("4");
            var pathFinder = new WeightlessPathFinder(metadata, sourceId, targetId, false);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            var actual = resultBuilder.Build(targetId).Edges;
            var expected = new HashSet<Edge>
            {
                new Edge(database.GetId("0"), database.GetId("1"), 10),
                new Edge(database.GetId("1"), database.GetId("3"), 1),
                new Edge(database.GetId("3"), database.GetId("4"), 20),
            };
            Assert.IsTrue(expected.SetEquals(actual));
        }
    }
}