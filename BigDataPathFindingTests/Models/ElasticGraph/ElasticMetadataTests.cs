using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BigDataPathFinding.Models.ElasticGraph.Tests
{
    [TestClass()]
    public class ElasticMetadataTests
    {
        [TestMethod()]
        public void GetOutputAdjacentByOneId()
        {
            var metadata = new ElasticMetadata("hosein2_connections");
            var id = new Guid("8732ab7c-e2c8-45d2-a6c9-c96d9acbe162");
            var listResult = metadata.GetOutputAdjacent(new List<Guid> { id }).Select(list => list.Select(edge => edge.TargetId));
            var singleResult = metadata.GetOutputAdjacent(id).Select(list => list.Select(adjacent => adjacent.Id));

            var actual = new HashSet<Guid>();
            foreach (var list in listResult)
            {
                actual.UnionWith(list);
            }

            var expected = new HashSet<Guid>();
            foreach (var list in singleResult)
            {
                expected.UnionWith(list);
            }

            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod()]
        public void GetOutputAdjacentByTwoIds()
        {
            var metadata = new ElasticMetadata("hosein2_connections");
            var one = new Guid("8732ab7c-e2c8-45d2-a6c9-c96d9acbe162");
            var two = new Guid("b47c1151-350f-4b41-83cd-c3093c04d34a");
            var listResult = metadata.GetOutputAdjacent(new List<Guid> { one, two }).Select(list => list.Select(edge => edge.TargetId));
            var oneResult = metadata.GetOutputAdjacent(one).Select(list => list.Select(adjacent => adjacent.Id));
            var twoResult = metadata.GetOutputAdjacent(two).Select(list => list.Select(adjacent => adjacent.Id));

            var actual = new HashSet<Guid>();
            foreach (var list in listResult)
            {
                actual.UnionWith(list);
            }

            var expected = new HashSet<Guid>();
            foreach (var list in oneResult)
            {
                expected.UnionWith(list);
            }
            foreach (var list in twoResult)
            {
                expected.UnionWith(list);
            }

            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod()]
        public void GetInputAdjacentByOneId()
        {
            var metadata = new ElasticMetadata("hosein2_connections");
            var id = new Guid("8732ab7c-e2c8-45d2-a6c9-c96d9acbe162");
            var listResult = metadata.GetInputAdjacent(new List<Guid> { id }).Select(list => list.Select(edge => edge.SourceId));
            var singleResult = metadata.GetInputAdjacent(id).Select(list => list.Select(adjacent => adjacent.Id));

            var actual = new HashSet<Guid>();
            foreach (var list in listResult)
            {
                actual.UnionWith(list);
            }

            var expected = new HashSet<Guid>();
            foreach (var list in singleResult)
            {
                expected.UnionWith(list);
            }

            Assert.IsTrue(expected.SetEquals(actual));
        }

        [TestMethod()]
        public void GetInputAdjacentByTwoIds()
        {
            var metadata = new ElasticMetadata("hosein2_connections");
            var one = new Guid("8732ab7c-e2c8-45d2-a6c9-c96d9acbe162");
            var two = new Guid("b47c1151-350f-4b41-83cd-c3093c04d34a");
            var listResult = metadata.GetInputAdjacent(new List<Guid> { one, two }).Select(list => list.Select(edge => edge.SourceId));
            var oneResult = metadata.GetInputAdjacent(one).Select(list => list.Select(adjacent => adjacent.Id));
            var twoResult = metadata.GetInputAdjacent(two).Select(list => list.Select(adjacent => adjacent.Id));

            var actual = new HashSet<Guid>();
            foreach (var list in listResult)
            {
                actual.UnionWith(list);
            }

            var expected = new HashSet<Guid>();
            foreach (var list in oneResult)
            {
                expected.UnionWith(list);
            }
            foreach (var list in twoResult)
            {
                expected.UnionWith(list);
            }

            Assert.IsTrue(expected.SetEquals(actual));
        }
    }
}