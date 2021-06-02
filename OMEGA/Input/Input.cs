namespace OMEGA
{
    public static class Input
    {
        public delegate void GenericInputEvent();
        public delegate void KeyInputEvent(Keys key);
        public delegate void MouseInputEvent(MouseButton button);
        public delegate void MouseMotionEvent(int x, int y);
        public delegate void CharInputEvent(char ch);

        public static bool KeyboardActive { get; set; } = true;
        public static bool MouseActive { get; set; } = true;
        public static bool GamePadActive { get; set; } = true;

        public static bool EnableMouseButtonPosEventPooling
        {
            get => Platform.ButtonPosEventPoolEnabled;
            set => Platform.ButtonPosEventPoolEnabled = value;
        }

        public static event GenericInputEvent OnMouseEnter;
        public static event GenericInputEvent OnMouseLeave;
        public static event MouseInputEvent OnMouseDown;
        public static event MouseInputEvent OnMouseUp;
        public static event MouseMotionEvent OnMouseMove;
        public static event KeyInputEvent OnKeyPress;
        public static event KeyInputEvent OnKeyDown;
        public static event KeyInputEvent OnKeyUp;
        public static event CharInputEvent OnTextInput;

        public static bool IsMouseOver { get; internal set; }

        private static Keys _mLastPressedKey;

        public static Point MousePos => Engine.Canvas.ConvertToLocalCoord(_msCurrentState.X, _msCurrentState.Y);

        public static int CurrentMouseWheel => _msCurrentState.ScrollWheelValue;

        public static bool MouseLeftDown()
        {
            return _msCurrentState.LeftButton == ButtonState.Pressed;
        }

        public static bool MouseLeftUp()
        {
            return _msCurrentState.LeftButton == ButtonState.Released;
        }

        public static bool MouseLeftPressed()
        {
            return _msCurrentState.LeftButton == ButtonState.Pressed && _msPrevState.LeftButton == ButtonState.Released;
        }

        public static bool MouseRightDown()
        {
            return _msCurrentState.RightButton == ButtonState.Pressed;
        }

        public static bool MouseRightUp()
        {
            return _msCurrentState.RightButton == ButtonState.Released;
        }

        public static bool MouseRightPressed()
        {
            return _msCurrentState.RightButton == ButtonState.Pressed && _msPrevState.RightButton == ButtonState.Released;
        }

        public static bool MouseMiddleDown()
        {
            return _msCurrentState.MiddleButton == ButtonState.Pressed;
        }

        public static bool MouseMiddleUp()
        {
            return _msCurrentState.MiddleButton == ButtonState.Released;
        }

        public static bool MouseMiddlePressed()
        {
            return _msCurrentState.MiddleButton == ButtonState.Pressed && _msPrevState.MiddleButton == ButtonState.Released;
        }

        /* GAMEPAD */
        public static GamePadDeadZone GamepadDeadZoneMode { get; set; } = GamePadDeadZone.Circular;

        public static bool GamePadDown(GamePadButtons button, GamePadIndex playerIndex = 0)
        {
            return _gpCurrentState[(int)playerIndex][button];
        }

        public static bool GamePadPressed(GamePadButtons button, GamePadIndex playerIndex = 0)
        {
            return _gpCurrentState[(int)playerIndex][button] && !_gpPrevState[(int)playerIndex][button];
        }

        public static bool GamePadReleased(GamePadButtons button, GamePadIndex playerIndex = 0)
        {
            return !_gpCurrentState[(int)playerIndex][button] && _gpPrevState[(int)playerIndex][button];
        }

        public static GamePadState GetGamePadState(GamePadIndex playerIndex = 0)
        {
            return _gpCurrentState[(int)playerIndex];
        }

        /* KEYBOARD */

        public static bool KeyDown(Keys key)
        {
            return _kbCurrentState.IsKeyDown(key);
        }

        public static bool KeyPressed(Keys key)
        {
            return _kbCurrentState.IsKeyDown(key) && _kbPrevState.IsKeyUp(key);
        }

        public static bool KeyReleased(Keys key)
        {
            return _kbCurrentState.IsKeyUp(key) && _kbPrevState.IsKeyDown(key);
        }


        /* ============================================== */

        private static GamePadState[] _gpCurrentState;
        private static GamePadState[] _gpPrevState;

        private static KeyboardState _kbCurrentState;
        private static KeyboardState _kbPrevState;

        private static MouseState _msCurrentState;
        private static MouseState _msPrevState;

        internal static void Init()
        {
            Platform.MouseEnter = OnPlatformMouseOver;
            Platform.MouseLeave = OnPlatformMouseLeave;
            Platform.MouseDown = OnPlatformMouseDown;
            Platform.MouseUp = OnPlatformMouseUp;
            Platform.MouseMove = OnPlatformMouseMove;
            Platform.KeyDown = OnPlatformKeyDown;
            Platform.KeyUp = OnPlatformKeyUp;
            Platform.CharInput = OnPlatformCharInput;

            var gamepad_mappings_text_file = Engine.Content.Get<TextFile>("gamecontrollerdb");

            var gamepad_mappings_text_content = gamepad_mappings_text_file.JoinedText;

            Platform.SetGamePadMappingsFile(gamepad_mappings_text_content);

            Platform.PreLookForGamepads();

            _gpCurrentState = new GamePadState[GamePad.GamepadMaxCount];
            _gpPrevState = new GamePadState[GamePad.GamepadMaxCount];
           
        }

       
        internal static void Update()
        {
            if (KeyboardActive)
            {
                _kbPrevState = _kbCurrentState;
                _kbCurrentState = Platform.GetKeyboardState();
            }

            if (MouseActive)
            {
                _msPrevState = _msCurrentState;
                _msCurrentState = Platform.GetMouseState();
            }

            if (GamePadActive && GamePad.ConnectedGamePads > 0)
            {
                if (GamePad.ConnectedGamePads == 1)
                {
                    _gpPrevState[0] = _gpCurrentState[0];
                    _gpCurrentState[0] = GamePad.GetState(GamePadIndex.One);                        
                }
                else
                {
                    for (int i = 0; i < GamePad.ConnectedGamePads; ++i)
                    {
                        _gpPrevState[i] = _gpCurrentState[i];
                        _gpCurrentState[i] = GamePad.GetState((GamePadIndex)i);                
                    }
                }
            }
        }

        private static void OnPlatformMouseLeave()
        {
            IsMouseOver = false;
            OnMouseLeave?.Invoke();
        }

        private static void OnPlatformMouseOver()
        {
            IsMouseOver = true;
            OnMouseEnter?.Invoke();
        }

        private static void OnPlatformMouseMove(int x, int y)
        {
            var converted = Engine.Canvas.ConvertToLocalCoord(x, y);
            OnMouseMove?.Invoke(converted.X, converted.Y);
        }

        private static void OnPlatformMouseUp(MouseButton button)
        {
            OnMouseUp?.Invoke(button);
        }

        private static void OnPlatformMouseDown(MouseButton button)
        {
            OnMouseDown?.Invoke(button);
        }

        private static void OnPlatformKeyDown(Keys key)
        {
            OnKeyDown?.Invoke(key);

            if (OnKeyPress != null)
            {
                if (_mLastPressedKey != key)
                {
                    _mLastPressedKey = key;

                    OnKeyPress.Invoke(key);
                }
            }
        }

        private static void OnPlatformKeyUp(Keys key)
        {
            OnKeyUp?.Invoke(key);
        }


        private static void OnPlatformCharInput(char ch)
        {
            OnTextInput?.Invoke(ch);
        }
    }
}
