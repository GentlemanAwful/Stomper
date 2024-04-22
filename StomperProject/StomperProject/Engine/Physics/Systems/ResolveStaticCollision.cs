using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Stomper.Scripts;

namespace Stomper.Engine.Physics
{
    public class ResolveStaticCollision : IECSSystem
    {
		public SystemType Type => SystemType.PHYSICS;
        public Type[] RequiredComponents => new Type[] { typeof(Position), typeof(BoxCollider) };
        public Type[] Exclusions => new Type[0];

        public void Initialize(FNAGame game, Config config)
        {

        }

        public void Dispose()
        {

        }

        public (List<Entity>, List<IGameEvent>) Execute(List<Entity> entities, List<IGameEvent> gameEvents)
        {
            List<Entity> updatedEntities = new List<Entity>();

            foreach (CollisionDetection.CollisionDetected collisionEvent in gameEvents.FindAll(ge => ge is CollisionDetection.CollisionDetected && ((CollisionDetection.CollisionDetected)ge).staticCollision))
            {
                // Getters
                Entity staticEntity = entities.Find(e => 
                    e.HasComponents(new List<Type> { typeof(Static) })
                    && new List<int> { collisionEvent.entity0, collisionEvent.entity1 }.Contains(e.ID)
                );

                Entity dynamicEntity = entities.Find(e =>
                    e.HasComponents(new List<Type> { typeof(Mass) })
                    && new List<int> { collisionEvent.entity0, collisionEvent.entity1 }.Contains(e.ID)
                );

                Position staticPosition = staticEntity.GetComponent<Position>();
                Position dynamicPosition = dynamicEntity.GetComponent<Position>();

                BoxCollider staticCollider = staticEntity.GetComponent<BoxCollider>();
                BoxCollider dynamicCollider = dynamicEntity.GetComponent<BoxCollider>();

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

            return (updatedEntities, new List<IGameEvent>());
        }
    }
}
