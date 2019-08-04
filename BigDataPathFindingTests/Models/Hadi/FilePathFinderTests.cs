using System.Collections.Generic;
using BigDataPathFinding.Models;
using BigDataPathFinding.Models.FileGraph;
using BigDataPathFinding.Models.Mahdi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BigDataPathFindingTests.Models.Hadi
{
    [TestClass()]
    public class FilePathFinderTests
    {
        private const string testFilesPath = @"../../../TestFiles/";
        private const string resultFilesPath = @"../../../results/";

        [TestMethod()]
        public void HadiPathFinderTest()
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
                new Edge(database.GetId("2"), database.GetId("1"), 1),
                new Edge(database.GetId("0"), database.GetId("1"), 1),
                new Edge(database.GetId("0"), database.GetId("2"), 1)
            };
            Assert.IsTrue(actual.SetEquals(expected));
        }
    }
}