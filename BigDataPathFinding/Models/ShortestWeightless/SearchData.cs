﻿using System;
using System.Collections.Generic;
using BigDataPathFinding.Models.Interfaces;

namespace BigDataPathFinding.Models.ShortestWeightless
{
    internal class SearchData : ISearchData
    {
        private Dictionary<Guid, NodeData> NodeSet { get; } = new Dictionary<Guid, NodeData>();
        public HashSet<Guid> CurrentBackwardNodes { get; private set; } = new HashSet<Guid>();
        public HashSet<Guid> CurrentForwardNodes { get; private set; } = new HashSet<Guid>();
        public int PathDistance { get; set; }
        public HashSet<Guid> Joints { get; } = new HashSet<Guid>();

        public Dictionary<Guid, NodeData> GetResultNodeSet()
        {
            return NodeSet;
        }

        public HashSet<Guid> GetJoints()
        {
            return Joints;
        }

        public double GetPathDistance()
        {
            return PathDistance;
        }

        public void AddJoint(Guid id)
        {
            Joints.Add(id);
        }

        public void AddToNodeSet(NodeData node)
        {
            NodeSet.Add(node.Id, node);
        }

        public NodeData GetNode(Guid id)
        {
            return !NodeSet.ContainsKey(id) ? null : NodeSet[id];
        }

        public void AddToCurrentForwardNodes(Guid id)
        {
            CurrentForwardNodes.Add(id);
        }

        public void UpdateCurrentForwardNodes(HashSet<Guid> edges)
        {
            CurrentForwardNodes = edges;
        }

        public void ClearCurrentForwardNodes()
        {
            CurrentForwardNodes = new HashSet<Guid>();
        }

        public void ClearCurrentBackwardNodes()
        {
            CurrentBackwardNodes = new HashSet<Guid>();
        }

        public void AddToCurrentBackwardNodes(Guid id)
        {
            CurrentBackwardNodes.Add(id);
        }

        public void UpdateCurrentBackwardNodes(HashSet<Guid> edges)
        {
            CurrentBackwardNodes = edges;
        }
    }
}