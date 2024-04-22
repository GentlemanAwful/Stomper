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
        public Type[] RequiredComponents { get; } = new Type[] { typeof(Sprite) };
        public Type[] Exclusions => new Type[0];

        private FNAGame m_game;

        public void Initialize(FNAGame game, Config config)
        {
            m_game = game;
        }

		public bool ActivationFilter(List<IGameEvent> gameEvents)
		{
			return gameEvents.Exists(ge => ge.GetType() == typeof(LoadSpritesEvent));
		}

		public void Execute(List<IECSComponent> components, List<IGameEvent> gameEvents)
		{
            List<IECSComponent> pendingSprites = components.FindAll(c => c.GetType() == typeof(Sprite) && ((Sprite)c).Texture == null);

            foreach (IECSComponent component in pendingSprites)
			{
				Sprite sprite = (Sprite)component;
				sprite.Texture = m_game.Content.Load<Texture2D>(sprite.TexturePath);
			}
		}

        public void Dispose()
        {
            m_game = null;
        }

        public (List<Entity>, List<IGameEvent>) Execute(List<Entity> entities, List<IGameEvent> gameEvents)
        {
            List<Entity> unloadedEntities = entities.FindAll(e => e.GetComponent<Sprite>().Texture == null);
            var updatedSprites = unloadedEntities.Select(e =>
            {
                Sprite sprite = e.GetComponent<Sprite>();
                sprite.Texture = m_game.Content.Load<Texture2D>(sprite.TexturePath);
                return sprite;
            });

            for(int i = 0; i < updatedSprites.Count(); i++)
            {
                unloadedEntities[i].UpdateComponent<Sprite>(updatedSprites.ToList()[i]);
            }

            return (unloadedEntities, new List<IGameEvent>());
        }
    }
}
