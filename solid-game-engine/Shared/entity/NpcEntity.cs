using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Graphics;
using Newtonsoft.Json;
using solid_game_engine.Shared.Entities;
using solid_game_engine.Shared.entity.NPCActions;
using solid_game_engine.Shared.Enums;
using solid_game_engine.Shared.helpers;

namespace solid_game_engine.Shared.entity
{
	public interface INpcEntity : IEntity
	{
		int ActionIndex { get; }
		NpcMovement MovementType { get; set; }
		NpcTriggerTypes TriggerType { get; set; }
		bool Pushable { get; set; }
		void PlayerActionTrigger(IPlayerEntity player);
		void DrawActions(SpriteBatch spriteBatch);
		void SetLocation(int x, int y, Vector2 origin);
		void AddAction(INPCActions action);
		void SetMoveMap(TileMap tileMap, TileSet tileSet);
	}
	public class NpcEntity : INpcEntity
	{
		public int ActionIndex { get { return RunningActionIndex; } }
		public NpcTriggerTypes TriggerType { get; set; }
		public AnimatedSprite _sprite { get; set; }
		public Direction _facing { get; set; }
		public float _speed { get; set; }
		public IShapeF Bounds { get; set;} = new RectangleF(0, 0, 21, 32);
		public bool _isMoving { get; set; }
		public bool _IsPlayer { get; } = false;
		public bool IsTile { get; } = false;
		public NpcMovement MovementType { get; set; }
		public Matrix matrix { get; set; }
		public SpriteSheet _spriteSheet { get; set; }
		public Dictionary<Direction, bool> CanMove { get; set; } = new Dictionary<Direction, bool>();
		public bool Pushable { get; set; } = true;
		public float X
		{
			get { return Bounds.Position.X; }
			set { 
				_position = new RectangleF(value, _position.Y, _position.Width, _position.Height); 
				Bounds.Position = new Vector2(_position.X, _position.Y);
			}
		}
		public float Y
		{
			get { return Bounds.Position.Y; }
			set { 
				_position = new RectangleF(_position.X, value, _position.Width, _position.Height); 
				Bounds.Position = new Vector2(_position.X, _position.Y);
			}
		}

