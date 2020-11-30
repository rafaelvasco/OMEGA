using System;
using System.Collections.Generic;
using System.IO;
using static SDL2.SDL;

namespace OMEGA
{
    internal static partial class Platform
    {
        private static IntPtr[] gamepad_devices = new IntPtr[GamePad.GAMEPAD_MAX_COUNT];
        private static readonly Dictionary<int, int> gamepad_instances = new Dictionary<int, int>();
        private static readonly string[] gamepad_guids = GenStringArray();

        private static readonly string[] gamepad_light_bars = GenStringArray();

        private static readonly GamePadState[] gamepad_states = new GamePadState[GamePad.GAMEPAD_MAX_COUNT];
        private static readonly GamePadCapabilities[] gamepad_caps = new GamePadCapabilities[GamePad.GAMEPAD_MAX_COUNT];

        private static readonly GamePadType[] gamepad_types = new GamePadType[]
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
            return gamepad_caps[index];
        }

        public static GamePadState GetGamePadState(int index, GamePadDeadZone deadZoneMode)
        {
            IntPtr device = gamepad_devices[index];
            if (device == IntPtr.Zero)
            {
                return new GamePadState();
            }

            GamePadButtons gc_buttonState = 0;

            // Sticks
            Vec2 stickLeft = new Vec2(
                SDL_GameControllerGetAxis(
                    device,
                    SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX
                ) / 32767.0f,
                SDL_GameControllerGetAxis(
                    device,
                    SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY
                ) / -32767.0f
            );
            Vec2 stickRight = new Vec2(
                SDL_GameControllerGetAxis(
                    device,
                    SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX
                ) / 32767.0f,
                SDL_GameControllerGetAxis(
                    device,
                    SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTY
                ) / -32767.0f
            );

            gc_buttonState |= ConvertStickValuesToButtons(
                stickLeft,
                GamePadButtons.LeftThumbstickLeft,
                GamePadButtons.LeftThumbstickRight,
                GamePadButtons.LeftThumbstickUp,
                GamePadButtons.LeftThumbstickDown,
                GamePad.LeftDeadZone
            );
            gc_buttonState |= ConvertStickValuesToButtons(
                stickRight,
                GamePadButtons.RightThumbstickLeft,
                GamePadButtons.RightThumbstickRight,
                GamePadButtons.RightThumbstickUp,
                GamePadButtons.RightThumbstickDown,
                GamePad.RightDeadZone
            );

            // Triggers
            float triggerLeft = SDL_GameControllerGetAxis(
                device,
                SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERLEFT
            ) / 32767.0f;
            float triggerRight = (float)SDL_GameControllerGetAxis(
                device,
                SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_TRIGGERRIGHT
            ) / 32767.0f;
            if (triggerLeft > GamePad.TriggerThreshold)
            {
                gc_buttonState |= GamePadButtons.LeftTrigger;
            }
            if (triggerRight > GamePad.TriggerThreshold)
            {
                gc_buttonState |= GamePadButtons.RightTrigger;
            }

            // Buttons
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A) != 0)
            {
                gc_buttonState |= GamePadButtons.A;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B) != 0)
            {
                gc_buttonState |= GamePadButtons.B;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X) != 0)
            {
                gc_buttonState |= GamePadButtons.X;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y) != 0)
            {
                gc_buttonState |= GamePadButtons.Y;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_BACK) != 0)
            {
                gc_buttonState |= GamePadButtons.Back;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_GUIDE) != 0)
            {
                gc_buttonState |= GamePadButtons.BigButton;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START) != 0)
            {
                gc_buttonState |= GamePadButtons.Start;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSTICK) != 0)
            {
                gc_buttonState |= GamePadButtons.LeftStick;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSTICK) != 0)
            {
                gc_buttonState |= GamePadButtons.RightStick;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER) != 0)
            {
                gc_buttonState |= GamePadButtons.LeftShoulder;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER) != 0)
            {
                gc_buttonState |= GamePadButtons.RightShoulder;
            }

            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_UP) != 0)
            {
                gc_buttonState |= GamePadButtons.DPadUp;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_DOWN) != 0)
            {
                gc_buttonState |= GamePadButtons.DPadDown;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_LEFT) != 0)
            {
                gc_buttonState |= GamePadButtons.DPadLeft;
            }
            if (SDL_GameControllerGetButton(device, SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_RIGHT) != 0)
            {
                gc_buttonState |= GamePadButtons.DPadRight;
            }

            // Build the GamePadState, increment PacketNumber if state changed.

            GamePadState gc_builtState = new GamePadState(
                new GamePadThumbSticks(stickLeft, stickRight, deadZoneMode),
                new GamePadTriggers(triggerLeft, triggerRight, deadZoneMode),
                gc_buttonState
            );
            gc_builtState.IsConnected = true;
            gc_builtState.PacketNumber = gamepad_states[index].PacketNumber;
            if (gc_builtState != gamepad_states[index])
            {
                gc_builtState.PacketNumber += 1;
                gamepad_states[index] = gc_builtState;
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
            if (string.IsNullOrEmpty(gamepad_light_bars[index]))
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
                gamepad_light_bars[index] = string.Empty;
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

        private static void AddGamePadInstance(int device_id)
        {
            if (GamePad.ConnectedGamePads == GamePad.GAMEPAD_MAX_COUNT)
            {
                return;
            }

            Console.WriteLine("GamePad Added");

            // Clear the error buffer. We're about to do a LOT of dangerous stuff.
            SDL_ClearError();

            int which = GamePad.ConnectedGamePads++;

            // Open the device!
            gamepad_devices[which] = SDL_GameControllerOpen(device_id);

            // We use this when dealing with GUID initialization.
            IntPtr thisJoystick = SDL_GameControllerGetJoystick(gamepad_devices[which]);

            int thisInstance = SDL_JoystickInstanceID(thisJoystick);

            gamepad_instances.Add(thisInstance, which);

            // Start with a fresh state.
            gamepad_states[which] = new GamePadState();
            gamepad_states[which].IsConnected = true;

            // Initialize the haptics for the joystick, if applicable.
            bool hasRumble = SDL_GameControllerRumble(
                gamepad_devices[which],
                0,
                0,
                0
            ) == 0;

            GamePadCapabilities caps = new GamePadCapabilities();

            caps.IsConnected = true;
            caps.GamePadType = gamepad_types[(int)SDL_JoystickGetType(thisJoystick)];
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
            gamepad_caps[which] = caps;

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
                    if (!string.IsNullOrEmpty(gamepad_light_bars[i]))
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
            if (!gamepad_instances.TryGetValue(dev, out int output))
            {
                return;
            }
            gamepad_instances.Remove(dev);
            SDL_GameControllerClose(gamepad_devices[output]);
            gamepad_devices[output] = IntPtr.Zero;
            gamepad_states[output] = new GamePadState();
            gamepad_guids[output] = string.Empty;

            SDL_ClearError();

            GamePad.ConnectedGamePads --;

        }

        private static GamePadButtons ConvertStickValuesToButtons(Vec2 stick, GamePadButtons left, GamePadButtons right, GamePadButtons up, GamePadButtons down, float DeadZoneSize)
        {
            GamePadButtons b = 0;

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
            string[] result = new string[GamePad.GAMEPAD_MAX_COUNT];
            for (int i = 0; i < result.Length; i += 1)
            {
                result[i] = string.Empty;
            }
            return result;
        }

        private static void InitGamePad()
        {
            string hint = SDL_GetHint(SDL_HINT_JOYSTICK_ALLOW_BACKGROUND_EVENTS);
            if (string.IsNullOrEmpty(hint))
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
