using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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
		private ISceneManager _sceneManager { get; }
		private PlayerActionSystem playerActionSystem{ get; }
		public List<IPlayerEntity> player { get {
			return _sceneManager.Game.Currents.Player;
		} }
		public Scene_Game(ISceneManager sceneManager, List<IMap> maps)
		{
			_sceneManager = sceneManager;
			playerActionSystem = new PlayerActionSystem(_sceneManager);
			CurrentLevel = new Level(this, _sceneManager, maps);
			
		}
		
		public void Initialize(GraphicsDeviceManager graphics)
		{
			CurrentLevel.Initialize(graphics);
		}

		public void LoadContent(ContentManager contentManager)
		{
			player.ForEach(playa =>{
				playa.MapDirections = CurrentLevel.currentMaps.GetMapDirections(_sceneManager.Game, playa.Input.PlayerIndex);
				var theMap = CurrentLevel.currentMaps.FindPlayersMap(playa);
				playerActionSystem.SetCurrentPlayerMap(playa, theMap);
			});
			LoadMaps(contentManager);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			transformMatrix = _sceneManager.camera.GetViewMatrix();
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

		private void LoadMaps(ContentManager contentManager)
		{
			CurrentLevel.LoadContent(contentManager);
			player.ForEach(playa =>{
				playa.LoadContent(contentManager);
			});
			CurrentLevel.MapChangeAction = (IPlayerEntity player) => {
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
			
		}
	}
}