using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataPathFinding.Models
{
    public class ResultNode : Node
    {
        public bool Explored { get; set; } = false;

        public ResultNode(Node node) : base(node.Id, node.Name)
        {
        }
    }
}
