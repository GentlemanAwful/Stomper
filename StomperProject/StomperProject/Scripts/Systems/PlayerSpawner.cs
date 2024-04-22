using Microsoft.Xna.Framework;
using Stomper.Engine;
using Stomper.Engine.Physics;
using Stomper.Engine.Renderer;
using Stomper.Scripts.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stomper.Scripts {
    class PlayerSpawner : IECSSystem {
        public SystemType Type => SystemType.LOGIC;

        public Type[] RequiredComponents => new Type[0];

        public Type[] Exclusions => new Type[0];

        private Vector2[] spawnPoints;
        private Random random;
        private Entity playerTemplate;

        public void Initialize(FNAGame game, Config config) {
            random = game.Random;
            spawnPoints = config.Spawns.ToArray();
            playerTemplate = config.PlayerTemplate;
        }

        public void Dispose() {
            spawnPoints = null;
            playerTemplate = new Entity();
        }

        public (List<Entity>, List<IGameEvent>) Execute(List<Entity> entities, List<IGameEvent> gameEvents) {
            List<IGameEvent> spawnEvents = gameEvents.FindAll(ge =>
                ge is Input.InputEvent
                && ((Input.InputEvent)ge).action is Input.Action.SPAWN
                && ((Input.InputEvent)ge).state is Input.InputState.PRESSED
            );
            List<Entity> newPlayers = new List<Entity>();
            foreach(Input.InputEvent spawnEvent in spawnEvents) {
                Entity newPlayer = new Entity {
                    ID = random.Next(),
                    Name = "Player",
                    Components = new List<IECSComponent>()
                };

                foreach(IECSComponent component in playerTemplate.Components) {
                    newPlayer.AddComponent(component);
                }
                Position position = newPlayer.GetComponent<Position>();
                position.position = spawnPoints[random.Next(spawnPoints.Length)];
                newPlayer.UpdateComponent(position);

                PlayerTag tag = newPlayer.GetComponent<PlayerTag>();
                tag.Value = 4;
                newPlayer.UpdateComponent(tag);

                newPlayers.Add(newPlayer);
            }

            return (newPlayers, new List<IGameEvent>());
        }
    }
}
