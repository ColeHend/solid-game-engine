using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using solid_game_engine.Shared.Enums;
using solid_game_engine.Shared.helpers;
using solid_game_engine.Shared.scenes;

namespace solid_game_engine.Shared.entity
{
	public class Option
	{
		public Option(string name, int position, Action action)
		{
			Name = name;
			Position = position;
			Action = action;
		}
		public Option(string name, int position)
		{
			Name = name;
			Position = position;
		}
		public string Name { get; set; }
		public int Position { get; set; }
		public Action Action{ get; set; }
	}
	public class OptionsWindow
	{
		private ISceneManager _sceneManager { get; }
		private Currents Currents { get {
			return _sceneManager.Game.Currents;
		}}
		private GameWindow Window { get {
			return _sceneManager.Game.Window;
		}}
		private int SelectedIndex { get; set; }
		public List<Option> Options { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		private InputWrap Input { get;}
		public OptionsWindow(ISceneManager sceneManager, int x, int y, int width, int height, List<Option> options, PlayerIndex? playerIndex)
		{
			_sceneManager = sceneManager;
			Options = options;
			X = x;
			Y = y;
			Width = width;
			Height = height;
			SelectedIndex = 0;
			var maxPlayers = _sceneManager.Game.Currents.Player != null ? _sceneManager.Game.Currents.Player.GetMaxPlayerIndex() : PlayerIndex.One;
			Input = new InputWrap(playerIndex ?? PlayerIndex.One, maxPlayers);
		}
		
		private Color optionColor(bool isSelected) 
		{
			if (isSelected)
			{
				return Color.White;
			}
			return Color.Silver;
		}
		private bool isSelected(Option option)
		{
			return SelectedIndex == Options.FindIndex(o => o.Name == option.Name); 
		}
		private Vector2 getOptionPosition(int optionNum)
		{
			var windowPos = new Vector2(X, (Y - 1) * 0.7f);
			var theX = (windowPos.X * 32) + 32;
			var theY = ((windowPos.Y - 1) * 32) + (32 * optionNum);
			return new Vector2(theX, theY);
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			var windowHeight = Window.ClientBounds.Height;
			var windowPos = new Vector2(X, (Y - 1) * 0.7f);
			
			spriteBatch.DrawWindow(Currents, (int)windowPos.X, (int)windowPos.Y, Width, Height);
			Options.Sort((a,b)=>a.Position.CompareTo(b.Position));
			int index = 0;
			spriteBatch.Begin();
			foreach (var opt in Options)
			{
				var pos = getOptionPosition(opt.Position);
				var col = optionColor(isSelected(opt));
				Currents.DrawScaledText(spriteBatch, opt.Name, pos.X, pos.Y, 1, col);
				index++;
			}
			spriteBatch.End();
		}

		public void Update(GameTime gameTime)
		{
			Input.Update(gameTime);
			if (Input.IsSinglePressed(Controls.UP))
			{
				if (SelectedIndex == 0)
				{
					SelectedIndex = Options.Count - 1;
				} else
				{
					SelectedIndex -= 1;
				}
			} else if (Input.IsSinglePressed(Controls.DOWN))
			{
				if (SelectedIndex == Options.Count - 1)
				{
					SelectedIndex = 0;
				} else
				{
					SelectedIndex += 1;
				}
			}

			if (Input.IsSinglePressed(Controls.A) || Input.IsSinglePressed(Controls.START))
			{
				if (Options[SelectedIndex].Action != null)
				{
					Options[SelectedIndex].Action();
				}
			}
		}
	}
}