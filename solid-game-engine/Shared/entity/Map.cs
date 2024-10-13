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

			public Map(SceneManager sceneManager, Microsoft.Xna.Framework.Vector2 origin, TileMap tileMap, TileSetEntity tileSetEntity, OrthographicCamera camera)
			{
				_game = sceneManager.Game;
				TileMap = tileMap;
				_camera = camera;
				width = tileMap.Tiles[0].Count;
				height = tileMap.Tiles.Count;
				TileSetEntity = tileSetEntity;
				TileSize = tileSetEntity.TileSize;
				Origin = new Microsoft.Xna.Framework.Vector2(origin.X * TileSize, origin.Y * TileSize);
				GameEntities = new List<GameEntity>();
				_collisionComponent = new CollisionComponent(new RectangleF(origin.X * TileSize, origin.Y * TileSize, width * TileSize, height * TileSize));
			}

			public readonly CollisionComponent _collisionComponent;
			public Microsoft.Xna.Framework.Vector2 Origin { get; set; }
			public List<GameEntity> GameEntities { get; set; }

			public TileSet tileSet{ get; set; }
			public int Width { get => (int)width * (int)(tileSet?.TileSize ?? 32); }
			public int Height { get => (int)height * (int)(tileSet?.TileSize ?? 32); }
			public List<List<Tile>> TileInfo { get; set; } = new List<List<Tile>>();


			public void Initialize(GraphicsDeviceManager _graphicsDeviceManager)
			{
				
			}

			public virtual void LoadContent(ContentManager contentManager)
			{
				Texture2D mapTexture = contentManager.Load<Texture2D>($"graphics\\{TileMap.TilesetName}");
				tileSet = new TileSet(mapTexture, TileSetEntity);

				Texture2D ghostTexture = contentManager.Load<Texture2D>("graphics\\playerGhost");
				var addOrigin = (int location, float origin) => location  + (int)origin;
				var loX = addOrigin(4 * (int)tileSet.TileSize, Origin.X);
				var loY = addOrigin(4 * (int)tileSet.TileSize, Origin.Y);
				GameEntities.Add(new GameEntity(ghostTexture, loX, loY, 32, 48));
				List<TileCollide> collisionTiles = new List<TileCollide>();
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
							var collideTile = new TileCollide(newTile);
							
							collisionTiles.Add(collideTile);
						}
					}
				}
				foreach (var tile in collisionTiles)
				{
					_collisionComponent.Insert(tile);
				}
				foreach (IEntity entity in GameEntities)
				{
					_collisionComponent.Insert(entity);
				}
			}

			public void Update(GameTime gameTime)
			{
				foreach (var GameEntity in GameEntities)
				{
					GameEntity.Update(gameTime);
				}
				_collisionComponent.Update(gameTime);
			}

			public void Draw(SpriteBatch _spriteBatch, Matrix transformMatrix)
			{
				_spriteBatch.Begin(transformMatrix: transformMatrix);
				var ScreenCoordinates = _camera.BoundingRectangle;
				TileMap.Tiles.DrawLayer(_spriteBatch, TileSize, Origin, tileSet, 1, ScreenCoordinates);
				TileMap.Tiles.DrawLayer(_spriteBatch, TileSize, Origin, tileSet, 2, ScreenCoordinates);
				_spriteBatch.End();

				foreach (var GameEntity in GameEntities)
				{
					GameEntity.matrix = transformMatrix;
					GameEntity.Draw(_spriteBatch);
				}
				_spriteBatch.Begin(transformMatrix:transformMatrix);
				TileMap.Tiles.DrawLayer(_spriteBatch, TileSize, Origin, tileSet, 3, ScreenCoordinates);
				_spriteBatch.End();
			}
    }
}