﻿using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

using Stomper.Engine;

namespace Stomper.Scripts.Components {
    public struct InputData : IECSComponent {
        private int m_entityID;
        public int entityID {
            get {
                return m_entityID;
            }
            set {
                m_entityID = value;
            }
        }

        //public Keys[] pressedKeys;
        public List<Input.InputEvent> inputs;
    }
}
