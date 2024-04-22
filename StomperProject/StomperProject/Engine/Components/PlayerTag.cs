namespace Stomper.Engine {
    public struct PlayerTag : IECSComponent {
        private int m_entityID;
        public int entityID {
            get {
                return m_entityID;
            }
            set {
                m_entityID = value;
            }
        }

        public int Value;
    }
}
