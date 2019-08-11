﻿using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public interface IMetadata
    {
        IEnumerable<IEnumerable<Adjacent>> GetOutputAdjacent(Guid id);

        IEnumerable<IEnumerable<Adjacent>> GetInputAdjacent(Guid id);

        IEnumerable<IEnumerable<Edge>> GetOutputAdjacent(IEnumerable<Guid> ids);

        IEnumerable<IEnumerable<Edge>> GetAllAdjacent(IEnumerable<Guid> ids);
    }
}