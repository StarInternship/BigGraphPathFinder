using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public interface IMetadata
    {
        IEnumerable<Adjacent> GetOutputAdjacents(Guid id);

        IEnumerable<Adjacent> GetInputAdjacents(Guid id);
    }
}