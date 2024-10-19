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
using solid_game_engine.Shared.Entities;
using solid_game_engine.Shared.entity.NPCActions;
using solid_game_engine.Shared.Enums;
using solid_game_engine.Shared.helpers;

namespace solid_game_engine.Shared.entity
{
	public interface INpcEntity : IEntity
	{
		int ActionIndex { get; }
		NpcTriggerTypes TriggerType { get; set; }
		void PlayerActionTrigger(IPlayerEntity player);
		void DrawActions(SpriteBatch spriteBatch);
		void SetLocation(int x, int y, Vector2 origin);
		void AddAction(INPCActions action);
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
		public Matrix matrix { get; set; }
		public SpriteSheet _spriteSheet { get; set; }
		public Dictionary<Direction, bool> CanMove { get; set; }
		private string _lastAnimation { get; set; } = "face-down";
		private RectangleF _position { get; set; }
		private List<Direction> MovementQue { get; set; } = new List<Direction>();
		public NpcEntity(ISceneManager sceneManager) 
		{
			_sceneManager = sceneManager;
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
				} else
				{
					NpcEntity npc = (NpcEntity)otherEntity;
					Debug.WriteLine($"NPC) X: {npc.X}, Y: {npc.Y}");
				}
			} else
			{
				TileCollide tile = (TileCollide)otherEntity;
				Debug.WriteLine($"Tile) X: {tile.Bounds.Position.X}, Y: {tile.Bounds.Position.Y}");
			}
			if (_IsPlayer)
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
			_isMoving = MovementQue.Count > 0;
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
			throw new NotImplementedException();
		}
		private string GetAnimationName()
		{
			string AnimeName = "";
			if (_isMoving)
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
	}

}