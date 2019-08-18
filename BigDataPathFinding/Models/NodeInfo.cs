using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public class NodeInfo
    {
        public NodeInfo(NodeInfo node) : this(node.Id, node.Data)
        {
        }

        public NodeInfo(Guid id, Dictionary<string, object> data)
        {
            Id = id;
            Data = data;
        }

        public Guid Id { get; }
        public Dictionary<string, object> Data { get; }

        public override bool Equals(object obj)
        {
            return this == obj || obj is NodeInfo node && node.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}