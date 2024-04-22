using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Stomper.Engine;
using Stomper.Engine.Physics;

namespace Stomper.Scripts {
    public class Jump : IECSSystem {
        public Type[] RequiredComponents { get; set; } = new Type[] { typeof(Mass) };

        public SystemType Type => SystemType.LOGIC;

        public Type[] Exclusions => new Type[0];

        // Specs
        private const float THETA = 0.01f;
        private float JumpAcceleration;

        public void Initialize(FNAGame game, Config config) {
            JumpAcceleration = config.JumpAcceleration;
        }

        public void Dispose() {
            JumpAcceleration = 0f;
        }

        public (List<Entity>, List<IGameEvent>) Execute(List<Entity> entities, List<IGameEvent> gameEvents) {

            //IEnumerable<Input.InputEvent> jumpEvents = gameEvents
            IEnumerable<int> playerTags = gameEvents
                .Where(ge => ge is Input.InputEvent)
                .Select(ge => (Input.InputEvent)ge)
                .Where(ie => ie.action is Input.Action.JUMP)
                .Select(je => je.playerID);

            IEnumerable<(int ID, Mass component)> results = entities
                //.Where(e => jumpEvents.Any(je => je.playerID == e.ID))
                .Where(e => playerTags.Any(tag => tag == e.GetComponent<PlayerTag>().Value))
                .Select(e => (e.ID, e.GetComponent<Mass>()))
                .Where(tuple => Math.Abs(tuple.Item2.Velocity.Y) < THETA) // Can't jump if in the air
                .Select(tuple => {
                    tuple.Item2.Velocity -= new Vector2(0f, JumpAcceleration * JumpAcceleration);
                    return tuple;
                });

            foreach(var tuple in results) {
                int index = entities.FindIndex(e => e.ID == tuple.ID);
                entities[index].UpdateComponent(tuple.component);
                Console.WriteLine($"UpdateComponent");
            }

            // Old style
            /*
            foreach(Input.InputEvent input in gameEvents.FindAll(ge => ge is Input.InputEvent)) {
                switch(input.action) {
                    case Input.Action.JUMP:
                    foreach(Entity entity in entities) {
                        Mass mass = entity.GetComponent<Mass>();
                        if(Math.Abs(mass.Velocity.Y) > 0.01f) // Can't jump if in the air
                            continue;
                        mass.Velocity -= new Vector2(0f, JumpAcceleration * JumpAcceleration);
                        entity.UpdateComponent(mass);
                    }
                    break;
                }
            }
            */
            return (entities, new List<IGameEvent>());
        }
    }
}
