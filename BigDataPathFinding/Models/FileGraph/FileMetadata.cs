using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models.FileGraph
{
    public class FileMetadata : IMetadata
    {
        private readonly IDatabase database;

        public FileMetadata(IDatabase database) => this.database = database;

        public IEnumerable<Adjacent> GetInputAdjacents(Guid id) => ((FileNode)database.GetNode(id)).InputAdjucents;

        public IEnumerable<Adjacent> GetOutputAdjacents(Guid id) => ((FileNode)database.GetNode(id)).OutputAdjucents;
    }
}
