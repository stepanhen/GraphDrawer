using System.Windows.Shapes;

namespace GraphDrawer
{
    /// <summary>
    /// Edge representation, also holds the drawn object for quicker manipulation.
    /// </summary>
    public class Edge
    {
        public Node node1;
        public Node node2;
        public int weight;
        public Shape line;
        public Edge(Node n1, Node n2, int w, Shape s) 
        {
            node1 = n1;
            node2 = n2;
            weight = w;
            line = s;
        }
    }
}