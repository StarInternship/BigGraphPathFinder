using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShortestPath_ElasticSearch.Models
{
    public interface ISearchData
    {
        Guid PopBestCurrentNode();

        void AddNode(Guid id);

        bool IsEmpty();
    }
}