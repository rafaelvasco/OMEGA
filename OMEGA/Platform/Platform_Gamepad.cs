using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;
using static SDL2.SDL;

namespace OMEGA
{
    internal static partial class Platform
    {
        private static IntPtr[] gamepad_devices = new IntPtr[GamePad.GAMEPAD_COUNT];
        private static Dictionary<int, int> _gamepad_instance_list = new Dictionary<int, int>();
        private static readonly string[] gamepad_guids = GenStringArray();

        private static readonly string[] gamepad_light_bars = GenStringArray();

        private static GamePadState[] _gamepad_states = new GamePadState[GamePad.GAMEPAD_COUNT];
        private static GamePadCapabilities[] _gamepad_capabilities = new GamePadCapabilities[GamePad.GAMEPAD_COUNT];

        private static readonly GamePadType[] _gamepad_types = new GamePadType[]
        {
            GamePadType.Unknown,
            GamePadType.GamePad,
            GamePadType.Wheel,
            GamePadType.ArcadeStick,
            GamePadType.FlightStick,
            GamePadType.DancePad,
            GamePadType.Guitar,
            GamePadType.DrumKit,
            GamePadType.BigButtonPad
        };


        public static GamePadCapabilities GetGamePadCapabilities(int index)
        {
            if (gamepad_devices[index] == IntPtr.Zero)
            {
                return new GamePadCapabilities();
            }
            return _gamepad_capabilities[index];
        }

