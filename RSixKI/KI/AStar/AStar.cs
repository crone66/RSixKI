using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RSixKI
{
    public class AStar
    {
        private List<Node> nodes;
        private List<Node> closedList;
        private List<Node> openList;

        public List<Node> Nodes
        {
            get
            {
                return nodes;
            }
        }

        public AStar()
        {
            nodes = new List<Node>();
        }

        /// <summary>
        /// Adds a new node to the pathfinding algorithm
        /// </summary>
        /// <param name="position">Node position</param>
        /// <param name="neighbors">Neighbor nodes</param>
        /// <returns>returns the new node</returns>
        public Node AddNode(Vector2 position, Node[] neighbors)
        {
            Node node = new Node(position, neighbors);
            nodes.Add(node);
            return node;
        }

        /// <summary>
        /// Adds a new node to the pathfinding algorithm
        /// </summary>
        /// <param name="position">Node position</param>
        /// <returns>returns the new node</returns>
        public Node AddNode(Vector2 position)
        {
            Node node = new Node(position);
            nodes.Add(node);
            return node;
        }

        /// <summary>
        /// Search the shortest path between two nodes
        /// </summary>
        /// <param name="fromPosition">current (start) position</param>
        /// <param name="toPosition">target position</param>
        /// <returns></returns>
        public Node Search(Vector2 fromPosition, Vector2 toPosition)
        {
            closedList = new List<Node>();
            openList = new List<Node>();

            Node startNode = FindNode(fromPosition);
            Node endNode = FindNode(toPosition);
            openList.Add(startNode);

            while (true)
            {
                if (openList.Count == 0)
                    return null;

                Node currentNode = FindSmallestNode();
                if (currentNode == endNode)
                {
                    return currentNode;
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                for (int i = 0; i < currentNode.Neighbors.Length; i++)
                {
                    Node neighbor = currentNode.Neighbors[i];
                    if (!closedList.Contains(neighbor))
                    {            
                        if (openList.Contains(neighbor))
                        {
                            if (neighbor.DistanceToStart < currentNode.DistanceToStart)
                            {
                                neighbor.DistanceToStart = Vector2.Distance(neighbor.position, currentNode.position) + currentNode.DistanceToStart;
                                neighbor.ExpectedTotalDistance = neighbor.DistanceToStart + neighbor.ExpectedDistanceToEnd;
                                neighbor.Parent = currentNode;
                            }
                        }
                        else
                        {
                            neighbor.DistanceToStart = Vector2.Distance(neighbor.position, currentNode.position) + currentNode.DistanceToStart;
                            neighbor.ExpectedDistanceToEnd = Vector2.Distance(neighbor.position, endNode.position);
                            neighbor.ExpectedTotalDistance = neighbor.DistanceToStart + neighbor.ExpectedDistanceToEnd;
                            neighbor.Parent = currentNode;
                            openList.Add(neighbor);
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Find node with the smallest "ExpectedTotalDistance"
        /// </summary>
        /// <returns>Retruns smallest node</returns>
        private Node FindSmallestNode()
        {
            Node smallest = openList[0];
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].ExpectedTotalDistance < smallest.ExpectedTotalDistance || (openList[i].ExpectedTotalDistance == smallest.ExpectedTotalDistance && openList[i].ExpectedDistanceToEnd < smallest.ExpectedDistanceToEnd))
                    smallest = openList[i];
            }
            return smallest;
        }

        /// <summary>
        /// Find nearest node
        /// </summary>
        /// <param name="position"></param>
        /// <returns>returns nearest node</returns>
        private Node FindNode(Vector2 position)
        {
            int index = -1;
            float minDistance = float.MaxValue;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].position == position)
                    return Nodes[i];
                else
                {
                    float distanceToNode = Vector2.Distance(nodes[i].position, position);
                    if (distanceToNode < minDistance)
                    {
                        minDistance = distanceToNode;
                        index = i;
                    }
                }
            }

            if(index >= 0)
                return nodes[index];

            return null;
        }
    }
}
