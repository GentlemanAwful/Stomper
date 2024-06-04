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
        public Type[] Archetype { get; } = new Type[] { typeof(Sprite), typeof(Position) };
        public Type[] Exclusions => new Type[] { typeof(CustomSpriteSize), typeof(SpriteTiling) };
        public void Initialize( FNAGame game, Config config )
        {
            batch = new SpriteBatch(game.GraphicsDevice);
        }
        public void Dispose()
        {
            batch.Dispose();
        }

        public (Entity[], IGameEvent[]) Execute(Entity[] entities, IGameEvent[] gameEvents) {
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
            return (new Entity[0], new IGameEvent[0]);
        }
    }
}
