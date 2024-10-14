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
		public static PlayerIndex GetMaxPlayerIndex(this List<PlayerEntity> players)
		{
			var maxIndex = players?.Max(p => p.Input.PlayerIndex);

			return maxIndex ?? PlayerIndex.One;
		}
		public static PlayerEntity FindNearestPlayer(this List<PlayerEntity> players, float X, float Y)
		{
			var targetLocation = new Vector2(X, Y);
			var nearestPlayer = players.OrderBy(p => Vector2.Distance(new Vector2(p.X, p.Y), targetLocation)).FirstOrDefault();
			return nearestPlayer;
		}
		public static PlayerEntity FindNearestPlayer(this List<PlayerEntity> players, GameEntity entity)
		{
			var targetLocation = new Vector2(entity.X, entity.Y);
			var nearestPlayer = players.OrderBy(p => Vector2.Distance(new Vector2(p.X, p.Y), targetLocation)).FirstOrDefault();
			return nearestPlayer;
		}

		public static bool DirectionCheck(this PlayerEntity _player, Direction direction, Map playerMap)
		{
			var canGoUp = true;
			var mapDict = _player.MapDirections;
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
		
		public static void GetCameraInput(this PlayerEntity _player, GameTime gameTime, float speed, Level CurrentLevel, OrthographicCamera camera)
		{
			
			var playerCanMove = _player.CanMove;
			var kState = Keyboard.GetState();
			var cameraDirection = Vector2.Zero;
			speed = speed * (float)0.6;
			var currentMap = CurrentLevel.currentMaps.FindPlayersMap(_player);
			var atTopEdge = camera.BoundingRectangle.Top < 16;
			var shouldPanDown = atTopEdge && _player.Y.InRange(camera.Center.Y - 64, camera.Center.Y + 64) || !atTopEdge;
			var atLeftEdge = camera.BoundingRectangle.Left < 16;

	    // --------------- Player Checks
			var mapDict = _player.MapDirections;
			// -------- Up
			bool canGoUp;
			if (mapDict.ContainsKey(Direction.UP))
			{
				canGoUp = _player.DirectionCheck(Direction.UP, currentMap);
			} else
			{
				canGoUp = _player.Y > currentMap.Origin.Y;
			}
			// -------- Down
			bool canGoDown;
			if (mapDict.ContainsKey(Direction.DOWN))
			{
				canGoDown = _player.DirectionCheck(Direction.DOWN, currentMap);
			} else
			{
				canGoDown = _player.Y < currentMap.Origin.Y + currentMap.Height && shouldPanDown;
			}
			// -------- Left
			bool canGoLeft;
			if (mapDict.ContainsKey(Direction.LEFT))
			{
				canGoLeft = _player.DirectionCheck(Direction.LEFT, currentMap);
			} else
			{
				canGoLeft = _player.X > currentMap.Origin.X;
			}
			// -------- Right
			bool canGoRight;
			if (mapDict.ContainsKey(Direction.RIGHT))
			{
				canGoRight = _player.DirectionCheck(Direction.RIGHT, currentMap);
			} else {
				canGoRight = _player.X < currentMap.Origin.X + currentMap.Width;
			}
			var upPressed = _player.Input.IsPressed(Controls.UP);
			var downPressed = _player.Input.IsPressed(Controls.DOWN);
			var leftPressed = _player.Input.IsPressed(Controls.LEFT);
			var rightPressed = _player.Input.IsPressed(Controls.RIGHT);

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
		public static void GetInput(this PlayerEntity _player, GameTime gameTime, Map CurrentMap, bool lockMovement = false)
		{
			var kState = Keyboard.GetState();
			var currentMap = CurrentMap;
			var mapDict = _player.MapDirections;
			// --------- Check Map Origin Can Move
			bool canGoUp;
			if (mapDict.ContainsKey(Direction.UP))
			{
				canGoUp = _player.DirectionCheck(Direction.UP, currentMap);
			} else
			{
				canGoUp = _player.Y > currentMap.Origin.Y;
			}
			// -------- Down
			bool canGoDown;
			if (mapDict.ContainsKey(Direction.DOWN))
			{
				canGoDown = _player.DirectionCheck(Direction.DOWN, currentMap);
			} else
			{
				canGoDown = _player.Y < currentMap.Origin.Y + currentMap.Height;
			}
			// -------- Left
			bool canGoLeft;
			if (mapDict.ContainsKey(Direction.LEFT))
			{
				canGoLeft = _player.DirectionCheck(Direction.LEFT, currentMap);
			} else
			{
				canGoLeft = _player.X > currentMap.Origin.X;
			}
			// -------- Right
			bool canGoRight;
			if (mapDict.ContainsKey(Direction.RIGHT))
			{
				canGoRight = _player.DirectionCheck(Direction.RIGHT, currentMap);
			} else {
				canGoRight = _player.X < currentMap.Origin.X + currentMap.Width;
			}
			var upPressed = _player.Input.IsPressed(Controls.UP);
			var downPressed = _player.Input.IsPressed(Controls.DOWN);
			var leftPressed = _player.Input.IsPressed(Controls.LEFT);
			var rightPressed = _player.Input.IsPressed(Controls.RIGHT);
			
			var totalUp = upPressed && !downPressed && !rightPressed && !leftPressed;
			var totalDown = downPressed && !upPressed && !rightPressed && !leftPressed;
			var totalLeft = leftPressed && !rightPressed && !upPressed && !downPressed;
			var totalRight = rightPressed && !leftPressed && !upPressed && !downPressed;

			if (totalUp && canGoUp && !lockMovement)
			{
				_player.Move(gameTime, Controls.UP);					
			} 
			if (totalDown && canGoDown && !lockMovement)
			{
				_player.Move(gameTime, Controls.DOWN);
			} 
			if (totalLeft && canGoLeft && !lockMovement)
			{
				_player.Move(gameTime, Controls.LEFT);					
			} 
			if (totalRight && canGoRight && !lockMovement)
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