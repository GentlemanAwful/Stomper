using System;
using System.Collections.Generic;
using Stomper.Scripts;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Stomper.Engine.Renderer
{
    public class ColouredLineRenderer : IECSSystem
    {
		public SystemType Type => SystemType.RENDERING;
        public Type[] Archetype { get; } = new Type[]
        {
            typeof(ColouredLine)
        };
        public Type[] Exclusions => new Type[0];

        private SpriteBatch batch;
        private Texture2D LineTexture;

        public void Dispose()
        {
            batch.Dispose();
        }

        public void Initialize(FNAGame game, Config config)
        {
            batch = new SpriteBatch(game.GraphicsDevice);
            LineTexture = new Texture2D(game.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        }

        public (Entity[], IGameEvent[]) Execute(Entity[] entities, IGameEvent[] gameEvents) {
            batch.Begin(SpriteSortMode.FrontToBack, BlendState.Opaque, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone);

            foreach (Entity entity in entities)
            {
                foreach (ColouredLine line in entity.GetComponents<ColouredLine>())
                {
                    batch.Draw (
                        LineTexture,
                        new Rectangle (
                            line.rect.X,
                            line.rect.Y,
                            line.rect.Width,
                            line.rect.Height
                        ),
                        line.colour
                    );
                }
            }

            batch.End();
            return (new Entity[0], new IGameEvent[0]);
        }
    }
}
