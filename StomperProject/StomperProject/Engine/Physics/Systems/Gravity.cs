using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Stomper.Scripts;
using Stomper.Scripts.Components;

namespace Stomper.Engine.Physics
{
    public class Gravity : IECSSystem
    {
        public SystemType Type => SystemType.PHYSICS;
        public Type[] RequiredComponents { get; set; } = new Type[] { typeof(Position), typeof(Mass) };

        public Type[] Exclusions => new Type[0];

        float gravity;

        public void Initialize(FNAGame game, Config config)
        {
            gravity = config.Gravity;
        }

        public void Dispose()
        {

        }
        public (List<Entity>, List<IGameEvent>) Execute(List<Entity> entities, List<IGameEvent> gameEvents)
        {
            foreach (var entity in entities)
            {
                double deltaTime = ((FNAGame.DeltaTimeEvent)gameEvents.Find(ge => ge is FNAGame.DeltaTimeEvent)).gameTime.ElapsedGameTime.TotalSeconds;

                Position position = entity.GetComponent<Position>();
                Mass mass = entity.GetComponent<Mass>();

                mass.Velocity.Y += (float)(gravity * gravity * deltaTime);
                position.position.Y += (float)(mass.Velocity.Y * deltaTime);

                entity.UpdateComponent(position);
                entity.UpdateComponent(mass);
            }

            return (entities, new List<IGameEvent>());
        }
    }
}
