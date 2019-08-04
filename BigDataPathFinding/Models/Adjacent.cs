using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShortestPath_ElasticSearch.Models
{
    public class Adjacent
    {
        public Guid id { get; }
        public double Weight { get; }

        public Adjacent(Guid id, double weight)
        {
            this.id = id;
            Weight = weight;
        }
    }
}