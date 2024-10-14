using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles;
using Newtonsoft.Json;
using solid_game_engine.Shared.entity;
using solid_game_engine.Shared.entity.NPCActions;
using solid_game_engine.Shared.helpers;

namespace solid_game_engine.Shared.Entities
{
    public class Map
    {
			private TileMap TileMap { get; set; }
			private TileSetEntity TileSetEntity { get; set; }
			public float width { get; set; }
			public float height { get; set; }
			private int TileSize { get; set; }
			private Game1 _game { get; set; }
			private OrthographicCamera _camera { get; set; }
			private SceneManager _sceneManager { get; set; }

			public Map(SceneManager sceneManager, Microsoft.Xna.Framework.Vector2 origin, TileMap tileMap, TileSetEntity tileSetEntity, OrthographicCamera camera)
			{
				_game = sceneManager.Game;
				_sceneManager = sceneManager;
				TileMap = tileMap;
				_camera = camera;
				width = tileMap.Tiles[0].Count;
				height = tileMap.Tiles.Count;
				TileSetEntity = tileSetEntity;
				TileSize = tileSetEntity.TileSize;
				Origin = new Microsoft.Xna.Framework.Vector2(origin.X * TileSize, origin.Y * TileSize);
				GameEntities = new List<NpcEntity>();
				_collisionComponent = new CollisionComponent(new RectangleF(origin.X * TileSize, origin.Y * TileSize, width * TileSize, height * TileSize));
			}

			public readonly CollisionComponent _collisionComponent;
			public Microsoft.Xna.Framework.Vector2 Origin { get; set; }
			public List<NpcEntity> GameEntities { get; set; }

			public TileSet tileSet{ get; set; }
			public int Width { get => (int)width * (int)(tileSet?.TileSize ?? 32); }
			public int Height { get => (int)height * (int)(tileSet?.TileSize ?? 32); }
			public List<List<Tile>> TileInfo { get; set; } = new List<List<Tile>>();
			public List<TileCollide> collisionTiles { get; set; } = new List<TileCollide>();

			public void Initialize(GraphicsDeviceManager _graphicsDeviceManager)
			{
				_collisionComponent.Initialize();
				_collisionComponent.IsEnabled = true;
				
			}

			private List<GameEntity> GetGameEntitiesDraw()
			{
					// Initialize gameEntities with NPC entities (GameEntities)
					var gameEntities = new List<GameEntity>(GameEntities);

					// Add player entities if they are on this map
					var playersOnThisMap = _game.Currents.Player.Where(player => new List<Map>(){this}.FindPlayersMap(player) != null);
					gameEntities.AddRange(playersOnThisMap);

					// Sort gameEntities in-place by the Y-axis (ascending order)
					gameEntities.Sort((a, b) => a.Y.CompareTo(b.Y));

					return gameEntities;
			}

			public virtual void LoadContent(ContentManager contentManager)
			{
				Texture2D mapTexture = contentManager.Load<Texture2D>($"graphics\\{TileMap.TilesetName}");
				tileSet = new TileSet(mapTexture, TileSetEntity);

				Texture2D ghostTexture = contentManager.Load<Texture2D>("graphics\\playerGhost");
				var addOrigin = (int location, float origin) => location  + (int)origin;
				var loX = addOrigin(15 * (int)tileSet.TileSize, Origin.X);
				var loY = addOrigin(15 * (int)tileSet.TileSize, Origin.Y);
				var TextBox = new MessageBox(_game.sceneManager);
				TextBox.Text = "Hello Test Message!";
				var actions = new List<INPCActions>()
				{
					TextBox
				};
				var testNPC = new NpcEntity(_sceneManager, ghostTexture, loX, loY, 32, 48, actions);
				testNPC.TriggerType = Enums.NpcTriggerTypes.Action;
				GameEntities.Add(testNPC);
				this.SetCollisionTiles();
				this.SetCollisionComponent();
				for (int i = 0; i < _game.Currents.Player.Count; i++)
				{
					_collisionComponent.Insert(_game.Currents.Player[i]);
				}
			}

