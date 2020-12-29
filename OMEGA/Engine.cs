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

        public static Canvas Canvas {get; private set;}

        public static IGame RunningGame => running_game;

        public static bool Active
        {
            get
            {
                return is_active;
            }
            internal set
            {
                if (is_active != value)
                {
                    is_active = value;
                    if (is_active)
                    {
                        running_game.OnActivated();
                    }
                    else
                    {
                        running_game.OnDeactivated();
                    }
                }
            }
        }

        public static bool MouseVisible
        {
            get
            {
                return is_mouse_visible;
            }
            set
            {
                is_mouse_visible = value;
                Platform.SetMouseCursorVisible(value);
            }
        }

        public static bool Fullscreen
        {
            get => fullscreen;
            set
            {
                if (fullscreen != value)
                {
                    fullscreen = value;

                    Platform.SetFullscreen(fullscreen);
                }
            }
        }

        private static bool fullscreen;

        private static bool is_active;

        private static bool is_mouse_visible;

        private static IGame running_game;

        public static void Init(IGame game)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            running_game = game;

            Platform.Init();

            MouseVisible = true;

            var game_info = game.GameInfo;

            fullscreen = game_info.StartFullscreen;

            Platform.CreateWindow(game_info.Title, game_info.ResolutionWidth, game_info.ResolutionHeight, fullscreen);

            Platform.InitGraphicsContext();

            Content = new ContentManager(game_info);

            Canvas = new Canvas(game_info.ResolutionWidth, game_info.ResolutionHeight);

            Input.Init();

            running_game.Load();

            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        }

        internal static void OnDisplayResize()
        {
            Canvas.NeedsResetDisplay = true;
        }

        public static void Shutdown()
        {
            Content.FreeEverything();
            GraphicsContext.Shutdown();
            Platform.Terminate();

#if DEBUG
            var gen0 = GC.CollectionCount(0);
            var gen1 = GC.CollectionCount(1);
            var gen2 = GC.CollectionCount(2);

            var gc_info = GC.GetGCMemoryInfo();

            Console.WriteLine(
                $"Gen-0: {gen0} | Gen-1: {gen1} | Gen-2: {gen2}"
            );
#endif
        }

        public static void Exit()
        {
            running_game.Exit();
        }

        public static void ProcessEvents()
        {
            Platform.PollEvents();
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
