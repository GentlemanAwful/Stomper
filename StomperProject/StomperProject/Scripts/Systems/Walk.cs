using System;
using System.Collections.Generic;
using System.Linq;

using Stomper.Engine;
using Stomper.Scripts.Components;

namespace Stomper.Scripts {
    public class Walk : IECSSystem {
        public SystemType Type => SystemType.LOGIC;
        public Type[] RequiredComponents { get; set; } = new Type[] { typeof(Position), typeof(WalkSpeed), typeof(InputData) };
        public Type[] Exclusions => new Type[0];

        public void Initialize(FNAGame game, Config config) {

        }

        public void Dispose() {

        }

        public (List<Entity>, List<IGameEvent>) Execute(List<Entity> entities, List<IGameEvent> gameEvents) {
            float deltaTime = ((FNAGame.DeltaTimeEvent)gameEvents.Find(ge => ge is FNAGame.DeltaTimeEvent)).gameTime.ElapsedGameTime.Milliseconds;

            IEnumerable<(int ID, Position position, WalkSpeed walkSpeed, int sign)> results = entities
                .Where(e => e.GetComponent<InputData>().inputs != null && e.GetComponent<InputData>().inputs.Exists(i => i.action == Input.Action.MOVE_LEFT || i.action == Input.Action.MOVE_RIGHT))
                .Select(e => (e.ID, e.GetComponent<Position>(), e.GetComponent<WalkSpeed>(), e.GetComponent<InputData>().inputs.Exists(i => i.action == Input.Action.MOVE_LEFT) ? -1 : 1))
                .Select(tuple => {
                    tuple.Item2.position.X += tuple.Item4 * tuple.Item3.Value * deltaTime;
                    return tuple;
                });

            foreach(var tuple in results) {
                int index = entities.FindIndex(e => e.ID == tuple.ID);
                entities[index].UpdateComponent(tuple.position);
            }

            return (entities, new List<IGameEvent>());
        }
    }
}
