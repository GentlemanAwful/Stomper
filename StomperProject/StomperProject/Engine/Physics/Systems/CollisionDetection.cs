using System;
using System.Collections.Generic;
using System.Linq;
using Stomper.Scripts;

namespace Stomper.Engine.Physics
{
    public class CollisionDetection : IECSSystem
    {
		public SystemType Type => SystemType.PHYSICS;
        public Type[] RequiredComponents { get; } = new Type[] { typeof(Position), typeof(BoxCollider) };
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
			BoxCollider boxCollider0 	= ECSSystemHelpers.GetComponentFromEntity<BoxCollider>(entityID0, components);

			Position position1 			= ECSSystemHelpers.GetComponentFromEntity<Position>(entityID1, components);
			BoxCollider boxCollider1 	= ECSSystemHelpers.GetComponentFromEntity<BoxCollider>(entityID1, components);

			// Horizontal
			bool horizontalDetection = (position0.position.X + boxCollider0.Offset.X) < (position1.position.X + boxCollider1.Offset.X) // check that box 1 isn't simply to the right of box 2
				&& (position0.position.X + boxCollider0.Offset.X + boxCollider0.Size.X) > (position1.position.X + boxCollider1.Offset.X);

			bool verticalDetection = 
				(position0.position.Y + boxCollider0.Offset.Y + boxCollider0.Size.Y) > (position1.position.Y + boxCollider1.Offset.Y)
				&&  (position0.position.Y + boxCollider0.Offset.Y) < (position1.position.Y + boxCollider1.Offset.Y + boxCollider1.Size.Y);

			return horizontalDetection && verticalDetection;
		}

        public (List<Entity>, List<IGameEvent>) Execute(List<Entity> entities, List<IGameEvent> gameEvents)
        {
            List<IGameEvent> collisionEvents = new List<IGameEvent>();

            foreach (Entity entity0 in entities)
            {
                foreach (Entity entity1 in entities)
                {
                    if (entity0.ID == entity1.ID) continue;

                    Position position0 = entity0.GetComponent<Position>();
                    BoxCollider boxCollider0 = entity0.GetComponent<BoxCollider>();

                    Position position1 = entity1.GetComponent<Position>();
                    BoxCollider boxCollider1 = entity1.GetComponent<BoxCollider>();

                    bool horizontalDetection =
                        (position0.position.X + boxCollider0.Offset.X) < (position1.position.X + boxCollider1.Offset.X) // check that box 1 isn't simply to the right of box 2
                        && (position0.position.X + boxCollider0.Offset.X + boxCollider0.Size.X) > (position1.position.X + boxCollider1.Offset.X);

                    bool verticalDetection =
                        (position0.position.Y + boxCollider0.Offset.Y + boxCollider0.Size.Y) > (position1.position.Y + boxCollider1.Offset.Y)
                        && (position0.position.Y + boxCollider0.Offset.Y) < (position1.position.Y + boxCollider1.Offset.Y + boxCollider1.Size.Y);

                    if (horizontalDetection && verticalDetection)
                    {
                        gameEvents.Add(new CollisionDetected{
                            entity0 = entity0.ID,
                            entity1 = entity1.ID,
                            staticCollision = entity0.HasComponent<Static>() || entity1.HasComponent<Static>()
                        });
                    }
                }
            }

            return (new List<Entity>(), collisionEvents);
        }
    }
}
