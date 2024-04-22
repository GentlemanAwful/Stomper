using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Stomper.Scripts;
using System.Linq;
using Stomper.Engine.Physics;

namespace Stomper.Engine.Renderer
{
    public class SpriteTiledRenderer : IECSSystem
    {
        SpriteBatch batch;
		public SystemType Type => SystemType.RENDERING;
        public Type[] RequiredComponents { get; } = new Type[]
        { 
            typeof(Sprite), 
            typeof(Position),
            typeof(CustomSpriteSize),
            typeof(SpriteTiling) 
        };

        public Type[] Exclusions => new Type[0];

        public void Initialize( FNAGame game, Config config )
        {
            batch = new SpriteBatch(game.GraphicsDevice);
        }
        public void Dispose()
        {
            batch.Dispose();
        }

        /*
        public void Execute(List<IECSComponent> components, List<IGameEvent> gameEvents)
		{
			batch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);

            var groupedComponentsByEntityID = components.GroupBy(c => c.entityID);
            foreach (Type rqt in RequiredComponents)
            {
                groupedComponentsByEntityID = groupedComponentsByEntityID.Where(g => g.Any(c => c.GetType() == rqt));
            }

            foreach (var entityComponents in groupedComponentsByEntityID)
            {
                Sprite sprite = entityComponents.First(c => c is Sprite) as Sprite;
                Position position = entityComponents.First(c => c is Position) as Position;
                CustomSpriteSize customSpriteSize = entityComponents.First(c => c is CustomSpriteSize) as CustomSpriteSize;
                SpriteTiling spriteTiling = entityComponents.First(c => c is SpriteTiling) as SpriteTiling;

                batch.Draw(
                    sprite.Texture,
                    new Rectangle((int)position.position.X, (int)position.position.Y, customSpriteSize.Width, customSpriteSize.Height),
                    new Rectangle(0, 0, sprite.Texture.Width * spriteTiling.HorizontalTiling, sprite.Texture.Height * spriteTiling.VerticalTiling),
                    Color.White
                );
            }
			batch.End();
		}
        */
        
        public void Execute(IEnumerable<IGrouping<int, IECSComponent>> entities, List<IGameEvent> gameEvents)
        {
            batch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);
            foreach (var entityGroup in entities)
            {
                ExecuteSingle(entityGroup.ToList(), gameEvents);
            }
            batch.End();
        }

        public void ExecuteSingle(List<IECSComponent> components, List<IGameEvent> gameEvents)
        {
            Sprite sprite = (Sprite)components.First(c => c is Sprite);
            Position position = (Position)components.First(c => c is Position);
            CustomSpriteSize customSpriteSize = (CustomSpriteSize)components.First(c => c is CustomSpriteSize);
            SpriteTiling spriteTiling = (SpriteTiling)components.First(c => c is SpriteTiling);

            batch.Draw(
                sprite.Texture,
                new Rectangle((int)position.position.X, (int)position.position.Y, customSpriteSize.Width, customSpriteSize.Height),
                new Rectangle(0, 0, sprite.Texture.Width * spriteTiling.HorizontalTiling, sprite.Texture.Height * spriteTiling.VerticalTiling),
                Color.White
            );
        }

        public (List<Entity>, List<IGameEvent>) Execute(List<Entity> entities, List<IGameEvent> gameEvents)
        {
            batch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);
            foreach (Entity entity in entities)
            {
                Sprite sprite = entity.GetComponent<Sprite>();
                Position position = entity.GetComponent<Position>();
                CustomSpriteSize customSpriteSize = entity.GetComponent<CustomSpriteSize>();
                SpriteTiling spriteTiling = entity.GetComponent<SpriteTiling>();

                batch.Draw(
                    sprite.Texture,
                    new Rectangle((int)position.position.X, (int)position.position.Y, customSpriteSize.Width, customSpriteSize.Height),
                    new Rectangle(0, 0, sprite.Texture.Width * spriteTiling.HorizontalTiling, sprite.Texture.Height * spriteTiling.VerticalTiling),
                    Color.White
                );
            }
            batch.End();
            return (new List<Entity>(), new List<IGameEvent>());
        }
    }
}
