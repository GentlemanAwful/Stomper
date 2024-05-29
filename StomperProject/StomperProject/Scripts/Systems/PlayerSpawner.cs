using Microsoft.Xna.Framework;

using Stomper.Engine;
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
        //private Entity playerTemplate; // TODO transfer this into spawndetails component

        public void Initialize(FNAGame game, Config config) {
            random = game.Random;
            spawnPoints = config.Spawns.ToArray();
            //playerTemplate = config.PlayerTemplate;
        }

        public void Dispose() {
            spawnPoints = null;
            //playerTemplate = new Entity();
        }

        public (List<Entity>, List<IGameEvent>) Execute(List<Entity> entities, List<IGameEvent> gameEvents) {
            IEnumerable<Entity> relevantSpawners = entities
                .Where(e => e.GetComponent<InputData>().inputs != null)
                .Where(e => e.GetComponent<InputData>().inputs.Exists(i => i.action == Input.Action.SPAWN))
                .Where(e => e.GetComponent<InputData>().inputs.Exists(i => i.state == Input.InputState.PRESSED));

            IEnumerable<(Entity template, int index)> playertemplates = relevantSpawners
                .Select(e => (e.GetComponent<SpawnDetails>().playerTemplates[e.GetComponent<SpawnDetails>().spawnedPlayers % e.GetComponent<SpawnDetails>().playerTemplates.Count()], e.GetComponent<SpawnDetails>().spawnedPlayers));

            List<Entity> newPlayers = playertemplates
                .Select(sd => new Entity {
                    ID = random.Next(),
                    Name = $"player{sd.index}",
                    Components = sd.template.Components
                                    .Select(c => c)
                                    .ToList()
                })
                .ToList();

            foreach(Entity spawner in relevantSpawners) {
                SpawnDetails spawnDetails = spawner.GetComponent<SpawnDetails>();
                spawnDetails.spawnedPlayers++;
                spawner.UpdateComponent(spawnDetails);
            }

            foreach(Entity newPlayer in newPlayers) {
                Position position = newPlayer.GetComponent<Position>();
                position.position = spawnPoints[random.Next(spawnPoints.Length)];
                newPlayer.UpdateComponent(position);

                PlayerTag tag = newPlayer.GetComponent<PlayerTag>();
                tag.Value = 4;
                newPlayer.UpdateComponent(tag);
            }

            return (newPlayers.Concat(relevantSpawners).ToList(), new List<IGameEvent>());
        }
    }
}
