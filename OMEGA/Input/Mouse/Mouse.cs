using System;

namespace OMEGA
{
    /// <summary>
	/// Allows reading position and button click information from mouse.
	/// </summary>
	public static class Mouse
	{
		public static Action<int> Clicked;

		public static bool IsRelativeMouseMode
		{
			get
			{
				return Platform.GetRelativeMouseMode();
			}
			set
			{
				Platform.SetRelativeMouseMode(value);
			}
		}

		internal static float DisplayScaleFactorX;

		internal static float DisplayScaleFactorY;

		private static int wheel_value = 0;

		/// <summary>
		/// Gets mouse state information that includes position and button
		/// presses for the provided window
		/// </summary>
		/// <returns>Current state of the mouse.</returns>
		public static MouseState GetState()
		{
            Platform.GetMouseState(
                out int x,
                out int y,
                out ButtonState left,
                out ButtonState middle,
                out ButtonState right,
                out ButtonState x1,
                out ButtonState x2
            );

            x = (int) ((double) x * DisplayScaleFactorX);
			y = (int) ((double) y * DisplayScaleFactorY);

			return new MouseState(
				x,
				y,
				wheel_value,
				left,
				middle,
				right,
				x1,
				x2
			);
		}

		/// <summary>
		/// Sets mouse cursor's relative position to game-window.
		/// </summary>
		/// <param name="x">Relative horizontal position of the cursor.</param>
		/// <param name="y">Relative vertical position of the cursor.</param>
		public static void SetPosition(int x, int y)
		{
			if (IsRelativeMouseMode)
			{
				return;
			}

			// Scale the mouse coordinates for the faux-backbuffer
			x = (int) ((double) x * DisplayScaleFactorX);
			y = (int) ((double) y * DisplayScaleFactorY);

			Platform.SetMousePosition(x, y);
		}

		internal static void ProcessClicked(int button)
		{
			Clicked?.Invoke(button);
		}

		internal static void ProcessMouseWheel(int wheel)
        {

        }

	}
}
