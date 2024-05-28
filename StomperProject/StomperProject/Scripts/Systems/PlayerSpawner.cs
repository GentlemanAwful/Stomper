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

        public Type[] RequiredComponents => new Type[] { typeof(SpawnDetails) };

        public Type[] Exclusions => new Type[0];

        private Vector2[] spawnPoints; // TODO get this from spawndetails entity and its position component
        private Random random;
        private Entity playerTemplate; // TODO transfer this into spawndetails component

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
            /*
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
            */
            foreach(Entity e in entities) {
                //Console.WriteLine($"e.Name: {e.Name}");
                //Console.WriteLine($"InputData: {e.GetComponent<InputData>()}");
                //Console.WriteLine($"inputs: {e.GetComponent<InputData>().inputs}");
            }

            List<Input.InputEvent> spawnEvents = entities
                .SelectMany(e => e.GetComponent<InputData>().inputs)
                .Where(i => i.action == Input.Action.SPAWN)
                .Where(i => i.state == Input.InputState.PRESSED)
                .ToList();

            List<IECSComponent> playerComponents = playerTemplate.Components
                .Select(c => c)
                .ToList();

            List<Entity> newPlayers = spawnEvents
                .Select(se => new Entity {
                    ID = random.Next(),
                    Name = "Player",
                    Components = playerTemplate.Components
                                    .Select(c => c)
                                    .ToList()
                })
                .ToList();

            foreach(Entity newPlayer in newPlayers) {
                //foreach(IECSComponent component in playerTemplate.Components) {
                //    newPlayer.AddComponent(component);
                //}
                Position position = newPlayer.GetComponent<Position>();
                position.position = spawnPoints[random.Next(spawnPoints.Length)];
                newPlayer.UpdateComponent(position);

                PlayerTag tag = newPlayer.GetComponent<PlayerTag>();
                tag.Value = 4;
                newPlayer.UpdateComponent(tag);

                //newPlayers.Add(newPlayer);
            }
            return (newPlayers, new List<IGameEvent>());
        }
    }
}
