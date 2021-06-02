using static SDL2.Sdl;

using System.Collections.Generic;
using System;
using System.Text;

namespace OMEGA
{
    internal static partial class Platform
    {
        public static bool UseScanCodeKeyDetection { get; set; } = true;

        public static Action<Keys> KeyDown;
        public static Action<Keys> KeyUp;
        public static Action<char> CharInput;

        private static readonly List<Keys> MKeys = new (10);

        public static KeyboardState GetKeyboardState()
        {
            return new KeyboardState(MKeys);
        }

        public static void StartTextInput()
        {
            SDL_StartTextInput();
        }

        public static void StopTextInput()
        {
            SDL_StopTextInput();
        }

        private static readonly Dictionary<int, Keys> KeycodeMap = new()
        {
            { (int) SdlKeycode.SdlkA,         Keys.A },
            { (int) SdlKeycode.SdlkB,         Keys.B },
            { (int) SdlKeycode.SdlkC,         Keys.C },
            { (int) SdlKeycode.SdlkD,         Keys.D },
            { (int) SdlKeycode.SdlkE,         Keys.E },
            { (int) SdlKeycode.SdlkF,         Keys.F },
            { (int) SdlKeycode.SdlkG,         Keys.G },
            { (int) SdlKeycode.SdlkH,         Keys.H },
            { (int) SdlKeycode.SdlkI,         Keys.I },
            { (int) SdlKeycode.SdlkJ,         Keys.J },
            { (int) SdlKeycode.SdlkK,         Keys.K },
            { (int) SdlKeycode.SdlkL,         Keys.L },
            { (int) SdlKeycode.SdlkM,         Keys.M },
            { (int) SdlKeycode.SdlkN,         Keys.N },
            { (int) SdlKeycode.SdlkO,         Keys.O },
            { (int) SdlKeycode.SdlkP,         Keys.P },
            { (int) SdlKeycode.SdlkQ,         Keys.Q },
            { (int) SdlKeycode.SdlkR,         Keys.R },
            { (int) SdlKeycode.SdlkS,         Keys.S },
            { (int) SdlKeycode.SdlkT,         Keys.T },
            { (int) SdlKeycode.SdlkU,         Keys.U },
            { (int) SdlKeycode.SdlkV,         Keys.V },
            { (int) SdlKeycode.SdlkW,         Keys.W },
            { (int) SdlKeycode.SdlkX,         Keys.X },
            { (int) SdlKeycode.SdlkY,         Keys.Y },
            { (int) SdlKeycode.SdlkZ,         Keys.Z },
            { (int) SdlKeycode.Sdlk0,         Keys.D0 },
            { (int) SdlKeycode.Sdlk1,         Keys.D1 },
            { (int) SdlKeycode.Sdlk2,         Keys.D2 },
            { (int) SdlKeycode.Sdlk3,         Keys.D3 },
            { (int) SdlKeycode.Sdlk4,         Keys.D4 },
            { (int) SdlKeycode.Sdlk5,         Keys.D5 },
            { (int) SdlKeycode.Sdlk6,         Keys.D6 },
            { (int) SdlKeycode.Sdlk7,         Keys.D7 },
            { (int) SdlKeycode.Sdlk8,         Keys.D8 },
            { (int) SdlKeycode.Sdlk9,         Keys.D9 },
            { (int) SdlKeycode.SdlkKp0,      Keys.NumPad0 },
            { (int) SdlKeycode.SdlkKp1,      Keys.NumPad1 },
            { (int) SdlKeycode.SdlkKp2,      Keys.NumPad2 },
            { (int) SdlKeycode.SdlkKp3,      Keys.NumPad3 },
            { (int) SdlKeycode.SdlkKp4,      Keys.NumPad4 },
            { (int) SdlKeycode.SdlkKp5,      Keys.NumPad5 },
            { (int) SdlKeycode.SdlkKp6,      Keys.NumPad6 },
            { (int) SdlKeycode.SdlkKp7,      Keys.NumPad7 },
            { (int) SdlKeycode.SdlkKp8,      Keys.NumPad8 },
            { (int) SdlKeycode.SdlkKp9,      Keys.NumPad9 },
            { (int) SdlKeycode.SdlkKpDecimal,    Keys.Decimal },
            { (int) SdlKeycode.SdlkKpDivide,     Keys.Divide },
            { (int) SdlKeycode.SdlkKpEnter,      Keys.Enter },
            { (int) SdlKeycode.SdlkKpMinus,      Keys.Subtract },
            { (int) SdlKeycode.SdlkKpMultiply,   Keys.Multiply },
            { (int) SdlKeycode.SdlkKpPlus,       Keys.Add },
            { (int) SdlKeycode.SdlkF1,        Keys.F1 },
            { (int) SdlKeycode.SdlkF2,        Keys.F2 },
            { (int) SdlKeycode.SdlkF3,        Keys.F3 },
            { (int) SdlKeycode.SdlkF4,        Keys.F4 },
            { (int) SdlKeycode.SdlkF5,        Keys.F5 },
            { (int) SdlKeycode.SdlkF6,        Keys.F6 },
            { (int) SdlKeycode.SdlkF7,        Keys.F7 },
            { (int) SdlKeycode.SdlkF8,        Keys.F8 },
            { (int) SdlKeycode.SdlkF9,        Keys.F9 },
            { (int) SdlKeycode.SdlkF10,       Keys.F10 },
            { (int) SdlKeycode.SdlkF11,       Keys.F11 },
            { (int) SdlKeycode.SdlkF12,       Keys.F12 },
            { (int) SdlKeycode.SdlkF13,       Keys.F13 },
            { (int) SdlKeycode.SdlkF14,       Keys.F14 },
            { (int) SdlKeycode.SdlkF15,       Keys.F15 },
            { (int) SdlKeycode.SdlkF16,       Keys.F16 },
            { (int) SdlKeycode.SdlkF17,       Keys.F17 },
            { (int) SdlKeycode.SdlkF18,       Keys.F18 },
            { (int) SdlKeycode.SdlkF19,       Keys.F19 },
            { (int) SdlKeycode.SdlkF20,       Keys.F20 },
            { (int) SdlKeycode.SdlkF21,       Keys.F21 },
            { (int) SdlKeycode.SdlkF22,       Keys.F22 },
            { (int) SdlKeycode.SdlkF23,       Keys.F23 },
            { (int) SdlKeycode.SdlkF24,       Keys.F24 },
            { (int) SdlKeycode.SdlkSpace,     Keys.Space },
            { (int) SdlKeycode.SdlkUp,        Keys.Up },
            { (int) SdlKeycode.SdlkDown,      Keys.Down },
            { (int) SdlKeycode.SdlkLeft,      Keys.Left },
            { (int) SdlKeycode.SdlkRight,     Keys.Right },
            { (int) SdlKeycode.SdlkLalt,      Keys.LeftAlt },
            { (int) SdlKeycode.SdlkRalt,      Keys.RightAlt },
            { (int) SdlKeycode.SdlkLctrl,     Keys.LeftControl },
            { (int) SdlKeycode.SdlkRctrl,     Keys.RightControl },
            { (int) SdlKeycode.SdlkLshift,        Keys.LeftShift },
            { (int) SdlKeycode.SdlkRshift,        Keys.RightShift },
            { (int) SdlKeycode.SdlkCapslock,      Keys.CapsLock },
            { (int) SdlKeycode.SdlkDelete,        Keys.Delete },
            { (int) SdlKeycode.SdlkEnd,       Keys.End },
            { (int) SdlKeycode.SdlkBackspace,     Keys.Back },
            { (int) SdlKeycode.SdlkReturn,        Keys.Enter },
            { (int) SdlKeycode.SdlkEscape,        Keys.Escape },
            { (int) SdlKeycode.SdlkHome,      Keys.Home },
            { (int) SdlKeycode.SdlkPageup,        Keys.PageUp },
            { (int) SdlKeycode.SdlkPagedown,      Keys.PageDown },
            { (int) SdlKeycode.SdlkPause,     Keys.Pause },
            { (int) SdlKeycode.SdlkTab,       Keys.Tab },
            { (int) SdlKeycode.SdlkVolumeup,      Keys.VolumeUp },
            { (int) SdlKeycode.SdlkVolumedown,    Keys.VolumeDown },
            { (int) SdlKeycode.SdlkUnknown,       Keys.None }
        };
        private static readonly Dictionary<int, Keys> KeyscanMap = new()
        {
            { (int) SdlScancode.SdlScancodeA,        Keys.A },
            { (int) SdlScancode.SdlScancodeB,        Keys.B },
            { (int) SdlScancode.SdlScancodeC,        Keys.C },
            { (int) SdlScancode.SdlScancodeD,        Keys.D },
            { (int) SdlScancode.SdlScancodeE,        Keys.E },
            { (int) SdlScancode.SdlScancodeF,        Keys.F },
            { (int) SdlScancode.SdlScancodeG,        Keys.G },
            { (int) SdlScancode.SdlScancodeH,        Keys.H },
            { (int) SdlScancode.SdlScancodeI,        Keys.I },
            { (int) SdlScancode.SdlScancodeJ,        Keys.J },
            { (int) SdlScancode.SdlScancodeK,        Keys.K },
            { (int) SdlScancode.SdlScancodeL,        Keys.L },
            { (int) SdlScancode.SdlScancodeM,        Keys.M },
            { (int) SdlScancode.SdlScancodeN,        Keys.N },
            { (int) SdlScancode.SdlScancodeO,        Keys.O },
            { (int) SdlScancode.SdlScancodeP,        Keys.P },
            { (int) SdlScancode.SdlScancodeQ,        Keys.Q },
            { (int) SdlScancode.SdlScancodeR,        Keys.R },
            { (int) SdlScancode.SdlScancodeS,        Keys.S },
            { (int) SdlScancode.SdlScancodeT,        Keys.T },
            { (int) SdlScancode.SdlScancodeU,        Keys.U },
            { (int) SdlScancode.SdlScancodeV,        Keys.V },
            { (int) SdlScancode.SdlScancodeW,        Keys.W },
            { (int) SdlScancode.SdlScancodeX,        Keys.X },
            { (int) SdlScancode.SdlScancodeY,        Keys.Y },
            { (int) SdlScancode.SdlScancodeZ,        Keys.Z },
            { (int) SdlScancode.SdlScancode0,        Keys.D0 },
            { (int) SdlScancode.SdlScancode1,        Keys.D1 },
            { (int) SdlScancode.SdlScancode2,        Keys.D2 },
            { (int) SdlScancode.SdlScancode3,        Keys.D3 },
            { (int) SdlScancode.SdlScancode4,        Keys.D4 },
            { (int) SdlScancode.SdlScancode5,        Keys.D5 },
            { (int) SdlScancode.SdlScancode6,        Keys.D6 },
            { (int) SdlScancode.SdlScancode7,        Keys.D7 },
            { (int) SdlScancode.SdlScancode8,        Keys.D8 },
            { (int) SdlScancode.SdlScancode9,        Keys.D9 },
            { (int) SdlScancode.SdlScancodeKp0,     Keys.NumPad0 },
            { (int) SdlScancode.SdlScancodeKp1,     Keys.NumPad1 },
            { (int) SdlScancode.SdlScancodeKp2,     Keys.NumPad2 },
            { (int) SdlScancode.SdlScancodeKp3,     Keys.NumPad3 },
            { (int) SdlScancode.SdlScancodeKp4,     Keys.NumPad4 },
            { (int) SdlScancode.SdlScancodeKp5,     Keys.NumPad5 },
            { (int) SdlScancode.SdlScancodeKp6,     Keys.NumPad6 },
            { (int) SdlScancode.SdlScancodeKp7,     Keys.NumPad7 },
            { (int) SdlScancode.SdlScancodeKp8,     Keys.NumPad8 },
            { (int) SdlScancode.SdlScancodeKp9,     Keys.NumPad9 },
            { (int) SdlScancode.SdlScancodeKpDecimal,   Keys.Decimal },
            { (int) SdlScancode.SdlScancodeKpDivide,    Keys.Divide },
            { (int) SdlScancode.SdlScancodeKpEnter,     Keys.Enter },
            { (int) SdlScancode.SdlScancodeKpMinus,     Keys.Subtract },
            { (int) SdlScancode.SdlScancodeKpMultiply,  Keys.Multiply },
            { (int) SdlScancode.SdlScancodeKpPlus,      Keys.Add },
            { (int) SdlScancode.SdlScancodeF1,       Keys.F1 },
            { (int) SdlScancode.SdlScancodeF2,       Keys.F2 },
            { (int) SdlScancode.SdlScancodeF3,       Keys.F3 },
            { (int) SdlScancode.SdlScancodeF4,       Keys.F4 },
            { (int) SdlScancode.SdlScancodeF5,       Keys.F5 },
            { (int) SdlScancode.SdlScancodeF6,       Keys.F6 },
            { (int) SdlScancode.SdlScancodeF7,       Keys.F7 },
            { (int) SdlScancode.SdlScancodeF8,       Keys.F8 },
            { (int) SdlScancode.SdlScancodeF9,       Keys.F9 },
            { (int) SdlScancode.SdlScancodeF10,      Keys.F10 },
            { (int) SdlScancode.SdlScancodeF11,      Keys.F11 },
            { (int) SdlScancode.SdlScancodeF12,      Keys.F12 },
            { (int) SdlScancode.SdlScancodeF13,      Keys.F13 },
            { (int) SdlScancode.SdlScancodeF14,      Keys.F14 },
            { (int) SdlScancode.SdlScancodeF15,      Keys.F15 },
            { (int) SdlScancode.SdlScancodeF16,      Keys.F16 },
            { (int) SdlScancode.SdlScancodeF17,      Keys.F17 },
            { (int) SdlScancode.SdlScancodeF18,      Keys.F18 },
            { (int) SdlScancode.SdlScancodeF19,      Keys.F19 },
            { (int) SdlScancode.SdlScancodeF20,      Keys.F20 },
            { (int) SdlScancode.SdlScancodeF21,      Keys.F21 },
            { (int) SdlScancode.SdlScancodeF22,      Keys.F22 },
            { (int) SdlScancode.SdlScancodeF23,      Keys.F23 },
            { (int) SdlScancode.SdlScancodeF24,      Keys.F24 },
            { (int) SdlScancode.SdlScancodeSpace,        Keys.Space },
            { (int) SdlScancode.SdlScancodeUp,       Keys.Up },
            { (int) SdlScancode.SdlScancodeDown,     Keys.Down },
            { (int) SdlScancode.SdlScancodeLeft,     Keys.Left },
            { (int) SdlScancode.SdlScancodeRight,        Keys.Right },
            { (int) SdlScancode.SdlScancodeLalt,     Keys.LeftAlt },
            { (int) SdlScancode.SdlScancodeRalt,     Keys.RightAlt },
            { (int) SdlScancode.SdlScancodeLctrl,        Keys.LeftControl },
            { (int) SdlScancode.SdlScancodeRctrl,        Keys.RightControl },
            { (int) SdlScancode.SdlScancodeLshift,       Keys.LeftShift },
            { (int) SdlScancode.SdlScancodeRshift,       Keys.RightShift },
            { (int) SdlScancode.SdlScancodeCapslock,     Keys.CapsLock },
            { (int) SdlScancode.SdlScancodeDelete,       Keys.Delete },
            { (int) SdlScancode.SdlScancodeEnd,      Keys.End },
            { (int) SdlScancode.SdlScancodeBackspace,    Keys.Back },
            { (int) SdlScancode.SdlScancodeReturn,       Keys.Enter },
            { (int) SdlScancode.SdlScancodeEscape,       Keys.Escape },
            { (int) SdlScancode.SdlScancodeHome,     Keys.Home },
            { (int) SdlScancode.SdlScancodePageup,       Keys.PageUp },
            { (int) SdlScancode.SdlScancodePagedown,     Keys.PageDown },
            { (int) SdlScancode.SdlScancodePause,        Keys.Pause },
            { (int) SdlScancode.SdlScancodeTab,      Keys.Tab },
            { (int) SdlScancode.SdlScancodeVolumeup,     Keys.VolumeUp },
            { (int) SdlScancode.SdlScancodeVolumedown,   Keys.VolumeDown },
            { (int) SdlScancode.SdlScancodeUnknown,      Keys.None },
        };

