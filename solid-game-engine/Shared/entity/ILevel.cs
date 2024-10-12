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

namespace solid_game_engine.Shared.Entities;

public class Level 
{
	private List<Map> Maps { get; set; }
	public List<Map> currentMaps { get; set; } = new List<Map>();
	public Dictionary<Direction, Map> MapDirections { get; set; }
	private readonly Game1 _game;

	public Level(Game1 game, List<Map> maps)
	{
		Maps = maps;
		_game = game;
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
		MapDirections = Maps.GetMapDirections(_game);
		currentMaps.AddRange(MapDirections.Values);
		foreach (var map in Maps)
		{
			map.LoadContent(contentManager);
			map._collisionComponent.Insert(_game.Currents.Player);
		}
	}

	public void Update(GameTime gameTime)
	{
		if (MapDirections.ContainsKey(Direction.NONE))
		{
			var currentMap = MapDirections[Direction.NONE];
			var player = _game.Currents.Player;
			var onCurrentX = player.X.InRange(currentMap.Origin.X, currentMap.Origin.X + currentMap.Width);
			var onCurrentY = player.Y.InRange(currentMap.Origin.Y, currentMap.Origin.Y + currentMap.Height);
			if (!onCurrentX || !onCurrentY)
			{
				MapDirections = Maps.GetMapDirections(_game);
				int currentMapLength = currentMaps.Count;
				currentMaps.AddRange(MapDirections.Values);
				currentMaps.RemoveRange(0, currentMapLength);
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
			map.Draw(_spriteBatch, _game.transformMatrix);
		}
	}
}

	
