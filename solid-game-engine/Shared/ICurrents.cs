using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using solid_game_engine.Shared.Entities;

namespace solid_game_engine;
public class Currents
{
		public Texture2D CurrentWindowSkin { get; set; }
		public Texture2D CurrentPlayerskin { get; set; }
		public SpriteFont CurrentFont { get; set; }
		public GameEntity Player { get; set; }
		public int TileSize { get; set; } = 32;		
}
