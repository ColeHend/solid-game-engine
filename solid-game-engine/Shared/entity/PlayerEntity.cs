using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using solid_game_engine.Shared.Entities;
using solid_game_engine.Shared.Enums;
using solid_game_engine.Shared.helpers;

namespace solid_game_engine.Shared.entity
{
    public class PlayerEntity : GameEntity
    {
			public InputWrap Input { get; set; }
			public Dictionary<Direction, Map> MapDirections { get; set; }
			public bool LockMovement { get; set; } = false;
			public Currents _currents { get; set; }
			public PlayerEntity(Currents currents, int X, int Y, PlayerIndex playerIndex = PlayerIndex.One) : base(currents.CurrentPlayerskin, X, Y, 32, 48, true, true)
			{
				_currents = currents;
				if (playerIndex == PlayerIndex.One)
				{
					Input = new InputWrap(playerIndex, currents.Player.GetMaxPlayerIndex());
				} else
				{
					Input = new InputWrap(playerIndex, currents.Player.GetMaxPlayerIndex());
				}
			}

			public void LoadContent(ContentManager content)
			{
				Input = new InputWrap(Input.PlayerIndex, _currents.Player.GetMaxPlayerIndex());
			}

			public override void OnCollision(CollisionEventArgs collisionInfo)
			{
				base.OnCollision(collisionInfo);
			} 

			public new void Update(GameTime gameTime)
			{
				_isMoving = Input.IsPressed(Controls.UP) || Input.IsPressed(Controls.DOWN) || Input.IsPressed(Controls.LEFT) || Input.IsPressed(Controls.RIGHT);
				Input.Update(gameTime);
				base.Update(gameTime);
			}

			public new void Draw(SpriteBatch spriteBatch)
			{
				base.Draw(spriteBatch);
			}       

    }

}