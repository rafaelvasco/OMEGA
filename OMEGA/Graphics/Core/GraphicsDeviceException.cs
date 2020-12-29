using System;

namespace OMEGA
{
    [Serializable]
    internal class GraphicsDeviceException : Exception 
    {
        public GraphicsDeviceException()
			: base()
		{
		}

		public GraphicsDeviceException(string message)
			: base(message)
		{
		}

		public GraphicsDeviceException(string message, Exception inner)
			: base(message, inner)
		{
		}
    }
}
