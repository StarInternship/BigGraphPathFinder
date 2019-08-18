using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models.Interfaces
{
    public interface ISearchData
    {
        Dictionary<Guid, NodeData> GetResultNodeSet();

        HashSet<Guid> GetJoints();

        void AddJoint(Guid id);

        double GetPathDistance();
    }
}