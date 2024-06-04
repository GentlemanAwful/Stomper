using Stomper.Engine;
using Stomper.Scripts.Components;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stomper.Scripts {
    public class Stomp : IECSSystem {
        public SystemType Type => SystemType.LOGIC;

        public Type[] Archetype { get; set; } = new Type[] { typeof(StompData), typeof(Position) };

        public Type[] Exclusions => new Type[0];

        public void Initialize(FNAGame game, Config config) {

        }

        public void Dispose() {

        }

        public (Entity[], IGameEvent[]) Execute(Entity[] entities, IGameEvent[] gameEvents) {
            var stompingEntities = entities
                .Where(e => e.GetComponent<StompData>().stomping);

            float deltaTime = ((FNAGame.DeltaTimeEvent)gameEvents.First(ge => ge is FNAGame.DeltaTimeEvent)).gameTime.ElapsedGameTime.Milliseconds;
            foreach(Entity stompingEntity in stompingEntities) {
                Position position = stompingEntity.GetComponent<Position>();
                position.position.Y += stompingEntity.GetComponent<StompData>().stompVelocity * deltaTime;
                stompingEntity.UpdateComponent(position);
            }

            return (stompingEntities.ToArray(), new IGameEvent[0]);
        }
    }
}
