using System;

namespace BigDataPathFinding.Models
{
    public class Adjacent
    {
        public Adjacent(Guid id, double weight)
        {
            Id = id;
            Weight = weight;
        }

        public Guid Id { get; }
        public double Weight { get; }

        public override string ToString() => FileGraph.FileGraph.Instance.GetNode(Id).Name + ", Weight: " + Weight; //TODO: change
    }
}