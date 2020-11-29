using System;

namespace OMEGA
{
    public static class Engine
    {
        public static ContentManager Content { get; private set; }

        public static DrawDevice DrawDevice => draw_device;

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

                    draw_device.ApplyVideoMode(display_width, display_height, fullscreen);
                }
            }
        }

        public static (int Width, int Height) DisplaySize
        {
            get => (display_width, display_height);
            set
            {
                if (display_width != value.Width || display_height != value.Height)
                {
                    display_width = value.Width;
                    display_height = value.Height;

                    if (!fullscreen)
                    {
                        draw_device.ApplyVideoMode(display_width, display_height, fullscreen);
                    }
                }
            }
        }

        private static DrawDevice draw_device;

        private static int display_width;

        private static int display_height;

        private static bool fullscreen;

        private static bool is_active;

        private static bool is_mouse_visible;

        private static IGame running_game;


        public static void Init(IGame game, GameInfo game_info)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            running_game = game;

            Platform.Init();

            MouseVisible = true;
            display_width = game_info.ResolutionWidth;
            display_height = game_info.ResolutionHeight;
            fullscreen = game_info.StartFullscreen;

            Platform.CreateWindow(game_info.Title, display_width, display_height, fullscreen);

            IntPtr render_surface_handle = Platform.GetRenderSurfaceHandle();

            draw_device = new DrawDevice(render_surface_handle, display_width, display_height);

            Content = new ContentManager(game_info);

            Input.Init();

            running_game.Load();
        }

        public static void Shutdown()
        {
            Content.FreeEverything();
            draw_device.Dispose();
            Platform.Terminate();
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
