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
		public static Dictionary<Direction, IMap> GetMapDirections(this List<IMap> Maps, Game1 _game, PlayerIndex playerIndex = PlayerIndex.One)
		{
			var mapDict = new Dictionary<Direction, IMap>();
			var thePlayers = _game.Currents.Player.Where(player => player != null && player.X >= 0 && player.Y >= 0).ToList();
			IMap currentMap = Maps.FindPlayersMap(thePlayers.Where(player => player.Input.PlayerIndex == playerIndex).FirstOrDefault());
			if (currentMap == null)
			{
				return mapDict;
			}
			IMap? UpMap = Maps.Find(x => x.Origin.X == currentMap.Origin.X && x.Origin.Y == currentMap.Origin.Y - currentMap.Height);
			if (UpMap != null)
			{
				mapDict.Add(Direction.UP, UpMap);
			}
			IMap? DownMap = Maps.Find(x => x.Origin.X == currentMap.Origin.X && x.Origin.Y == currentMap.Origin.Y + currentMap.Height);
			if (DownMap != null)
			{
				mapDict.Add(Direction.DOWN,DownMap);
			}
			IMap? LeftMap = Maps.Find(x => x.Origin.X == currentMap.Origin.X - currentMap.Width && x.Origin.Y == currentMap.Origin.Y);
			if (LeftMap != null)
			{
				mapDict.Add(Direction.LEFT,LeftMap);
			}
			IMap? RightMap = Maps.Find(x => x.Origin.X == currentMap.Origin.X + currentMap.Width && x.Origin.Y == currentMap.Origin.Y);
			if (RightMap != null)
			{
				mapDict.Add(Direction.RIGHT, RightMap);
			}
			mapDict.Add(Direction.NONE, currentMap);
			return mapDict;
		}
		public static TileMap LoadTileMap(int mapNumber)
		{
			string currentDir = Directory.GetCurrentDirectory();
			TileMap tileMap;
			using (StreamReader reader = new StreamReader($"{currentDir}\\maps\\map0{mapNumber}.jsonc"))
			{
				string json = reader.ReadToEnd();
				tileMap = JsonConvert.DeserializeObject<TileMap>(json);
			}

			return tileMap;
		}

		public static TileSetEntity LoadTileSet(this TileMap tileMap)
		{
			var currentDir = Directory.GetCurrentDirectory();
			using (StreamReader reader = new StreamReader($"{currentDir}\\maps\\tilesets\\{tileMap.TilesetName}.jsonc"))
			{
				string json = reader.ReadToEnd();
				return JsonConvert.DeserializeObject<TileSetEntity>(json);
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
		public static IMap FindPlayersMap(this List<IMap> maps, IEntity _player)
		{
			IMap maybeMap = null;
			if (_player == null || _player.X < 0 || _player.Y < 0)
			{
				return null;
			}
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
				return null;
			}
		}
	}
}