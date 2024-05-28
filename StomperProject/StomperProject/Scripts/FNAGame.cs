using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Stomper.Engine;
using Stomper.Engine.Renderer;
using Stomper.Engine.Physics;

namespace Stomper.Scripts {
    public class FNAGame : Game {
        static void Main(string[] args) {
            using(FNAGame g = new FNAGame()) {
                g.Run();
            }
        }

        private Random m_random;
        public Random Random => m_random;
        private Config m_config;
        private List<IECSSystem> m_systems;
        private List<Entity> m_entities;


        public struct DeltaTimeEvent : IGameEvent {
            public GameTime gameTime;
        }

        private FNAGame() {
            GraphicsDeviceManager gdm = new GraphicsDeviceManager(this);

            // Typically you would load a config here...
            gdm.PreferredBackBufferWidth = 1920;
            gdm.PreferredBackBufferHeight = 1080;
            gdm.IsFullScreen = false;
            gdm.SynchronizeWithVerticalRetrace = true;

            // All content loaded will be in a "Content" folder
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent() {
            // Load textures, sounds, and so on in here...
            base.LoadContent();

            Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings()
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All
            };
            m_entities = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Entity>>(System.IO.File.ReadAllText("Content/game.json"), settings);
            m_config = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText("Content/config.json"), settings);

            m_random = new Random(m_config.RandomSeed);
            for(int id = 0; id < m_entities.Count; id++) {
                Entity updatedEntity = m_entities[id];
                updatedEntity.ID = m_random.Next();
                m_entities[id] = updatedEntity;
            }
        }

        protected override void Initialize() {
            /* This is a nice place to start up the engine, after
             * loading configuration stuff in the constructor
             */
            base.Initialize();

            m_systems = new List<IECSSystem> {
                // TODO: Put these into game.json or config.json

                // These run in order!!

                // Physics first
                new Gravity(),
                new CollisionDetection(),
                new ResolveStaticCollision(),
                new ResolveDynamicCollision(),

                // Logic second
                new Input(),
                new PlayerSpawner(),
                new Jump(),
                new Walk(),
                new DebugBoxColliderUpdatePosition(),
                new SpriteLoader(),

                // Render last
                new SpriteRenderer(),
                new SpriteTiledRenderer(),
                new SpriteCustomSizeRenderer(),

                // Debug rendering
                new ColouredLineRenderer(),
            };

            
            foreach(IECSSystem physicsSystem in m_systems.FindAll(s => s.Type == SystemType.PHYSICS)) {
                physicsSystem.Initialize(this, m_config);
            }

            foreach(IECSSystem logicSystem in m_systems.FindAll(s => s.Type == SystemType.LOGIC)) {
                logicSystem.Initialize(this, m_config);
            }

            foreach(IECSSystem renderingSystem in m_systems.FindAll(s => s.Type == SystemType.RENDERING)) {
                renderingSystem.Initialize(this, m_config);
            }
            
        }

        protected override void UnloadContent() {
            // Clean up after yourself!
            base.UnloadContent();

            
            foreach(IECSSystem system in m_systems) {
                system.Dispose();
            }
        }

        // Run game logic in here. Do NOT render anything here!
        protected override void Update(GameTime gameTime) {
            List<IGameEvent> gameEvents = new List<IGameEvent>();
            gameEvents.Add(new DeltaTimeEvent { gameTime = gameTime });

            foreach(IECSSystem system in m_systems.FindAll(s => s.Type == SystemType.PHYSICS)) {
                List<Entity> filteredEntities = m_entities.FindAll(e => system.RequiredComponents.All(rqt => e.Components.Any(c => c.GetType() == rqt)));
                filteredEntities = filteredEntities.FindAll(e => !system.Exclusions.Any(rqt => e.Components.Any(c => c.GetType() == rqt)));
                (List<Entity>, List<IGameEvent>) updatedEntities = system.Execute(filteredEntities, gameEvents);

                foreach(Entity updatedEntity in updatedEntities.Item1) {
                    int index = m_entities.FindIndex(e => e.ID == updatedEntity.ID);
                    if(index >= 0)
                        m_entities[index] = updatedEntity;
                    else
                        m_entities.Add(updatedEntity);
                }

                gameEvents.AddRange(updatedEntities.Item2);
            }

            foreach(IECSSystem system in m_systems.FindAll(s => s.Type == SystemType.LOGIC)) {
                List<Entity> filteredEntities = m_entities.FindAll(e => system.RequiredComponents.All(rqt => e.Components.Any(c => c.GetType() == rqt)));
                filteredEntities = filteredEntities.FindAll(e => !system.Exclusions.Any(rqt => e.Components.Any(c => c.GetType() == rqt)));
                (List<Entity>, List<IGameEvent>) updatedEntities = system.Execute(filteredEntities, gameEvents);

                foreach(Entity updatedEntity in updatedEntities.Item1) {
                    int index = m_entities.FindIndex(e => e.ID == updatedEntity.ID);
                    if(index >= 0)
                        m_entities[index] = updatedEntity;
                    else
                        m_entities.Add(updatedEntity);
                }

                gameEvents.AddRange(updatedEntities.Item2);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            // Render stuff in here. Do NOT run game logic in here!
            List<IGameEvent> gameEvents = new List<IGameEvent>();
            gameEvents.Add(new DeltaTimeEvent { gameTime = gameTime });
            GraphicsDevice.Clear(m_config.DefaultClearColour);

            foreach(IECSSystem system in m_systems.FindAll(s => s.Type == SystemType.RENDERING)) {
                List<Entity> filteredEntities = m_entities.FindAll(e => system.RequiredComponents.All(rqt => e.Components.Any(c => c.GetType() == rqt)));
                filteredEntities = filteredEntities.FindAll(e => !system.Exclusions.Any(rqt => e.Components.Any(c => c.GetType() == rqt)));
                system.Execute(filteredEntities, gameEvents);
            }

            base.Draw(gameTime);
        }
    }
}