        private static Keys ConvertKey(ref SdlKeysym key)
        {
            if (UseScanCodeKeyDetection)
            {
                if (KeyscanMap.TryGetValue((int)key.scancode, out Keys retVal))
                {
                    return retVal;
                }
            }
            else
            {
                if (KeycodeMap.TryGetValue((int)key.sym, out Keys retVal))
                {
                    return retVal;
                }
            }

            return Keys.None;
        }

        private static void ProcessTextInputEvent(SdlEvent evt)
        {
            if (evt.type == SdlEventType.SdlTextinput)
            {
                unsafe
                {
                    int bytes = MeasureStringLength(evt.text.text);
                    if (bytes > 0)
                    {
                        char* chars_buffer = stackalloc char[bytes];
                        int chars = Encoding.UTF8.GetChars(
                            evt.text.text,
                            bytes,
                            chars_buffer,
                            bytes
                        );

                        for (int i = 0; i < chars; i += 1)
                        {
                            CharInput?.Invoke(chars_buffer[i]);
                        }
                    }
                }
            }
        }

        private static void ProcessKeyEvent(SdlEvent evt)
        {
            if (evt.type == SdlEventType.SdlKeydown)
            {
                Keys key = ConvertKey(ref evt.key.keysym);

                KeyDown.Invoke(key);

                if (!MKeys.Contains(key))
                {
                    MKeys.Add(key);
                }
            }
            else if (evt.type == SdlEventType.SdlKeyup)
            {
                Keys key = ConvertKey(ref evt.key.keysym);

                KeyUp.Invoke(key);

                MKeys.Remove(key);
            }
        }

        private unsafe static int MeasureStringLength(byte* ptr)
        {
            int bytes;
            for (bytes = 0; *ptr != 0; ptr += 1, ++bytes) ;
            return bytes;
        }

    }
}
