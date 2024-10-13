using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using solid_game_engine.Shared.entity;
using solid_game_engine.Shared.Enums;
using solid_game_engine.Shared.helpers;

namespace solid_game_engine.Shared.scenes
{
	public enum StartOptions
	{
		NewGame,
		LoadGame,
		Settings,
		Exit
	}
	public class Scene_Title : IScene
	{
		public string Name { get; } = "Scene_Title";
		private SceneManager _sceneManager { get; }
		private StartOptions SelectedOption { get; set; }
		private InputWrap inputState { get; set; } = new InputWrap();
		private Currents Currents { get {
			return _sceneManager.Game.Currents;
		}}
		private GameWindow Window { get {
			return _sceneManager.Game.Window;
		}}
		private OptionsWindow InitialOptions { get; set; }
		public Scene_Title(SceneManager sceneManager)
		{
			_sceneManager = sceneManager;
			var windowHeight = Window.ClientBounds.Height;
			// --- Text Positions
			
			var windowPos = new Vector2(4, windowHeight / 32 * 0.9f);
			var options = new List<Option>()
			{
				new Option("New Game", 1, ()=>{
					NewGame();
				}),
				new Option("Load Game", 2, ()=>{
					LoadGame();
				}),
				new Option("Settings", 3, ()=>{
					Settings();
				}),
				new Option("Exit", 4, ()=>{
					_sceneManager.Game.Exit();
				}),
			};
			InitialOptions = new OptionsWindow(_sceneManager, (int)windowPos.X, (int)windowPos.Y, 12, 5, options, null);
		}
		// -------- Menu Option Functions ----
		public void NewGame()
		{
			_sceneManager.LoadScene("Scene_Game");
		}
		
		public void LoadGame()
		{

		}

		private void Settings()
		{

		}

		// -----------------------------------
		public void Initialize(GraphicsDeviceManager graphics)
		{
			
		}

		public void LoadContent(ContentManager contentManager)
		{
			
		}
		public void Draw(SpriteBatch spriteBatch)
		{
			var titleTextPos = new Vector2(128, Window.ClientBounds.Height / 4);
			spriteBatch.Begin();
			Currents.DrawScaledText(spriteBatch, "Game Title", titleTextPos.X, titleTextPos.Y, 3);
			spriteBatch.End();
			InitialOptions.Draw(spriteBatch);
		}

		public void Update(GameTime gameTime)
		{
			InitialOptions.Update(gameTime);

		}

	}
}