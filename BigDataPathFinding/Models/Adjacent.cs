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
    }
}