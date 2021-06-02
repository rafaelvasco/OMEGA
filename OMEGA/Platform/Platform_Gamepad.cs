using System;
using System.Collections.Generic;
using System.IO;
using static SDL2.Sdl;

namespace OMEGA
{
    internal static partial class Platform
    {
        private static IntPtr[] _gamepadDevices = new IntPtr[GamePad.GamepadMaxCount];
        private static readonly Dictionary<int, int> GamepadInstances = new Dictionary<int, int>();
        private static readonly string[] GamepadGuids = GenStringArray();

        private static readonly string[] GamepadLightBars = GenStringArray();

        private static readonly GamePadState[] GamepadStates = new GamePadState[GamePad.GamepadMaxCount];
        private static readonly GamePadCapabilities[] GamepadCaps = new GamePadCapabilities[GamePad.GamepadMaxCount];

        private static readonly GamePadType[] GamepadTypes = new GamePadType[]
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
            if (_gamepadDevices[index] == IntPtr.Zero)
            {
                return new GamePadCapabilities();
            }
            return GamepadCaps[index];
        }

        public static GamePadState GetGamePadState(int index, GamePadDeadZone deadZoneMode)
        {
            IntPtr device = _gamepadDevices[index];
            if (device == IntPtr.Zero)
            {
                return new GamePadState();
            }

            GamePadButtons gc_buttonState = 0;

            // Sticks
            Vec2 stickLeft = new Vec2(
                SDL_GameControllerGetAxis(
                    device,
                    SdlGameControllerAxis.SdlControllerAxisLeftx
                ) / 32767.0f,
                SDL_GameControllerGetAxis(
                    device,
                    SdlGameControllerAxis.SdlControllerAxisLefty
                ) / -32767.0f
            );
            Vec2 stickRight = new Vec2(
                SDL_GameControllerGetAxis(
                    device,
                    SdlGameControllerAxis.SdlControllerAxisRightx
                ) / 32767.0f,
                SDL_GameControllerGetAxis(
                    device,
                    SdlGameControllerAxis.SdlControllerAxisRighty
                ) / -32767.0f
            );

            gc_buttonState |= ConvertStickValuesToButtons(
                stickLeft,
                GamePadButtons.LeftThumbstickLeft,
                GamePadButtons.LeftThumbstickRight,
                GamePadButtons.LeftThumbstickUp,
                GamePadButtons.LeftThumbstickDown,
                GamePad.LEFT_DEAD_ZONE
            );
            gc_buttonState |= ConvertStickValuesToButtons(
                stickRight,
                GamePadButtons.RightThumbstickLeft,
                GamePadButtons.RightThumbstickRight,
                GamePadButtons.RightThumbstickUp,
                GamePadButtons.RightThumbstickDown,
                GamePad.RIGHT_DEAD_ZONE
            );

            // Triggers
            float triggerLeft = SDL_GameControllerGetAxis(
                device,
                SdlGameControllerAxis.SdlControllerAxisTriggerleft
            ) / 32767.0f;
            float triggerRight = (float)SDL_GameControllerGetAxis(
                device,
                SdlGameControllerAxis.SdlControllerAxisTriggerright
            ) / 32767.0f;
            if (triggerLeft > GamePad.TRIGGER_THRESHOLD)
            {
                gc_buttonState |= GamePadButtons.LeftTrigger;
            }
            if (triggerRight > GamePad.TRIGGER_THRESHOLD)
            {
                gc_buttonState |= GamePadButtons.RightTrigger;
            }

            // Buttons
            if (SDL_GameControllerGetButton(device, SdlGameControllerButton.SdlControllerButtonA) != 0)
            {
                gc_buttonState |= GamePadButtons.A;
            }
            if (SDL_GameControllerGetButton(device, SdlGameControllerButton.SdlControllerButtonB) != 0)
            {
                gc_buttonState |= GamePadButtons.B;
            }
            if (SDL_GameControllerGetButton(device, SdlGameControllerButton.SdlControllerButtonX) != 0)
            {
                gc_buttonState |= GamePadButtons.X;
            }
            if (SDL_GameControllerGetButton(device, SdlGameControllerButton.SdlControllerButtonY) != 0)
            {
                gc_buttonState |= GamePadButtons.Y;
            }
            if (SDL_GameControllerGetButton(device, SdlGameControllerButton.SdlControllerButtonBack) != 0)
            {
                gc_buttonState |= GamePadButtons.Back;
            }
            if (SDL_GameControllerGetButton(device, SdlGameControllerButton.SdlControllerButtonGuide) != 0)
            {
                gc_buttonState |= GamePadButtons.BigButton;
            }
            if (SDL_GameControllerGetButton(device, SdlGameControllerButton.SdlControllerButtonStart) != 0)
            {
                gc_buttonState |= GamePadButtons.Start;
            }
            if (SDL_GameControllerGetButton(device, SdlGameControllerButton.SdlControllerButtonLeftstick) != 0)
            {
                gc_buttonState |= GamePadButtons.LeftStick;
            }
            if (SDL_GameControllerGetButton(device, SdlGameControllerButton.SdlControllerButtonRightstick) != 0)
            {
                gc_buttonState |= GamePadButtons.RightStick;
            }
            if (SDL_GameControllerGetButton(device, SdlGameControllerButton.SdlControllerButtonLeftshoulder) != 0)
            {
                gc_buttonState |= GamePadButtons.LeftShoulder;
            }
            if (SDL_GameControllerGetButton(device, SdlGameControllerButton.SdlControllerButtonRightshoulder) != 0)
            {
                gc_buttonState |= GamePadButtons.RightShoulder;
            }

            if (SDL_GameControllerGetButton(device, SdlGameControllerButton.SdlControllerButtonDpadUp) != 0)
            {
                gc_buttonState |= GamePadButtons.DPadUp;
            }
            if (SDL_GameControllerGetButton(device, SdlGameControllerButton.SdlControllerButtonDpadDown) != 0)
            {
                gc_buttonState |= GamePadButtons.DPadDown;
            }
            if (SDL_GameControllerGetButton(device, SdlGameControllerButton.SdlControllerButtonDpadLeft) != 0)
            {
                gc_buttonState |= GamePadButtons.DPadLeft;
            }
            if (SDL_GameControllerGetButton(device, SdlGameControllerButton.SdlControllerButtonDpadRight) != 0)
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
            gc_builtState.PacketNumber = GamepadStates[index].PacketNumber;
            if (gc_builtState != GamepadStates[index])
            {
                gc_builtState.PacketNumber += 1;
                GamepadStates[index] = gc_builtState;
            }

            return gc_builtState;
        }

