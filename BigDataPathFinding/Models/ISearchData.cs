using System;

namespace BigDataPathFinding.Models
{
    public interface ISearchData
    {
        NodeData PopBestCurrentNode();

        void AddToQueue(NodeData node);

        void AddToNodeSet(NodeData node);

        NodeData GetNode(Guid id);

        bool IsEmpty();
    }
}