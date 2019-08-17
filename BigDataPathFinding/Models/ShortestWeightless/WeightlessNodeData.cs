using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    class WeightlessNodeData : NodeData
    {

        public Seen Seen { get; }
        public WeightlessNodeData(Guid id, double distance, Seen seen) : base(id, distance) => Seen = seen;

        public void AddBackwardAdjacent(Adjacent adjacent) => ForwardAdjacent.Add(adjacent);

    }

    public enum Seen
    {
        forward,
        backward
    }
}
