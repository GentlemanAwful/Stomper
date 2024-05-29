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
        public Type[] RequiredComponents { get; set; } = new Type[] { typeof(InputData), typeof(Keybinds) };
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

        //private Dictionary<Keys, string> KeyboardBindings;
        private Dictionary<GamepadButtons, string> GamepadBindings;

        private KeyboardState   previousKeyboardState;
        private GamePadState[]  previousGamepadState;

        public void Initialize(FNAGame game, Config config) {
            //KeyboardBindings = config.Keybindings;
            GamepadBindings = config.GamepadBindings;
            previousGamepadState = new GamePadState[4];
        }
        public void Dispose() {
            //KeyboardBindings = null;
            GamepadBindings = null;
            previousGamepadState = new GamePadState[4];
            previousKeyboardState = new KeyboardState();
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

            if(gamepadState.Buttons.Start == ButtonState.Pressed) {
                pressed.Add(GamepadButtons.START);
            }
            if(gamepadState.Buttons.Back == ButtonState.Pressed) {
                pressed.Add(GamepadButtons.SELECT);
            }

            return pressed.ToArray();
        }

        public (List<Entity>, List<IGameEvent>) Execute(List<Entity> entities, List<IGameEvent> gameEvents) {
            foreach(Entity entity in entities) {
                List<InputEvent> newInputs = new List<InputEvent>();
                Dictionary<Keys, string> KeyboardBindings = entity.GetComponent<Keybinds>().Keybindings;

                // --------------------------------------------------------
                // Keyboard input
                KeyboardState currentKeyboardState = Keyboard.GetState();

                IEnumerable<Keys> heldKeys      = currentKeyboardState.GetPressedKeys().Intersect(previousKeyboardState.GetPressedKeys());
                IEnumerable<Keys> pressedKeys   = currentKeyboardState.GetPressedKeys().Except(previousKeyboardState.GetPressedKeys());
                IEnumerable<Keys> releasedKeys  = previousKeyboardState.GetPressedKeys().Except(currentKeyboardState.GetPressedKeys());

                // Only check keys with bindings
                heldKeys = heldKeys.Where(key => KeyboardBindings.Keys.Any(binding => key == binding));
                pressedKeys = pressedKeys.Where(key => KeyboardBindings.Keys.Any(binding => key == binding));
                releasedKeys = releasedKeys.Where(key => KeyboardBindings.Keys.Any(binding => key == binding));
                
                newInputs.AddRange(heldKeys.Select(k => KeyboardToInput(KeyboardBindings[k], InputState.HELD)));
                newInputs.AddRange(pressedKeys.Select(k => KeyboardToInput(KeyboardBindings[k], InputState.PRESSED)));
                newInputs.AddRange(releasedKeys.Select(k => KeyboardToInput(KeyboardBindings[k], InputState.RELEASED)));

                previousKeyboardState = currentKeyboardState;

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
                //entityInputData.inputs.AddRange(newInputs);
                entityInputData.inputs = newInputs;
                entity.UpdateComponent(entityInputData);
            }

            return (entities, new List<IGameEvent>());
        }
    }
}
