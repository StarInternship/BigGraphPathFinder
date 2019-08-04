using System;

namespace BigDataPathFinding.Models
{
    public class Adjacent
    {
        public Guid Id { get; }
        public double Weight { get; }

        public Adjacent(Guid id, double weight)
        {
            Id = id;
            Weight = weight;
        }
    }
}