using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace solid_game_engine.Shared.Systems.interfaces
{
    public interface IMenuSystem
    {
			void Show(PlayerIndex playerIndex);
			void Hide();
			void Update(GameTime gameTime);
			void Draw(SpriteBatch spriteBatch);
    }
}