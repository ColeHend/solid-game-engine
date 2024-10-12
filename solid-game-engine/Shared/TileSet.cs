using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Graphics;
using solid_game_engine.Shared.helpers;

namespace solid_game_engine;

public class TileSet
{
	public SpriteSheet Tileset {get; set;}
	public float TileSize { get; set; }
	public float Width { get; set; }
	public float Height { get; set; }
	public TileSet(Texture2D texture, float width, float height, float tileSize)
	{
		TileSize = tileSize;
		Width = width;
		Height = height;
		Tileset = texture.GetSpriteSheet((int)TileSize, (int)TileSize);
	}			
}
