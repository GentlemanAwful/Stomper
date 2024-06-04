using System;
using System.Collections.Generic;
using System.Linq;

using Stomper.Engine;
using Stomper.Scripts.Components;

namespace Stomper.Scripts {
    public class Walk : IECSSystem {
        public SystemType Type => SystemType.LOGIC;
        public Type[] Archetype { get; set; } = new Type[] { typeof(Position), typeof(WalkSpeed), typeof(InputData) };
        public Type[] Exclusions => new Type[0];

        public void Initialize(FNAGame game, Config config) {

        }

        public void Dispose() {

        }

        public (Entity[], IGameEvent[]) Execute(Entity[] entities, IGameEvent[] gameEvents) {
            float deltaTime = ((FNAGame.DeltaTimeEvent)gameEvents.First(ge => ge is FNAGame.DeltaTimeEvent)).gameTime.ElapsedGameTime.Milliseconds;

            IEnumerable<(int ID, Position position, WalkSpeed walkSpeed, int sign)> results = entities
                .Where(e => e.GetComponent<InputData>().inputs != null && e.GetComponent<InputData>().inputs.Exists(i => i.action == Input.Action.MOVE_LEFT || i.action == Input.Action.MOVE_RIGHT))
                .Select(e => (e.ID, e.GetComponent<Position>(), e.GetComponent<WalkSpeed>(), e.GetComponent<InputData>().inputs.Exists(i => i.action == Input.Action.MOVE_LEFT) ? -1 : 1))
                .Select(tuple => {
                    tuple.Item2.position.X += tuple.Item4 * tuple.Item3.Value * deltaTime;
                    return tuple;
                });

            foreach(var tuple in results) {
                //int index = entities.FindIndex(e => e.ID == tuple.ID);
                int index = Array.FindIndex(entities, e => e.ID == tuple.ID);
                entities[index].UpdateComponent(tuple.position);
            }

            return (entities, new IGameEvent[0]);
        }
    }
}
