using System;
namespace Stomper.Engine.Physics
{
    public struct Static : IECSComponent
    {
		public int m_entityID;
		public int entityID {
			get {
				return m_entityID;
			}
			set {
				m_entityID = value;
			}
		}

        public static Static Create()
        {
            return new Static {};
        }
    }
}
