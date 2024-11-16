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
using solid_game_engine.Shared.Systems;
using solid_game_engine.Shared.Systems.interfaces;
using Steamworks;

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
	public NetworkManager NetworkManager { get; set; }
	private ScriptSystem _scriptSystem;
	public Game1() : base()
	{
		var services = ConfigureServices();
		_serviceProvider = services.BuildServiceProvider();
		_graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
		sceneManager = _serviceProvider.GetRequiredService<ISceneManager>();
		_scriptSystem = new ScriptSystem();
		_scriptSystem.AddHostObject("Game", this);
		_scriptSystem.AddHostObject("SceneManager", sceneManager);
		_scriptSystem.AddHostObject("Currents", Currents);
		_scriptSystem.AddHostObject("GraphicsDevice", _graphics);
		
		try
		{
			NetworkManager = new NetworkManager(480);
		}
		catch (Exception e)
		{
			Console.WriteLine("Steam Client failed to initialize", e);
		}
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
		services.AddTransient<IMenuSystem, MenuSystem>();
		 

		return services;
	}

	protected override void Initialize()
	{
		_scriptSystem.LoadAndExecute();
		sceneManager.Initialize(_graphics);
		base.Initialize();
		_scriptSystem.RunInit(CoreScripts.Game);
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
			_scriptSystem.RunLoadContent(CoreScripts.Game, _spriteBatch);
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
		// var kState = Keyboard.GetState();
		// var gamepad1State = GamePad.GetState(PlayerIndex.One);
		// if (gamepad1State.Buttons.Back == ButtonState.Pressed || kState.IsKeyDown(Keys.Escape)) Exit();
		SteamClient.RunCallbacks();
		sceneManager.Update(gameTime);
		base.Update(gameTime);
		var friends = SteamFriends.GetFriends();
		Console.WriteLine("Friends: ", JsonConvert.SerializeObject(friends));
		_scriptSystem.RunUpdate(CoreScripts.Game, gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		Color lightBlack = new Color(15, 15, 15, 255);
		GraphicsDevice.Clear(lightBlack);
		
		sceneManager.Draw(_spriteBatch);
		base.Draw(gameTime);
		_scriptSystem.RunDraw(CoreScripts.Game, _spriteBatch);
	}

	protected override void Dispose(bool disposing)
	{
		SteamClient.Shutdown();
		base.Dispose(disposing);
	}
}
