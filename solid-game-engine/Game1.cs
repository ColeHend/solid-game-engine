using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using Newtonsoft.Json;
using solid_game_engine.Shared;
using solid_game_engine.Shared.Entities;
using solid_game_engine.Shared.entity;
using solid_game_engine.Shared.helpers;

namespace solid_game_engine;

public class Game1 : Game
{
	// --- Private Variables ---
	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;
	// --- Public Variables ---
	public Currents Currents { get; set; } = new Currents();
	public SceneManager sceneManager{ get; set; }
	public Game1()
	{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			
			sceneManager = new SceneManager(this);
			
	}

	protected override void Initialize()
	{
		sceneManager.Initialize(_graphics);

		// --- Initialize Level ---
		
		base.Initialize();
	}

	protected override void LoadContent()
	{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			// ----- Set Currents -----------
			Currents.CurrentWindowSkin = Content.Load<Texture2D>("graphics\\blackWindowSkin");
			Currents.CurrentFont = Content.Load<SpriteFont>("fonts\\GameFont");
			Currents.CurrentPlayerskin = Content.Load<Texture2D>("graphics\\player");
			// ----------
			sceneManager.LoadContent(Content);
	}

	protected override void Update(GameTime gameTime)
	{
		var kState = Keyboard.GetState();
		var gamepad1State = GamePad.GetState(PlayerIndex.One);
		if (gamepad1State.Buttons.Back == ButtonState.Pressed || kState.IsKeyDown(Keys.Escape)) Exit();
		sceneManager.Update(gameTime);
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		Color lightBlack = new Color(15, 15, 15, 255);
		GraphicsDevice.Clear(lightBlack);
		
		sceneManager.Draw(_spriteBatch);
		base.Draw(gameTime);
	}
}
