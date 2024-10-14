using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using solid_game_engine.Shared.Entities;
using solid_game_engine.Shared.entity;
using solid_game_engine.Shared.Enums;
using solid_game_engine.Shared.helpers;

namespace solid_game_engine.Shared.Systems
{
    public class PlayerActionSystem
    {
			private SceneManager _sceneManager { get; }
			private Dictionary<PlayerEntity, Map> CurrentPlayerMap { get; set; } = new Dictionary<PlayerEntity, Map>();
			private List<PlayerEntity> Players { get {
				return _sceneManager.Game.Currents.Player;
			}}
			public PlayerActionSystem(SceneManager sceneManager)
			{
				_sceneManager = sceneManager;
			}
			// --------- Private Functions -------------------------
			private List<NpcEntity> GetEntitiesNearPlayer(PlayerEntity Player)
			{
				var CurrentMap = CurrentPlayerMap[Player];
				if (CurrentMap == null)
				{
					return new List<NpcEntity>();
				}
				var tileSize = CurrentMap.TileInfo[0][0].Size + 16;
				Vector2 playerLocation = new Vector2(Player.X, Player.Y);
				var checkEntityX = (GameEntity entity) => Helpers.InRange(entity.X, playerLocation.X-tileSize, playerLocation.X+tileSize);
				var checkEntityY = (GameEntity entity) => Helpers.InRange(entity.Y, playerLocation.Y-tileSize, playerLocation.Y+tileSize);
				var nearby = CurrentMap.GameEntities.Where(gameEntity=> checkEntityX(gameEntity) && checkEntityY(gameEntity)).ToList();
				List<NpcEntity> npcEntities = nearby.Select(g => (NpcEntity)g).ToList();
				return npcEntities.Where(npc=> npc.TriggerType == NpcTriggerTypes.Action).ToList();
			}


			//  ----------------------------------------------------
			public void SetCurrentPlayerMap(PlayerEntity player, Map map)
			{
				CurrentPlayerMap[player] = map;
			}
      public void Update(GameTime gameTime)
			{
				foreach (var Player in Players)
				{
					var nearPlayer = GetEntitiesNearPlayer(Player);
					var CurrentMap = CurrentPlayerMap[Player];
					if (nearPlayer.Count() > 0)
					{
						if (Player.Input.IsSinglePressed(Controls.A))
						{
							foreach (var entity in nearPlayer)
							{
								var entityIndex = CurrentMap.GameEntities.FindIndex(e=>e.X == entity.X && e.Y == entity.Y);
								var isPlayer = entity._IsPlayer;
								if (entityIndex != -1 && !isPlayer)
								{
									if (CurrentMap.GameEntities[entityIndex].ActionIndex == -1)
									{
										CurrentMap.GameEntities[entityIndex].PlayerActionTrigger(Player);
									}
								}
							}
						}
					}
				}

			}

			public void Draw(SpriteBatch spriteBatch)
			{

			}
    }
}