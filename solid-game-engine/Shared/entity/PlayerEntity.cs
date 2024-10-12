using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using solid_game_engine.Shared.Entities;

namespace solid_game_engine.Shared.entity
{
    public class PlayerEntity : GameEntity
    {
			public PlayerEntity(Currents currents, int X, int Y) : base(currents.CurrentPlayerskin, X, Y, 32, 48, true, true)
			{
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