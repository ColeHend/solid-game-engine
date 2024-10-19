using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using solid_game_engine.Shared.Enums;
using solid_game_engine.Shared.helpers;

namespace solid_game_engine.Shared.entity.NPCActions
{
	public class MessageBox : INPCActions
	{
		public MessageBox(ISceneManager sceneManager)
		{
			SceneManager = sceneManager;
			currents = SceneManager.Game.Currents;
		}
		private ISceneManager SceneManager { get; set;}
		public string Done { get; set; } = null;
		private Currents currents { get; set;}
		public string Text { get; set; }
		public Action Action { get; set;}
		private InputWrap Input { get; set; }
		private double timer { get; set; }
		public void SetInput(InputWrap input)
		{
			Input = input;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			var theX = 1;
			var theY = (int)(SceneManager.Game.Window.ClientBounds.Height / 32 / 4 * 3);
			var theWidth = SceneManager.Game.Window.ClientBounds.Width / 32 - 4;
			spriteBatch.DrawWindow(currents, theX, theY, theWidth, Text);
		}

		public void Update(GameTime gameTime)
		{
			if (Input != null)
			{
				if (Input.IsSinglePressed(Controls.A))
				{
					if (timer > 0)
					{
						if ((gameTime.TotalGameTime.TotalMilliseconds - timer) > 500)
						{
							Done = "Done!";
							Action();
							timer = 0;
						}
					} else
					{
						timer = gameTime.TotalGameTime.TotalMilliseconds;
					}
				}
			}
		}
	}
}