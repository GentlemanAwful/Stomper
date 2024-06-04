using System;
using Microsoft.Xna.Framework;

namespace Stomper.Engine.Physics
{
    public struct HitboxSquare : IECSComponent
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

        public Vector2 Offset;
        public Vector2 Size;

        public static HitboxSquare Create( Vector2 origin, Vector2 size )
        {
            return new HitboxSquare { Offset = origin, Size = size };
        }
    }
}
