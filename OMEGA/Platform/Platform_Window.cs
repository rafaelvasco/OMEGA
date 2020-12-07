using System;
using static SDL2.SDL;

namespace OMEGA
{
    internal static partial class Platform
    {
        public static Action OnMouseEnter;
        public static Action OnMouseLeave;

        private static IntPtr _window;

        private static int prev_display_w;

        private static int prev_display_h;

        private static int display_w;

        private static int display_h;

        private static readonly bool OSXUseSpaces = (
            SDL_GetPlatform().Equals("Mac OS X") &&
            SDL_GetHintBoolean(SDL_HINT_VIDEO_MAC_FULLSCREEN_SPACES, SDL_bool.SDL_TRUE) == SDL_bool.SDL_TRUE
        );

        public static void CreateWindow(string title, int width, int height, bool fullscreen)
        {
            prev_display_w = width;
            prev_display_h = height;

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

            if (fullscreen)
            {
                SDL_GetDisplayMode(0, 0, out var mode);

                display_w = mode.w;
                display_h = mode.h;
            }
            else
            {
                display_w = prev_display_w;
                display_h = prev_display_h;
            }

            SDL_DisableScreenSaver();
        }

        public static void InitGraphicsContext()
        {
            GraphicsContext.SetPlatformData(GetRenderSurfaceHandle());

            var display_size = GetDisplaySize();

            GraphicsContext.Initialize(display_size.Width, display_size.Height, RendererType.Direct3D11);
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

        public static void SetDisplaySize(int width, int height)
        {
            if (IsFullscreen())
            {
                return;
            }

            prev_display_w = width;
            prev_display_h = height;

            SDL_SetWindowSize(_window, width, height);
            SDL_SetWindowPosition(_window, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
        }

        public static (int Width, int Height) GetDisplaySize()
        {
            return (display_w, display_h);
        }

        public static bool IsFullscreen()
        {
            return (SDL_GetWindowFlags(_window) & (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0;
        }

        public static void SetFullscreen(bool enabled)
        {
            if (IsFullscreen() != enabled)
            {
                if (enabled)
                {
                    if (!IsFullscreen())
                    {
                        SDL_GetCurrentDisplayMode(
                            0,
                            out SDL_DisplayMode mode
                        );
                        SDL_SetWindowSize(_window, mode.w, mode.h);
                    }
                    SDL_SetWindowFullscreen(_window, (uint)SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);
                }
                else
                {
                    SDL_SetWindowFullscreen(_window, 0);

                    if (prev_display_w != display_w || prev_display_h != display_h)
                    {
                        SDL_RestoreWindow(_window);
                        SetDisplaySize(prev_display_w, prev_display_h);
                    }
                }
            }
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
                var w = evt.window.data1;
                var h = evt.window.data2;
                display_w = w;
                display_h = h;

                Engine.OnDisplayResize();
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