		private List<INPCActions> Actions { get; } = new List<INPCActions>();
		private int RunningActionIndex { get; set; } = -1;
		private ISceneManager _sceneManager { get; set; }
		private Action FreePlayer { get; set; }
		private string _lastAnimation { get; set; } = "face-down";
		private RectangleF _position { get; set; }
		private List<Direction> MovementQue { get; set; } = new List<Direction>();
		private INpcMoveHandler _moveHandler { get; set; }
		private Vector2 _origin { get; set; }
		public NpcEntity(ISceneManager sceneManager, INpcMoveHandler moveHandler) 
		{
			_sceneManager = sceneManager;
			_moveHandler = moveHandler;
		}
		public void SetMoveMap(TileMap tileMap, TileSet tileSet)
		{
			_moveHandler.SetMapInfo(tileMap, tileSet);
		}
		public void AddAction(INPCActions action)
		{
			Actions.Add(action);
		}
		public void SetSpritesheet(Texture2D texture, int Width, int Height)
		{
			_spriteSheet = texture.GetSpriteSheet(Width, Height).AddWalking();
			_sprite = new AnimatedSprite(_spriteSheet, "face-down");
			Bounds = new RectangleF(X+2, Y, 21, 32);
		}
		public void SetLocation(int x, int y, Vector2 origin)
		{
			_origin = origin;
			X = (x * _sceneManager.Game.Currents.TileSize) + origin.X;
			Y = (y * _sceneManager.Game.Currents.TileSize) + origin.Y;
		}
		public void PlayerActionTrigger(IPlayerEntity player)
		{
			if (Actions.Count > 0)
			{
				for (int i = 0; i < Actions.Count; i++)
				{
					Actions[i].Action = ()=>{
						RunningActionIndex += 1;
					};
					Actions[i].SetInput(player.Input);
				}
				_sceneManager.Game.Currents.Player.ForEach(p=>p.LockMovement = true);
				FreePlayer = ()=>{
					_sceneManager.Game.Currents.Player.ForEach(p=>p.LockMovement = false);
				};
				RunningActionIndex = 0;
			}
		}
		public void OnCollision(CollisionEventArgs collisionInfo)
		{
			IEntity otherEntity = (IEntity)collisionInfo.Other;
			if (!otherEntity.IsTile)
			{
				if (otherEntity._IsPlayer)
				{
					PlayerEntity player = (PlayerEntity)otherEntity;
					if (TriggerType == NpcTriggerTypes.Touch)
					{
						for (int i = 0; i < Actions.Count; i++)
						{
							Actions[i].SetInput(player.Input);
						}
						PlayerActionTrigger(player);
					}
				}
			} 
			if (_IsPlayer || Pushable)
			{
				X -= collisionInfo.PenetrationVector.X;
				Y -= collisionInfo.PenetrationVector.Y;
				if (collisionInfo.PenetrationVector.X > 0)
				{
					CanMove[Direction.RIGHT] = false;
				} else 
				{
					CanMove[Direction.RIGHT] = true;
				}

				if (collisionInfo.PenetrationVector.X < 0)
				{
					CanMove[Direction.LEFT] = false;
				} else
				{
					CanMove[Direction.LEFT] = true;
				}

				if (collisionInfo.PenetrationVector.Y > 0)
				{
					CanMove[Direction.DOWN] = false;
				} else
				{
					CanMove[Direction.DOWN] = true;
				} 
				
				if (collisionInfo.PenetrationVector.Y < 0)
				{
					CanMove[Direction.UP] = false;
				} else
				{
					CanMove[Direction.UP] = true;
				}
			}
		}

		public void Update(GameTime gameTime)
		{
			var nextNode = _moveHandler.GetNextNode();
			_isMoving = nextNode != null && (nextNode.realX != X || nextNode.realY != Y);
			_sprite.Update(gameTime);
			if (RunningActionIndex == Actions.Count)
			{
				RunningActionIndex = -1;
				FreePlayer();
			}

			if (Actions.Count > 0 && RunningActionIndex < Actions.Count && RunningActionIndex >= 0)
			{
				var preIndex = RunningActionIndex;
				Actions[RunningActionIndex].Update(gameTime);
				if (preIndex != RunningActionIndex)
				{
					if (Actions[RunningActionIndex - 1].Done != null)
					{
						Actions[RunningActionIndex - 1].Done = null;
					}
				} else
				{
					if (Actions[RunningActionIndex].Done != null)
					{
						Actions[RunningActionIndex].Done = null;
						RunningActionIndex += 1;
					}
					
				}
			}
			
			var reducedX = (int)(X - _origin.X) / _sceneManager.Game.Currents.TileSize;
			var reducedY = (int)(Y - _origin.Y) / _sceneManager.Game.Currents.TileSize;

			_moveHandler.SetPosition(reducedX, reducedY);
			_moveHandler.Update(gameTime);
			SetMoveType(gameTime);
			
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: matrix);
			var toAnimateName = GetAnimationName();
			if (_lastAnimation != toAnimateName)
			{
				_sprite.SetAnimation(toAnimateName);
				_lastAnimation = toAnimateName;
			}
			spriteBatch.Draw(_sprite, new Vector2(_position.X, _position.Y));
			spriteBatch.End();
			if (RunningActionIndex == Actions.Count)
			{
				RunningActionIndex = -1;
				FreePlayer();
			}
			_moveHandler.Draw(spriteBatch);
		}

		public void DrawActions(SpriteBatch spriteBatch)
		{
			if (Actions.Count > 0 && RunningActionIndex < Actions.Count && RunningActionIndex >= 0)
			{
				Actions[RunningActionIndex].Draw(spriteBatch);
			}
		}

