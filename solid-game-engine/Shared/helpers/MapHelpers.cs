using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using Newtonsoft.Json;
using solid_game_engine.Shared.Entities;

namespace solid_game_engine.Shared.helpers
{
    public static class MapHelpers
	{
		public static Dictionary<Direction, Map> GetMapDirections(this List<Map> Maps, Game1 _game)
		{
			var mapDict = new Dictionary<Direction, Map>();
			Map currentMap = Maps.FindPlayersMap(_game.Currents.Player);
			Map? UpMap = Maps.Find(x => x.Origin.X == currentMap.Origin.X && x.Origin.Y == currentMap.Origin.Y - currentMap.Height);
			if (UpMap != null)
			{
				mapDict.Add(Direction.UP, UpMap);
			}
			Map? DownMap = Maps.Find(x => x.Origin.X == currentMap.Origin.X && x.Origin.Y == currentMap.Origin.Y + currentMap.Height);
			if (DownMap != null)
			{
				mapDict.Add(Direction.DOWN,DownMap);
			}
			Map? LeftMap = Maps.Find(x => x.Origin.X == currentMap.Origin.X - currentMap.Width && x.Origin.Y == currentMap.Origin.Y);
			if (LeftMap != null)
			{
				mapDict.Add(Direction.LEFT,LeftMap);
			}
			Map? RightMap = Maps.Find(x => x.Origin.X == currentMap.Origin.X + currentMap.Width && x.Origin.Y == currentMap.Origin.Y);
			if (RightMap != null)
			{
				mapDict.Add(Direction.RIGHT, RightMap);
			}
			mapDict.Add(Direction.NONE, currentMap);
			return mapDict;
		}
		public static TileMap LoadTileMap(int mapNumber)
		{
				string thePath = $"{Directory.GetCurrentDirectory()}\\maps\\map0{mapNumber}.jsonc";
				using (StreamReader reader = new StreamReader(thePath))
				{
					string json = reader.ReadToEnd();
					TileMap tileMap = JsonConvert.DeserializeObject<TileMap>(json);
					return tileMap;
				}
		}
		public static void DrawLayer(this List<List<List<int>>> TileMap, SpriteBatch _spriteBatch, int TileSize, Vector2 Origin, TileSet tileSet, int layer, RectangleF screenCoordinates)
		{
			for (int i = 0; i < TileMap[layer].Count; i++)
				{
					int drawX = (i * TileSize) + (int)Origin.X;
					for (int j = 0; j < TileMap[i].Count; j++)
					{
						var drawY = j * TileSize + (int)Origin.Y;
						var inScreen = (int screenX, int screenY) => ((float)screenX).InRange(screenCoordinates.Left-TileSize, screenCoordinates.Right+TileSize) && ((float)screenY).InRange(screenCoordinates.Top-TileSize, screenCoordinates.Bottom+TileSize);
						var tileLayers = TileMap[i][j];
						if (tileLayers.Count >= layer && layer >= 1 && inScreen(drawX, drawY))
						{
							var tileNumber = tileLayers[layer - 1];
							var tileAtlas = tileSet.Tileset.TextureAtlas;
							var singleTile = tileAtlas.ElementAt((int)tileNumber);
							var tilePos = new Vector2(drawX, drawY);
							_spriteBatch.Draw(singleTile, tilePos, Color.White);
						}
					}
				}
		}
		public static List<List<int>> GetLayer(this List<List<List<int>>> tileMap, int layer)
		{
			return tileMap[layer];
		}
		public static Map FindPlayersMap(this List<Map> maps, GameEntity _player)
		{
			Map maybeMap = null;
			foreach (var map in maps)
			{
				var lowX = map.Origin.X;
				var highX = map.Origin.X + map.Width;
				var lowY = map.Origin.Y;
				var highY = map.Origin.Y + map.Height;
				if (_player.X.InRange(lowX, highX) && _player.Y.InRange(lowY, highY))
				{
					return map;
				}
				var equalX = _player.X == lowX || _player.X == highX;
				var equalY = _player.Y == lowY || _player.Y == highY;
				if ((_player.X.InRange(lowX, highX) || equalX) && (_player.Y.InRange(lowY, highY) || equalY))
				{
					maybeMap = map;
				} 
			}
			if (maybeMap != null)
			{
				return maybeMap;
			} else {
				throw new Exception("Unable To Find Players Map!");
			}
		}
	}
}