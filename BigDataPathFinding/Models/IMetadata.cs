using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public interface IMetadata
    {
        IEnumerable<IEnumerable<Adjacent>> GetOutputAdjacent(Guid id);

        IEnumerable<IEnumerable<Adjacent>> GetInputAdjacent(Guid id);
    }
}