			private void SetCollisionTiles()
			{
				collisionTiles = new List<TileCollide>();
				for (int X = 0; X < TileMap.Tiles.Count; X++)
				{
					TileInfo.Add(new List<Tile>());
					for (int Y = 0; Y < TileMap.Tiles[X].Count; Y++)
					{
						TileInfo[X].Add(new Tile());
						var tileNumber = TileMap.Tiles[X][Y];
						var newTile = new Tile();
						newTile.TileNums = tileNumber;
						newTile.X = X;
						newTile.Y = Y;
						newTile.Size = TileSetEntity.TileSize;
						// --- Passable ---
						var hasLayerOne = tileSet.Passable.ContainsKey(tileNumber[0]);
						if (hasLayerOne)
						{
							var layerOnePassable = tileSet.Passable[tileNumber[0]];
							newTile.Passable = layerOnePassable;
						}
						if (tileNumber.Count >= 2)
						{
							var hasLayerTwo = tileSet.Passable.ContainsKey(tileNumber[1]);
							if (hasLayerTwo)
							{
								var layerTwoPassable = tileSet.Passable[tileNumber[1]];
								newTile.Passable = layerTwoPassable;
							}
						}
						// --- Effects ---
						var hasLayerOneEffect = tileSet.Effects.ContainsKey(tileNumber[0]);
						if (hasLayerOneEffect)
						{
							var layerOneEffect = tileSet.Effects[tileNumber[0]];
							newTile.Effect = layerOneEffect;
						}
						if (tileNumber.Count >= 2)
						{
							var hasLayerTwoEffect = tileSet.Effects.ContainsKey(tileNumber[1]);
							if (hasLayerTwoEffect)
							{
								var layerTwoEffect = tileSet.Effects[tileNumber[1]];
								newTile.Effect = layerTwoEffect;
							}
							if (!hasLayerOneEffect && !hasLayerTwoEffect)
							{
								newTile.Effect = 0;
							}
							
						} else if (!hasLayerOneEffect)
						{
							newTile.Effect = 0;
						}
						TileInfo[X][Y] = newTile;
						if (!newTile.Passable)
						{
							var collideTile = new TileCollide(newTile, Origin);
							
							collisionTiles.Add(collideTile);
						}
					}
				}
			}

			public void SetCollisionComponent()
			{
				foreach (var tile in collisionTiles)
				{
					_collisionComponent.Remove(tile);
					_collisionComponent.Insert(tile);
				}
				foreach (IEntity entity in GameEntities)
				{
					_collisionComponent.Remove(entity);
					_collisionComponent.Insert(entity);
				}
			}
			public void Update(GameTime gameTime)
			{
				foreach (var GameEntity in GameEntities)
				{
					var preUpdateActionIndex = GameEntity.ActionIndex;
					GameEntity.Update(gameTime);
					if (preUpdateActionIndex >= 0 && GameEntity.ActionIndex == -1)
					{
						var nearestPlayer = _game.Currents.Player.FindNearestPlayer(GameEntity).Input.PlayerIndex;
						var nearestPlayerIndex = _game.Currents.Player.FindIndex(player => player.Input.PlayerIndex == nearestPlayer);
						_game.Currents.Player[nearestPlayerIndex].LockMovement = false;
					}
				}
				foreach (var playa in _game.Currents.Player)
				{
					var CurrentMap = new List<Map>(){this}.FindPlayersMap(playa);
					if (CurrentMap != null)
					{
						playa.Update(gameTime);
						playa.GetInput(gameTime, CurrentMap, playa.LockMovement);
						if (playa.Input.PlayerIndex == PlayerIndex.One)
						{
							playa.GetFollowCamera(_camera);
						}
					}
				}
				_collisionComponent.Update(gameTime);
			}

			public void Draw(SpriteBatch _spriteBatch)
			{
				var transformMatrix = _camera.GetViewMatrix();

				_spriteBatch.Begin(transformMatrix: transformMatrix);
				var ScreenCoordinates = _camera.BoundingRectangle;
				TileMap.Tiles.DrawLayer(_spriteBatch, TileSize, Origin, tileSet, 1, ScreenCoordinates);
				TileMap.Tiles.DrawLayer(_spriteBatch, TileSize, Origin, tileSet, 2, ScreenCoordinates);
				_spriteBatch.End();

				var gameEntities = GetGameEntitiesDraw();

				foreach (var GameEntity in gameEntities)
				{
					GameEntity.matrix = transformMatrix;
					GameEntity.Draw(_spriteBatch);
				}
				foreach (var entity in gameEntities.Where(entity => entity is NpcEntity))
				{
					((NpcEntity)entity).DrawActions(_spriteBatch);
				}
				_spriteBatch.Begin(transformMatrix:transformMatrix);
				TileMap.Tiles.DrawLayer(_spriteBatch, TileSize, Origin, tileSet, 3, ScreenCoordinates);
				_spriteBatch.End();
			}
    }
}