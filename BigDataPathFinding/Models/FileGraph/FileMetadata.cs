using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models.FileGraph
{
    public class FileMetadata : IMetadata
    {
        private readonly FileGraph _database;

        public FileMetadata(FileGraph database)
        {
            _database = database;
        }

        public IEnumerable<Adjacent> GetInputAdjacents(Guid id)
        {
            return ((FileNode) _database.GetNode(id)).InputAdjucents;
        }

        public IEnumerable<Adjacent> GetOutputAdjacents(Guid id)
        {
            return ((FileNode) _database.GetNode(id)).OutputAdjucents;
        }
    }
}