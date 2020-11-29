using System;
using static SDL2.SDL;

namespace OMEGA
{
    internal static partial class Platform
    {
        private static bool SupportsGlobalMouse;

        public static void GetMouseState(
            out int x,
            out int y,
            out ButtonState left,
            out ButtonState middle,
            out ButtonState right,
            out ButtonState x1,
            out ButtonState x2
        )
        {
            uint flags;
            if (GetRelativeMouseMode())
            {
                flags = SDL_GetRelativeMouseState(out x, out y);
            }
            else if (SupportsGlobalMouse)
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

            left = (ButtonState)(flags & SDL_BUTTON_LMASK);
            middle = (ButtonState)((flags & SDL_BUTTON_MMASK) >> 1);
            right = (ButtonState)((flags & SDL_BUTTON_RMASK) >> 2);
            x1 = (ButtonState)((flags & SDL_BUTTON_X1MASK) >> 3);
            x2 = (ButtonState)((flags & SDL_BUTTON_X2MASK) >> 4);
        }

        public static void SetMousePosition(int x, int y)
        {
            SDL_WarpMouseInWindow(_window, x, y);
        }

        public static bool GetRelativeMouseMode()
        {
            return SDL_GetRelativeMouseMode() == SDL_bool.SDL_TRUE;
        }

        public static void SetRelativeMouseMode(bool enable)
        {
            SDL_SetRelativeMouseMode(
                enable ?
                    SDL_bool.SDL_TRUE :
                    SDL_bool.SDL_FALSE
            );
        }

        private static void ProcessMouseEvent(SDL_Event evt)
        {
            if (evt.type == SDL_EventType.SDL_MOUSEBUTTONDOWN)
            {
                Mouse.ProcessClicked(evt.button.button - 1);
            }

            else if (evt.type == SDL_EventType.SDL_MOUSEWHEEL)
            {
                Mouse.ProcessMouseWheel(evt.wheel.y * 120);
            }
        }

        private static void InitMouse()
        {
            SupportsGlobalMouse = 
                RunningPlatform == RunningPlatform.Windows ||
                RunningPlatform == RunningPlatform.Mac ||
                RunningPlatform == RunningPlatform.Linux;
        }

    }
}
