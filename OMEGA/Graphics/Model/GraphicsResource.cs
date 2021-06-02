using System;

namespace OMEGA
{
    public abstract class GraphicsResource : IDisposable
    {
        protected virtual void Free()
        {
           GraphicsContext.UnregisterAllocatedResource(this);
        }

        ~GraphicsResource()
        {
            Free();
            throw new Exception("Graphics Resource Leak");
        }

        public void Dispose()
        {
            Free();
            GC.SuppressFinalize(this);
        }
    }
}
