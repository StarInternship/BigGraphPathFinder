using System.Collections.Generic;
using BigDataPathFinding.Models;
using BigDataPathFinding.Models.FileGraph;
using BigDataPathFinding.Models.ShortestWeightless;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BigDataPathFindingTests.Models
{
    
    [TestClass]
    public class Hadi
    {
        private const string TestFilesPath = @"../../../TestFiles/";

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
            var pathFinder = new WeightlessPathFinder(metadata, sourceId, targetId, true,10,0);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetSearchData());
            var actual = resultBuilder.Build().Edges;
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
            var pathFinder = new WeightlessPathFinder(metadata, sourceId, targetId, false,10,0);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetSearchData());
            var actual = resultBuilder.Build().Edges;
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