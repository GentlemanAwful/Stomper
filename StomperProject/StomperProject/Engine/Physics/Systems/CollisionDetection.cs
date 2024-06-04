using System;
using System.Collections.Generic;
using System.Linq;
using Stomper.Scripts;

namespace Stomper.Engine.Physics
{
    public class CollisionDetection : IECSSystem
    {
		public SystemType Type => SystemType.PHYSICS;
        public Type[] Archetype { get; } = new Type[] { typeof(Position), typeof(HitboxSquare) };
        public Type[] Exclusions => new Type[0];

        public struct CollisionDetected : IGameEvent
        {
            public int entity0;
            public int entity1;
            public bool staticCollision;
        }

        public void Initialize(FNAGame game, Config config)
        {

        }

        public void Dispose()
        {

        }

		public static bool CheckCollision(int entityID0, int entityID1, List<IECSComponent> components)
		{
			Position position0 			= ECSSystemHelpers.GetComponentFromEntity<Position>(entityID0, components);
            HitboxSquare boxCollider0 	= ECSSystemHelpers.GetComponentFromEntity<HitboxSquare>(entityID0, components);

			Position position1 			= ECSSystemHelpers.GetComponentFromEntity<Position>(entityID1, components);
            HitboxSquare boxCollider1 	= ECSSystemHelpers.GetComponentFromEntity<HitboxSquare>(entityID1, components);

			// Horizontal
			bool horizontalDetection = (position0.position.X + boxCollider0.Offset.X) < (position1.position.X + boxCollider1.Offset.X) // check that box 1 isn't simply to the right of box 2
				&& (position0.position.X + boxCollider0.Offset.X + boxCollider0.Size.X) > (position1.position.X + boxCollider1.Offset.X);

			bool verticalDetection = 
				(position0.position.Y + boxCollider0.Offset.Y + boxCollider0.Size.Y) > (position1.position.Y + boxCollider1.Offset.Y)
				&&  (position0.position.Y + boxCollider0.Offset.Y) < (position1.position.Y + boxCollider1.Offset.Y + boxCollider1.Size.Y);

			return horizontalDetection && verticalDetection;
		}

        public (Entity[], IGameEvent[]) Execute(Entity[] entities, IGameEvent[] gameEvents)
        {
            List<IGameEvent> collisionEvents = new List<IGameEvent>();

            foreach (Entity entity0 in entities)
            {
                foreach (Entity entity1 in entities)
                {
                    if (entity0.ID == entity1.ID) continue;

                    Position position0 = entity0.GetComponent<Position>();
                    HitboxSquare boxCollider0 = entity0.GetComponent<HitboxSquare>();

                    Position position1 = entity1.GetComponent<Position>();
                    HitboxSquare boxCollider1 = entity1.GetComponent<HitboxSquare>();

                    bool horizontalDetection =
                        (position0.position.X + boxCollider0.Offset.X) < (position1.position.X + boxCollider1.Offset.X) // check that box 1 isn't simply to the right of box 2
                        && (position0.position.X + boxCollider0.Offset.X + boxCollider0.Size.X) > (position1.position.X + boxCollider1.Offset.X);

                    bool verticalDetection =
                        (position0.position.Y + boxCollider0.Offset.Y + boxCollider0.Size.Y) > (position1.position.Y + boxCollider1.Offset.Y)
                        && (position0.position.Y + boxCollider0.Offset.Y) < (position1.position.Y + boxCollider1.Offset.Y + boxCollider1.Size.Y);

                    if (horizontalDetection && verticalDetection)
                    {
                        collisionEvents.Add(new CollisionDetected{
                            entity0 = entity0.ID,
                            entity1 = entity1.ID,
                            staticCollision = entity0.HasComponent<Static>() || entity1.HasComponent<Static>()
                        });
                    }
                }
            }

            return (new Entity[0], collisionEvents.ToArray());
        }
    }
}
