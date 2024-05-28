using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Stomper.Engine;

namespace Stomper.Scripts.Components {
    public struct InputData : IECSComponent {
        private int m_entityID;
        public int entityID {
            get {
                return m_entityID;
            }
            set {
                m_entityID = value;
            }
        }

        public List<Input.InputEvent> inputs;
    }
}
