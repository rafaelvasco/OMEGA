using System;
using System.Runtime;

namespace OMEGA
{
    /* ╔═════════════╗ */
    /* ║OMEGA ENGINE ║ */                                            
    /* ╚═════════════╝ */

    public static class Engine
    {
        public static ContentManager Content { get; private set; }

        public static Canvas2D Canvas {get; private set;}

        public static IGame RunningGame => _runningGame;

        public  delegate void WindowResizeHandler(int width, int height);
        public static event WindowResizeHandler OnWindowResize ;

        public static bool Active
        {
            get => _isActive;
            internal set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    if (_isActive)
                    {
                        _runningGame.OnActivated();
                    }
                    else
                    {
                        _runningGame.OnDeactivated();
                    }
                }
            }
        }

        public static bool MouseVisible
        {
            get => _isMouseVisible;
            set
            {
                _isMouseVisible = value;
                Platform.SetMouseCursorVisible(value);
            }
        }

        public static bool Fullscreen
        {
            get => _fullscreen;
            set
            {
                if (_fullscreen != value)
                {
                    _fullscreen = value;

                    Platform.SetFullscreen(_fullscreen);
                }
            }
        }

        private static bool _fullscreen;

        private static bool _isActive;

        private static bool _isMouseVisible;

        private static IGame _runningGame;

        public static void Init(IGame game)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            _runningGame = game;

            Platform.Init();

            MouseVisible = true;

            var game_info = game.GameInfo;

            _fullscreen = game_info.StartFullscreen;

            Platform.CreateWindow(game_info);

            Platform.InitGraphicsContext();

            Content = new ContentManager(game_info);

            Blitter.GetDefaultAssets();

            Canvas = new Canvas2D(game_info.ResolutionWidth, game_info.ResolutionHeight);

            Input.Init();

            _runningGame.Load();

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        }

        internal static void HandleWindowResize()
        {
            Canvas.NeedsResetDisplay = true;

            OnWindowResize?.Invoke(Platform.GetDisplaySize().Width, Platform.GetDisplaySize().Height);
        }

        internal static void Shutdown()
        {
            Content.FreeEverything();
            GraphicsContext.Shutdown();
            Platform.Terminate();

#if DEBUG
            var gen0 = GC.CollectionCount(0);
            var gen1 = GC.CollectionCount(1);
            var gen2 = GC.CollectionCount(2);

            Console.WriteLine(
                $"Gen-0: {gen0} | Gen-1: {gen1} | Gen-2: {gen2}"
            );
#endif
        }

        public static void Exit()
        {
            _runningGame.Exit();
        }

        internal static void ProcessEvents()
        {
            Platform.PollEvents();
        }

        internal static void ProcessInput()
        {
            Input.Update();
        }

        public static void ShowWindow(bool show)
        {
            Platform.ShowWindow(show);
        }

        //TODO:
        public static void SaveScreenSnapShot() {}

        public static void ToggleFullscreen()
        {
            Fullscreen = !Fullscreen;
        }

        private static void ShowExceptionMessage(Exception exception)
        {
            if (exception is NoAudioHardwareException)
            {
                Platform.ShowRuntimeError(
                    "OMEGA Engine",
                    "Could not find a suitable audio device. " +
                    " Verify that a sound card is\ninstalled," +
                    " and check the driver properties to make" +
                    " sure it is not disabled."
                );
            }
            if (exception is GraphicsDeviceException)
            {
                Platform.ShowRuntimeError(
                    "OMEGA Engine",
                    "Could not find a suitable graphics device." +
                    " More information:\n\n" + exception.Message
                );
            }
            else
            {
                Platform.ShowRuntimeError(
                    "OMEGA Engine",
                    "An error occurred: " + exception.Message
                );
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ShowExceptionMessage(e.ExceptionObject as Exception);
        }

    }
}
