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
using solid_game_engine.Shared.entity;
using solid_game_engine.Shared.helpers;
using solid_game_engine.Shared.scenes;

namespace solid_game_engine.Shared.Entities;

public class Level 
{
	private List<Map> Maps { get; set; }
	public List<Map> currentMaps { get; set; } = new List<Map>();
	private readonly Game1 _game;

	public Func<Dictionary<Direction, Map>, int, int, Tile> GetTile { get; set; }
	public Func<List<Tile>> GetPlayerTile { get; set; }
	public Scene_Game _sceneGame { get; }
	public Func<PlayerEntity, string> MapChangeAction { get; set; }
	public Level(Scene_Game sceneGame, SceneManager sceneManager, List<Map> maps)
	{
		Maps = maps;
		_sceneGame = sceneGame;
		_game = sceneManager.Game;
	}

	public void Initialize(GraphicsDeviceManager _graphicsDeviceManager)
	{
		foreach (var map in Maps)
		{
			map.Initialize(_graphicsDeviceManager);
		}
	}

	public void LoadContent(ContentManager contentManager)
	{
		_game.Currents.Player.ForEach(player => {
			player.MapDirections = Maps.GetMapDirections(_game);
		});
		currentMaps.AddRange(_game.Currents.Player[0].MapDirections.Values);
		for (int i = 0; i < Maps.Count; i++)
		{
			Maps[i].LoadContent(contentManager);
		}
		GetTile = (Dictionary<Direction, Map> Dictionary, int x, int y) => {
			return Dictionary[Direction.NONE].TileInfo[y][x];
		};
		GetPlayerTile = () => {
			var playerTiles = _game.Currents.Player.Select(player => {
				var playerX = (int)player.X / 32;
				var playerY = (int)player.Y / 32;
				return GetTile(player.MapDirections, playerX, playerY);
			});
			return playerTiles.ToList();
		};
	}

	public void Update(GameTime gameTime)
	{
		foreach (var player in _game.Currents.Player)
		{
			if (player.MapDirections != null && player.MapDirections.ContainsKey(Direction.NONE))
			{
				var currentMap = player.MapDirections[Direction.NONE];
				var onCurrentX = player.X.InRange(currentMap.Origin.X, currentMap.Origin.X + currentMap.Width);
				var onCurrentY = player.Y.InRange(currentMap.Origin.Y, currentMap.Origin.Y + currentMap.Height);
				if (!onCurrentX || !onCurrentY)
				{
					player.MapDirections = Maps.GetMapDirections(_game, player.Input.PlayerIndex);
					int currentMapLength = currentMaps.Count;
					currentMaps.AddRange(player.MapDirections.Values);
					currentMaps.RemoveRange(0, currentMapLength);
					if (MapChangeAction != null)
					{
						MapChangeAction(player);
					}
				}
			}
		}

		foreach (var map in currentMaps)
		{
			map.Update(gameTime);
		}
	}

	public void Draw(SpriteBatch _spriteBatch)
	{
		foreach (var map in currentMaps)
		{
			map.Draw(_spriteBatch);
		}
	}
}

	
