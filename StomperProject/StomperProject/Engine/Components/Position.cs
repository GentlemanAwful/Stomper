using Microsoft.Xna.Framework;

namespace Stomper.Engine
{
	public struct Position : IECSComponent
	{
		public Vector2 position;
		private int m_entityID;
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
