using System.Collections.Generic;
using BigDataPathFinding.Models;
using BigDataPathFinding.Models.FileGraph;
using BigDataPathFinding.Models.Hadi;
using BigDataPathFinding.Models.Mahdi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BigDataPathFindingTests.Models
{
    [TestClass()]
    public class FilePathFinderTests
    {
        private const string testFilesPath = @"../../../TestFiles/";
        private const string resultFilesPath = @"../../../results/";

        [TestMethod()]
        public void FilePathFinderTest()
        {
            var database = new FileGraph(testFilesPath + "K3AllPathSearch.csv");
            var metadata = new FileMetadata(database);
            var sourceId = database.GetId("0");
            var targetId = database.GetId("1");
            var pathFinder = new MahdiPathFinder(metadata, sourceId, targetId, true);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            var actual = resultBuilder.Build(targetId).Edges;
            var expected = new HashSet<Edge>
            {
                new Edge(database.GetId("0"), database.GetId("1"), 1)
            };
            Assert.IsTrue(actual.Count==expected.Count);
        }
    }

    [TestClass()]
    public class Mahdi1
    {
        private const string testFilesPath = @"../../../TestFiles/";
        private const string resultFilesPath = @"../../../results/";

        [TestMethod()]
        public void FilePathFinderTest()
        {
            var database = new FileGraph(testFilesPath + "mahdi1.txt");
            var metadata = new FileMetadata(database);
            var sourceId = database.GetId("A");
            var targetId = database.GetId("F");
            var pathFinder = new MahdiPathFinder(metadata, sourceId, targetId, true);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            var actual = resultBuilder.Build(targetId).Edges;
            Assert.IsTrue(actual.Count == 4);
        }
    }

    [TestClass()]
    public class Hadi
    {
        private const string testFilesPath = @"../../../TestFiles/";
        private const string resultFilesPath = @"../../../results/";

        [TestMethod()]
        public void FilePathFinderTest()
        {
            var database = new FileGraph(testFilesPath + "mahdi1.txt");
            var metadata = new FileMetadata(database);
            var sourceId = database.GetId("A");
            var targetId = database.GetId("F");
            var pathFinder = new HadiPathFinder(metadata, sourceId, targetId, true);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            var actual = resultBuilder.Build(targetId).Edges;
            var expected = new HashSet<Edge>
            {
                new Edge(database.GetId("0"), database.GetId("1"), 1),
                new Edge(database.GetId("1"), database.GetId("3"), 1),
                new Edge(database.GetId("0"), database.GetId("3"), 1),
                new Edge(database.GetId("3"), database.GetId("4"), 1)
            };
            //Assert.AreEqual(actual,expected);
            //Assert.IsTrue(actual.SetEquals(expected));
            Assert.IsTrue(actual.Count == expected.Count);
        }
    }
}