using System;
using static SDL2.SDL;

namespace OMEGA
{
    internal static partial class Platform
    {
        public static Action OnMouseEnter;
        public static Action OnMouseLeave;

        private static IntPtr _window;

        private static readonly bool OSXUseSpaces = (
            SDL_GetPlatform().Equals("Mac OS X") &&
            SDL_GetHintBoolean(SDL_HINT_VIDEO_MAC_FULLSCREEN_SPACES, SDL_bool.SDL_TRUE) == SDL_bool.SDL_TRUE
        );

        public static void CreateWindow(string title, int width, int height, bool fullscreen)
        {
            var window_flags = SDL_WindowFlags.SDL_WINDOW_HIDDEN |
                SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS |
                SDL_WindowFlags.SDL_WINDOW_MOUSE_FOCUS;

            if (fullscreen)
            {
                window_flags |= SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP;
            }

            _window = SDL_CreateWindow(
                title,
                SDL_WINDOWPOS_CENTERED,
                SDL_WINDOWPOS_CENTERED,
                width,
                height,
                window_flags
            );

            SDL_DisableScreenSaver();
        }

        public static void ShowWindow(bool show)
        {
            if (show)
            {
                SDL_ShowWindow(_window);
            }
            else
            {
                SDL_HideWindow(_window);
            }
        }

        public static IntPtr GetRenderSurfaceHandle()
        {
            var info = new SDL_SysWMinfo();

            SDL_GetWindowWMInfo(_window, ref info);

            switch (RunningPlatform)
            {
                case RunningPlatform.Windows:
                    return info.info.win.window;

                case RunningPlatform.Linux:
                    return info.info.x11.window;

                case RunningPlatform.Mac:
                    return info.info.cocoa.window;
            }

            throw new Exception(
                "SDLGamePlatform [GetRenderSurfaceHandle]: " +
                "Invalid OS, could not retrive native renderer surface handle.");
        }

        public static void SetVideoMode(int width, int height, bool fullscreen)
        {
            bool center = false;

            if (!fullscreen)
            {
                bool resize;
                if ((SDL_GetWindowFlags(_window) & (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0)
                {
                    SDL_SetWindowFullscreen(_window, 0);
                    resize = true;
                }
                else
                {
                    int w, h;
                    SDL_GetWindowSize(
                        _window,
                        out w,
                        out h
                    );
                    resize = (width != w || height != h);
                }
                if (resize)
                {
                    SDL_RestoreWindow(_window);
                    SDL_SetWindowSize(_window, width, height);
                    center = true;
                }
            }

            if (center)
            {
                SDL_SetWindowPosition(_window, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
            }

            if (fullscreen)
            {
                if ((SDL_GetWindowFlags(_window) & (uint)SDL_WindowFlags.SDL_WINDOW_SHOWN) == 0)
                {
                    SDL_DisplayMode mode;
                    SDL_GetCurrentDisplayMode(
                        0,
                        out mode
                    );
                    SDL_SetWindowSize(_window, mode.w, mode.h);
                }
                SDL_SetWindowFullscreen(
                    _window,
                    (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP
                );
            }
        }

        public static Rect GetWindowBounds()
        {
            Rect result;

            if ((SDL_GetWindowFlags(_window) & (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0)
            {
                SDL_GetCurrentDisplayMode(
                    SDL_GetWindowDisplayIndex(
                        _window
                    ),
                    out SDL_DisplayMode mode
                );

                result = new Rect(0, 0, mode.w, mode.h);
            }
            else
            {
                SDL_GetWindowPosition(
                    _window,
                    out var X,
                    out var Y
                );
                SDL_GetWindowSize(
                    _window,
                    out var Width,
                    out var Height
                );

                result = new Rect(X, Y, Width, Height);
            }
            return result;
        }

        public static bool GetWindowResizable()
        {
            return (SDL_GetWindowFlags(_window) & (uint)SDL_WindowFlags.SDL_WINDOW_RESIZABLE) != 0;
        }

        public static void SetWindowResizable(bool resizable)
        {
            SDL_SetWindowResizable(
                _window,
                resizable ?
                    SDL_bool.SDL_TRUE :
                    SDL_bool.SDL_FALSE
            );
        }

        public static bool GetWindowBorderless()
        {
            return (SDL_GetWindowFlags(_window) & (uint)SDL_WindowFlags.SDL_WINDOW_BORDERLESS) != 0;
        }

        public static void SetWindowBorderless(bool borderless)
        {
            SDL_SetWindowBordered(
                _window,
                borderless ?
                    SDL_bool.SDL_FALSE :
                    SDL_bool.SDL_TRUE
            );
        }

        public static void SetWindowTitle(string title)
        {
            SDL_SetWindowTitle(
                _window,
                title
            );
        }

        public static void DestroyWindow()
        {
            SDL_SetHintWithPriority(
                SDL_HINT_VIDEO_MINIMIZE_ON_FOCUS_LOSS,
                "0",
                SDL_HintPriority.SDL_HINT_OVERRIDE
            );

            SDL_DestroyWindow(_window);
        }

        public static void SetMouseCursorVisible(bool visible)
        {
            SDL_ShowCursor(visible ? 1 : 0);
        }

        public static void SetTextInputRectangle(Rect rectangle)
        {
            SDL_Rect rect = new SDL_Rect
            {
                x = rectangle.X1,
                y = rectangle.Y1,
                w = rectangle.Width,
                h = rectangle.Height
            };
            SDL_SetTextInputRect(ref rect);
        }

        private static void SetWindowIcon()
        {

        }

        private static void ProcessWindowEvent(SDL_Event evt)
        {
            // Window Focus
            if (evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED)
            {
                Engine.Active = true;

                if (!OSXUseSpaces)
                {
                    SDL_SetWindowFullscreen(
                        _window,
                        Engine.Fullscreen ?
                            (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP :
                            0
                    );
                }

                // Disable the screensaver when we're back.
                SDL_DisableScreenSaver();
            }
            else if (evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST)
            {
                Engine.Active = false;

                if (!OSXUseSpaces)
                {
                    SDL_SetWindowFullscreen(_window, 0);
                }

                SDL_EnableScreenSaver();
            }

            // Window Resize
            else if (evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED)
            {
                Mouse.DisplayScaleFactorX = Engine.DrawDevice.BackBufferWidth / evt.window.data1;
                Mouse.DisplayScaleFactorY = Engine.DrawDevice.BackBufferHeight / evt.window.data2;
            }

            else if (evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
            {
                uint flags = SDL_GetWindowFlags(_window);
                if ((flags & (uint)SDL_WindowFlags.SDL_WINDOW_RESIZABLE) != 0 &&
                    (flags & (uint)SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS) != 0)
                {
                    Engine.RunningGame.OnResize();
                }
            }

            else if (evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_EXPOSED)
            {
                Engine.RunningGame.Tick();
            }

            else if (evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_ENTER)
            {
                SDL_DisableScreenSaver();
                OnMouseEnter?.Invoke();
            }
            else if (evt.window.windowEvent == SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE)
            {
                SDL_EnableScreenSaver();
                OnMouseLeave?.Invoke();
            }
        }
    }
}
