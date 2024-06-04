using Stomper.Engine;
using Stomper.Engine.Physics;
using Stomper.Scripts.Components;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stomper.Scripts {
    public class StartStomp : IECSSystem {
        public SystemType Type => SystemType.LOGIC;

        public Type[] Archetype { get; set; } = new Type[] { typeof(StompData), typeof(Position), typeof(InputData) };

        public Type[] Exclusions => new Type[0];

        public void Initialize(FNAGame game, Config config) {

        }

        public void Dispose() {

        }

        public (Entity[], IGameEvent[]) Execute(Entity[] entities, IGameEvent[] gameEvents) {
            var startingStomp = entities
                .Where(e => e.GetComponent<InputData>().inputs != null)
                .Where(e => e.GetComponent<InputData>().inputs.Exists(i => i.action == Input.Action.ATTACK))
                .Where(e => e.GetComponent<InputData>().inputs.Exists(i => i.state == Input.InputState.PRESSED));

            foreach(Entity entity in startingStomp) {
                StompData stompData = entity.GetComponent<StompData>();
                stompData.stomping = true;
                entity.UpdateComponent(stompData);

                Collider collider = entity.GetComponent<Collider>();
                entity.RemoveComponent(collider);
            }

            return (startingStomp.ToArray(), new IGameEvent[0]);
        }
    }
}
