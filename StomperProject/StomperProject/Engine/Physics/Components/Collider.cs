using System;
using Microsoft.Xna.Framework;

namespace Stomper.Engine.Physics
{
    public struct Collider : IECSComponent
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
    }
}
