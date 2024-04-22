using System;
namespace Stomper.Engine.Renderer
{
    public struct SpriteTiling : IECSComponent
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

        public int HorizontalTiling;
        public int VerticalTiling;
    }
}
