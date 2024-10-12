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

			public Map(Game1 game, Microsoft.Xna.Framework.Vector2 origin, TileMap tileMap, TileSetEntity tileSetEntity)
			{
				_game = game;
				TileMap = tileMap;
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
				var ScreenCoordinates = _game._camera.BoundingRectangle;
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