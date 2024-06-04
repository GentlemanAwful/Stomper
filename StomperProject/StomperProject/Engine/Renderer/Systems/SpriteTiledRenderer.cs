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
        public Type[] Archetype { get; } = new Type[]
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

        public (Entity[], IGameEvent[]) Execute(Entity[] entities, IGameEvent[] gameEvents) {
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
            return (new Entity[0], new IGameEvent[0]);
        }
    }
}
