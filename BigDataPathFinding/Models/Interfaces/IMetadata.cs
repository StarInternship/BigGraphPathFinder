using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models.Interfaces
{
    public interface IMetadata
    {
        IEnumerable<IEnumerable<Adjacent>> GetOutputAdjacent(Guid id);

        IEnumerable<IEnumerable<Adjacent>> GetInputAdjacent(Guid id);

        IEnumerable<IEnumerable<Adjacent>> GetAllAdjacent(Guid id);

        IEnumerable<IEnumerable<Edge>> GetOutputEdges(IEnumerable<Guid> ids);

        IEnumerable<IEnumerable<Edge>> GetInputEdges(IEnumerable<Guid> ids);

        IEnumerable<IEnumerable<Edge>> GetAllEdges(IEnumerable<Guid> ids);
    }
}