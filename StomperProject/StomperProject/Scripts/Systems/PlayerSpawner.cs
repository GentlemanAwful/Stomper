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

        private Random random;

        public void Initialize(FNAGame game, Config config) {
            random = game.Random;
        }

        public void Dispose() {
        }

        public (List<Entity>, List<IGameEvent>) Execute(List<Entity> entities, List<IGameEvent> gameEvents) {
            IEnumerable<Entity> relevantSpawners = entities
                .Where(e => e.GetComponent<InputData>().inputs != null)
                .Where(e => e.GetComponent<InputData>().inputs.Exists(i => i.action == Input.Action.SPAWN))
                .Where(e => e.GetComponent<InputData>().inputs.Exists(i => i.state == Input.InputState.PRESSED));

            IEnumerable<(Entity template, int index, Vector2 spawnPosition)> playertemplates = relevantSpawners
                .Select(e => (
                    e.GetComponent<SpawnDetails>().playerTemplates[e.GetComponent<SpawnDetails>().spawnedPlayers % e.GetComponent<SpawnDetails>().playerTemplates.Count()], 
                    e.GetComponent<SpawnDetails>().spawnedPlayers,
                    e.GetComponent<SpawnDetails>().spawnPoints[random.Next(e.GetComponent<SpawnDetails>().spawnPoints.Length)]
                ));

            List<(Entity, Vector2)> newPlayers = playertemplates
                .Select(sd => (new Entity {
                    ID = random.Next(),
                    Name = $"player{sd.index}",
                    Components = sd.template.Components
                                    .Select(c => c)
                                    .ToList()
                }, sd.spawnPosition)
                )
                .ToList();

            // Update spawn counter for each spawner entity. Don't forget that only one spawn per frame works ATM!
            foreach(Entity spawner in relevantSpawners) {
                SpawnDetails spawnDetails = spawner.GetComponent<SpawnDetails>();
                spawnDetails.spawnedPlayers++;
                spawner.UpdateComponent(spawnDetails);
            }

            foreach((Entity newPlayer, Vector2 spawnPosition) in newPlayers) {
                Position position = newPlayer.GetComponent<Position>();
                position.position = spawnPosition;
                newPlayer.UpdateComponent(position);

                PlayerTag tag = newPlayer.GetComponent<PlayerTag>();
                tag.Value = 4;
                newPlayer.UpdateComponent(tag);
            }

            return (newPlayers.Select(np => np.Item1).Concat(relevantSpawners).ToList(), new List<IGameEvent>());
        }
    }
}
