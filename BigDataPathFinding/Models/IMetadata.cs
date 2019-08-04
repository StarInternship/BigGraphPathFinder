using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShortestPath_ElasticSearch.Models
{
    public interface IMetadata
    {
        IEnumerable<Adjacent> GetOutputAdjacents(Guid id);
        IEnumerable<Adjacent> GetInputAdjacents(Guid id);
    }
}
