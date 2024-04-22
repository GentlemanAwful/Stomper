using Stomper.Engine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StomperProject.Engine
{
    public class ECSManager
    {
        private List<IECSComponent> m_components;
        //public List<IECSComponent> Components => m_components;

        public void Initialize()
        {
            m_components = new List<IECSComponent>();
        }

        public void Dispose()
        {
            m_components.Clear();
        }

        public void AddComponents(List<IECSComponent> newComponents)
        {
            m_components.AddRange(newComponents);
        }


        public IEnumerable<IGrouping<int, IECSComponent>> GetRequiredComponents(List<Type> requiredComponents, List<Type> exclusions) // TODO exclusions not implemented yet
        {
            // Group entity components together
            IEnumerable<IGrouping<int, IECSComponent>> groupedByID = m_components.GroupBy(c => c.entityID);
            // Select entities that contain all required components
            IEnumerable<IGrouping<int, IECSComponent>> entitiesWithRequiredComponents = groupedByID.Where(v => requiredComponents.All(rqt => v.Any(c => c.GetType() == rqt)));
            return entitiesWithRequiredComponents;
        }

        public List<Entity> FilterEntities(List<Type> requiredComponents, List<Type> exclusions)
        {
            List<Entity> apropriateEntities = new List<Entity>();

            IEnumerable<int> ids = m_components.Select(c => c.entityID).Distinct();
            foreach(int id in ids)
            {
                IEnumerable<IECSComponent> entityComponents = m_components.Where(c => c.entityID == id);
                if(requiredComponents.All(rqt => entityComponents.Any(ec => ec.GetType() == rqt))   // Entity contains all required components
                    && !exclusions.Any(rqt => entityComponents.Any(ec => ec.GetType() == rqt)))     // And no excluded components
                {
                    apropriateEntities.Add(new Entity {
                        ID = id,
                        Components = entityComponents.ToList()
                    });
                }
            }

            return apropriateEntities;
        }
    }
}
