using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using solid_game_engine.Shared.Entities;
using solid_game_engine.Shared.Enums;

namespace solid_game_engine.Shared.entity
{
    public class PlayerEntity : GameEntity
    {
			public InputWrap Input { get; set; }
			public PlayerEntity(Currents currents, int X, int Y, PlayerIndex playerIndex = PlayerIndex.One) : base(currents.CurrentPlayerskin, X, Y, 32, 48, true, true)
			{
				var giveKeyboard = false;
				if (playerIndex == PlayerIndex.One)
				{
					giveKeyboard = true;
				}
				Input = new InputWrap(playerIndex, giveKeyboard);
			} 

			public new void Update(GameTime gameTime)
			{
				Input.Update();
				base.Update(gameTime);
			}

			public new void Draw(SpriteBatch spriteBatch)
			{
				base.Draw(spriteBatch);
			}       

    }

}