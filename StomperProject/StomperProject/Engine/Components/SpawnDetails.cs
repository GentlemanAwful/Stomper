using System;
using System.Collections.Generic;
using System.Linq;

namespace Stomper.Engine {
    public class SpawnDetails : IECSComponent {
        private int m_entityID;
        public int entityID {
            get {
                return m_entityID;
            }
            set {
                m_entityID = value;
            }
        }
    }
}
