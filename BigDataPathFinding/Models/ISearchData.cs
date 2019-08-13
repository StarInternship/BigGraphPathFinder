using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public interface ISearchData
    {
        Dictionary<Guid, NodeData> GetResultNodeSet();
        HashSet<Guid> GetJoints();
        int GetPathDistance();
    }
}