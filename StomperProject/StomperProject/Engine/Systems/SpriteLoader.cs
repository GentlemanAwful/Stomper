using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Stomper.Engine.Renderer;
using Stomper.Scripts;

namespace Stomper.Engine
{
	// Events
	public class LoadSpritesEvent : IGameEvent {}

    public class SpriteLoader : IECSSystem
    {
		public SystemType Type => SystemType.LOGIC;
        public Type[] Archetype { get; } = new Type[] { typeof(Sprite) };
        public Type[] Exclusions => new Type[0];

        private FNAGame m_game;

        public void Initialize(FNAGame game, Config config) {
            m_game = game;
        }

        public void Dispose() {
            m_game = null;
        }

        public (Entity[], IGameEvent[]) Execute(Entity[] entities, IGameEvent[] gameEvents) {
            Entity[] unloadedEntities = entities.Where(e => e.GetComponent<Sprite>().Texture == null).ToArray();
            var updatedSprites = unloadedEntities.Select(e => {
                Sprite sprite = e.GetComponent<Sprite>();
                sprite.Texture = m_game.Content.Load<Texture2D>(sprite.TexturePath);
                return sprite;
            });

            for(int i = 0; i < updatedSprites.Count(); i++)
            {
                unloadedEntities[i].UpdateComponent<Sprite>(updatedSprites.ToList()[i]);
            }

            return (unloadedEntities, new IGameEvent[0]);
        }
    }
}
