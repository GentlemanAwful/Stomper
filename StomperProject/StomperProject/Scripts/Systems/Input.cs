using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Stomper.Engine;
using Stomper.Scripts.Components;

namespace Stomper.Scripts {
    public class Input : IECSSystem {
        public SystemType Type => SystemType.LOGIC;
        public Type[] Archetype { get; set; } = new Type[] { typeof(InputData), typeof(Keybinds) };
        public Type[] Exclusions => new Type[0];

        public enum InputState {
            UNDEFINED = 0,

            PRESSED,    // Button down this frame
            HELD,       // button held down from before
            RELEASED    // Button released this frame
        }
        public struct InputEvent : IGameEvent {
            public InputState   state;
            public Action       action;
            //public int          playerID;
        }

        // TODO This is game specific, doesn't belong in engine. Put into json.
        /// <summary>
        /// All the actions a player can make
        /// </summary>
        public enum Action {
            UNDEFINED = 0,

            JUMP,
            MOVE_LEFT,
            MOVE_RIGHT,
            ATTACK,

            SPAWN
        }

        public enum GamepadButtons {
            UNDEFINED = 0,

            DPAD_LEFT,
            DPAD_RIGHT,
            DPAD_UP,
            DPAD_DOWN,

            A,
            B,
            X,
            Y,

            START,
            SELECT
        }

        private Dictionary<GamepadButtons, string> GamepadBindings;     // TODO put this into Keybindings component

        private Keys[] previousKeys = new Keys[0]; // TODO infer this from InputData component?
        private GamePadState[]  previousGamepadState; // TODO infer this from InputData component ?

        

        public void Initialize(FNAGame game, Config config) {
            GamepadBindings = config.GamepadBindings;
            previousGamepadState = new GamePadState[4];
        }
        public void Dispose() {
            GamepadBindings = null;
            previousGamepadState = new GamePadState[4];
            //previousKeyboardState = new KeyboardState();
        }

        /// <summary>
        /// Construct an InputEvent from supplied keyboard input data
        /// </summary>
        /// <param name="input">Pressed button</param>
        /// <param name="newState">State of press (pressed, held, released)</param>
        /// <returns>InputEvent with appropriate state and pressed button</returns>
        private static InputEvent KeyboardToInput(string input, InputState newState) {
            return new InputEvent {
                state = newState,
                action = (Action)Enum.Parse(
                            typeof(Action),
                            input
                        )
            };
        }

        private InputEvent GamepadToInput(GamepadButtons button, InputState newState, int playerID) {
            return new InputEvent {
                state = newState,
                action = (Action)Enum.Parse(
                            typeof(Action),
                            GamepadBindings[button]
                        ),
                //playerID = playerID
            };
        }

        private GamepadButtons[] GamepadStateToButtons(GamePadState gamepadState) {
            List<GamepadButtons> pressed = new List<GamepadButtons>();

            // DPad
            if(gamepadState.DPad.Left == ButtonState.Pressed) {
                pressed.Add(GamepadButtons.DPAD_LEFT);
            }
            if(gamepadState.DPad.Right == ButtonState.Pressed) {
                pressed.Add(GamepadButtons.DPAD_RIGHT);
            }
            if(gamepadState.DPad.Up == ButtonState.Pressed) {
                pressed.Add(GamepadButtons.DPAD_UP);
            }
            if(gamepadState.DPad.Down == ButtonState.Pressed) {
                pressed.Add(GamepadButtons.DPAD_DOWN);
            }

            // Face buttons
            if(gamepadState.Buttons.A == ButtonState.Pressed) {
                pressed.Add(GamepadButtons.A);
            }
            if(gamepadState.Buttons.B == ButtonState.Pressed) {
                pressed.Add(GamepadButtons.B);
            }
            if(gamepadState.Buttons.X == ButtonState.Pressed) {
                pressed.Add(GamepadButtons.X);
            }
            if(gamepadState.Buttons.Y == ButtonState.Pressed) {
                pressed.Add(GamepadButtons.Y);
            }

            // Menu buttons
            if(gamepadState.Buttons.Start == ButtonState.Pressed) {
                pressed.Add(GamepadButtons.START);
            }
            if(gamepadState.Buttons.Back == ButtonState.Pressed) {
                pressed.Add(GamepadButtons.SELECT);
            }

            return pressed.ToArray();
        }

        public (Entity[], IGameEvent[]) Execute(Entity[] entities, IGameEvent[] gameEvents) {
            Keys[] currentPressed = Keyboard.GetState().GetPressedKeys();

            foreach(Entity entity in entities) {
                List<InputEvent> newInputs = new List<InputEvent>();
                Dictionary<Keys, string> bindings = entity.GetComponent<Keybinds>().Keybindings;

                // --------------------------------------------------------
                // Keyboard
                IEnumerable<Keys> boundCurrentKeys  = currentPressed.Where(key => bindings.ContainsKey(key));   // Ignore unbound keys
                IEnumerable<Keys> boundPreviousKeys = previousKeys.Where(key => bindings.ContainsKey(key));     // Ignore unbound keys

                IEnumerable<Keys> heldKeys      = boundCurrentKeys.Intersect(boundPreviousKeys);
                IEnumerable<Keys> pressedKeys   = boundCurrentKeys.Except(boundPreviousKeys);
                IEnumerable<Keys> releasedKeys  = boundPreviousKeys.Except(boundCurrentKeys);

                // Transform into input events
                newInputs.AddRange(heldKeys.Select(k => KeyboardToInput(bindings[k], InputState.HELD)));
                newInputs.AddRange(pressedKeys.Select(k => KeyboardToInput(bindings[k], InputState.PRESSED)));
                newInputs.AddRange(releasedKeys.Select(k => KeyboardToInput(bindings[k], InputState.RELEASED)));

                //previousKeyboardState = currentKeyboardState;

                // --------------------------------------------------------
                // Gamepad
                for(PlayerIndex playerIndex = PlayerIndex.One; playerIndex <= PlayerIndex.Four; playerIndex++) {
                    GamePadState state = GamePad.GetState(playerIndex);

                    if(!state.IsConnected)
                        continue;

                    GamePadState gamepadState = GamePad.GetState(playerIndex);

                    GamepadButtons[] currentGamepadButtons  = GamepadStateToButtons(gamepadState);
                    GamepadButtons[] previousGamepadButtons = GamepadStateToButtons(previousGamepadState[(int)playerIndex]);
                    IEnumerable<GamepadButtons> heldGamepadButtons      = currentGamepadButtons.Intersect(previousGamepadButtons);
                    IEnumerable<GamepadButtons> pressedGamepadButtons   = currentGamepadButtons.Except(previousGamepadButtons);
                    IEnumerable<GamepadButtons> releasedGamepadButtons  = previousGamepadButtons.Except(currentGamepadButtons);

                    newInputs.AddRange(heldGamepadButtons.Select(b => GamepadToInput(b, InputState.HELD, (int)playerIndex)));
                    newInputs.AddRange(pressedGamepadButtons.Select(b => GamepadToInput(b, InputState.PRESSED, (int)playerIndex)));
                    newInputs.AddRange(releasedGamepadButtons.Select(b => GamepadToInput(b, InputState.RELEASED, (int)playerIndex)));

                    previousGamepadState[(int)playerIndex] = gamepadState;
                }

                InputData entityInputData = entity.GetComponent<InputData>();
                entityInputData.inputs = newInputs;
                entity.UpdateComponent(entityInputData);
            }

            previousKeys = currentPressed;

            return (entities, new IGameEvent[0]);
        }
    }
}
