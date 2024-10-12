using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace solid_game_engine.Shared.helpers
{
    public static class DrawHelpers
	{
		public static void DrawWindow(this SpriteBatch _spriteBatch, Currents _currents, int x, int y, int width, int height)
		{
			var windowSkin = _currents.CurrentWindowSkin.GetSpriteSheet(32, 32);
			if (width < 4) width = 4;
			if (height < 4) height = 4;
			var actualX = x * _currents.TileSize;
			var actualY = y * _currents.TileSize;
			var windowMap = new List<List<int>>();
			for (int h = 0; h < height; h++)
			{
				windowMap.Add(new List<int>()); 
				for (int w = 0; w < width; w++)
				{
					windowMap[h].Add(7);
					if (w == width - 1) // ---- Right Column
					{
						if (h == 0)
						{
							windowMap[h][w] = 3;
						}
						if (h == 1)
						{
							windowMap[h][w] = 9;
						}
						else if (h == height - 1)
						{
							windowMap[h][w] = 21;
						}
						else 
						{
							windowMap[h][w] = 15;
						}
					}
					else
					{
						if (h == 0) // ---- Top Row
						{
							if (w == 0)
							{
								windowMap[h][w] = 0;
							}
							else if (w == 1)
							{
								windowMap[h][w] = 1;
							}
							else if (w == width - 2)
							{
								windowMap[h][w] = 2;
							}
							else 
							{
								windowMap[h][w] = 2;
							}
						} 
						else // ---- Bottom Row
						{
							if (w == width - 1)
							{
								windowMap[h][w] = 21;
							}
							else 
							{
								windowMap[h][w] = 20;
							}
						}
					}
				}
			}

			// ---- Draw Window
			_spriteBatch.Begin();
			for (int h = 0; h < windowMap.Count; h++)
			{
				for (int w = 0; w < windowMap[h].Count; w++)
				{
					var tileNumber = windowMap[h][w];
					var tileAtlas = windowSkin;
					var singleTile = tileAtlas.TextureAtlas[(int)tileNumber];
					var tilePos = new Vector2(actualX + (w * _currents.TileSize), actualY + (h * _currents.TileSize));
					_spriteBatch.Draw(singleTile, tilePos, Color.White);

				}
			}
			_spriteBatch.End();
		}

		public static void DrawWindow(this SpriteBatch _spriteBatch, Currents _currents, int x, int y, int width, string text = "")
		{
			var textX = (x * _currents.TileSize) + 32;
			var textY = (y * _currents.TileSize) + 32;
			var maxWidth = width * _currents.TileSize;

			var height = (int)_spriteBatch.GetStringWrappedSize(_currents, text, new Vector2(0, 0), Color.White, maxWidth) / _currents.TileSize;
			_spriteBatch.DrawWindow(_currents, x, y, width + 2, height + 2);
			_spriteBatch.DrawStringWrapped(_currents, text, new Vector2(textX, textY), Color.White, maxWidth);
		}

		public static void DrawStringWrapped(this SpriteBatch spriteBatch, Currents _current, string text, Vector2 position, Color color, float maxLineWidth)
		{
			string[] words = text.Split(' ');

			string currentLine = string.Empty;
			var font = _current.CurrentFont;
			float lineHeight = font.LineSpacing;
			float currentY = position.Y;
			spriteBatch.Begin();
			foreach (string word in words)
			{
				string testLine = currentLine.Length == 0 ? word : currentLine + " " + word;
				Vector2 size = font.MeasureString(testLine);

				if (size.X > maxLineWidth)
				{
						spriteBatch.DrawString(font, currentLine, new Vector2(position.X, currentY), color);

						currentLine = word;
						currentY += lineHeight;
				}
				else
				{
						currentLine = testLine;
				}
			}

			if (currentLine.Length > 0)
			{
					spriteBatch.DrawString(font, currentLine, new Vector2(position.X, currentY), color);
			}
			spriteBatch.End();
		}

		public static float GetStringWrappedSize(this SpriteBatch spriteBatch, Currents _current, string text, Vector2 position, Color color, float maxLineWidth)
		{
			string[] words = text.Split(' ');

			string currentLine = string.Empty;
			var font = _current.CurrentFont;
			float lineHeight = font.LineSpacing;
			float currentY = position.Y;  
			spriteBatch.Begin();
			foreach (string word in words)
			{
					string testLine = currentLine.Length == 0 ? word : currentLine + " " + word;
					Vector2 size = font.MeasureString(testLine);

					if (size.X > maxLineWidth)
					{
							spriteBatch.DrawString(font, currentLine, new Vector2(position.X, currentY), color);

							currentLine = word;  
							currentY += lineHeight;  
					}
					else
					{
							currentLine = testLine;  
					}
			}
			if (currentLine.Length > 0)
			{
					spriteBatch.DrawString(font, currentLine, new Vector2(position.X, currentY), color);
					currentY += lineHeight;  
			}
			spriteBatch.End();

			return currentY;
		}
	}
}