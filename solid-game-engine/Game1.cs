using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using solid_game_engine.Shared.Entities;
using solid_game_engine.Shared.helpers;

namespace solid_game_engine;

public class Game1 : Game
{
	// --- Private Variables ---
	private GraphicsDeviceManager _graphics;
	private SpriteBatch _spriteBatch;
	// --- Public Variables ---
	public Level CurrentLevel { get; }
	public Currents Currents { get; set; } = new Currents();
	public Matrix transformMatrix { get; set;}
	public OrthographicCamera _camera;
	public Game1()
	{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			// --- Load Level ---
			TileMap tileMap = MapHelpers.LoadTileMap(1);
			Map map1 = new Map(this, Vector2.Zero, tileMap);
			Map map2 = new Map(this, new Vector2(map1.width, 0), tileMap);
			CurrentLevel = new Level(this, new List<Map>()
			{
				map1,
				map2
			});
	}

	protected override void Initialize()
	{
		var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
		_camera = new OrthographicCamera(viewportAdapter);

		// --- Initialize Level ---
		CurrentLevel.Initialize(_graphics);
		base.Initialize();
	}

	protected override void LoadContent()
	{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			// ----- Set Currents -----------
			Currents.CurrentPlayerskin = Content.Load<Texture2D>("graphics\\player");
			Currents.CurrentWindowSkin = Content.Load<Texture2D>("graphics\\blackWindowSkin");
			Currents.CurrentFont = Content.Load<SpriteFont>("fonts\\GameFont");
			// -------------
			Vector2 playerOrigin = new Vector2(12 * 32, 8 * 32);
			var player = new GameEntity(Currents.CurrentPlayerskin, (int)playerOrigin.X, (int)playerOrigin.Y, 32, 48, true, true);
			player._speed = 100f;
			Currents.Player = player;

			// --- Load Level Content ---
			CurrentLevel.LoadContent(Content);
	}

	protected override void Update(GameTime gameTime)
	{
		var kState = Keyboard.GetState();
		var gamepad1State = GamePad.GetState(PlayerIndex.One);
		if (gamepad1State.Buttons.Back == ButtonState.Pressed || kState.IsKeyDown(Keys.Escape)) Exit();
		// --- Update Player ---
		Currents.Player.Update(gameTime);
		Currents.Player.GetInput(gameTime, this, CurrentLevel.MapDirections);
		this.GetInput(gameTime, Currents.Player._speed, CurrentLevel.MapDirections);

		// --- Update Level ---
		CurrentLevel.Update(gameTime);

		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		GraphicsDevice.Clear(Color.Black);
		transformMatrix = _camera.GetViewMatrix();
		
		// --- Draw Level ---
		CurrentLevel.Draw(_spriteBatch);
		// - Draw Player --
		Currents.Player.matrix = transformMatrix;
		Currents.Player.Draw(_spriteBatch);

		base.Draw(gameTime);
	}
}
