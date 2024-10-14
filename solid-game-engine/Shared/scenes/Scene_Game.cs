using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using solid_game_engine.Shared.Entities;
using solid_game_engine.Shared.entity;
using solid_game_engine.Shared.helpers;
using solid_game_engine.Shared.Systems;

namespace solid_game_engine.Shared.scenes
{
	public class Scene_Game : IScene
	{
		public string Name { get; } = "Scene_Game";
		public Level CurrentLevel { get; set;}
		public OrthographicCamera camera { get; set; }
		public Matrix transformMatrix { get; set;}
		private SceneManager _sceneManager { get; }
		private PlayerActionSystem playerActionSystem{ get; }

		public PlayerEntity player { get {
			return _sceneManager.Game.Currents.Player;
		} }
		public Scene_Game(SceneManager sceneManager)
		{
			_sceneManager = sceneManager;
			playerActionSystem = new PlayerActionSystem(_sceneManager);
		}
		
		public void Initialize(GraphicsDeviceManager graphics)
		{
			var viewportAdapter = new BoxingViewportAdapter(_sceneManager.Game.Window, graphics.GraphicsDevice, 800, 480);
			camera = new OrthographicCamera(viewportAdapter);
			TileMap tileMap = MapHelpers.LoadTileMap(1);
			TileSetEntity tileSet = tileMap.LoadTileSet();

			Map map1 = new Map(_sceneManager, Vector2.Zero, tileMap, tileSet, camera);
			Map map2 = new Map(_sceneManager, new Vector2(map1.width, 0), tileMap, tileSet, camera);
			CurrentLevel = new Level(this, _sceneManager, new List<Map>()
			{
				map1,
				map2
			});

			CurrentLevel.Initialize(graphics);
		}

		public void LoadContent(ContentManager contentManager)
		{
			Vector2 playerOrigin = new Vector2(12 * 32, 8 * 32);
			var player = new PlayerEntity(_sceneManager.Game.Currents, (int)playerOrigin.X, (int)playerOrigin.Y);
			player._speed = 100f;
			_sceneManager.Game.Currents.Player = player;
			CurrentLevel.LoadContent(contentManager);
			CurrentLevel.MapChangeAction = ()=>{
				playerActionSystem.SetCurrentMap(CurrentLevel.currentMaps.FindPlayersMap(player));
			};
			playerActionSystem.SetCurrentMap(CurrentLevel.currentMaps.FindPlayersMap(player));
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			transformMatrix = camera.GetViewMatrix();
		
			// --- Draw Level ---
			CurrentLevel.Draw(spriteBatch);
			// - Draw Player --
			player.matrix = transformMatrix;
			player.Draw(spriteBatch);
			playerActionSystem.Draw(spriteBatch);
		}

		public void Update(GameTime gameTime)
		{
			player.Update(gameTime);
			player.GetInput(gameTime, CurrentLevel, CurrentLevel.MapDirections, player.LockMovement);
			player.GetFollowCamera(camera);

			// --- Update Level ---
			CurrentLevel.Update(gameTime);
			playerActionSystem.Update(gameTime);
		}
	}
}