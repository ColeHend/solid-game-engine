namespace solid_game_engine.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

// Define the Node class used in the A* algorithm
public class Node
{
    public int X { get; set; } // X position on the map
    public int Y { get; set; } // Y position on the map
    public bool Passable { get; set; } // Whether the tile is passable
    public Node Parent { get; set; } // Parent node in the path
    public float G { get; set; } // Cost from the start node
    public float H { get; set; } // Heuristic cost to the end node
    public float F => G + H; // Total cost

    // Override Equals and GetHashCode for proper comparison in collections
    public override bool Equals(object obj)
    {
        if (obj is Node other)
            return X == other.X && Y == other.Y;
        return false;
    }

    public override int GetHashCode() => X * 31 + Y;
}

public class Pathfinding
{
    // Heuristic function using Manhattan distance
    private float Heuristic(Node a, Node b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    // Get neighboring nodes (up, down, left, right)
    private List<Node> GetNeighbors(Node node, Node[,] nodes, int mapWidth, int mapHeight)
    {
        List<Node> neighbors = new List<Node>();

        int x = node.X;
        int y = node.Y;

        // Up
        if (y > 0)
            neighbors.Add(nodes[x, y - 1]);
        // Down
        if (y < mapHeight - 1)
            neighbors.Add(nodes[x, y + 1]);
        // Left
        if (x > 0)
            neighbors.Add(nodes[x - 1, y]);
        // Right
        if (x < mapWidth - 1)
            neighbors.Add(nodes[x + 1, y]);

        // Uncomment below to allow diagonal movement
        /*
        // Top-left
        if (x > 0 && y > 0)
            neighbors.Add(nodes[x - 1, y - 1]);
        // Top-right
        if (x < mapWidth - 1 && y > 0)
            neighbors.Add(nodes[x + 1, y - 1]);
        // Bottom-left
        if (x > 0 && y < mapHeight - 1)
            neighbors.Add(nodes[x - 1, y + 1]);
        // Bottom-right
        if (x < mapWidth - 1 && y < mapHeight - 1)
            neighbors.Add(nodes[x + 1, y + 1]);
        */

        return neighbors;
    }

    // Main A* pathfinding function
    public List<Node> FindPath(TileMap tileMap, TileSet tileSet, int startX, int startY, int endX, int endY)
    {
        int layers = tileMap.Tiles.Count;
        int mapHeight = tileMap.Tiles[0].Count;
        int mapWidth = tileMap.Tiles[0][0].Count;

        // Initialize nodes grid
        Node[,] nodes = new Node[mapWidth, mapHeight];

        // Populate nodes with passability information
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                bool passable = true;

                // Check each layer for passability at this tile
                for (int l = 0; l < layers; l++)
                {
                    int tileID = tileMap.Tiles[l][y][x];

                    if (tileSet.Passable.TryGetValue(tileID, out bool isPassable))
                    {
                        if (!isPassable)
                        {
                            passable = false;
                            break; // No need to check other layers
                        }
                    }
                    else
                    {
                        // If tileID is not specified, assume it's non-passable
                        passable = false;
                        break;
                    }
                }

                nodes[x, y] = new Node
                {
                    X = x,
                    Y = y,
                    Passable = passable
                };
            }
        }

        Node startNode = nodes[startX, startY];
        Node endNode = nodes[endX, endY];

        // The set of nodes to be evaluated
        List<Node> openList = new List<Node> { startNode };
        // The set of nodes already evaluated
        HashSet<Node> closedList = new HashSet<Node>();

        startNode.G = 0;
        startNode.H = Heuristic(startNode, endNode);

        while (openList.Count > 0)
        {
            // Get the node with the lowest F score
            Node currentNode = openList.OrderBy(n => n.F).First();

            // Check if we have reached the goal
            if (currentNode.Equals(endNode))
            {
                // Reconstruct the path
                List<Node> path = new List<Node>();
                while (currentNode != null)
                {
                    path.Add(currentNode);
                    currentNode = currentNode.Parent;
                }
                path.Reverse(); // Reverse the path to start-to-end order
                return path;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // Get passable neighboring nodes
            foreach (Node neighbor in GetNeighbors(currentNode, nodes, mapWidth, mapHeight))
            {
                if (!neighbor.Passable || closedList.Contains(neighbor))
                    continue;

                float tentativeG = currentNode.G + 1; // Assume cost between adjacent nodes is 1

                bool inOpenList = openList.Contains(neighbor);

                if (!inOpenList || tentativeG < neighbor.G)
                {
                    neighbor.G = tentativeG;
                    neighbor.H = Heuristic(neighbor, endNode);
                    neighbor.Parent = currentNode;

                    if (!inOpenList)
                        openList.Add(neighbor);
                }
            }
        }

        // No path found
        return null;
    }
}
