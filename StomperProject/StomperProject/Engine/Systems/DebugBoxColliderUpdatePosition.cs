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
        public Type[] RequiredComponents { get; } = new Type[]
        {
            typeof(Position),
            typeof(Physics.BoxCollider)
        };

        public Type[] Exclusions => new Type[0];

        public void Initialize(FNAGame game, Config config)
        {

        }
        public void Dispose()
        {

        }

        public (List<Entity>, List<IGameEvent>) Execute(List<Entity> entities, List<IGameEvent> gameEvents)
        {
            //if (entities == null)
            //    return;

            List<Renderer.ColouredLine> lines = new List<Renderer.ColouredLine>();
            foreach(Entity entity in entities)
            {
                Position position = entity.GetComponent<Position>();
                Physics.BoxCollider boxCollider = entity.GetComponent<Physics.BoxCollider>();

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
                        (int)position.position.X + (int)boxCollider.Offset.X,
                        (int)position.position.Y + (int)boxCollider.Offset.Y,
                        (int)boxCollider.Size.X,
                        2
                    )
                };

                Renderer.ColouredLine rightLine = new Renderer.ColouredLine
                {
                    colour = Color.LightGreen,
                    rect = new Rectangle
                    (
                        (int)position.position.X + (int)boxCollider.Offset.X + (int)boxCollider.Size.X,
                        (int)position.position.Y + (int)boxCollider.Offset.Y,
                        2,
                        (int)boxCollider.Size.Y
                    )
                };

                Renderer.ColouredLine bottomLine = new Renderer.ColouredLine
                {
                    colour = Color.LightGreen,
                    rect = new Rectangle
                    (
                        (int)position.position.X + (int)boxCollider.Offset.X,
                        (int)position.position.Y + (int)boxCollider.Offset.Y + (int)boxCollider.Size.Y,
                        (int)boxCollider.Size.X,
                        2
                    )
                };

                Renderer.ColouredLine leftLine = new Renderer.ColouredLine
                {
                    colour = Color.LightGreen,
                    rect = new Rectangle
                    (
                        (int)position.position.X + (int)boxCollider.Offset.X,
                        (int)position.position.Y + (int)boxCollider.Offset.Y,
                        2,
                        (int)boxCollider.Size.Y
                    )
                };

                entity.AddComponent(topLine);
                entity.AddComponent(rightLine);
                entity.AddComponent(bottomLine);
                entity.AddComponent(leftLine);
            }
            return (entities, new List<IGameEvent>());
        }
    }
}
