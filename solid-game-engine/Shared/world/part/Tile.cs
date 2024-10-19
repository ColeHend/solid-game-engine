using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Graphics;
using Newtonsoft.Json;
using solid_game_engine.Shared.Entities;
using solid_game_engine.Shared.Enums;

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

		public bool _IsPlayer { get; } = false;
		public bool IsTile { get; } = true;
		public float X { get ; set ; }
		public float Y { get ; set ; }
		public AnimatedSprite _sprite { get ; set ; }
		public Direction _facing { get ; set ; }
		public float _speed { get ; set ; }
		IShapeF IEntity.Bounds { get ; set ; }
		public SpriteSheet _spriteSheet { get ; set ; }
		public bool _isMoving { get ; set ; }
		public Matrix matrix { get ; set ; }
		public Dictionary<Direction, bool> CanMove { get ; set ; }

		public TileCollide(Tile tile, Vector2 Origin)
		{
			double diff = 0.6;
			var smallerWidth = (int)(tile.Size * diff);
			var smallerHeight = (int)(tile.Size * diff);
			var smallDiff = tile.Size - smallerWidth;

			Bounds = new RectangleF(tile.MinX + smallDiff + Origin.X, tile.MinY + smallDiff + Origin.Y, smallerWidth, smallerHeight);
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

		public void SetSpritesheet(Texture2D texture, int Width, int Height)
		{
		}

		public void Move(GameTime gameTime, Controls dir)
		{
			
		}
	}
}