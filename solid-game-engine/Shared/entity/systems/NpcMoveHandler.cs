using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace solid_game_engine.Shared.entity
{
    public class NpcMoveHandler
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
				path = pathfinder.FindPath(CurrentTileMap, CurrentTileSet, (int)CurrentPosition.X, (int)CurrentPosition.Y, (int)end.X, (int)end.Y);
			}

			public void ClearPath()
			{
				path = null;
			}
			
			public void Update(GameTime gameTime)
			{

			}

			public void Draw(SpriteBatch spriteBatch)
			{

			}
    }
}