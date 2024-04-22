using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Stomper.Scripts;
using System.Linq;
using Stomper.Scripts.Components;

namespace Stomper.Engine.Renderer
{
    public class SpriteCustomSizeRenderer : IECSSystem
    {
		public SystemType Type => SystemType.RENDERING;
        private SpriteBatch batch;

        public Type[] RequiredComponents { get; } = new Type[]
        { 
            typeof(Sprite), 
            typeof(Position),
            typeof(CustomSpriteSize)
        };
        public Type[] Exclusions => new Type[] { typeof(SpriteTiling) };

        public void Initialize(FNAGame game, Config config)
        {
            batch = new SpriteBatch(game.GraphicsDevice);
        }

		/*
		public void Execute(List<Entity> entities, List<IGameEvent> gameEvents)
        {
            if ( entities == null )
                return;

            batch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);

            foreach ( Entity entity in entities.FindAll((e) => Entity.HasComponents(e, RequiredComponents)) )
            {
                // I'm thinking this isn't cache friendly, should get positions and textures directly
                Sprite sprite                       = Entity.GetComponent<Sprite>(entity);
                Position position                   = Entity.GetComponent<Position>(entity);
                CustomSpriteSize customSpriteSize   = Entity.GetComponent<CustomSpriteSize>(entity);

                batch.Draw(
                    sprite.Texture,
                    new Rectangle((int)position.position.X, (int)position.position.Y, customSpriteSize.Width, customSpriteSize.Height),
                    Color.White
                );
            }
            batch.End();
        }
		*/

        /*
		public void Execute(List<IECSComponent> components, List<IGameEvent> gameEvents)
		{
			batch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);

			foreach(int entityID in ECSSystemHelpers.GetUniqueIDs(components))
			{
				// I'm thinking this isn't cache friendly, should get positions and textures directly
				Sprite sprite 		= ECSSystemHelpers.GetComponentFromEntity<Sprite>(entityID, components);
				Position position 	= ECSSystemHelpers.GetComponentFromEntity<Position>(entityID, components);
				CustomSpriteSize customSpriteSize = ECSSystemHelpers.GetComponentFromEntity<CustomSpriteSize>(entityID, components);

				batch.Draw(
					sprite.Texture,
					new Rectangle((int)position.position.X, (int)position.position.Y, customSpriteSize.Width, customSpriteSize.Height),
					Color.White
				);
			}
			batch.End();
		}
        */

        public void Dispose()
        {
            batch.Dispose();
        }

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

            batch.Draw(
                sprite.Texture,
                new Rectangle((int)position.position.X, (int)position.position.Y, customSpriteSize.Width, customSpriteSize.Height),
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

                batch.Draw(
                    sprite.Texture,
                    new Rectangle((int)position.position.X, (int)position.position.Y, customSpriteSize.Width, customSpriteSize.Height),
                    Color.White
                );
            }
            batch.End();
            return (new List<Entity>(), new List<IGameEvent>());
        }
    }
}
