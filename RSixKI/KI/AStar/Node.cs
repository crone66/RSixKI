using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSixKI
{
    public class Node
    {
        public Node Parent;
        public Node[] Neighbors;
        public Vector2 position;
        public float DistanceToStart;
        public float ExpectedDistanceToEnd;
        public float ExpectedTotalDistance;

        public Node(Vector2 position)
        {
            this.position = position;
        }

        public Node(Vector2 position, Node[] neighbors)
        {
            this.position = position;
            this.Neighbors = neighbors;
        }
    }
}
