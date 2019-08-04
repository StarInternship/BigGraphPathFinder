using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace BigDataPathFinding.Models.FileGraph
{
    public class FileGraph : IDatabase
    {
        private static readonly Regex Regex = new Regex(@"^(.+),(.+),(\d+.?\d*)$");
        private readonly Dictionary<Guid, FileNode> nodes = new Dictionary<Guid, FileNode>();
        private readonly Dictionary<string, Guid> ids = new Dictionary<string, Guid>();

        public FileGraph(string path)
        {
            if (!File.Exists(path)) return;
            var edges = File.ReadAllLines(path);

            foreach (var edge in edges)
            {
                ReadEdge(edge);
            }
        }

        private void ReadEdge(string edge)
        {
            var groups = Regex.Matches(edge)[0].Groups;

            var source = groups[1].ToString();
            var target = groups[2].ToString();
            var weight = double.Parse(groups[3].ToString());

            AddEdge(source, target, weight);
        }

        private void AddEdge(string sourceName, string targetName, double weight)
        {
            if (!ids.ContainsKey(sourceName)) AddNode(sourceName);
            if (!ids.ContainsKey(targetName)) AddNode(targetName);

            nodes[ids[sourceName]].AddOutput(ids[targetName], weight);
            nodes[ids[targetName]].AddInput(ids[sourceName], weight);
        }

        private void AddNode(string name)
        {
            var id = Guid.NewGuid();
            var node = new FileNode(id, name);
            ids[name] = id;
            nodes[id] = node;
        }

        public Node GetNode(Guid id) => nodes?[id];
    }
}
