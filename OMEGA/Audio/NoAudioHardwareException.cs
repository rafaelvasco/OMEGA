using System;
using System.Runtime.InteropServices;

namespace OMEGA
{
    [Serializable]
	public sealed class NoAudioHardwareException : ExternalException
	{
		public NoAudioHardwareException()
		{
		}

		public NoAudioHardwareException(String message)
			: base(message)
		{
		}

		public NoAudioHardwareException(String message, Exception innerException)
			: base(message, innerException)
		{
		}

        public NoAudioHardwareException(string message, int errorCode) : base(message, errorCode)
        {
        }
    }
}
