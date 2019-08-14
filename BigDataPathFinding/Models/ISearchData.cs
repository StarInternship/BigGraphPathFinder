using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public interface ISearchData
    {
        Dictionary<Guid, NodeData> GetResultNodeSet();

        HashSet<Guid> GetJoints();

        void AddJoint(Guid id);

        int GetPathDistance();
    }
}