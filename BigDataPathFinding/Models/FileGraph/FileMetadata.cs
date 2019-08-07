using System;
using System.Collections.Generic;

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
    }
}