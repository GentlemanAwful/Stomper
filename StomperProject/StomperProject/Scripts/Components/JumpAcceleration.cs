using Stomper.Engine;

namespace Stomper.Scripts.Components {
    public struct JumpAcceleration : IECSComponent {
        private int m_entityID;
        public int entityID {
            get {
                return m_entityID;
            }
            set {
                m_entityID = value;
            }
        }

        public float Value;
    }
}
