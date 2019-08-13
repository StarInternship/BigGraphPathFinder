using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigDataPathFinding.Models
{
    public static class DictionaryExtension
    {
        public static string MakeString(this Dictionary<string, object> source)
        {
            StringBuilder result = new StringBuilder("{");
            foreach (var key in source.Keys)
            {
                result.Append("[" + key + "]: " + source[key] + ", ");
            }
            result.Append("}");
            return result.ToString();
        }
    }
}
