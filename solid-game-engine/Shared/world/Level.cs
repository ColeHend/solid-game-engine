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
public interface ILevel
{
	List<IMap> currentMaps { get; set; }
	Scene_Game _sceneGame { get; }
	Func<IPlayerEntity, string> MapChangeAction { get; set; }
	Func<Dictionary<Direction, IMap>, int, int, Tile> GetTile { get; set; }
	void Initialize(GraphicsDeviceManager _graphicsDeviceManager);
	void LoadContent(ContentManager contentManager);
	void Update(GameTime gameTime);
	void Draw(SpriteBatch _spriteBatch);
}
public class Level : ILevel
{
	private List<IMap> Maps { get; set; }
	public List<IMap> currentMaps { get; set; } = new List<IMap>();
	private readonly Game1 _game;

	public Func<Dictionary<Direction, IMap>, int, int, Tile> GetTile { get; set; }
	public Func<List<Tile>> GetPlayerTile { get; set; }
	public Scene_Game _sceneGame { get; }
	public Func<IPlayerEntity, string> MapChangeAction { get; set; }
	private ISceneManager _sceneManager { get; }
	public Level(Scene_Game sceneGame, ISceneManager sceneManager, List<IMap> maps)
	{
		Maps = maps;
		_sceneGame = sceneGame;
		_sceneManager = sceneManager;
		_game = _sceneManager.Game;
	}
	public Level(ISceneManager sceneManager)
	{
		_sceneManager = sceneManager;
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
		GetTile = (Dictionary<Direction, IMap> Dictionary, int x, int y) => {
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
			if (player.MapDirections == null)
			{
				player.MapDirections = Maps.GetMapDirections(_game, player.Input.PlayerIndex);
			}
			if (player.MapDirections.ContainsKey(Direction.NONE))
			{
				var currentMap = player.MapDirections[Direction.NONE];
				var onCurrentX = player.X.InRange(currentMap.Origin.X, currentMap.Origin.X + currentMap.Width);
				var onCurrentY = player.Y.InRange(currentMap.Origin.Y, currentMap.Origin.Y + currentMap.Height);
				if (!onCurrentX || !onCurrentY || currentMaps.Count == 0)
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

	
