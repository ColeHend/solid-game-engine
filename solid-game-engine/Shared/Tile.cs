using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using Newtonsoft.Json;
using solid_game_engine.Shared.Entities;

namespace solid_game_engine.Shared
{
    public class Tile
    {
      public List<int> TileNums { get; set; }
			public int X { get; set; }
			public int Y { get; set; }
			public int Size { get; set; }
			public bool Passable { get; set; } = true;
			public int Effect { get; set; }
			public int MinX { get {
				return X * Size;
			}}
			public int MaxX { get {
				return (X * Size) + Size;
			}}
			public int MinY { get {
				return Y * Size;
			}}
			public int MaxY { get {
				return (Y * Size) + Size;
			}}
    }

	public class TileCollide : IEntity
	{
		public IShapeF Bounds { get; }

		public TileCollide(Tile tile)
		{
			double diff = 0.6;
			var smallerWidth = (int)(tile.Size * diff);
			var smallerHeight = (int)(tile.Size * diff);
			var smallDiff = tile.Size - smallerWidth;

			Bounds = new RectangleF(tile.MinX + smallDiff, tile.MinY + smallDiff, smallerWidth, smallerHeight);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
		}

		public void OnCollision(CollisionEventArgs collisionInfo)
		{
			
		}

		public void Update(GameTime gameTime)
		{
		}
	}
}