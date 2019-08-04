using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataPathFinding.Models
{
    public class Node
    {
        public Guid Id { get; }
        public string Name { get; }

        public Node(Guid id, string name)
        {
            Id = id;
            Name = name;
        }


        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            return obj is Node node && node.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}