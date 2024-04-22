using System;
using System.Collections.Generic;

namespace Stomper.Engine {
    public struct Entity {
        public int ID;
        public string               Name;
        public List<IECSComponent>  Components;

        public void Init() {
            ID          = -1;
            Name        = "undefined";
            Components  = new List<IECSComponent>();
        }

        public static void Clear(Entity entity) {
            entity.ID = -1;
            entity.Name = "undefined";
            foreach(IECSComponent component in entity.Components) {
                component.entityID = -1;
            }
            entity.Components.Clear();
        }

        public void AddComponent(IECSComponent component) {
            Components.Add(component);
        }

        public void RemoveComponent(IECSComponent component) // TODO Change this
        {
            Components.Remove(component);
        }

        public void UpdateComponent<T>(T updatedComponent) where T : IECSComponent {
            int componentIndex = Components.FindIndex(c => c is T);
            Components[componentIndex] = updatedComponent;
        }

        public bool HasComponents(List<Type> requiredComponentTypes) {
            foreach(Type requiredComponentType in requiredComponentTypes) {
                if(Components.Find((c) => c.GetType() == requiredComponentType) == null) {
                    return false;
                }
            }
            return true;
        }
        /*
        public Dictionary<Type, IECSComponent> GetRequiredComponents()
        {
            return (T)Components.Find((c) => c.GetType() == typeof(T));
        }
        */
        public T GetComponent<T>() where T : IECSComponent {
            return (T)Components.Find((c) => c.GetType() == typeof(T));
        }

        public List<IECSComponent> GetComponents<T>() where T : IECSComponent {
            return Components.FindAll((c) => c.GetType() == typeof(T));
        }

        public bool HasComponent<T>() where T : IECSComponent {
            return Components.Exists((c) => c.GetType() == typeof(T));
        }

    }
}
