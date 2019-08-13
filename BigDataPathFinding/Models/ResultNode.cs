namespace BigDataPathFinding.Models
{
    public class ResultNode : Node
    {
        public ResultNode(Node node) : base(node.Id, node.Data)
        {
        }

        public bool Explored { get; set; } = false;
    }
}