using System;
using System.Collections.Generic;

namespace Stomper.Engine {
    public struct Entity {
        // These can't be readonly because they are auto converted from json
        public int                  ID;
        public string               Name;
        public List<IECSComponent>  Components;

        public Entity(int newID, string newName, List<IECSComponent> newComponents) {
            ID = newID;
            Name = newName;
            Components = newComponents;
        }

        public void AddComponent(IECSComponent component) {
            Components.Add(component);
        }

        public void RemoveComponent(IECSComponent component) {
            bool success = Components.Remove(component);
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
