using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using solid_game_engine.Shared.helpers;

namespace solid_game_engine.Shared.entity
{
	public interface INpcMoveHandler
	{
		Node GetNextNode();
		void RemoveNode();
		void SetMapInfo(TileMap tileMap, TileSet tileSet);
		void SetPosition(int x, int y);
		void SetPath(Vector2 end);
		void ClearPath();
		void Update(GameTime gameTime);
		void Draw(SpriteBatch spriteBatch);
	}
	public class NpcMoveHandler : INpcMoveHandler
	{
		private List<Node> path { get; set; }
		private Pathfinding pathfinder { get; set; }
		private TileMap CurrentTileMap { get; set; }
		private TileSet CurrentTileSet { get; set; }
		private Vector2 CurrentPosition { get; set; }
		public NpcMoveHandler()
		{
			pathfinder = new Pathfinding();
		}

		public void SetMapInfo(TileMap tileMap, TileSet tileSet)
		{
			CurrentTileMap = tileMap;
			CurrentTileSet = tileSet;
		}
		public void SetPosition(int x, int y)
		{
			CurrentPosition = new Vector2(x, y);
		}

		public void SetPath(Vector2 end)
		{
			path = pathfinder.FindPath(CurrentTileMap, CurrentTileSet, (int)CurrentPosition.X, (int)CurrentPosition.Y, (int)end.X, (int)end.Y).Reverse<Node>().ToList();
			RemoveNode();
		}

		public List<Node> GetPath()
		{
			return path;
		}

		public Node GetNextNode()
		{
			if (path == null || path.Count == 0)
			{
				return null;
			}
			return path[0];
		}

		public void RemoveNode()
		{
			if (path == null || path.Count == 0)
			{
				return;
			}
			path.RemoveAt(0);
		}

		public void ClearPath()
		{
			path = null;
		}
		
		public void Update(GameTime gameTime)
		{
			var nextNode = GetNextNode();
			if (nextNode != null && CurrentPosition.X.InRange(nextNode.X - 1, nextNode.X + 1) && CurrentPosition.Y.InRange(nextNode.Y - 1, nextNode.Y + 1))
			{
				RemoveNode();
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{

		}
	}
}