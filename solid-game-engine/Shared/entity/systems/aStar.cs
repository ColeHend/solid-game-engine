namespace solid_game_engine.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Node class for A* pathfinding
/// </summary>
public class Node
{
	public int X { get; set; } // X position on the map
	public int Y { get; set; } // Y position on the map
	public int realX { get {
		return X * _tileSize + (_tileSize / 2);
	}}
	public int realY { get {
		return Y * _tileSize + (_tileSize / 2);
	}}
	public int _tileSize { get; set; }
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
/// <summary>
/// A* pathfinding algorithm
/// </summary>
public class Pathfinding
{
	// Heuristic function using Manhattan distance
	private float Heuristic(Node a, Node b)
	{
		return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
	}

	/// <summary>
	/// Get neighboring nodes (up, down, left, right)
	/// </summary>
	/// <param name="node"></param>
	/// <param name="nodes"></param>
	/// <param name="mapWidth"></param>
	/// <param name="mapHeight"></param>
	/// <returns></returns>
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

	/// <summary>
	/// Main A* pathfinding function
	/// </summary>
	/// <param name="tileMap"></param>
	/// <param name="tileSet"></param>
	/// <param name="startX">Start X</param>
	/// <param name="startY">Start Y</param>
	/// <param name="endX">End X</param>
	/// <param name="endY">End Y</param>
	/// <returns></returns>
	public List<Node> FindPath(TileMap tileMap, TileSet tileSet, int startX, int startY, int endX, int endY)
	{
		int mapHeight = tileMap.Tiles.Count;
		int mapWidth = tileMap.Tiles[0].Count;

		Node[,] nodes = new Node[mapWidth, mapHeight];
		if (startX < 0 || startY < 0 || endX < 0 || endY < 0 || startX >= mapWidth || startY >= mapHeight || endX >= mapWidth || endY >= mapHeight)
		{
			return null;
		}
		for (int y = 0; y < tileMap.Tiles.Count; y++)
		{
			for (int x = 0; x < tileMap.Tiles[y].Count; x++)
			{
				bool passable = true;
				for (int l = 0; l < tileMap.Tiles[y][x].Count; l++)
				{
					int tileID = tileMap.Tiles[y][x][l];

					if (tileSet.Passable.TryGetValue(tileID, out bool isPassable))
					{
						if (!isPassable)
						{
							passable = false;
							break; 
						}
					}
					else
					{
						passable = true;
						break;
					}
				}

				nodes[x, y] = new Node
				{
					X = x,
					Y = y,
					Passable = passable,
					_tileSize = (int)tileSet.TileSize
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
				// path.Reverse(); // Reverse the path to start-to-end order
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
