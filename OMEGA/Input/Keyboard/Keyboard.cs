using System.Collections.Generic;

namespace OMEGA
{
    /// <summary>
	/// Allows getting keystrokes from keyboard.
	/// </summary>
	public static class Keyboard
	{
		public static KeyboardState GetState()
		{
			return new KeyboardState(Keys);
		}

		internal static List<Keys> Keys = new List<Keys>();

	}
}
