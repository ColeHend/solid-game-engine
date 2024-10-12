using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Graphics;

namespace solid_game_engine.Shared.helpers
{
    public static class SpriteHelpers
	{
		public static SpriteSheet GetSpriteSheet(this Texture2D texture, int Width, int Height)
		{
			Texture2DAtlas atlas = Texture2DAtlas.Create(texture.Name, texture, Width, Height);
			return new SpriteSheet(texture.Name, atlas);
		}

		public static SpriteSheet AddWalking(this SpriteSheet spriteSheet, bool looping = true, double timing = 0.1)
		{
			spriteSheet.DefineAnimation("face-down", builder => {
				builder.IsLooping(false)
					.AddFrame(0, TimeSpan.MaxValue);
			});

			spriteSheet.DefineAnimation("walk-down", builder => {
				builder.IsLooping(looping)
					.AddFrame(0, TimeSpan.FromSeconds(timing))
					.AddFrame(1, TimeSpan.FromSeconds(timing))
					.AddFrame(2, TimeSpan.FromSeconds(timing))
					.AddFrame(3, TimeSpan.FromSeconds(timing));
			});

			spriteSheet.DefineAnimation("face-left", builder => {
				builder.IsLooping(false)
					.AddFrame(4, TimeSpan.MaxValue);
			});
			spriteSheet.DefineAnimation("walk-left", builder => {
				builder.IsLooping(looping)
					.AddFrame(4, TimeSpan.FromSeconds(timing))
					.AddFrame(5, TimeSpan.FromSeconds(timing))
					.AddFrame(6, TimeSpan.FromSeconds(timing))
					.AddFrame(7, TimeSpan.FromSeconds(timing));
			});

			spriteSheet.DefineAnimation("face-right", builder => {
				builder.IsLooping(false)
					.AddFrame(8, TimeSpan.MaxValue);
			});
			spriteSheet.DefineAnimation("walk-right", builder => {
				builder.IsLooping(looping)
					.AddFrame(8, TimeSpan.FromSeconds(timing))
					.AddFrame(9, TimeSpan.FromSeconds(timing))
					.AddFrame(10, TimeSpan.FromSeconds(timing))
					.AddFrame(11, TimeSpan.FromSeconds(timing));
			});

			spriteSheet.DefineAnimation("face-up", builder => {
				builder.IsLooping(false)
					.AddFrame(12, TimeSpan.MaxValue);
			});
			spriteSheet.DefineAnimation("walk-up", builder => {
				builder.IsLooping(looping)
					.AddFrame(12, TimeSpan.FromSeconds(timing))
					.AddFrame(13, TimeSpan.FromSeconds(timing))
					.AddFrame(14, TimeSpan.FromSeconds(timing))
					.AddFrame(15, TimeSpan.FromSeconds(timing));
			});
			return spriteSheet;
		}

		public static Dictionary<string, AnimationController> addWalkingAnimations(this Dictionary<string, AnimationController> dictionary, SpriteSheet _spriteSheet)
			{
				List<string> directions = new List<string>(){"up", "down", "left", "right" };
				List<string> aniTypes = new List<string>() { "walk-", "face-" };
				foreach (var faceType in aniTypes)
				{
					foreach (var dirs in directions)
					{
						string controllerName = faceType + dirs;
						dictionary.Add(controllerName, new AnimationController(_spriteSheet.GetAnimation(controllerName)));
					}				
				}
				return dictionary;
			}
	}
}