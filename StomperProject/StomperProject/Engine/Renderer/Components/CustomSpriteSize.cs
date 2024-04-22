using System;
namespace Stomper.Engine.Renderer
{
    public struct CustomSpriteSize : IECSComponent
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

        public int Width;
        public int Height;

        /*
        public static CustomSpriteSize Create( int width, int height )
        {
            return new CustomSpriteSize { Width = width, Height = height };
        }
        */
    }
}
