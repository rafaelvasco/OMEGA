using System;
using static Bgfx.Bgfx;
using static SDL2.Sdl;

namespace OMEGA
{
    internal static partial class Platform
    {
        private static IntPtr _window;

        private static int _prevDisplayW;

        private static int _prevDisplayH;

        private static int _displayW;

        private static int _displayH;

        private static readonly bool OsxUseSpaces = (
            SDL_GetPlatform().Equals("Mac OS X") &&
            SDL_GetHintBoolean(SDL_HINT_VIDEO_MAC_FULLSCREEN_SPACES, SdlBool.SdlTrue) == SdlBool.SdlTrue
        );

        public static void CreateWindow(GameInfo gameInfo)
        {
            _prevDisplayW = gameInfo.ResolutionWidth;
            _prevDisplayH = gameInfo.ResolutionHeight;

            var window_flags = SdlWindowFlags.SdlWindowHidden |
                SdlWindowFlags.SdlWindowInputFocus |
                SdlWindowFlags.SdlWindowMouseFocus;

            if (gameInfo.ResizableWindow)
            {
                window_flags |= SdlWindowFlags.SdlWindowResizable;
            }

            if (gameInfo.StartFullscreen)
            {
                window_flags |= SdlWindowFlags.SdlWindowFullscreenDesktop;
            }

            _window = SDL_CreateWindow(
                gameInfo.Title,
                SDL_WINDOWPOS_CENTERED,
                SDL_WINDOWPOS_CENTERED,
                gameInfo.ResolutionWidth,
                gameInfo.ResolutionHeight,
                window_flags
            );

            if (gameInfo.StartFullscreen)
            {
                SDL_GetDisplayMode(0, 0, out var mode);

                _displayW = mode.w;
                _displayH = mode.h;
            }
            else
            {
                _displayW = _prevDisplayW;
                _displayH = _prevDisplayH;
            }

            SDL_DisableScreenSaver();
        }

        public static void InitGraphicsContext()
        {
            GraphicsContext.SetPlatformData(GetRenderSurfaceHandle());

            var (width, height) = GetDisplaySize();

            GraphicsContext.Initialize(width, height, RendererType.Direct3D11);
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
            var info = new SdlSysWMinfo();

            SDL_GetWindowWMInfo(_window, ref info);

            switch (RunningPlatform)
            {
                case RunningPlatform.Windows:
                    return info.info.win.window;

                case RunningPlatform.Linux:
                    return info.info.x11.window;

                case RunningPlatform.Mac:
                    return info.info.cocoa.window;
                case RunningPlatform.Unknown:
                    break;
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

            _prevDisplayW = width;
            _prevDisplayH = height;

            SDL_SetWindowSize(_window, width, height);
            SDL_SetWindowPosition(_window, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
        }

        public static (int Width, int Height) GetDisplaySize()
        {
            return (_displayW, _displayH);
        }

        public static bool IsFullscreen()
        {
            return (SDL_GetWindowFlags(_window) & (uint)SdlWindowFlags.SdlWindowFullscreen) != 0;
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
                            out SdlDisplayMode mode
                        );
                        SDL_SetWindowSize(_window, mode.w, mode.h);
                    }
                    SDL_SetWindowFullscreen(_window, (uint)SdlWindowFlags.SdlWindowFullscreenDesktop);
                }
                else
                {
                    SDL_SetWindowFullscreen(_window, 0);

                    if (_prevDisplayW != _displayW || _prevDisplayH != _displayH)
                    {
                        SDL_RestoreWindow(_window);
                        SetDisplaySize(_prevDisplayW, _prevDisplayH);
                    }
                }
            }
        }

        public static bool GetWindowBorderless()
        {
            return (SDL_GetWindowFlags(_window) & (uint)SdlWindowFlags.SdlWindowBorderless) != 0;
        }

        public static void SetWindowBorderless(bool borderless)
        {
            SDL_SetWindowBordered(
                _window,
                borderless ?
                    SdlBool.SdlFalse :
                    SdlBool.SdlTrue
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
                SdlHintPriority.SdlHintOverride
            );

            SDL_DestroyWindow(_window);
        }

        public static void SetMouseCursorVisible(bool visible)
        {
            SDL_ShowCursor(visible ? 1 : 0);
        }

        public static void SetTextInputRectangle(Rect rectangle)
        {
            SdlRect rect = new()
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

        private static void ProcessWindowEvent(SdlEvent evt)
        {
            // Window Focus
            if (evt.window.windowEvent == SdlWindowEventId.SdlWindoweventFocusGained)
            {
                Engine.Active = true;

                if (!OsxUseSpaces)
                {
                    SDL_SetWindowFullscreen(
                        _window,
                        Engine.Fullscreen ?
                            (uint)SdlWindowFlags.SdlWindowFullscreenDesktop :
                            0
                    );
                }

                // Disable the screensaver when we're back.
                SDL_DisableScreenSaver();
            }
            else if (evt.window.windowEvent == SdlWindowEventId.SdlWindoweventFocusLost)
            {
                Engine.Active = false;

                if (!OsxUseSpaces)
                {
                    SDL_SetWindowFullscreen(_window, 0);
                }

                SDL_EnableScreenSaver();
            }

            // Window Resize
            else if (evt.window.windowEvent == SdlWindowEventId.SdlWindoweventSizeChanged)
            {
                var w = evt.window.data1;
                var h = evt.window.data2;
                _displayW = w;
                _displayH = h;

                Engine.HandleWindowResize();
            }

            else if (evt.window.windowEvent == SdlWindowEventId.SdlWindoweventExposed)
            {
                Engine.RunningGame.Tick();
            }

            else if (evt.window.windowEvent == SdlWindowEventId.SdlWindoweventEnter)
            {
                SDL_DisableScreenSaver();
                MouseEnter?.Invoke();
            }
            else if (evt.window.windowEvent == SdlWindowEventId.SdlWindoweventLeave)
            {
                SDL_EnableScreenSaver();
                MouseLeave?.Invoke();
            }
        }
    }
}
