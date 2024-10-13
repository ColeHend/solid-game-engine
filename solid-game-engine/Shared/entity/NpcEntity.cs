using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using solid_game_engine.Shared.Entities;

namespace solid_game_engine.Shared.entity
{
	public class NpcEntity : GameEntity
	{
		private List<Direction> MovementQue { get; set; } = new List<Direction>();
		public NpcEntity(Texture2D texture, int X, int Y, int Width, int Height) : base(texture, X, Y, Width, Height, false, false)
		{
		}

		public new void OnCollision(CollisionEventArgs collisionInfo)
		{
			base.OnCollision(collisionInfo);

		}

		public new void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		public new void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
		}
	}

}