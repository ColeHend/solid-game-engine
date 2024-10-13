using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using solid_game_engine.Shared.Entities;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Graphics;
using solid_game_engine.Shared.Enums;
using solid_game_engine.Shared.helpers;

namespace solid_game_engine.Shared.entity.NPCActions
{
	public class MessageChoices : INPCActions
	{

		public Action Action { get; }
		private OptionsWindow? optionsWindow { get; set; } = null;
		private SceneManager _sceneManager { get; }
		private Vector2 Position { get; set; }
		private List<Option> Options { get; set; }
		public MessageChoices(SceneManager sceneManager, List<Option> options)
		{
			var position = new Vector2(4, sceneManager.Game.Window.ClientBounds.Height / 32 * 0.9f);
			Position = position;
			_sceneManager = sceneManager;
			Options = options;
			
		}
		public void Show(PlayerIndex playerIndex)
		{
			optionsWindow = new OptionsWindow(_sceneManager, (int)Position.X, (int)Position.Y, 12, 4, Options, playerIndex);
		}

		public void Hide()
		{
			optionsWindow = null;
		}
		public void ChangeOptions(List<Option> options)
		{
			Options = options;
		}
		public void Draw(SpriteBatch spriteBatch)
		{
			optionsWindow?.Draw(spriteBatch);
		}

		public void Update(GameTime gameTime)
		{
			optionsWindow?.Update(gameTime);
		}
	}
}