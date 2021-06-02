using System;
using static SDL2.Sdl;

namespace OMEGA
{
    internal static partial class Platform
    {
        private static bool _supportsGlobalMouse;

        public static Action MouseEnter;
        public static Action MouseLeave;
        public static Action<MouseButton> MouseUp;
        public static Action<MouseButton> MouseDown;
        public static Action<int, int> MouseMove;

        private static int _mWheelValue = 0;

        public static bool ButtonPosEventPoolEnabled { get;set;} = false;

        public static MouseState GetMouseState()
        {
            uint flags;
            int x, y;
            if (GetRelativeMouseMode())
            {
                flags = SDL_GetRelativeMouseState(out x, out y);
            }
            else if (_supportsGlobalMouse)
            {
                flags = SDL_GetGlobalMouseState(out x, out y);
                SDL_GetWindowPosition(_window, out int wx, out int wy);
                x -= wx;
                y -= wy;
            }
            else
            {
                flags = SDL_GetMouseState(out x, out y);
            }

            var left = (ButtonState)(flags & SdlButtonLmask);
            var middle = (ButtonState)((flags & SdlButtonMmask) >> 1);
            var right = (ButtonState)((flags & SdlButtonRmask) >> 2);
            var x1 = (ButtonState)((flags & SdlButtonX1Mask) >> 3);
            var x2 = (ButtonState)((flags & SdlButtonX2Mask) >> 4);

            return new MouseState(x, y, _mWheelValue, left, middle, right, x1, x2);
        }

        public static void SetMousePosition(int x, int y)
        {
            SDL_WarpMouseInWindow(_window, x, y);
        }

        public static bool GetRelativeMouseMode()
        {
            return SDL_GetRelativeMouseMode() == SdlBool.SdlTrue;
        }

        public static void SetRelativeMouseMode(bool enable)
        {
            _ = SDL_SetRelativeMouseMode(
                enable ?
                    SdlBool.SdlTrue :
                    SdlBool.SdlFalse
            );
        }

        private static void ProcessMouseEvent(SdlEvent evt)
        {
            if (ButtonPosEventPoolEnabled)
            {
                var button = TranslatePlatformMouseButton(evt.button.button);

                switch (evt.type)
                {
                    case SdlEventType.SdlMousemotion:
                        MouseMove(evt.motion.x, evt.motion.y);
                        break;
                    case SdlEventType.SdlMousebuttondown:
                        MouseDown(button);
                        break;
                    case SdlEventType.SdlMousebuttonup:
                        MouseUp(button);
                        break;
                }
            }

            if (evt.type == SdlEventType.SdlMousewheel)
            {
                _mWheelValue = evt.wheel.y * 120;
            }
        }

        private static MouseButton TranslatePlatformMouseButton(byte button)
        {
            return button switch
            {
                1 => MouseButton.Left,
                2 => MouseButton.Middle,
                3 => MouseButton.Right,
                _ => MouseButton.None,
            };
        }

        private static void InitMouse()
        {
            _supportsGlobalMouse = 
                RunningPlatform == RunningPlatform.Windows ||
                RunningPlatform == RunningPlatform.Mac ||
                RunningPlatform == RunningPlatform.Linux;
        }

    }
}
