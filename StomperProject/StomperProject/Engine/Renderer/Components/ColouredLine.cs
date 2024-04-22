using System;
using Microsoft.Xna.Framework;

namespace Stomper.Engine.Renderer
{
    public struct ColouredLine : IECSComponent
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

        public Rectangle rect;
        public Color colour;
    }
}
