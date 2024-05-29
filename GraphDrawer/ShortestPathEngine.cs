using System.Collections.Generic;
using System.Linq;

namespace GraphDrawer
{
    /// <summary>
    /// Class with algorithms for shortest path calculation.
    /// </summary>
    public class ShortestPathEngine
    {
        private Graph _graph;
        public ShortestPathEngine(Graph graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// If the graph is not weighted, uses BFS to find the shortest path, else uses Dijkstra's algorithm.
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <returns>List of nodes on the shortest path, including the start and end node.</returns>
        public List<Node> FindShortestPath(Node startNode, Node endNode)
        {
            if (_graph.isWeighted)
            {
                return Dijsktra(startNode, endNode);
            }
            else
            {
                List<Node> parents = Enumerable.Repeat(new Node(), _graph.nodes.Count).ToList();

                List<int> distances = Enumerable.Repeat(int.MaxValue, _graph.nodes.Count).ToList();
                BFS(startNode, endNode, distances, parents);
                if (distances[endNode.nodeID] == int.MaxValue)
                {
                    // Disconnected
                    return new List<Node>();
                }
                List<Node> path = new List<Node>();
                Node currentNode = endNode;
                path.Add(endNode);

                while (parents[currentNode.nodeID].nodeID != -1)
                {
                    path.Add(parents[currentNode.nodeID]);
                    currentNode = parents[currentNode.nodeID];
                }
                path.Reverse();
                return path;
            }
        }

        private void BFS(Node startNode, Node endNode, List<int> distnaces, List<Node> parents)
        {
            Queue<Node> queue = new Queue<Node>();
            queue.Enqueue(startNode);
            distnaces[startNode.nodeID] = 0;

            while (queue.Count > 0)
            {
                Node node = queue.Dequeue();

                foreach (Node neighbour in _graph.neighbouringNodes[node])
                {
                    if (distnaces[neighbour.nodeID] == int.MaxValue)
                    {
                        parents[neighbour.nodeID] = node;
                        distnaces[neighbour.nodeID] = distnaces[node.nodeID] + 1;
                        queue.Enqueue(neighbour);
                    }
                }
            }
        }
        private int minDistance(List<int> distances, List<bool> shortestPathSet)
        {
            int min = int.MaxValue;
            int min_i = -1;

            for (int i = 0; i < distances.Count; i++)
            {
                if (!shortestPathSet[i] && distances[i] <= min)
                {
                    min = distances[i];
                    min_i = i;
                }
            }
            return min_i;
        }
        private List<Node> Dijsktra(Node startNode, Node endNode)
        {
            int n = _graph.nodes.Count;
            List<int> distances = Enumerable.Repeat(int.MaxValue, n).ToList();
            List<bool> shortestPathSet = Enumerable.Repeat(false, n).ToList();
            List<int> predecessors = Enumerable.Repeat(-1, n).ToList();

            distances[startNode.nodeID] = 0;

            for (int i = 0; i < n - 1; i++)
            {
                int min_node = minDistance(distances, shortestPathSet);
                shortestPathSet[min_node] = true;

                for (int j = 0; j < n; j++)
                {
                    // If node j is not processed, edge (min_node, j) exists and distance to j through this edge is smaller than it was
                    if (    !shortestPathSet[j] 
                        &&  _graph.edges.ContainsKey((min_node, j))
                        &&  distances[min_node] != int.MaxValue
                        &&  distances[min_node] + _graph.edges[(min_node, j)].weight < distances[j])
                    {
                        distances[j] = distances[min_node] + _graph.edges[(min_node, j)].weight;
                        predecessors[j] = min_node;

                    }
                }
            }

            List<Node> shortestPath = new List<Node>();
            for (int i = endNode.nodeID; i != -1; i = predecessors[i])
            {
                shortestPath.Add(_graph.nodes[i]);
            }
            shortestPath.Reverse();

            return shortestPath;
        }
    }
}
