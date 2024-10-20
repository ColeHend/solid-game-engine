using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
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
	public ISceneManager sceneManager { get; set; }
	private IServiceProvider _serviceProvider;
	public Game1() : base()
	{
		var services = ConfigureServices();
		_serviceProvider = services.BuildServiceProvider();
		_graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
		sceneManager = _serviceProvider.GetRequiredService<ISceneManager>();
			
	}

	private IServiceCollection ConfigureServices()
	{
		var services = new ServiceCollection();

		services.AddSingleton<Game1>(this);
		services.AddSingleton<ISceneManager, SceneManager>();
		services.AddTransient<IMap, Map>();
		services.AddTransient<INpcEntity, NpcEntity>();
		services.AddTransient<INpcMoveHandler, NpcMoveHandler>();
		services.AddTransient<IPlayerEntity, PlayerEntity>();
		 

		return services;
	}

	protected override void Initialize()
	{
		sceneManager.Initialize(_graphics);
		base.Initialize();
	}

	protected override void LoadContent()
	{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			// ----- Set Currents -----------
			Currents.CurrentWindowSkin = Content.Load<Texture2D>("graphics\\blackWindowSkin");
			Currents.CurrentFont = Content.Load<SpriteFont>("fonts\\GameFont");
			Currents.CurrentPlayerskin = Content.Load<Texture2D>("graphics\\player");
			this.LoadPlayers();
			// ----------
			sceneManager.LoadContent(Content);
	}

	private void LoadPlayers()
			{
				Vector2 playerOrigin = new Vector2(12, 8);
				var player1 = _serviceProvider.GetRequiredService<IPlayerEntity>();
				player1.SetSpritesheet(Currents.CurrentPlayerskin, 32, 48);
				player1.SetLocation((int)playerOrigin.X, (int)playerOrigin.Y, Vector2.Zero);
				player1.SetPlayer(PlayerIndex.One);
				player1._speed = 100f;

				var player2 = _serviceProvider.GetRequiredService<IPlayerEntity>();
				player2.SetSpritesheet(Currents.CurrentPlayerskin, 32, 48);
				player2.SetLocation((int)playerOrigin.X + 2, (int)playerOrigin.Y, Vector2.Zero);
				player2.SetPlayer(PlayerIndex.Two);
				player2._speed = 100f;
				

				var players = new List<IPlayerEntity>(){
					player1,
					player2
				};

				Currents.Player = players;
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
