
using System;

namespace OMEGA
{
    public static class TextInput
	{

		/// <summary>
		/// Use this event to retrieve text for objects like textboxes.
		/// This event is not raised by noncharacter keys.
		/// This event also supports key repeat.
		/// For more information this event is based off:
		/// http://msdn.microsoft.com/en-AU/library/system.windows.forms.control.keypress.aspx
		/// </summary>
		public static event Action<char> OnTextInput;

		/// <summary>
		/// This event notifies you of in-progress text composition happening in an IME or other tool
		///  and allows you to display the draft text appropriately before it has become input.
		/// For more information, see SDL's tutorial: https://wiki.libsdl.org/Tutorials/TextInput
		/// </summary>
		public static event Action<string, int, int> OnTextEditing;


		public static void StartTextInput()
		{
			Platform.StartTextInput();
		}

		public static void StopTextInput()
		{
			Platform.StopTextInput();
		}

		/// <summary>
		/// Sets the location within the game window where the text input is located.
		/// This is used to set the location of the IME suggestions
		/// </summary>
		/// <param name="rectangle">Text input location relative to GameWindow.ClientBounds</param>
		public static void SetInputRectangle(Rect rectangle)
		{
			Platform.SetTextInputRectangle(rectangle);
		}

		internal static void ProcessTextInput(char c)
		{
			OnTextInput?.Invoke(c);
		}

		internal static void ProcessTextEditing(string text, int start, int length)
		{
			OnTextEditing?.Invoke(text, start, length);
		}
	}
}
