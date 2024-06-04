using Stomper.Engine;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Stomper.Scripts.Components {
    public struct StompData : IECSComponent {
        private int m_entityID;
        public int entityID {
            get {
                return m_entityID;
            }
            set {
                m_entityID = value;
            }
        }

        // public bool active { get; }

        public bool stomping;
        public float stompVelocity;
    }
}
