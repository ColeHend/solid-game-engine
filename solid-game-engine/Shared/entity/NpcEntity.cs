using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions;
using solid_game_engine.Shared.Entities;
using solid_game_engine.Shared.entity.NPCActions;
using solid_game_engine.Shared.Enums;

namespace solid_game_engine.Shared.entity
{
	public class NpcEntity : GameEntity
	{
		private List<Direction> MovementQue { get; set; } = new List<Direction>();
		private List<INPCActions> Actions { get; }
		private int RunningActionIndex { get; set; } = -1;
		public int ActionIndex { get { return RunningActionIndex; } }
		public NpcTriggerTypes TriggerType { get; set; }
		public NpcEntity(Texture2D texture, int X, int Y, int Width, int Height) : base(texture, X, Y, Width, Height, false, false)
		{
			Actions = new List<INPCActions>();
		}
		public NpcEntity(Texture2D texture, int X, int Y, int Width, int Height, List<INPCActions> actions) : base(texture, X, Y, Width, Height, false, false)
		{
			Actions = actions;
		}
		private Action FreePlayer { get; set; }

		public void PlayerActionTrigger(PlayerEntity player)
		{
			if (Actions.Count > 0)
			{
				for (int i = 0; i < Actions.Count; i++)
				{
					Actions[i].Action = ()=>{
						RunningActionIndex += 1;
					};
					Actions[i].SetInput(player.Input);
				}
				player.LockMovement = true;
				FreePlayer = ()=>{
					player.LockMovement = false;
				};
				RunningActionIndex = 0;
			}
		}
		public override void OnCollision(CollisionEventArgs collisionInfo)
		{
			IEntity otherEntity = (IEntity)collisionInfo.Other;
			if (!otherEntity.IsTile)
			{
				if (otherEntity._IsPlayer)
				{
					PlayerEntity player = (PlayerEntity)otherEntity;
					if (TriggerType == NpcTriggerTypes.Touch)
					{
						for (int i = 0; i < Actions.Count; i++)
						{
							Actions[i].SetInput(player.Input);
						}
						PlayerActionTrigger(player);
					}
				} else
				{
					NpcEntity npc = (NpcEntity)otherEntity;
					Debug.WriteLine($"NPC) X: {npc.X}, Y: {npc.Y}");
				}
			} else
			{
				TileCollide tile = (TileCollide)otherEntity;
				Debug.WriteLine($"Tile) X: {tile.Bounds.Position.X}, Y: {tile.Bounds.Position.Y}");
			}
			base.OnCollision(collisionInfo);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			if (RunningActionIndex == Actions.Count)
			{
				RunningActionIndex = -1;
				FreePlayer();
			}

			if (Actions.Count > 0 && RunningActionIndex < Actions.Count && RunningActionIndex >= 0)
			{
				var preIndex = RunningActionIndex;
				Actions[RunningActionIndex].Update(gameTime);
				if (preIndex != RunningActionIndex)
				{
					if (Actions[RunningActionIndex - 1].Done != null)
					{
						Actions[RunningActionIndex - 1].Done = null;
					}
				} else
				{
					if (Actions[RunningActionIndex].Done != null)
					{
						Actions[RunningActionIndex].Done = null;
						RunningActionIndex += 1;
					}
					
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			if (RunningActionIndex == Actions.Count)
			{
				RunningActionIndex = -1;
				FreePlayer();
			}
			if (Actions.Count > 0 && RunningActionIndex < Actions.Count && RunningActionIndex >= 0)
			{
				Actions[RunningActionIndex].Draw(spriteBatch);
			}
		}
	}

}