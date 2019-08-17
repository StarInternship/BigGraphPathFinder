using System;

namespace BigDataPathFinding.Models.Interfaces
{
    public interface IDatabase
    {
        NodeInfo GetNode(Guid id);
    }
}