namespace OMEGA
{
    public static class Input
    {
        public delegate void GenericInputEvent();

        public static bool KeyboardActive { get; set; } = true;
        public static bool MouseActive { get; set; } = true;
        public static bool GamePadActive { get; set; } = true;

        /* MOUSE */

        public static event GenericInputEvent OnMouseEnter;
        public static event GenericInputEvent OnMouseLeave;

        public static bool IsMouseOver { get; internal set; }

        public static Point MousePos => new Point(
          ms_current_state.X,
          ms_current_state.Y);

        public static int MouseWheel => ms_current_state.ScrollWheelValue;

        public static bool MouseLeftDown()
        {
            return ms_current_state.LeftButton == ButtonState.Pressed;
        }

        public static bool MouseLeftUp()
        {
            return ms_current_state.LeftButton == ButtonState.Released;
        }

        public static bool MouseLeftJustPressed()
        {
            return ms_current_state.LeftButton == ButtonState.Pressed && ms_prev_state.LeftButton == ButtonState.Released;
        }

        public static bool MouseRightDown()
        {
            return ms_current_state.RightButton == ButtonState.Pressed;
        }

        public static bool MouseRightUp()
        {
            return ms_current_state.RightButton == ButtonState.Released;
        }

        public static bool MouseRightJustPressed()
        {
            return ms_current_state.RightButton == ButtonState.Pressed && ms_prev_state.RightButton == ButtonState.Released;
        }

        public static bool MouseMiddleDown()
        {
            return ms_current_state.MiddleButton == ButtonState.Pressed;
        }

        public static bool MouseMiddleUp()
        {
            return ms_current_state.MiddleButton == ButtonState.Released;
        }

        public static bool MouseMiddleJustPressed()
        {
            return ms_current_state.MiddleButton == ButtonState.Pressed && ms_prev_state.MiddleButton == ButtonState.Released;
        }

        /* GAMEPAD */
        public static GamePadDeadZone GamepadDeadZoneMode { get; set; } = GamePadDeadZone.Circular;

        public static bool GamePadDown(GamePadButtons button, GamePadIndex player_index = 0)
        {
            return gp_current_state[(int)player_index][button];
        }

        public static bool GamePadPressed(GamePadButtons button, GamePadIndex player_index = 0)
        {
            return gp_current_state[(int)player_index][button] && !gp_prev_state[(int)player_index][button];
        }

        public static bool GamePadReleased(GamePadButtons button, GamePadIndex player_index = 0)
        {
            return !gp_current_state[(int)player_index][button] && gp_prev_state[(int)player_index][button];
        }

        public static GamePadState GetGamePadState(GamePadIndex player_index = 0)
        {
            return gp_current_state[(int)player_index];
        }

        /* KEYBOARD */

        public static bool KeyDown(Keys key)
        {
            return kb_current_state.IsKeyDown(key);
        }

        public static bool KeyPressed(Keys key)
        {
            return kb_current_state.IsKeyDown(key) && kb_prev_state.IsKeyUp(key);
        }

        public static bool KeyReleased(Keys key)
        {
            return kb_current_state.IsKeyUp(key) && kb_prev_state.IsKeyDown(key);
        }


        /* ============================================== */

        private static GamePadState[] gp_current_state;
        private static GamePadState[] gp_prev_state;

        private static KeyboardState kb_current_state;
        private static KeyboardState kb_prev_state;

        private static MouseState ms_current_state;
        private static MouseState ms_prev_state;

        internal static void Init()
        {
            Platform.OnMouseEnter = OnPlatformMouseOver;
            Platform.OnMouseLeave = OnPlatformMouseLeave;

            var gamepad_mappings_text_file = Engine.Content.Get<TextFile>("gamecontrollerdb");

            var gamepad_mappings_text_content = gamepad_mappings_text_file.JoinedText;

            Platform.SetGamePadMappingsFile(gamepad_mappings_text_content);

            Platform.PreLookForGamepads();

            gp_current_state = new GamePadState[GamePad.GAMEPAD_MAX_COUNT];
            gp_prev_state = new GamePadState[GamePad.GAMEPAD_MAX_COUNT];
           
        }

        internal static void Update()
        {
            if (KeyboardActive)
            {
                kb_prev_state = kb_current_state;
                kb_current_state = Keyboard.GetState();
            }

            if (MouseActive)
            {
                ms_prev_state = ms_current_state;
                ms_current_state = Mouse.GetState();
            }

            if (GamePadActive && GamePad.ConnectedGamePads > 0)
            {
                if (GamePad.ConnectedGamePads == 1)
                {
                    gp_prev_state[0] = gp_current_state[0];
                    gp_current_state[0] = GamePad.GetState(GamePadIndex.One);                        
                }
                else
                {
                    for (int i = 0; i < GamePad.ConnectedGamePads; ++i)
                    {
                        gp_prev_state[i] = gp_current_state[i];
                        gp_current_state[i] = GamePad.GetState((GamePadIndex)i);                
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
    }
}
