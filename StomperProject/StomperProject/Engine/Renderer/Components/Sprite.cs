using Microsoft.Xna.Framework.Graphics;

namespace Stomper.Engine.Renderer
{
    public struct Sprite : IECSComponent
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

        public string          TexturePath;
        public Texture2D       Texture;
    }
}
