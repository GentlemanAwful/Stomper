using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Stomper.Scripts;

namespace Stomper.Engine.Physics {
    public class ResolveDynamicCollision : IECSSystem {
		public SystemType Type => SystemType.PHYSICS;
        public Type[] Archetype => new Type[] { typeof(Position), typeof(HitboxSquare), typeof(Collider) };
        public Type[] Exclusions => new Type[0];

        public void Initialize(FNAGame game, Config config) {

        }

        public void Dispose() {

        }

        public (Entity[], IGameEvent[]) Execute(Entity[] entities, IGameEvent[] gameEvents) {
            List<Entity> updatedEntities = new List<Entity>();

            IEnumerable<CollisionDetection.CollisionDetected> relevantCollisionEvents = gameEvents
                .Where(ge => ge is CollisionDetection.CollisionDetected)
                .Select(ge => (CollisionDetection.CollisionDetected)ge)
                .Where(cd => !cd.staticCollision);

            foreach(CollisionDetection.CollisionDetected collisionEvent in relevantCollisionEvents) {
                // Getters
                bool entitiesHaveCollider = entities
                    .Where(e => new List<int> { collisionEvent.entity0, collisionEvent.entity1 }.Contains(e.ID))
                    .Where(e => e.HasComponent<Collider>())
                    .Count() > 1;

                if(!entitiesHaveCollider) continue;

                Entity entity0 = entities.First(e => e.ID == collisionEvent.entity0);
                Entity entity1 = entities.First(e => e.ID == collisionEvent.entity1);

                Mass mass0              = entity0.GetComponent<Mass>();
                Position position0      = entity0.GetComponent<Position>();
                HitboxSquare collider0   = entity0.GetComponent<HitboxSquare>();

                Mass mass1              = entity1.GetComponent<Mass>();
                Position position1      = entity1.GetComponent<Position>();
                HitboxSquare collider1   = entity1.GetComponent<HitboxSquare>();
                
                // Do work
                Vector2 center0 = position0.position + (collider0.Size * 0.5f);
                Vector2 center1 = position1.position + (collider1.Size * 0.5f);

                Vector2 distance    = center0 - center1;
                float heightOverlap = collider0.Size.Y - collider1.Size.Y - Math.Abs(distance.Y);
                float widthOverlap  = collider0.Size.X - collider1.Size.X - Math.Abs(distance.X);

                position1.position.Y = position0.position.Y - collider1.Size.Y;
                mass1.Velocity = Vector2.Zero;

                entity1.UpdateComponent(position1);
                entity1.UpdateComponent(mass1);

                updatedEntities.Add(entity0);
                updatedEntities.Add(entity1);
            }

            return (updatedEntities.ToArray(), new IGameEvent[0]);
        }
    }
}