        public static bool SetGamePadVibration(int index, float leftMotor, float rightMotor)
        {
            IntPtr device = _gamepadDevices[index];
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

        public static string GetGamePadGuid(int index)
        {
            return GamepadGuids[index];
        }

        public static void SetGamePadLightBar(int index, Color color)
        {
            if (string.IsNullOrEmpty(GamepadLightBars[index]))
            {
                return;
            }

            string baseDir = GamepadLightBars[index];
            try
            {
                File.WriteAllText(baseDir + "red/brightness", color.R.ToString());
                File.WriteAllText(baseDir + "green/brightness", color.G.ToString());
                File.WriteAllText(baseDir + "blue/brightness", color.B.ToString());
            }
            catch
            {
                // If something went wrong, assume the worst and just remove it.
                GamepadLightBars[index] = string.Empty;
            }
        }

        public static void SetGamePadMappingsFile(string fileContent)
        {
            SDL_GameControllerAddMapping(fileContent);
        }

        public static void PreLookForGamepads()
        {
            var evt = new SdlEvent[1];
            SDL_PumpEvents();
            while (SDL_PeepEvents(
                evt,
                1,
                SdlEventaction.SdlGetevent,
                SdlEventType.SdlControllerdeviceadded,
                SdlEventType.SdlControllerdeviceadded
            ) == 1)
            {
                AddGamePadInstance(evt[0].cdevice.which);
            }
        }

        private static void AddGamePadInstance(int deviceId)
        {
            if (GamePad.ConnectedGamePads == GamePad.GamepadMaxCount)
            {
                return;
            }

            Console.WriteLine("GamePad Added");

            // Clear the error buffer. We're about to do a LOT of dangerous stuff.
            SDL_ClearError();

            int which = GamePad.ConnectedGamePads++;

            // Open the device!
            _gamepadDevices[which] = SDL_GameControllerOpen(deviceId);

            // We use this when dealing with GUID initialization.
            IntPtr thisJoystick = SDL_GameControllerGetJoystick(_gamepadDevices[which]);

            int thisInstance = SDL_JoystickInstanceID(thisJoystick);

            GamepadInstances.Add(thisInstance, which);

            // Start with a fresh state.
            GamepadStates[which] = new GamePadState();
            GamepadStates[which].IsConnected = true;

            // Initialize the haptics for the joystick, if applicable.
            bool hasRumble = SDL_GameControllerRumble(
                _gamepadDevices[which],
                0,
                0,
                0
            ) == 0;

            GamePadCapabilities caps = new GamePadCapabilities();

            caps.IsConnected = true;
            caps.GamePadType = GamepadTypes[(int)SDL_JoystickGetType(thisJoystick)];
            caps.HasAButton = SDL_GameControllerGetBindForButton(
                _gamepadDevices[which],
                SdlGameControllerButton.SdlControllerButtonA
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasBButton = SDL_GameControllerGetBindForButton(
                _gamepadDevices[which],
                SdlGameControllerButton.SdlControllerButtonB
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasXButton = SDL_GameControllerGetBindForButton(
                _gamepadDevices[which],
                SdlGameControllerButton.SdlControllerButtonX
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasYButton = SDL_GameControllerGetBindForButton(
                _gamepadDevices[which],
                SdlGameControllerButton.SdlControllerButtonY
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasBackButton = SDL_GameControllerGetBindForButton(
                _gamepadDevices[which],
                SdlGameControllerButton.SdlControllerButtonBack
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasBigButton = SDL_GameControllerGetBindForButton(
                _gamepadDevices[which],
                SdlGameControllerButton.SdlControllerButtonGuide
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasStartButton = SDL_GameControllerGetBindForButton(
                _gamepadDevices[which],
                SdlGameControllerButton.SdlControllerButtonStart
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasLeftStickButton = SDL_GameControllerGetBindForButton(
                _gamepadDevices[which],
                SdlGameControllerButton.SdlControllerButtonLeftstick
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasRightStickButton = SDL_GameControllerGetBindForButton(
                _gamepadDevices[which],
                SdlGameControllerButton.SdlControllerButtonRightstick
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasLeftShoulderButton = SDL_GameControllerGetBindForButton(
                _gamepadDevices[which],
                SdlGameControllerButton.SdlControllerButtonLeftshoulder
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasRightShoulderButton = SDL_GameControllerGetBindForButton(
                _gamepadDevices[which],
                SdlGameControllerButton.SdlControllerButtonRightshoulder
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasDPadUpButton = SDL_GameControllerGetBindForButton(
                _gamepadDevices[which],
                SdlGameControllerButton.SdlControllerButtonDpadUp
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasDPadDownButton = SDL_GameControllerGetBindForButton(
                _gamepadDevices[which],
                SdlGameControllerButton.SdlControllerButtonDpadDown
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasDPadLeftButton = SDL_GameControllerGetBindForButton(
                _gamepadDevices[which],
                SdlGameControllerButton.SdlControllerButtonDpadLeft
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasDPadRightButton = SDL_GameControllerGetBindForButton(
                _gamepadDevices[which],
                SdlGameControllerButton.SdlControllerButtonDpadRight
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasLeftXThumbStick = SDL_GameControllerGetBindForAxis(
                _gamepadDevices[which],
                SdlGameControllerAxis.SdlControllerAxisLeftx
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasLeftYThumbStick = SDL_GameControllerGetBindForAxis(
                _gamepadDevices[which],
                SdlGameControllerAxis.SdlControllerAxisLefty
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasRightXThumbStick = SDL_GameControllerGetBindForAxis(
                _gamepadDevices[which],
                SdlGameControllerAxis.SdlControllerAxisRightx
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasRightYThumbStick = SDL_GameControllerGetBindForAxis(
                _gamepadDevices[which],
                SdlGameControllerAxis.SdlControllerAxisRighty
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasLeftTrigger = SDL_GameControllerGetBindForAxis(
                _gamepadDevices[which],
                SdlGameControllerAxis.SdlControllerAxisTriggerleft
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasRightTrigger = SDL_GameControllerGetBindForAxis(
                _gamepadDevices[which],
                SdlGameControllerAxis.SdlControllerAxisTriggerright
            ).bindType != SdlGameControllerBindType.SdlControllerBindtypeNone;

            caps.HasLeftVibrationMotor = hasRumble;
            caps.HasRightVibrationMotor = hasRumble;

            caps.HasVoiceSupport = false;
            GamepadCaps[which] = caps;

            ushort vendor = SDL_JoystickGetVendor(thisJoystick);
            ushort product = SDL_JoystickGetProduct(thisJoystick);
            if (vendor == 0x00 && product == 0x00)
            {
                GamepadGuids[which] = "xinput";
            }
            else
            {
                GamepadGuids[which] = string.Format(
                    "{0:x2}{1:x2}{2:x2}{3:x2}",
                    vendor & 0xFF,
                    vendor >> 8,
                    product & 0xFF,
                    product >> 8
                );
            }

            // Initialize light bar
            if (RunningPlatform == RunningPlatform.Linux &&
                (GamepadGuids[which].Equals("4c05c405") ||
                    GamepadGuids[which].Equals("4c05cc09")))
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
                for (int i = 0; i < GamepadLightBars.Length; i += 1)
                {
                    if (!string.IsNullOrEmpty(GamepadLightBars[i]))
                    {
                        numLights += 1;
                    }
                }
                // If all are not already in use, use the first unused light
                if (numLights < ledList.Count)
                {
                    GamepadLightBars[which] = ledList[numLights];
                }
            }

        }

        private static void RemoveGamePadInstance(int dev)
        {
            Console.WriteLine("GamePad Removed");
            if (!GamepadInstances.TryGetValue(dev, out int output))
            {
                return;
            }
            GamepadInstances.Remove(dev);
            SDL_GameControllerClose(_gamepadDevices[output]);
            _gamepadDevices[output] = IntPtr.Zero;
            GamepadStates[output] = new GamePadState();
            GamepadGuids[output] = string.Empty;

            SDL_ClearError();

            GamePad.ConnectedGamePads --;

        }

        private static GamePadButtons ConvertStickValuesToButtons(Vec2 stick, GamePadButtons left, GamePadButtons right, GamePadButtons up, GamePadButtons down, float deadZoneSize)
        {
            GamePadButtons b = 0;

            if (stick.X > deadZoneSize)
            {
                b |= right;
            }
            if (stick.X < -deadZoneSize)
            {
                b |= left;
            }
            if (stick.Y > deadZoneSize)
            {
                b |= up;
            }
            if (stick.Y < -deadZoneSize)
            {
                b |= down;
            }

            return b;
        }

        private static string[] GenStringArray()
        {
            string[] result = new string[GamePad.GamepadMaxCount];
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
                SdlHintPriority.SdlHintOverride
            );
        }

        private static void ProcessGamePadEvent(SdlEvent evt)
        {
            if (evt.type == SdlEventType.SdlControllerdeviceadded)
            {
                AddGamePadInstance(evt.cdevice.which);
            }
            else if (evt.type == SdlEventType.SdlControllerdeviceremoved)
            {
                RemoveGamePadInstance(evt.cdevice.which);
            }
        }
    }
}
