using System.Collections.Generic;
using System.Text;

namespace BigDataPathFinding.Models
{
    public static class DictionaryExtension
    {
        public static string MakeString(this Dictionary<string, object> source)
        {
            var result = new StringBuilder("{");
            foreach (var value in source.Values)
            {
                result.Append("[" + value + "] ");
            }
            result.Append("}");
            return result.ToString();
        }
    }
}
