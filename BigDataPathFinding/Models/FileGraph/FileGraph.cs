using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace BigDataPathFinding.Models.FileGraph
{
    public class FileGraph : IDatabase
    {
        private static readonly Regex Regex = new Regex(@"^(.+),(.+),(\d+.?\d*)$");
        private readonly Dictionary<string, Guid> _ids = new Dictionary<string, Guid>();
        private readonly Dictionary<Guid, FileNode> _nodes = new Dictionary<Guid, FileNode>();

        public FileGraph(string path)
        {
            if (!File.Exists(path)) return;
            Instance = this;
            var edges = File.ReadAllLines(path);

            foreach (var edge in edges) ReadEdge(edge);
        }

        public static FileGraph Instance { get; private set; }

        public Node GetNode(Guid id)
        {
            return !_nodes.ContainsKey(id) ? null : _nodes[id];
        }

        public Guid GetId(string name)
        {
            return !_ids.ContainsKey(name) ? Guid.Empty : _ids[name];
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
            if (!_ids.ContainsKey(sourceName)) AddNode(sourceName);
            if (!_ids.ContainsKey(targetName)) AddNode(targetName);

            _nodes[_ids[sourceName]].AddOutput(_ids[targetName], weight);
            _nodes[_ids[targetName]].AddInput(_ids[sourceName], weight);
        }

        private void AddNode(string name)
        {
            var id = Guid.NewGuid();
            var node = new FileNode(id, name);
            _ids[name] = id;
            _nodes[id] = node;
        }
    }
}