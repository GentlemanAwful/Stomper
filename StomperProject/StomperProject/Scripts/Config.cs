using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Stomper.Scripts {
    public struct Config {
        public int RandomSeed;
        public Color DefaultClearColour;
        public float Gravity;
        public float JumpAcceleration;
        public float WalkSpeed;
		public int 	 DebugLineWidth;
        public Dictionary<Keys, string> Keybindings;
        public Dictionary<Input.GamepadButtons, string> GamepadBindings;
        public Stomper.Engine.Entity PlayerTemplate;
    }
}
