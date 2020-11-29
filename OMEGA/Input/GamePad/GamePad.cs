namespace OMEGA
{
    public static class GamePad
	{
		/* Based on the XInput constants */
		internal const float LeftDeadZone = 7849.0f / 32768.0f;
		internal const float RightDeadZone = 8689.0f / 32768.0f;
		internal const float TriggerThreshold = 30.0f / 255.0f;


		internal static readonly int GAMEPAD_COUNT = 4;

		public static GamePadCapabilities GetCapabilities(GamePadIndex gamepad_index)
		{
			return Platform.GetGamePadCapabilities((int) gamepad_index);
		}

		public static GamePadState GetState(GamePadIndex gamepad_index)
		{
			return Platform.GetGamePadState(
				(int) gamepad_index,
				GamePadDeadZone.IndependentAxes
			);
		}

		public static GamePadState GetState(GamePadIndex gamepad_index, GamePadDeadZone deadZoneMode)
		{
			return Platform.GetGamePadState(
				(int) gamepad_index,
				deadZoneMode
			);
		}

		public static bool SetVibration(GamePadIndex gamepad_index, float leftMotor, float rightMotor)
		{
			return Platform.SetGamePadVibration(
				(int) gamepad_index,
				leftMotor,
				rightMotor
			);
		}

		public static string GetGuid(GamePadIndex gamepad_index)
		{
			return Platform.GetGamePadGUID((int) gamepad_index);
		}

		public static void SetLightBar(GamePadIndex gamepad_index, Color color)
		{
			Platform.SetGamePadLightBar((int) gamepad_index, color);
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
