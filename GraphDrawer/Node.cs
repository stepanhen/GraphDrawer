using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace GraphDrawer
{
    /// <summary>
    /// Node representation, also holds the drawn object for quicker manipulation.
    /// </summary>
    public class Node
    {
        public int nodeID;

        public Ellipse ellipse;

        public Node()
        {
            nodeID = -1;
        }

        public Node(int nodeID, Ellipse e) 
        {
            this.nodeID = nodeID;
            this.ellipse = e;
        }
    }
}