        public static GamePadState GetGamePadState(int index, GamePadDeadZone deadZoneMode)
        {
            IntPtr device = gamepad_devices[index];
            if (device == IntPtr.Zero)
            {
                return new GamePadState();
            }

            // The "master" button state is built from this.
            Buttons gc_buttonState = (Buttons)0;

            // Sticks
            Vector2 stickLeft = new Vector2(
                (float)SDL_GameControllerGetAxis(
                    device,
                    SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX
                ) / 32767.0f,
                (float)SDL_GameControllerGetAxis(
                    device,
                    SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY
                ) / -32767.0f
            );
            Vector2 stickRight = new Vector2(
                (float)SDL_GameControllerGetAxis(
                    device,
                    SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX
                ) / 32767.0f,
                (float)SDL_GameControllerGetAxis(
                    device,
                    SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTY
                ) / -32767.0f
            );

            gc_buttonState |= ConvertStickValuesToButtons(
                stickLeft,
                Buttons.LeftThumbstickLeft,
                Buttons.LeftThumbstickRight,
                Buttons.LeftThumbstickUp,
                Buttons.LeftThumbstickDown,
                GamePad.LeftDeadZone
            );
            gc_buttonState |= ConvertStickValuesToButtons(
                stickRight,
                Buttons.RightThumbstickLeft,
                Buttons.RightThumbstickRight,
                Buttons.RightThumbstickUp,
                Buttons.RightThumbstickDown,
                GamePad.RightDeadZone
            );

            // Triggers
            float triggerLeft = (float)SDL_GameControllerGetAxis(
                device,
                SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERLEFT
            ) / 32767.0f;
            float triggerRight = (float)SDL_GameControllerGetAxis(
                device,
                SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERRIGHT
            ) / 32767.0f;
            if (triggerLeft > GamePad.TriggerThreshold)
            {
                gc_buttonState |= Buttons.LeftTrigger;
            }
            if (triggerRight > GamePad.TriggerThreshold)
            {
                gc_buttonState |= Buttons.RightTrigger;
            }

            // Buttons
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A) != 0)
            {
                gc_buttonState |= Buttons.A;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B) != 0)
            {
                gc_buttonState |= Buttons.B;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X) != 0)
            {
                gc_buttonState |= Buttons.X;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y) != 0)
            {
                gc_buttonState |= Buttons.Y;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_BACK) != 0)
            {
                gc_buttonState |= Buttons.Back;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_GUIDE) != 0)
            {
                gc_buttonState |= Buttons.BigButton;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START) != 0)
            {
                gc_buttonState |= Buttons.Start;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSTICK) != 0)
            {
                gc_buttonState |= Buttons.LeftStick;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSTICK) != 0)
            {
                gc_buttonState |= Buttons.RightStick;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER) != 0)
            {
                gc_buttonState |= Buttons.LeftShoulder;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER) != 0)
            {
                gc_buttonState |= Buttons.RightShoulder;
            }

            // DPad
            ButtonState dpadUp = ButtonState.Released;
            ButtonState dpadDown = ButtonState.Released;
            ButtonState dpadLeft = ButtonState.Released;
            ButtonState dpadRight = ButtonState.Released;
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_UP) != 0)
            {
                gc_buttonState |= Buttons.DPadUp;
                dpadUp = ButtonState.Pressed;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_DOWN) != 0)
            {
                gc_buttonState |= Buttons.DPadDown;
                dpadDown = ButtonState.Pressed;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_LEFT) != 0)
            {
                gc_buttonState |= Buttons.DPadLeft;
                dpadLeft = ButtonState.Pressed;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_RIGHT) != 0)
            {
                gc_buttonState |= Buttons.DPadRight;
                dpadRight = ButtonState.Pressed;
            }

            // Build the GamePadState, increment PacketNumber if state changed.

            GamePadState gc_builtState = new GamePadState(
                new GamePadThumbSticks(stickLeft, stickRight, deadZoneMode),
                new GamePadTriggers(triggerLeft, triggerRight, deadZoneMode),
                new GamePadButtons(gc_buttonState),
                new GamePadDPad(dpadUp, dpadDown, dpadLeft, dpadRight)
            );
            gc_builtState.IsConnected = true;
            gc_builtState.PacketNumber = _gamepad_states[index].PacketNumber;
            if (gc_builtState != _gamepad_states[index])
            {
                gc_builtState.PacketNumber += 1;
                _gamepad_states[index] = gc_builtState;
            }

            return gc_builtState;
        }

        public static bool SetGamePadVibration(int index, float leftMotor, float rightMotor)
        {
            IntPtr device = gamepad_devices[index];
            if (device == IntPtr.Zero)
            {
                return false;
            }

            return SDL_GameControllerRumble(
                device,
                (ushort)(Calc.Clamp(leftMotor, 0.0f, 1.0f) * 0xFFFF),
                (ushort)(Calc.Clamp(rightMotor, 0.0f, 1.0f) * 0xFFFF),
                0
            ) == 0;
        }

        public static string GetGamePadGUID(int index)
        {
            return gamepad_guids[index];
        }

        public static void SetGamePadLightBar(int index, Color color)
        {
            if (String.IsNullOrEmpty(gamepad_light_bars[index]))
            {
                return;
            }

            string baseDir = gamepad_light_bars[index];
            try
            {
                File.WriteAllText(baseDir + "red/brightness", color.R.ToString());
                File.WriteAllText(baseDir + "green/brightness", color.G.ToString());
                File.WriteAllText(baseDir + "blue/brightness", color.B.ToString());
            }
            catch
            {
                // If something went wrong, assume the worst and just remove it.
                gamepad_light_bars[index] = String.Empty;
            }
        }

        public static void SetGamePadMappingsFile(string file_content)
        {
            SDL_GameControllerAddMapping(file_content);
        }

        public static void PreLookForGamepads()
        {
            var evt = new SDL_Event[1];
            SDL_PumpEvents();
            while (SDL_PeepEvents(
                evt,
                1,
                SDL_eventaction.SDL_GETEVENT,
                SDL_EventType.SDL_CONTROLLERDEVICEADDED,
                SDL_EventType.SDL_CONTROLLERDEVICEADDED
            ) == 1)
            {
                AddGamePadInstance(evt[0].cdevice.which);
            }
        }

        private static void AddGamePadInstance(int dev)
        {
            Console.WriteLine("GamePad Added");
            int which = -1;
            for (int i = 0; i < gamepad_devices.Length; i += 1)
            {
                if (gamepad_devices[i] == IntPtr.Zero)
                {
                    which = i;
                    break;
                }
            }
            if (which == -1)
            {
                return; // Ignoring more than 4 controllers.
            }

            // Clear the error buffer. We're about to do a LOT of dangerous stuff.
            SDL_ClearError();

            // Open the device!
            gamepad_devices[which] = SDL_GameControllerOpen(dev);

            // We use this when dealing with GUID initialization.
            IntPtr thisJoystick = SDL_GameControllerGetJoystick(gamepad_devices[which]);

            int thisInstance = SDL_JoystickInstanceID(thisJoystick);

            _gamepad_instance_list.Add(thisInstance, which);

            // Start with a fresh state.
            _gamepad_states[which] = new GamePadState();
            _gamepad_states[which].IsConnected = true;

            // Initialize the haptics for the joystick, if applicable.
            bool hasRumble = SDL_GameControllerRumble(
                gamepad_devices[which],
                0,
                0,
                0
            ) == 0;

            GamePadCapabilities caps = new GamePadCapabilities();

            caps.IsConnected = true;
            caps.GamePadType = _gamepad_types[(int)SDL_JoystickGetType(thisJoystick)];
            caps.HasAButton = SDL_GameControllerGetBindForButton(
                gamepad_devices[which],
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasBButton = SDL_GameControllerGetBindForButton(
                gamepad_devices[which],
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasXButton = SDL_GameControllerGetBindForButton(
                gamepad_devices[which],
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasYButton = SDL_GameControllerGetBindForButton(
                gamepad_devices[which],
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasBackButton = SDL_GameControllerGetBindForButton(
                gamepad_devices[which],
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_BACK
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasBigButton = SDL_GameControllerGetBindForButton(
                gamepad_devices[which],
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_GUIDE
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasStartButton = SDL_GameControllerGetBindForButton(
                gamepad_devices[which],
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasLeftStickButton = SDL_GameControllerGetBindForButton(
                gamepad_devices[which],
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSTICK
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasRightStickButton = SDL_GameControllerGetBindForButton(
                gamepad_devices[which],
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSTICK
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasLeftShoulderButton = SDL_GameControllerGetBindForButton(
                gamepad_devices[which],
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasRightShoulderButton = SDL_GameControllerGetBindForButton(
                gamepad_devices[which],
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasDPadUpButton = SDL_GameControllerGetBindForButton(
                gamepad_devices[which],
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_UP
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasDPadDownButton = SDL_GameControllerGetBindForButton(
                gamepad_devices[which],
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_DOWN
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasDPadLeftButton = SDL_GameControllerGetBindForButton(
                gamepad_devices[which],
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_LEFT
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasDPadRightButton = SDL_GameControllerGetBindForButton(
                gamepad_devices[which],
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_RIGHT
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasLeftXThumbStick = SDL_GameControllerGetBindForAxis(
                gamepad_devices[which],
                SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasLeftYThumbStick = SDL_GameControllerGetBindForAxis(
                gamepad_devices[which],
                SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasRightXThumbStick = SDL_GameControllerGetBindForAxis(
                gamepad_devices[which],
                SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasRightYThumbStick = SDL_GameControllerGetBindForAxis(
                gamepad_devices[which],
                SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTY
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasLeftTrigger = SDL_GameControllerGetBindForAxis(
                gamepad_devices[which],
                SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERLEFT
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasRightTrigger = SDL_GameControllerGetBindForAxis(
                gamepad_devices[which],
                SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERRIGHT
            ).bindType != SDL_GameControllerBindType.SDL_CONTROLLER_BINDTYPE_NONE;
            caps.HasLeftVibrationMotor = hasRumble;
            caps.HasRightVibrationMotor = hasRumble;
            caps.HasVoiceSupport = false;
            _gamepad_capabilities[which] = caps;

            /* Store the GUID string for this device
			 * FIXME: Replace GetGUIDEXT string with 3 short values -flibit
			 */
            ushort vendor = SDL_JoystickGetVendor(thisJoystick);
            ushort product = SDL_JoystickGetProduct(thisJoystick);
            if (vendor == 0x00 && product == 0x00)
            {
                gamepad_guids[which] = "xinput";
            }
            else
            {
                gamepad_guids[which] = string.Format(
                    "{0:x2}{1:x2}{2:x2}{3:x2}",
                    vendor & 0xFF,
                    vendor >> 8,
                    product & 0xFF,
                    product >> 8
                );
            }

            // Initialize light bar
            if (RunningPlatform == RunningPlatform.Linux &&
                (gamepad_guids[which].Equals("4c05c405") ||
                    gamepad_guids[which].Equals("4c05cc09")))
            {
                // Get all of the individual PS4 LED instances
                List<string> ledList = new List<string>();
                string[] dirs = Directory.GetDirectories("/sys/class/leds/");
                foreach (string dir in dirs)
                {
                    if (dir.EndsWith("blue") &&
                        (dir.Contains("054C:05C4") ||
                            dir.Contains("054C:09CC")))
                    {
                        ledList.Add(dir.Substring(0, dir.LastIndexOf(':') + 1));
                    }
                }
                // Find how many of these are already in use
                int numLights = 0;
                for (int i = 0; i < gamepad_light_bars.Length; i += 1)
                {
                    if (!String.IsNullOrEmpty(gamepad_light_bars[i]))
                    {
                        numLights += 1;
                    }
                }
                // If all are not already in use, use the first unused light
                if (numLights < ledList.Count)
                {
                    gamepad_light_bars[which] = ledList[numLights];
                }
            }

        }

        private static void RemoveGamePadInstance(int dev)
        {
            Console.WriteLine("GamePad Removed");
            int output;
            if (!_gamepad_instance_list.TryGetValue(dev, out output))
            {
                // Odds are, this is controller 5+ getting removed.
                return;
            }
            _gamepad_instance_list.Remove(dev);
            SDL_GameControllerClose(gamepad_devices[output]);
            gamepad_devices[output] = IntPtr.Zero;
            _gamepad_states[output] = new GamePadState();
            gamepad_guids[output] = String.Empty;

            SDL_ClearError();

        }

        private static Buttons ConvertStickValuesToButtons(Vector2 stick, Buttons left, Buttons right, Buttons up, Buttons down, float DeadZoneSize)
        {
            Buttons b = (Buttons)0;

            if (stick.X > DeadZoneSize)
            {
                b |= right;
            }
            if (stick.X < -DeadZoneSize)
            {
                b |= left;
            }
            if (stick.Y > DeadZoneSize)
            {
                b |= up;
            }
            if (stick.Y < -DeadZoneSize)
            {
                b |= down;
            }

            return b;
        }

        private static string[] GenStringArray()
        {
            string[] result = new string[GamePad.GAMEPAD_COUNT];
            for (int i = 0; i < result.Length; i += 1)
            {
                result[i] = String.Empty;
            }
            return result;
        }

        private static void InitGamePad()
        {
            string hint = SDL_GetHint(SDL_HINT_JOYSTICK_ALLOW_BACKGROUND_EVENTS);
            if (String.IsNullOrEmpty(hint))
            {
                SDL_SetHint(
                    SDL_HINT_JOYSTICK_ALLOW_BACKGROUND_EVENTS,
                    "1"
                );
            }

            SDL_SetHintWithPriority(
                SDL_HINT_GAMECONTROLLER_USE_BUTTON_LABELS,
                "0",
                SDL_HintPriority.SDL_HINT_OVERRIDE
            );
        }

        private static void ProcessGamePadEvent(SDL_Event evt)
        {
            if (evt.type == SDL_EventType.SDL_CONTROLLERDEVICEADDED)
            {
                AddGamePadInstance(evt.cdevice.which);
            }
            else if (evt.type == SDL_EventType.SDL_CONTROLLERDEVICEREMOVED)
            {
                RemoveGamePadInstance(evt.cdevice.which);
            }
        }
    }
}
