using System;

namespace BigDataPathFinding.Models
{
    public interface IDatabase
    {
        Node GetNode(Guid id);
    }
}