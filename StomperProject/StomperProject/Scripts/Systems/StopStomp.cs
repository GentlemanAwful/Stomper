using Stomper.Engine;
using Stomper.Engine.Physics;
using Stomper.Scripts.Components;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stomper.Scripts {
    public class StopStomp : IECSSystem {
        public SystemType Type => SystemType.LOGIC;

        public Type[] Archetype { get; set; } = new Type[] { typeof(HitboxSquare) };

        public Type[] Exclusions => new Type[0];

        public void Initialize(FNAGame game, Config config) {

        }

        public void Dispose() {

        }

        public (Entity[], IGameEvent[]) Execute(Entity[] entities, IGameEvent[] gameEvents) {
            // I think this can be improved, but it works for now
            IEnumerable<Entity> stompingEntities = entities
                .Where(e => e.HasComponent<StompData>())
                .Where(e => e.GetComponent<StompData>().stomping); // Only players that are currently stomping

            IEnumerable<Entity> stompBlockers = entities
                .Where(e => e.HasComponent<StompBlocker>());

            IEnumerable<CollisionDetection.CollisionDetected> relevantCollisionEvents = gameEvents
                .Where(ge => ge is CollisionDetection.CollisionDetected)
                .Select(ge => (CollisionDetection.CollisionDetected)ge)
                .Where(cd => stompingEntities.Any(se => se.ID == cd.entity0) || stompingEntities.Any(se => se.ID == cd.entity1))
                .Where(cd => stompBlockers.Any(sb => sb.ID == cd.entity0) || stompBlockers.Any(sb => sb.ID == cd.entity1));

            var entitiesToStop = stompingEntities
                .Where(e => relevantCollisionEvents.Any(ce => ce.entity0 == e.ID) || relevantCollisionEvents.Any(ce => ce.entity1 == e.ID))
                .ToList();

            foreach(Entity entity in entitiesToStop) {
                entity.AddComponent(new Collider() { entityID = entity.ID });

                StompData stompData = entity.GetComponent<StompData>();
                stompData.stomping = false;
                entity.UpdateComponent(stompData);
            }

            return (entitiesToStop.ToArray(), new IGameEvent[0]);
        }
    }
}
