namespace OMEGA
{
    public static class GamePad
	{
		/* Based on the XInput constants */
		internal const float LEFT_DEAD_ZONE = 7849.0f / 32768.0f;
		internal const float RIGHT_DEAD_ZONE = 8689.0f / 32768.0f;
		internal const float TRIGGER_THRESHOLD = 30.0f / 255.0f;

		public static int ConnectedGamePads {get; internal set;}

		public static readonly int GamepadMaxCount = 4;

		public static GamePadCapabilities GetCapabilities(GamePadIndex gamepadIndex)
		{
			return Platform.GetGamePadCapabilities((int) gamepadIndex);
		}

		public static GamePadState GetState(GamePadIndex gamepadIndex)
		{
			return Platform.GetGamePadState(
				(int) gamepadIndex,
				GamePadDeadZone.IndependentAxes
			);
		}

		public static GamePadState GetState(GamePadIndex gamepadIndex, GamePadDeadZone deadZoneMode)
		{
			return Platform.GetGamePadState(
				(int) gamepadIndex,
				deadZoneMode
			);
		}

		public static bool SetVibration(GamePadIndex gamepadIndex, float leftMotor, float rightMotor)
		{
			return Platform.SetGamePadVibration(
				(int) gamepadIndex,
				leftMotor,
				rightMotor
			);
		}

		public static string GetGuid(GamePadIndex gamepadIndex)
		{
			return Platform.GetGamePadGuid((int) gamepadIndex);
		}

		public static void SetLightBar(GamePadIndex gamepadIndex, Color color)
		{
			Platform.SetGamePadLightBar((int) gamepadIndex, color);
		}

		internal static float ExcludeAxisDeadZone(float value, float deadZone)
		{
			if (value < -deadZone)
			{
				value += deadZone;
			}
			else if (value > deadZone)
			{
				value -= deadZone;
			}
			else
			{
				return 0.0f;
			}
			return value / (1.0f - deadZone);
		}

	}
}
