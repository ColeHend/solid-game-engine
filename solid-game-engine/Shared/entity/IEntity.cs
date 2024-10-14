using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Graphics;
using solid_game_engine.Shared.entity;
using solid_game_engine.Shared.Enums;
using solid_game_engine.Shared.helpers;

namespace solid_game_engine.Shared.Entities;

public enum Direction
{
	NONE,
	UP,
	DOWN,
	LEFT,
	RIGHT
}
	public interface IEntity : ICollisionActor
	{
		bool _IsPlayer { get; }
		bool IsTile { get;}
		public void Update(GameTime gameTime);
		public void Draw(SpriteBatch spriteBatch);
	}

public class GameEntity : IEntity
	{
		public AnimatedSprite _sprite { get; set; }
		public Direction _facing { get; set; }
		public float _speed { get; set; } = 100f;
		public IShapeF Bounds { get; }
		public bool IsTile { get; } = false;
		private SpriteSheet _spriteSheet { get; set; }

		private RectangleF _position { get; set; }
		private bool _isMoving { get; set; } = false;
		private string _lastAnimation { get; set; } = "face-down";
		
		private List<Direction> MovementQue { get; set; } = new List<Direction>();
		public bool _IsPlayer { get; set; }
		private bool _pushable { get; set; }
		public Dictionary<Direction, bool> CanMove { get; set; } = new Dictionary<Direction, bool>();

		public GameEntity(Texture2D texture, int X, int Y, int Width, int Height, bool isPlayer = false, bool pushable = false)
		{
			_spriteSheet = texture.GetSpriteSheet(Width, Height).AddWalking();
			_sprite = new AnimatedSprite(_spriteSheet, "face-down");
			_position = new RectangleF(X, Y, Width, Height);
			_facing = Direction.DOWN;
			_IsPlayer = isPlayer;
			Bounds = new RectangleF(X, Y, Width, Height);
			_pushable = pushable;
			CanMove.Add(Direction.DOWN, true);
			CanMove.Add(Direction.UP, true);
			CanMove.Add(Direction.LEFT, true);
			CanMove.Add(Direction.RIGHT, true);
		}

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

		public virtual void OnCollision(CollisionEventArgs collisionInfo)
		{
			if (_pushable || _IsPlayer)
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
		
		public virtual void Draw(SpriteBatch spriteBatch)
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
		}

		public Matrix matrix { get; set;}

		public virtual void Update(GameTime gameTime)
		{
			if (_IsPlayer)
			{
				var kState = Keyboard.GetState();
				_isMoving = kState.IsKeyDown(Keys.Left) || kState.IsKeyDown(Keys.Right) || kState.IsKeyDown(Keys.Up) || kState.IsKeyDown(Keys.Down);
			} else 
			{
				_isMoving = MovementQue.Count > 0;
			}
			_sprite.Update(gameTime);
		}
	}
