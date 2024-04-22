using Stomper.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StomperProject.Scripts.Components {
    public struct PlayerNumber : IECSComponent {
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