		public void Move(GameTime gameTime, Controls dir)
		{
			var speedF = (float speed) => (int)(speed * (float)gameTime.ElapsedGameTime.TotalSeconds);
			var speed = speedF(_speed);
			switch (dir)
			{
				case Controls.UP:
					Y -= speed;
					_facing = Direction.UP;
					break;
				case Controls.DOWN:
					Y += speed;
					_facing = Direction.DOWN;
					break;
				case Controls.LEFT:
					X -= speed;
					_facing = Direction.LEFT;
					break;
				case Controls.RIGHT:
					X += speed;
					_facing = Direction.RIGHT;
					break;
			}
			_moveHandler.SetPosition((int)X, (int)Y);
		}
		private string GetAnimationName()
		{
			string AnimeName = "";
			var ActionNotRunning = !(Actions.Count > 0 && RunningActionIndex < Actions.Count && RunningActionIndex >= 0);
			if (_isMoving && ActionNotRunning)
			{
				AnimeName = "walk-";
			} else {
				AnimeName = "face-";
			}

			if (_facing == Direction.DOWN)
			{
				return AnimeName + "down";
			} else if (_facing == Direction.UP)
			{
				return AnimeName + "up";
			} else if (_facing == Direction.LEFT)
			{
				return AnimeName + "left";
			} else if (_facing == Direction.RIGHT)
			{
				return AnimeName + "right";
			}

			return "face-down";
		}
		private void SetMoveType(GameTime gameTime)
		{
			if (!(Actions.Count > 0 && RunningActionIndex < Actions.Count && RunningActionIndex >= 0))
			{
				switch (MovementType.MovementType)
				{
					case MovementTypes.None:
						_moveHandler.ClearPath();
						break;
					case MovementTypes.Random:
						break;
					case MovementTypes.FollowPath:
						SetMoveFollowPath(gameTime);
						break;
					case MovementTypes.Follow:
						break;
					default:
					break;
				}
			}
		}

		private void SetMoveFollowPath(GameTime gameTime)
		{
			if (X == 0 && Y == 0) return;
			var reducedX = (X - _origin.X) / _sceneManager.Game.Currents.TileSize;
			var reducedY = (Y - _origin.Y) / _sceneManager.Game.Currents.TileSize;
			var nextNode = _moveHandler.GetNextNode();

			if (nextNode == null)
			{
				if (MovementType.Start == null)
				{
					if (!reducedX.InRange(MovementType.Target.X - 1, MovementType.Target.X + 1) || !reducedY.InRange(MovementType.Target.Y - 1, MovementType.Target.Y + 1))
					{
						_moveHandler.ClearPath();
						_moveHandler.SetPath(MovementType.Target);
					}
				} 
				else
				{
					int rangeVariance = 1;
					bool xCompare = reducedX.InRange(MovementType.Target.X - rangeVariance, MovementType.Target.X + rangeVariance);
					bool yCompare = reducedY.InRange(MovementType.Target.Y - rangeVariance, MovementType.Target.Y + rangeVariance);
					if (xCompare && yCompare)
					{
						_moveHandler.ClearPath();
						_moveHandler.SetPath(MovementType.Start.Value);
					} else
					{
						_moveHandler.ClearPath();
						_moveHandler.SetPath(MovementType.Target);
					}
				}
			} 
			else
			{
				switch (true)
				{
					case var _ when X < nextNode.realX:
						Move(gameTime, Controls.RIGHT);
						break;
					case var _ when X > nextNode.realX:
						Move(gameTime, Controls.LEFT);
						break;
					case var _ when Y < nextNode.realY:
						Move(gameTime, Controls.DOWN);
						break;
					case var _ when Y > nextNode.realY:
						Move(gameTime, Controls.UP);
						break;
				}
			}
		}
	}

}