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

        private Random m_random;            // TODO maybe this should be an event or component
        public Random Random => m_random;   
        private Config m_config;

        private IECSSystem[] m_systems;
        private IECSSystem[] m_physicsSystems;
        private IECSSystem[] m_logicSystems;
        private IECSSystem[] m_renderingSystems;

        private List<Entity> m_entities;


        public struct DeltaTimeEvent : IGameEvent { // TODO this should probably be in a separate Defs.cs or Events.cs or something
            public GameTime gameTime;
        }

        private FNAGame() {
            GraphicsDeviceManager gdm = new GraphicsDeviceManager(this);

            // Typically you would load a config here...
            gdm.PreferredBackBufferWidth    = 1920; // TODO support changing resolution in game
            gdm.PreferredBackBufferHeight   = 1080;
            gdm.IsFullScreen = false;
            gdm.SynchronizeWithVerticalRetrace = true;

            Content.RootDirectory = "Content"; // TODO put this path into config.json
        }

        protected override void LoadContent() {
            // Load textures, sounds, and so on in here...
            base.LoadContent();

            Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings() { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All };

            m_random    = new Random(m_config.RandomSeed);
            m_entities  = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Entity>>(System.IO.File.ReadAllText("Content/game.json"), settings);
            m_config    = Newtonsoft.Json.JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText("Content/config.json"), settings);
            
            for(int id = 0; id < m_entities.Count; id++) {
                m_entities[id] = new Entity(m_random.Next(), m_entities[id].Name, m_entities[id].Components);
            }
        }

        protected override void Initialize() {
            /* This is a nice place to start up the engine, after
             * loading configuration stuff in the constructor
             */
            base.Initialize();

            // TODO: Put into config.json?
            // These run in order!!
            m_systems = new IECSSystem[] {
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
                //new DebugBoxColliderUpdatePosition(),
                new SpriteLoader(),
                new StartStomp(),
                new Stomp(),
                new StopStomp(),

                // Render last
                new SpriteRenderer(),
                new SpriteTiledRenderer(),
                new SpriteCustomSizeRenderer(),

                // Debug rendering
                //new ColouredLineRenderer(),
            };

            // Sort systems into categories (no need to do it each frame)
            m_physicsSystems    = Array.FindAll(m_systems, s => s.Type == SystemType.PHYSICS);
            m_logicSystems      = Array.FindAll(m_systems, s => s.Type == SystemType.LOGIC);
            m_renderingSystems  = Array.FindAll(m_systems, s => s.Type == SystemType.RENDERING);

            foreach(IECSSystem physicsSystem in m_physicsSystems) {
                physicsSystem.Initialize(this, m_config);
            }

            foreach(IECSSystem logicSystem in m_logicSystems) {
                logicSystem.Initialize(this, m_config);
            }

            foreach(IECSSystem renderingSystem in m_renderingSystems) {
                renderingSystem.Initialize(this, m_config);
            }
        }

        protected override void UnloadContent() {
            // Clean up after yourself!
            base.UnloadContent();

            foreach(IECSSystem system in m_systems) {
                system.Dispose();
            }

            m_entities.Clear();
            m_config = default;
        }

        // Run game logic in here. Do NOT render anything here!
        protected override void Update(GameTime gameTime) {
            List<IGameEvent> gameEvents = new List<IGameEvent>();
            gameEvents.Add(new DeltaTimeEvent { gameTime = gameTime });

            foreach(IECSSystem system in m_physicsSystems) {
                List<Entity> filteredEntities = m_entities.FindAll(e => system.Archetype.All(rqt => e.Components.Any(c => c.GetType() == rqt)));
                filteredEntities = filteredEntities.FindAll(e => !system.Exclusions.Any(rqt => e.Components.Any(c => c.GetType() == rqt)));
                (Entity[], IGameEvent[]) updatedEntities = system.Execute(filteredEntities.ToArray(), gameEvents.ToArray());

                foreach(Entity updatedEntity in updatedEntities.Item1) {
                    int index = m_entities.FindIndex(e => e.ID == updatedEntity.ID);
                    if(index >= 0)
                        m_entities[index] = updatedEntity;
                    else
                        m_entities.Add(updatedEntity);
                }

                gameEvents.AddRange(updatedEntities.Item2);
            }

            foreach(IECSSystem system in m_logicSystems) {
                List<Entity> filteredEntities = m_entities.FindAll(e => system.Archetype.All(rqt => e.Components.Any(c => c.GetType() == rqt)));
                filteredEntities = filteredEntities.FindAll(e => !system.Exclusions.Any(rqt => e.Components.Any(c => c.GetType() == rqt)));
                (Entity[], IGameEvent[]) updatedEntities = system.Execute(filteredEntities.ToArray(), gameEvents.ToArray());

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

            foreach(IECSSystem system in m_renderingSystems) {
                List<Entity> filteredEntities = m_entities.FindAll(e => system.Archetype.All(rqt => e.Components.Any(c => c.GetType() == rqt)));
                filteredEntities = filteredEntities.FindAll(e => !system.Exclusions.Any(rqt => e.Components.Any(c => c.GetType() == rqt)));
                system.Execute(filteredEntities.ToArray(), gameEvents.ToArray());
            }

            base.Draw(gameTime);
        }
    }
}