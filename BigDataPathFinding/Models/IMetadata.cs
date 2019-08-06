using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public interface IMetadata
    {
        IEnumerable<Adjacent> GetOutputAdjacent(Guid id);

        IEnumerable<Adjacent> GetInputAdjacent(Guid id);
    }
}