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

        public Type[] Archetype { get; } = new Type[]
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
                CustomSpriteSize customSpriteSize = entity.GetComponent<CustomSpriteSize>();

                batch.Draw(
                    sprite.Texture,
                    new Rectangle((int)position.position.X, (int)position.position.Y, customSpriteSize.Width, customSpriteSize.Height),
                    Color.White
                );
            }
            batch.End();
            return (new Entity[0], new IGameEvent[0]);
        }
    }
}
