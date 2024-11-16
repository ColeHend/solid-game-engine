using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using solid_game_engine.Shared.Entities;
using solid_game_engine.Shared.entity;

namespace solid_game_engine;
public class Currents
{
		public Texture2D CurrentWindowSkin { get; set; }
		public Texture2D CurrentPlayerskin { get; set; }
		public SpriteFont CurrentFont { get; set; }
		public List<IPlayerEntity> Player { get; set; }
		public int TileSize { get; set; } = 32;
		public Vector2 ScreenResolution {get; set;} = new Vector2(1280, 720);		
}
