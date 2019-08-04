using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataPathFinding.Models
{
    public class NodeData
    {
        public Guid Id { get; }
        public bool Explored { get; set; }
        public double Distance { get; set; }
        public HashSet<Adjacent> PreviousAdjacents { get; } = new HashSet<Adjacent>();
    }
}
