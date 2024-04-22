using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Stomper.Scripts;
using System.Linq;

namespace Stomper.Engine.Renderer
{
    public class SpriteRenderer : IECSSystem
    {
        SpriteBatch batch;
		public SystemType Type => SystemType.RENDERING;
        public Type[] RequiredComponents { get; } = new Type[] { typeof(Sprite), typeof(Position) };
        public Type[] Exclusions => new Type[] { typeof(CustomSpriteSize), typeof(SpriteTiling) };
        public void Initialize( FNAGame game, Config config )
        {
            batch = new SpriteBatch(game.GraphicsDevice);
        }
        public void Dispose()
        {
            batch.Dispose();
        }
        /*
        public void Execute( List<IECSComponent> components, List<IGameEvent> gameEvents)
		{
			batch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);

			foreach(int entityID in ECSSystemHelpers.GetUniqueIDs(components))
			{
				// I'm thinking this isn't cache friendly, should get positions and textures directly
				Sprite sprite 		= ECSSystemHelpers.GetComponentFromEntity<Sprite>(entityID, components);
				Position position 	= ECSSystemHelpers.GetComponentFromEntity<Position>(entityID, components);
				batch.Draw(sprite.Texture, position.position, Color.White);
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

            batch.Draw(
                sprite.Texture,
                position.position,
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

                batch.Draw(
                    sprite.Texture,
                    position.position,
                    Color.White
                );
            }
            batch.End();
            return (new List<Entity>(), new List<IGameEvent>());
        }
    }
}
