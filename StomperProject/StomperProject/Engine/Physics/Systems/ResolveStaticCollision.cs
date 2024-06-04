using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Stomper.Scripts;

namespace Stomper.Engine.Physics {
    public class ResolveStaticCollision : IECSSystem {
		public SystemType Type => SystemType.PHYSICS;
        public Type[] Archetype => new Type[] { typeof(Position), typeof(HitboxSquare), typeof(Collider) };
        public Type[] Exclusions => new Type[0];

        public void Initialize(FNAGame game, Config config) {

        }

        public void Dispose() {

        }

        public (Entity[], IGameEvent[]) Execute(Entity[] entities, IGameEvent[] gameEvents) {
            List<Entity> updatedEntities = new List<Entity>();

            var relevantCollisionEvents = gameEvents
                .Where(ge => ge is CollisionDetection.CollisionDetected)
                .Select(ge => (CollisionDetection.CollisionDetected)ge)
                .Where(cd => cd.staticCollision);

            foreach (CollisionDetection.CollisionDetected collisionEvent in relevantCollisionEvents) {
                // Getters
                var staticEntities = entities
                    .Where(e => e.HasComponents(new List<Type> { typeof(Static), typeof(Collider) }))
                    .Where(e => new List<int> { collisionEvent.entity0, collisionEvent.entity1 }.Contains(e.ID));

                var dynamicEntities = entities
                    .Where(e => e.HasComponents(new List<Type> { typeof(Mass), typeof(Collider) }))
                    .Where(e => new List<int> { collisionEvent.entity0, collisionEvent.entity1 }.Contains(e.ID));

                if (staticEntities.Count() < 1 || dynamicEntities.Count() < 1) continue;

                Entity staticEntity     = staticEntities.First();
                Entity dynamicEntity    = dynamicEntities.First();

                Position staticPosition = staticEntity.GetComponent<Position>();
                Position dynamicPosition = dynamicEntity.GetComponent<Position>();

                HitboxSquare staticCollider = staticEntity.GetComponent<HitboxSquare>();
                HitboxSquare dynamicCollider = dynamicEntity.GetComponent<HitboxSquare>();

                Mass dynamicMass = dynamicEntity.GetComponent<Mass>();

                // Do work
                Vector2 staticCenter = staticPosition.position + (staticCollider.Size * 0.5f);
                Vector2 dynamicCenter = dynamicPosition.position + (dynamicCollider.Size * 0.5f);

                Vector2 distance = staticCenter - dynamicCenter;
                //float widthOverlap = dynamicCollider.Size.X - staticCollider.Size.X - Math.Abs(distance.X);
                float heightOverlap = staticCollider.Size.Y - dynamicCollider.Size.Y - Math.Abs(distance.Y);

                //dynamicPosition.position.Y -= (heightOverlap * Math.Sign(heightOverlap)) - dynamicCollider.Size.Y;
                dynamicPosition.position.Y = staticPosition.position.Y - dynamicCollider.Size.Y;
                dynamicMass.Velocity = Vector2.Zero;

                dynamicEntity.UpdateComponent(dynamicPosition);
                dynamicEntity.UpdateComponent(dynamicMass);

                updatedEntities.Add(staticEntity);
                updatedEntities.Add(dynamicEntity);
            }

            return (updatedEntities.ToArray(), new IGameEvent[0]);
        }
    }
}
