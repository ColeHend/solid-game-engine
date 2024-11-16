using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Graphics;
using solid_game_engine.Shared.Entities;
using solid_game_engine.Shared.Enums;
using solid_game_engine.Shared.helpers;

namespace solid_game_engine.Shared.entity;

public interface IPlayerEntity : IEntity
{
	InputWrap Input { get; set; }
	Dictionary<Direction, IMap> MapDirections { get; set; }
	bool LockMovement { get; set; }
	Currents _currents { get; set; }
	void SetLocation(int x, int y, Vector2 origin);
	void SetPlayer(PlayerIndex playerIndex);
	void LoadContent(ContentManager content);
}

public class PlayerEntity : IPlayerEntity
{
	public InputWrap Input { get; set; }
	public Dictionary<Direction, IMap> MapDirections { get; set; }
	public bool LockMovement { get; set; } = false;
	public Currents _currents { get; set; }
	public AnimatedSprite _sprite { get; set; }
	public Direction _facing { get; set; }
	public float _speed { get; set; }
	public IShapeF Bounds { get; set; } = new RectangleF(0, 0, 21, 32);
	public SpriteSheet _spriteSheet { get; set; }
	public bool _isMoving { get; set; }
	public bool _IsPlayer { get; set; } = true;
	public bool IsTile { get; set; } = false;
	public Matrix matrix { get; set; }
	public RectangleF _position { get; set; }
	private ISceneManager _sceneManager { get; set; }
	public Dictionary<Direction, bool> CanMove { get; set; } = new Dictionary<Direction, bool>();
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
	private string _lastAnimation { get; set; } = "face-down";

	public PlayerEntity(ISceneManager sceneManager)
	{
		_currents = sceneManager.Game.Currents;
		_sceneManager = sceneManager;
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

	public void SetPlayer(PlayerIndex playerIndex)
	{
		if (playerIndex == PlayerIndex.One)
		{
			Input = new InputWrap(playerIndex, _currents.Player.GetMaxPlayerIndex());
		} else
		{
			Input = new InputWrap(playerIndex, _currents.Player.GetMaxPlayerIndex());
		}
	}

	public void LoadContent(ContentManager content)
	{
		Input = new InputWrap(Input.PlayerIndex, _currents.Player.GetMaxPlayerIndex());
	}

	public void OnCollision(CollisionEventArgs collisionInfo)
	{
		if ( _IsPlayer)
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
		_isMoving = Input.IsPressed(Controls.UP) || Input.IsPressed(Controls.DOWN) || Input.IsPressed(Controls.LEFT) || Input.IsPressed(Controls.RIGHT);
		Input.Update(gameTime);
		_sprite.Update(gameTime);
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
}
