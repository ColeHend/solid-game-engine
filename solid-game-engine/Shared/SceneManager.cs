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
using solid_game_engine.Shared.scenes;

namespace solid_game_engine.Shared
{ 
	public interface ISceneManager
	{
		public IScene CurrentScene { get; }
		public Game1 Game { get; }
		Level CurrentLevel { get; }
		OrthographicCamera camera { get; set; }
		void LoadScene(string SceneName);
		void Initialize(GraphicsDeviceManager graphics);
		void LoadContent(ContentManager contentManager);
		void Update(GameTime gameTime);
		void Draw(SpriteBatch spriteBatch);

	}
    public class SceneManager : ISceneManager
    {
			private List<IScene> AllScenes { get; set; }
			private int CurrentSceneIndex { get; set; }
			public IScene CurrentScene { get {
				return AllScenes[CurrentSceneIndex];
			} }
			public Game1 Game { get; }
			private IServiceProvider _serviceProvider { get; }
			public OrthographicCamera camera { get; set; }
			public Level CurrentLevel { get {
				return ((Scene_Game)AllScenes[1]).CurrentLevel;
			} }
			public SceneManager(IServiceProvider serviceProvider)
			{
				Game = serviceProvider.GetRequiredService<Game1>();
				CurrentSceneIndex = 0;
				_serviceProvider = serviceProvider;
			}

			public void LoadScene(string SceneName)
			{
				// Scene_Title, Scene_Game
				var newScene = AllScenes.FindIndex(x=>x.Name.ToLower() == SceneName.ToLower());
				if (newScene != -1)
				{
					CurrentSceneIndex = newScene;
				}
			}
			public void Initialize(GraphicsDeviceManager graphics)
			{
				var res = new Vector2(800, 480);
				var viewportAdapter = new BoxingViewportAdapter(Game.Window, graphics.GraphicsDevice, (int)res.X, (int)res.Y);
				camera = new OrthographicCamera(viewportAdapter);
				AllScenes = new List<IScene>(){
					new Scene_Title(this),
					new Scene_Game(this, LoadMaps(graphics))
				};
				for (int i = 0; i < AllScenes.Count; i++)
				{
					AllScenes[i].Initialize(graphics);
				}
			}

			public void LoadContent(ContentManager contentManager)
			{
				for (int i = 0; i < AllScenes.Count; i++)
				{
					AllScenes[i].LoadContent(contentManager);
				}
			}

			public void Update(GameTime gameTime)
			{
				CurrentScene.Update(gameTime);
			}

			public void Draw(SpriteBatch spriteBatch)
			{
				CurrentScene.Draw(spriteBatch);
			}

			private List<IMap> LoadMaps(GraphicsDeviceManager graphics)
			{
				
				// -------------------------------
				TileMap tileMap = MapHelpers.LoadTileMap(1);
				TileSetEntity tileSet = tileMap.LoadTileSet();
				
				var maps = new List<int>{1,2}.Select((x, i) => {
					var map = _serviceProvider.GetRequiredService<IMap>();
					return map;
				}).ToList();

				maps = maps.Select((x, i)=>{
					if (i == 0)
					{
						x.SetMap(Vector2.Zero, tileMap, tileSet, camera);
					}
					if (i == 1)
					{
						x.SetMap(new Vector2(maps[0].width, 0), tileMap, tileSet, camera);
					}
					return x;
				}).ToList();
				return maps;
			}

			
    }

		public interface IScene
		{
			string Name { get; }
			void Initialize(GraphicsDeviceManager graphics);
			void LoadContent(ContentManager contentManager);
			void Update(GameTime gameTime);
			void Draw(SpriteBatch spriteBatch);
		}
}