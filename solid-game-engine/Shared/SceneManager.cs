using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using solid_game_engine.Shared.scenes;

namespace solid_game_engine.Shared
{ 
    public class SceneManager
    {
			private List<IScene> AllScenes { get; set; }
			private int CurrentSceneIndex { get; set; }
			public IScene CurrentScene { get {
				return AllScenes[CurrentSceneIndex];
			} }
			public Game1 Game { get; }
			public SceneManager(Game1 game)
			{
				Game = game;
				AllScenes = new List<IScene>(){
					new Scene_Title(this),
					new Scene_Game(this)
				};
				CurrentSceneIndex = 0;
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