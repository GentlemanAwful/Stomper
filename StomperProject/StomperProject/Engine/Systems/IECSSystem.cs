using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

using Stomper.Scripts;

namespace Stomper.Engine {
    public class ECSSystemHelpers {
        public static List<int> GetUniqueIDs(List<IECSComponent> components) {
            return components.Select(c => c.entityID).Distinct().ToList(); // Get IDs from all components, then only keep unique
        }

        public static T GetComponentFromEntity<T>(int entityID, List<IECSComponent> components) where T : IECSComponent {
            return (T)GetAllEntityComponents(entityID, components).Find((c) => c.GetType() == typeof(T));
        }

        public static List<IECSComponent> GetComponentsFromEntity<T>(int entityID, List<IECSComponent> components) where T : IECSComponent {
            return GetAllEntityComponents(entityID, components).FindAll((c) => c.GetType() == typeof(T));
        }

        public static List<IECSComponent> GetAllEntityComponents(int entityID, List<IECSComponent> components) {
            return components.FindAll(c => c.entityID == entityID);
        }
        /*
		public static IGameEvent GetEventByType<T>(List<IGameEvent> gameEvents)
		{
			return gameEvents.Find(ge => ge.payload.GetType() == typeof(T));
		}
		*/
        /*
		public static List<IGameEvent> GetAllEventsByType<T>(List<IGameEvent> gameEvents)
		{
			return gameEvents.FindAll(ge => ge.payload.GetType() == typeof(T));
		}
		*/
        public static bool EntityHasComponent<T>(int entityID, List<IECSComponent> components) where T : IECSComponent {
            return components.Exists(c => c.entityID == entityID);
        }
    }

    public enum SystemType {
        PHYSICS,
        LOGIC,
        RENDERING
    };

    public interface IECSSystem {
        SystemType Type { get; }
        Type[] RequiredComponents { get; }
        Type[] Exclusions { get; }
        void Initialize(FNAGame game, Config config);
        void Dispose();
        (List<Entity>, List<IGameEvent>) Execute(List<Entity> entities, List<IGameEvent> gameEvents);
        //(Entity[], IGameEvent[]) Execute(Entity[] entities, IGameEvent[] gameEvents);
    }
}
