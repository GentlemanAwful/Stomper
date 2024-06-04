using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Stomper.Engine;
using Stomper.Engine.Physics;

using Stomper.Scripts.Components;

namespace Stomper.Scripts {
    public class Jump : IECSSystem {
        public Type[] Archetype { get; set; } = new Type[] { typeof(Mass), typeof(JumpAcceleration), typeof(InputData) };

        public SystemType Type => SystemType.LOGIC;

        public Type[] Exclusions => new Type[0];

        // Specs
        private const float THETA = 0.01f;
        
        public void Initialize(FNAGame game, Config config) {
        }

        public void Dispose() {
        }

        public (Entity[], IGameEvent[]) Execute(Entity[] entities, IGameEvent[] gameEvents) {
            IEnumerable<(int ID, Mass mass, JumpAcceleration jumpAcceleration)> results = entities
                .Where(e => e.GetComponent<InputData>().inputs != null && e.GetComponent<InputData>().inputs.Exists(ie => ie.action == Input.Action.JUMP)) // TODO fix null inputs list
                .Select(e => (e.ID, e.GetComponent<Mass>(), e.GetComponent<JumpAcceleration>()))
                .Where(tuple => Math.Abs(tuple.Item2.Velocity.Y) < THETA) // Can't jump if in the air
                .Select(tuple => {
                    tuple.Item2.Velocity -= new Vector2(0f, tuple.Item3.Value * tuple.Item3.Value);
                    return tuple;
                });

            foreach(var tuple in results) {
                //int index = entities.FindIndex(e => e.ID == tuple.ID);
                int index = Array.FindIndex(entities, e => e.ID == tuple.ID);
                entities[index].UpdateComponent(tuple.mass);
            }
            return (entities, new IGameEvent[0]);
        }
    }
}
