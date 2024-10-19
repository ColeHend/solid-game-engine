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
    public class PlayerActionSystem : IPlayerActionSystem
    {
			private ISceneManager _sceneManager { get; }
			private Dictionary<IPlayerEntity, IMap> CurrentPlayerMap { get; set; } = new Dictionary<IPlayerEntity, IMap>();
			private List<IPlayerEntity> Players { get {
				return _sceneManager.Game.Currents.Player;
			}}
			public PlayerActionSystem(ISceneManager sceneManager)
			{
				_sceneManager = sceneManager;
			}
			// --------- Private Functions -------------------------
			private List<NpcEntity> GetEntitiesNearPlayer(IPlayerEntity Player)
			{
				if (CurrentPlayerMap.ContainsKey(Player) == false || CurrentPlayerMap[Player] == null)
				{
					var CurrentLevel = _sceneManager.CurrentLevel;
					Player.MapDirections = CurrentLevel.currentMaps.GetMapDirections(_sceneManager.Game, Player.Input.PlayerIndex);
					var theMap = CurrentLevel.currentMaps.FindPlayersMap(Player);
					if (theMap.TileInfo.Count > 0)
					{
						SetCurrentPlayerMap(Player, theMap);
					}
				} else
				{
					var CurrentMap = CurrentPlayerMap[Player];
					if (CurrentMap != null && CurrentMap.TileInfo.Count > 0)
					{
						var tileSize = CurrentMap.TileInfo[0][0].Size;
						Vector2 playerLocation = new Vector2(Player.X, Player.Y);
						var checkEntityX = (IEntity entity) => Helpers.InRange(entity.X, playerLocation.X-tileSize, playerLocation.X+tileSize);
						var checkEntityY = (IEntity entity) => Helpers.InRange(entity.Y, playerLocation.Y-tileSize, playerLocation.Y+ tileSize + 4);
						var nearby = CurrentMap.GameEntities.Where(gameEntity=> checkEntityX(gameEntity) && checkEntityY(gameEntity)).ToList();
						List<NpcEntity> npcEntities = nearby.Select(g => (NpcEntity)g).ToList();
						return npcEntities.Where(npc=> {
							var npcHasAction = npc.TriggerType == NpcTriggerTypes.Action;
							var playerX = Player.X;
							var playerY = Player.Y;
							var npcX = npc.X;
							var npcY = npc.Y;
							var isFacing = false;
							switch (Player._facing)
							{
								case Direction.UP:
									isFacing = playerY > npcY;
									break;
								case Direction.DOWN:
									isFacing = playerY < npcY;
									break;
								case Direction.LEFT:
									isFacing = playerX > npcX;
									break;
								case Direction.RIGHT:
									isFacing = playerX < npcX;
									break;
							}
							return npcHasAction && isFacing;
						}).ToList();
					}
				}
				return new List<NpcEntity>();
			}


			//  ----------------------------------------------------
			public void SetCurrentPlayerMap(IPlayerEntity player, IMap map)
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
									if (entity.ActionIndex == -1)
									{
										entity.PlayerActionTrigger(Player);
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