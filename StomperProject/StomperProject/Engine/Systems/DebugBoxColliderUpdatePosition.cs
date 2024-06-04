using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Stomper.Scripts;

namespace Stomper.Engine
{
    public class DebugBoxColliderUpdatePosition : IECSSystem
    {
		public SystemType Type => SystemType.LOGIC;
        public Type[] Archetype { get; } = new Type[] {
            typeof(Position),
            typeof(Physics.HitboxSquare)
        };

        public Type[] Exclusions => new Type[0];

        public void Initialize(FNAGame game, Config config) {

        }
        public void Dispose() {

        }

        public (Entity[], IGameEvent[]) Execute(Entity[] entities, IGameEvent[] gameEvents) {
            //if (entities == null)
            //    return;

            List<Renderer.ColouredLine> lines = new List<Renderer.ColouredLine>();
            foreach(Entity entity in entities)
            {
                Position position = entity.GetComponent<Position>();
                Physics.HitboxSquare hitboxSquare = entity.GetComponent<Physics.HitboxSquare>();

                // Remove old lines...
                foreach(Renderer.ColouredLine existingLine in entity.GetComponents<Renderer.ColouredLine>())
                {
                    entity.RemoveComponent(existingLine);
                }

                // ...Then add updated lines
                Renderer.ColouredLine topLine = new Renderer.ColouredLine
                {
                    colour = Color.LightGreen,
                    rect = new Rectangle
                    (
                        (int)position.position.X + (int)hitboxSquare.Offset.X,
                        (int)position.position.Y + (int)hitboxSquare.Offset.Y,
                        (int)hitboxSquare.Size.X,
                        2
                    )
                };

                Renderer.ColouredLine rightLine = new Renderer.ColouredLine
                {
                    colour = Color.LightGreen,
                    rect = new Rectangle
                    (
                        (int)position.position.X + (int)hitboxSquare.Offset.X + (int)hitboxSquare.Size.X,
                        (int)position.position.Y + (int)hitboxSquare.Offset.Y,
                        2,
                        (int)hitboxSquare.Size.Y
                    )
                };

                Renderer.ColouredLine bottomLine = new Renderer.ColouredLine
                {
                    colour = Color.LightGreen,
                    rect = new Rectangle
                    (
                        (int)position.position.X + (int)hitboxSquare.Offset.X,
                        (int)position.position.Y + (int)hitboxSquare.Offset.Y + (int)hitboxSquare.Size.Y,
                        (int)hitboxSquare.Size.X,
                        2
                    )
                };

                Renderer.ColouredLine leftLine = new Renderer.ColouredLine
                {
                    colour = Color.LightGreen,
                    rect = new Rectangle
                    (
                        (int)position.position.X + (int)hitboxSquare.Offset.X,
                        (int)position.position.Y + (int)hitboxSquare.Offset.Y,
                        2,
                        (int)hitboxSquare.Size.Y
                    )
                };

                entity.AddComponent(topLine);
                entity.AddComponent(rightLine);
                entity.AddComponent(bottomLine);
                entity.AddComponent(leftLine);
            }
            return (entities, new IGameEvent[0]);
        }
    }
}
