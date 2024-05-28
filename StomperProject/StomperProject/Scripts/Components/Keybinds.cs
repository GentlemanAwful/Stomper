using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

using Stomper.Engine;

namespace Stomper.Scripts.Components {
    public struct Keybinds : IECSComponent {
        private int m_entityID;
        public int entityID {
            get {
                return m_entityID;
            }
            set {
                m_entityID = value;
            }
        }

        public Dictionary<Keys, string> Keybindings;
    }
}
