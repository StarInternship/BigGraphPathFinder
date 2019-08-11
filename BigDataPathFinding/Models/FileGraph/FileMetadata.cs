using System;
using System.Collections.Generic;
using System.Linq;

namespace BigDataPathFinding.Models.FileGraph
{
    public class FileMetadata : IMetadata
    {
        private readonly FileGraph _database;

        public FileMetadata(FileGraph database) => _database = database;


        public IEnumerable<IEnumerable<Adjacent>> GetInputAdjacent(Guid id)
        {
            yield return ((FileNode)_database.GetNode(id)).InputAdjucents;
        }

        public IEnumerable<IEnumerable<Adjacent>> GetOutputAdjacent(Guid id)
        {
            yield return ((FileNode)_database.GetNode(id)).OutputAdjucents;
        }

        public IEnumerable<IEnumerable<Edge>> GetOutputAdjacent(IEnumerable<Guid> ids)
        {
            foreach (var id in ids)
            {
                yield return ((FileNode)_database.GetNode(id)).OutputAdjucents.Select(
                    adjacent => new Edge(id, adjacent.Id, adjacent.Weight)
                );
            }
        }
        public IEnumerable<IEnumerable<Edge>> GetInputAdjacent(IEnumerable<Guid> ids)
        {
            foreach (var id in ids)
            {
                yield return ((FileNode)_database.GetNode(id)).InputAdjucents.Select(
                    adjacent => new Edge(adjacent.Id, id, adjacent.Weight)
                );
            }
        }
    }
}