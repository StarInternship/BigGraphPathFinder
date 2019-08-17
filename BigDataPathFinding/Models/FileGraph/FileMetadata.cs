using System;
using System.Collections.Generic;
using System.Linq;
using BigDataPathFinding.Models.Interfaces;

namespace BigDataPathFinding.Models.FileGraph
{
    public class FileMetadata : IMetadata
    {
        private readonly FileGraph _database;

        public FileMetadata(FileGraph database) => _database = database;


        public IEnumerable<IEnumerable<Adjacent>> GetInputEdges(Guid id)
        {
            yield return ((FileNodeInfo)_database.GetNode(id)).InputAdjacent;
        }

        public IEnumerable<IEnumerable<Adjacent>> GetOutputEdges(Guid id)
        {
            yield return ((FileNodeInfo)_database.GetNode(id)).OutputAdjacent;
        }

        public IEnumerable<IEnumerable<Adjacent>> GetAllAdjacent(Guid id)
        {
            yield return ((FileNodeInfo) _database.GetNode(id)).GetAllAdjacent();
        }

        public IEnumerable<IEnumerable<Edge>> GetOutputEdges(IEnumerable<Guid> ids)
        {
            return ids.Select(id => ((FileNodeInfo)_database.GetNode(id)).OutputAdjacent.Select(
                adjacent => new Edge(id, adjacent.Id, adjacent.Weight)
            ));
        }
        public IEnumerable<IEnumerable<Edge>> GetInputEdges(IEnumerable<Guid> ids)
        {
            return ids.Select(id => ((FileNodeInfo)_database.GetNode(id)).InputAdjacent.Select(
                adjacent => new Edge(adjacent.Id, id, adjacent.Weight)
            ));
        }

        public IEnumerable<IEnumerable<Edge>> GetAllEdges(IEnumerable<Guid> ids)
        {
            return ids.Select(id => ((FileNodeInfo)_database.GetNode(id)).GetAllAdjacent().Select(
                adjacent => new Edge(adjacent.Id, id, adjacent.Weight)
            ));
        }
    }
}