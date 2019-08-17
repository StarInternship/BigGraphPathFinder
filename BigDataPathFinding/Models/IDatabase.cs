using System;

namespace BigDataPathFinding.Models
{
    public interface IDatabase
    {
        NodeInfo GetNode(Guid id);
    }
}