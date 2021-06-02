using System;
using System.Runtime.InteropServices;
using static SDL2.Sdl;

namespace OMEGA
{
    public enum RunningPlatform
    {
        Windows,
        Mac,
        Linux,
        Unknown
    }


    internal static partial class Platform
    {
        public static RunningPlatform RunningPlatform {get; private set;}

        public static void Init()
        {
            DetectRunningPlatform();

            SDL_SetMainReady();

            if (RunningPlatform == RunningPlatform.Windows)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    SDL_SetHint(
                        SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING,
                        "1"
                    );
                }
            }

            _ = SDL_Init(
                SDL_INIT_VIDEO |
                SDL_INIT_JOYSTICK |
                SDL_INIT_GAMECONTROLLER |
                SDL_INIT_HAPTIC
            );

            InitMouse();
            InitGamePad();
        }

        public static void Terminate()
        {
            DestroyWindow();
            SDL_Quit();
        }

        public static void PollEvents()
        {
            while (SDL_PollEvent(out SdlEvent evt) == 1)
            {
                if (evt.type == SdlEventType.SdlKeydown || evt.type == SdlEventType.SdlKeyup)
                {
                    ProcessKeyEvent(evt);
                }

                else if (evt.type == SdlEventType.SdlMousemotion || evt.type == SdlEventType.SdlMousebuttondown || evt.type == SdlEventType.SdlMousebuttonup || evt.type == SdlEventType.SdlMousewheel)
                {
                    ProcessMouseEvent(evt);
                }

                else if (evt.type == SdlEventType.SdlWindowevent)
                {
                    ProcessWindowEvent(evt);
                }

                else if (evt.type == SdlEventType.SdlControllerdeviceadded || evt.type == SdlEventType.SdlControllerdeviceremoved)
                {
                    ProcessGamePadEvent(evt);
                }

                else if (evt.type == SdlEventType.SdlTextinput || evt.type == SdlEventType.SdlTextediting)
                {
                    ProcessTextInputEvent(evt);
                }

                else if (evt.type == SdlEventType.SdlQuit)
                {
                    Engine.Exit();
                    break;
                }
            }
        }

        public static double GetPerformanceFrequency()
        {
            return SDL_GetPerformanceFrequency();
        }

        public static double GetPerformanceCounter()
        {
            return SDL_GetPerformanceCounter();
        }

        public static bool NeedsPlatformMainLoop()
        {
            return SDL_GetPlatform().Equals("Emscripten");
        }

        public static void ShowRuntimeError(string title, string message)
        {
            SDL_ShowSimpleMessageBox(
                SdlMessageBoxFlags.SdlMessageboxError,
                title ?? "",
                message ?? "",
                IntPtr.Zero
            );
        }

        private static void DetectRunningPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                RunningPlatform = RunningPlatform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                RunningPlatform = RunningPlatform.Mac;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                RunningPlatform = RunningPlatform.Linux;
            }
            else
            {
                RunningPlatform = RunningPlatform.Unknown;
            }
        }
    }
}
