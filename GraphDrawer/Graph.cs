using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Shapes;

namespace GraphDrawer
{
    /// <summary>
    /// Core of the graph representation. Holds all edges, nodes and methods above them.
    /// </summary>
    public class Graph
    {
        // Lookup tables to quickly access all kinds of data.
        public SortedDictionary<int, Node> nodes = new();
        public Dictionary<(int, int), Edge> edges = new();
        public Dictionary<Node, List<Node>> neighbouringNodes = new();

        public bool isWeighted;
        public Graph()
        {
            isWeighted = false;
        }

        internal void CreateNode(int nodeID, Ellipse e)
        {
            Node node = new Node(nodeID, e);
            nodes.Add(nodeID, node);
            neighbouringNodes.Add(node, new List<Node>());
        }
        internal void AddWeightedEdge(Shape s,int startNodeID, int targetNodeID, int weight)
        {
            Edge edge = new Edge(nodes[startNodeID], nodes[targetNodeID], weight, s);
            edges.Add((startNodeID, targetNodeID), edge);
            edges.Add((targetNodeID, startNodeID), edge);

            Node startNode = nodes[startNodeID];
            Node targetNode = nodes[targetNodeID];
            neighbouringNodes[startNode].Add(targetNode);
            neighbouringNodes[targetNode].Add(startNode);

            isWeighted = true;
        }

        internal void AddEdge(Shape s, int startNodeID, int targetNodeID)
        {
            Edge edge = new Edge(nodes[startNodeID], nodes[targetNodeID], 1, s);
            edges.Add((startNodeID, targetNodeID), edge);
            edges.Add((targetNodeID, startNodeID), edge);

            Node startNode = nodes[startNodeID];
            Node targetNode = nodes[targetNodeID];
            neighbouringNodes[startNode].Add(targetNode);
            neighbouringNodes[targetNode].Add(startNode);
        }
        internal bool HasEdge(int startNodeID, int targetNodeID)
        {
            return edges.ContainsKey((startNodeID, targetNodeID));
        }
        internal void RemoveNode(int nodeID)
        {
            Node node = nodes[nodeID];
            nodes.Remove(nodeID);

            // Remove all edges incident with the node
            List<(int, int)> edgesToRemove = new List<(int, int)>();
            foreach (var kv in edges)
            {
                if (kv.Key.Item1 == nodeID || kv.Key.Item2 == nodeID)
                {
                    edgesToRemove.Add(kv.Key);
                }
            }

            foreach (var edge in edgesToRemove)
            {
                RemoveEdge(edge.Item1, edge.Item2);
            }
            neighbouringNodes.Remove(node);
        }
        internal void RemoveEdge(int startNodeID, int targetNodeID)
        {
            if (edges.ContainsKey((startNodeID, targetNodeID))) {
                edges.Remove((startNodeID, targetNodeID));
                edges.Remove((targetNodeID, startNodeID));

                Node startNode = nodes[startNodeID];
                Node targetNode = nodes[targetNodeID];
                neighbouringNodes[startNode].Remove(targetNode);
                neighbouringNodes[targetNode].Remove(startNode);
            }
        }
        internal List<Shape> FindShortestPath(int startNodeID, int endNodeID)
        {
            ShortestPathEngine spe = new ShortestPathEngine(this);
            var path = spe.FindShortestPath(nodes[startNodeID], nodes[endNodeID]);

            List<Shape> shortestPath = new List<Shape>();

            // Convert the shortest path to list of shapes on that path
            for (int i = 0; i < path.Count; i++)
            {
                shortestPath.Add(path[i].ellipse);
                if (i < path.Count - 1)
                {
                    shortestPath.Add(edges[(path[i].nodeID, path[i + 1].nodeID)].line);
                }
            }
            return shortestPath;
        }
    }
}
