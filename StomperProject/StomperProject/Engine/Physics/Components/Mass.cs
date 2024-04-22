using System;
using Microsoft.Xna.Framework;

namespace Stomper.Engine.Physics
{
    public struct Mass : IECSComponent
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

        public float Amount;
        public Vector2 Velocity;

        public static Mass Create( float amount )
        {
            return new Mass { Amount = amount, Velocity = Vector2.Zero };
        }
    }
}
