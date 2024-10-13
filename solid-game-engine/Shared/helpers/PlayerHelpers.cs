using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using solid_game_engine.Shared.Entities;
using solid_game_engine.Shared.entity;
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
		
		public static void GetFollowCamera(this PlayerEntity Player, OrthographicCamera camera)
		{
			camera.LookAt(new Vector2(Player.X, Player.Y));
		}
		
		public static void GetCameraInput(this PlayerEntity Player, GameTime gameTime, float speed, Dictionary<Direction, Map> mapDict, Level CurrentLevel, OrthographicCamera camera)
		{
			
			var playerCanMove = Player.CanMove;
			var kState = Keyboard.GetState();
			var cameraDirection = Vector2.Zero;
			speed = speed * (float)0.6;
			var currentMap = CurrentLevel.currentMaps.FindPlayersMap(Player);
			var atTopEdge = camera.BoundingRectangle.Top < 16;
			var shouldPanDown = atTopEdge && Player.Y.InRange(camera.Center.Y - 64, camera.Center.Y + 64) || !atTopEdge;
			var atLeftEdge = camera.BoundingRectangle.Left < 16;

	    // --------------- Player Checks
			var _player = Player;
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
				canGoDown = _player.Y < currentMap.Origin.Y + currentMap.Height && shouldPanDown;
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
			var upPressed = _player.Input.isPressed(Controls.UP);
			var downPressed = _player.Input.isPressed(Controls.DOWN);
			var leftPressed = _player.Input.isPressed(Controls.LEFT);
			var rightPressed = _player.Input.isPressed(Controls.RIGHT);

			var totalUp = upPressed && !downPressed && !rightPressed && !leftPressed;
			var totalDown = downPressed && !upPressed && !rightPressed && !leftPressed;
			var totalLeft = leftPressed && !rightPressed && !upPressed && !downPressed;
			var totalRight = rightPressed && !leftPressed && !upPressed && !downPressed;
			
			// ---------------
			
			if (playerCanMove[Direction.UP] && canGoUp && totalUp)
			{
				cameraDirection -= Vector2.UnitY;			
				camera.Move(cameraDirection * speed * gameTime.GetElapsedSeconds());
			} 
			if (playerCanMove[Direction.DOWN] && canGoDown && totalDown) // downCheck
			{
				cameraDirection += Vector2.UnitY;
				camera.Move(cameraDirection * speed * gameTime.GetElapsedSeconds());
			} 
			if (playerCanMove[Direction.LEFT] && canGoLeft && totalLeft)
			{
				cameraDirection -= Vector2.UnitX;				
				camera.Move(cameraDirection * speed * gameTime.GetElapsedSeconds());
			} 
			if (playerCanMove[Direction.RIGHT] && canGoRight && totalRight) //  rightCheck
			{
				cameraDirection += Vector2.UnitX;
				camera.Move(cameraDirection * speed * gameTime.GetElapsedSeconds());
			}
		}
		public static void GetInput(this PlayerEntity _player, GameTime gameTime, Level CurrentLevel, Dictionary<Direction, Map> mapDict)
		{
			var kState = Keyboard.GetState();
			var currentMap = CurrentLevel.currentMaps.FindPlayersMap(_player);
			// --------- Check Map Origin Can Move
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
			var upPressed = _player.Input.isPressed(Controls.UP);
			var downPressed = _player.Input.isPressed(Controls.DOWN);
			var leftPressed = _player.Input.isPressed(Controls.LEFT);
			var rightPressed = _player.Input.isPressed(Controls.RIGHT);
			
			var totalUp = upPressed && !downPressed && !rightPressed && !leftPressed;
			var totalDown = downPressed && !upPressed && !rightPressed && !leftPressed;
			var totalLeft = leftPressed && !rightPressed && !upPressed && !downPressed;
			var totalRight = rightPressed && !leftPressed && !upPressed && !downPressed;

			if (totalUp && canGoUp)
			{
				_player.Move(gameTime, Controls.UP);					
			} 
			if (totalDown && canGoDown)
			{
				_player.Move(gameTime, Controls.DOWN);
			} 
			if (totalLeft && canGoLeft)
			{
				_player.Move(gameTime, Controls.LEFT);					
			} 
			if (totalRight && canGoRight)
			{
				_player.Move(gameTime, Controls.RIGHT);
			} 
			if (!totalUp && !totalDown && !totalLeft && !totalRight)
			{
				_player.Move(gameTime, Controls.NONE);
			}
		}
	}
}