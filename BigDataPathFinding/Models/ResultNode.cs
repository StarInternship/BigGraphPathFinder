namespace BigDataPathFinding.Models
{
    public class ResultNode : Node
    {
        public ResultNode(Node node) : base(node.Id, node.Name)
        {
        }

        public bool Explored { get; set; } = false;
    }
}