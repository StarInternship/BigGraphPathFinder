﻿using System;

namespace BigDataPathFinding.Models
{
    public class Node
    {
        public Node(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }
        public string Name { get; }

        public override bool Equals(object obj) => this == obj || (obj is Node node && node.Id.Equals(Id));

        public override int GetHashCode() => Id.GetHashCode();
    }
}