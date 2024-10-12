using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using solid_game_engine.Shared.Entities;
using solid_game_engine.Shared.Enums;

namespace solid_game_engine.Shared.helpers
{
    public static class PlayerHelpers
	{
		public static bool DirectionCheck(this GameEntity _player, Direction direction, Dictionary<Direction, Map> mapDict, Map playerMap)
		{
			var canGoUp = true;
			if (direction == Direction.UP)
			{
				var upMap = mapDict[Direction.UP];
				bool xMatch = upMap.Origin.X == playerMap.Origin.X;
				bool wMatch = upMap.Width == playerMap.Width;
				if (!xMatch || !wMatch)
				{
					bool playerXLinedUp = ((float)_player.X).InRange(upMap.Origin.X, upMap.Origin.X + upMap.Width);
					if (!playerXLinedUp)
					{
						canGoUp = false;
					}
				}
				return canGoUp;
			} 
			var canGoDown = true;
			if (direction == Direction.DOWN)
			{
				var downMap = mapDict?[Direction.DOWN];
				bool xMatch = downMap.Origin.X == playerMap.Origin.X;
				bool wMatch = downMap.Width == playerMap.Width;
				if (!xMatch || !wMatch)
				{
					bool playerXLinedUp = ((float)_player.X).InRange(downMap.Origin.X, downMap.Origin.X + downMap.Width);
					if (!playerXLinedUp)
					{
						canGoDown = false;
					}
				}
				return canGoDown;
			} 
			var canGoLeft = true;
			if (direction == Direction.LEFT)
			{
				var leftMap = mapDict?[Direction.LEFT];
				bool yMatch = leftMap.Origin.Y == playerMap.Origin.Y;
				bool hMatch = leftMap.Height == playerMap.Height;
				if (!yMatch || !hMatch)
				{
					bool playerYLinedUp = ((float)_player.Y).InRange(leftMap.Origin.Y, leftMap.Origin.Y + leftMap.Height);
					if (!playerYLinedUp)
					{
						canGoLeft = false;
					}
				}
				return canGoLeft;
			} 
			var canGoRight = true;
			if (direction == Direction.RIGHT)
			{
				var rightMap = mapDict?[Direction.RIGHT];
				bool yMatch = rightMap.Origin.Y == playerMap.Origin.Y;
				bool hMatch = rightMap.Height == playerMap.Height;
				if (!yMatch || !hMatch)
				{
					bool playerYLinedUp = ((float)_player.Y).InRange(rightMap.Origin.Y, rightMap.Origin.Y + rightMap.Height);
					if (!playerYLinedUp)
					{
						canGoRight = false;
					}
				}
				return canGoRight;
			}
			return true; 
		}
		public static void GetInput(this Game1 game, GameTime gameTime, float speed, Dictionary<Direction, Map> mapDict)
		{
			var camera = game._camera;
			var playerCanMove = game.Currents.Player.CanMove;
			var kState = Keyboard.GetState();
			var cameraDirection = Vector2.Zero;
			speed = speed * (float)0.6;
			var currentMap = game.CurrentLevel.currentMaps.FindPlayersMap(game.Currents.Player);
			// Camera Direction Checks
			// ---- Is Centered -----
			var centerX = camera.Center.X;
			var centerY = camera.Center.Y;
			var tile = 48;
			bool centeredX = game.Currents.Player.X.InRange(centerX - tile, centerX + tile);
			bool centeredY = game.Currents.Player.Y.InRange(centerY - tile, centerY + tile);
			// --- UP
			bool upCheck = game.Currents.Player.Y >= 0;
			// --- DOWN
			bool downCheck = camera.BoundingRectangle.Y + camera.BoundingRectangle.Height <= currentMap.Height && centeredY;
			// --- LEFT
			bool leftCheck = game.Currents.Player.X >= 0;
			// --- RIGHT
			bool rightCheck = camera.BoundingRectangle.X + camera.BoundingRectangle.Width <= currentMap.Width && centeredX;

	    // --------------- Player Checks
			var _player = game.Currents.Player;
			// -------- Up
			bool canGoUp;
			if (mapDict.ContainsKey(Direction.UP))
			{
				canGoUp = _player.DirectionCheck(Direction.UP, mapDict, currentMap);
			} else
			{
				canGoUp = _player.Y > currentMap.Origin.Y;
			}
			// -------- Down
			bool canGoDown;
			if (mapDict.ContainsKey(Direction.DOWN))
			{
				canGoDown = _player.DirectionCheck(Direction.DOWN, mapDict, currentMap);
			} else
			{
				canGoDown = _player.Y < currentMap.Origin.Y + currentMap.Height;
			}
			// -------- Left
			bool canGoLeft;
			if (mapDict.ContainsKey(Direction.LEFT))
			{
				canGoLeft = _player.DirectionCheck(Direction.LEFT, mapDict, currentMap);
			} else
			{
				canGoLeft = _player.X > currentMap.Origin.X;
			}
			// -------- Right
			bool canGoRight;
			if (mapDict.ContainsKey(Direction.RIGHT))
			{
				canGoRight = _player.DirectionCheck(Direction.RIGHT, mapDict, currentMap);
			} else {
				canGoRight = _player.X < currentMap.Origin.X + currentMap.Width;
			}
			
			// ---------------
			if (playerCanMove[Direction.UP] && kState.IsKeyDown(Keys.Up) && canGoUp)
			{
				cameraDirection -= Vector2.UnitY;			
				camera.Move(cameraDirection * speed * gameTime.GetElapsedSeconds());
			} else if (playerCanMove[Direction.DOWN] && kState.IsKeyDown(Keys.Down) && canGoDown) // && downCheck
			{
				cameraDirection += Vector2.UnitY;
				camera.Move(cameraDirection * speed * gameTime.GetElapsedSeconds());
			} else if (playerCanMove[Direction.LEFT] && kState.IsKeyDown(Keys.Left)  && canGoLeft)
			{
				cameraDirection -= Vector2.UnitX;				
				camera.Move(cameraDirection * speed * gameTime.GetElapsedSeconds());
			} else if (playerCanMove[Direction.RIGHT] && kState.IsKeyDown(Keys.Right) && canGoRight) //  && rightCheck
			{
				cameraDirection += Vector2.UnitX;
				camera.Move(cameraDirection * speed * gameTime.GetElapsedSeconds());
			}
		}
		public static void GetInput(this GameEntity _player, GameTime gameTime, Game1 game, Dictionary<Direction, Map> mapDict)
		{
			var kState = Keyboard.GetState();
			var currentMap = game.CurrentLevel.currentMaps.FindPlayersMap(_player);
			bool canGoUp;
			if (mapDict.ContainsKey(Direction.UP))
			{
				canGoUp = _player.DirectionCheck(Direction.UP, mapDict, currentMap);
			} else
			{
				canGoUp = _player.Y > currentMap.Origin.Y;
			}
			// -------- Down
			bool canGoDown;
			if (mapDict.ContainsKey(Direction.DOWN))
			{
				canGoDown = _player.DirectionCheck(Direction.DOWN, mapDict, currentMap);
			} else
			{
				canGoDown = _player.Y < currentMap.Origin.Y + currentMap.Height;
			}
			// -------- Left
			bool canGoLeft;
			if (mapDict.ContainsKey(Direction.LEFT))
			{
				canGoLeft = _player.DirectionCheck(Direction.LEFT, mapDict, currentMap);
			} else
			{
				canGoLeft = _player.X > currentMap.Origin.X;
			}
			// -------- Right
			bool canGoRight;
			if (mapDict.ContainsKey(Direction.RIGHT))
			{
				canGoRight = _player.DirectionCheck(Direction.RIGHT, mapDict, currentMap);
			} else {
				canGoRight = _player.X < currentMap.Origin.X + currentMap.Width;
			}
			
			if (kState.IsKeyDown(Keys.Up) && canGoUp)
			{
				_player.Move(gameTime, Controls.UP);					
			} else if (kState.IsKeyDown(Keys.Down) && canGoDown)
			{
				_player.Move(gameTime, Controls.DOWN);
			} else if (kState.IsKeyDown(Keys.Left) && canGoLeft)
			{
				_player.Move(gameTime, Controls.LEFT);					
			} else if (kState.IsKeyDown(Keys.Right) && canGoRight)
			{
				_player.Move(gameTime, Controls.RIGHT);
			} else
			{
				_player.Move(gameTime, Controls.NONE);
			}
		}
	}
}