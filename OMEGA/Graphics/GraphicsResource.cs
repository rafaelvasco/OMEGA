using System;

namespace OMEGA
{
    public abstract class GraphicsResource
    {
        protected abstract void Dispose();

        public void Dispose(bool unregister)
        {
            Dispose();

            if (unregister)
            {
                GraphicsContext.UnregisterAllocatedResource(this);
            }
        }
    }
}
