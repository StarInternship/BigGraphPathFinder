using System;
using System.Collections.Generic;

namespace BigDataPathFinding.Models
{
    public class Node
    {
        public Node(Guid id, Dictionary<string, object> data)
        {
            Id = id;
            Data = data;
        }

        public Guid Id { get; }
        public Dictionary<string, object> Data { get; }

        public override bool Equals(object obj) => this == obj || (obj is Node node && node.Id.Equals(Id));

        public override int GetHashCode() => Id.GetHashCode();
    }
}