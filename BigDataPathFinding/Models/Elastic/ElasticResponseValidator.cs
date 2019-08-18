using Nest;

namespace BigDataPathFinding.Models.Elastic
{
    public static class ElasticResponseValidator
    {
        public static TResponse Validate<TResponse>(this TResponse response) where TResponse : IResponse
        {
            if (!response.IsValid)
            {
                throw new ResponseException(response);
            }

            return response;
        }

        public static Edge Reversed(this Edge edge)
        {
            return new Edge(edge.TargetId, edge.SourceId, edge.Weight);
        }
    }
}