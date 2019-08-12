using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    class WeightlessNodeData : NodeData
    {
        public HashSet<Adjacent> ForwardAdjacents { get; } = new HashSet<Adjacent>();

        public WeightlessNodeData(Guid id, double distance) : base(id, distance)
        {

        }

        public void AddForwardAdjacent(Adjacent adjacent) => ForwardAdjacents.Add(adjacent);

    }
}
