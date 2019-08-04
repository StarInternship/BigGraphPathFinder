using System.Collections.Generic;
using BigDataPathFinding.Models;
using BigDataPathFinding.Models.FileGraph;
using BigDataPathFinding.Models.Mahdi;

namespace BigDataPathFinding
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string testFilesPath = @"../../../TestFiles/";
            string resultFilesPath = @"../../../results/";

            var database = new FileGraph(testFilesPath + "K3AllPathSearch.csv");
            var metadata = new FileMetadata(database);
            var sourceId = database.GetId("0");
            var targetId = database.GetId("1");
            var pathFinder = new MahdiPathFinder(metadata, sourceId, targetId, true);
            pathFinder.FindPath();
            var resultBuilder = new ResultBuilder(database, pathFinder.GetResultNodeSet());
            var actual = resultBuilder.Build(targetId).Edges;
            /*var expected = new HashSet<Edge>
            {
                new Edge(database.GetId("2"), database.GetId("1"), 1),
                new Edge(database.GetId("0"), database.GetId("1"), 1),
                new Edge(database.GetId("0"), database.GetId("2"), 1)
            };*/
            var expected = new HashSet<Edge>
            {
                new Edge(database.GetId("0"), database.GetId("1"), 1)
            };
        }
    }
}
