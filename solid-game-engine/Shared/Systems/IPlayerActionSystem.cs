using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using solid_game_engine.Shared.Entities;
using solid_game_engine.Shared.entity;

namespace solid_game_engine.Shared.Systems
{
    public interface IPlayerActionSystem
    {
			void SetCurrentPlayerMap(IPlayerEntity player, IMap map);
			void Update(GameTime gameTime);
			void Draw(SpriteBatch spriteBatch);
    }
}