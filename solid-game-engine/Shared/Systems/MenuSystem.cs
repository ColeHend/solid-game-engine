using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using solid_game_engine.Shared.entity;
using solid_game_engine.Shared.helpers;
using solid_game_engine.Shared.Systems.interfaces;

namespace solid_game_engine.Shared.Systems
{
	public class MenuSystem : IMenuSystem
	{
		private PlayerIndex? _playerIndex { get; set; } = null;
		private ISceneManager _sceneManager { get; }
		private Currents Currents { get {
			return _sceneManager.Game.Currents;
		}}
		public MenuSystem(ISceneManager sceneManager)
		{
			_sceneManager = sceneManager;
			
		}
		public void Hide()
		{
			_playerIndex = null;
		}

		public void Show(PlayerIndex playerIndex)
		{
			_playerIndex = playerIndex;
			var optionList = new List<Option>
			{
				new Option("Title Screen", 1, () =>
				{
					_sceneManager.LoadScene("Scene_Title");
				})
			};
			var options = new OptionsWindow(_sceneManager, 0,0,4,8, optionList, _playerIndex);
		}
		public void Draw(SpriteBatch spriteBatch)
		{
			// Windows

			// spriteBatch.DrawWindow(Currents, 0, 0, 4, "Text");
		}
		public void Update(GameTime gameTime)
		{
			
		}
	}
}