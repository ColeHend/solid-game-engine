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

		public List<PlayerEntity> player { get {
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
			var player1 = new PlayerEntity(_sceneManager.Game.Currents, (int)playerOrigin.X, (int)playerOrigin.Y, PlayerIndex.One);
			player1._speed = 100f;
			var player2 = new PlayerEntity(_sceneManager.Game.Currents, (int)playerOrigin.X + 2, (int)playerOrigin.Y + 2, PlayerIndex.Two);
			player2._speed = 100f;
			_sceneManager.Game.Currents.Player = new List<PlayerEntity>(){
				player1,
				player2
			};
			CurrentLevel.LoadContent(contentManager);
			CurrentLevel.MapChangeAction = (PlayerEntity player)=>{
				player.MapDirections = CurrentLevel.currentMaps.GetMapDirections(_sceneManager.Game, player.Input.PlayerIndex);
				playerActionSystem.SetCurrentPlayerMap(player, CurrentLevel.currentMaps.FindPlayersMap(player));
				foreach (var map in CurrentLevel.currentMaps)
				{
					map.SetCollisionComponent();
					map._collisionComponent.Remove(player);
					map._collisionComponent.Insert(player); 
				}
				
				return "Map Changed";
			};
			player.ForEach(playa =>{
				playa.MapDirections = CurrentLevel.currentMaps.GetMapDirections(_sceneManager.Game, playa.Input.PlayerIndex);
				playerActionSystem.SetCurrentPlayerMap(playa, CurrentLevel.currentMaps.FindPlayersMap(playa));
				playa.LoadContent(contentManager);
			});
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			transformMatrix = camera.GetViewMatrix();
			// --- Draw Level ---
			CurrentLevel.Draw(spriteBatch);
			playerActionSystem.Draw(spriteBatch);
		}

		public void Update(GameTime gameTime)
		{
			// --- Update Level ---
			CurrentLevel.Update(gameTime);
			playerActionSystem.Update(gameTime);
		}
	}
}