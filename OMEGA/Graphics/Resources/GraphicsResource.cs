using System;

namespace OMEGA
{
    public abstract class GraphicsResource
    {
        protected abstract void Dispose();

        public void Dispose(bool unregister = true)
        {
            Dispose();

            if (unregister)
            {
                GraphicsContext.UnregisterAllocatedResource(this);
            }
        }
    }
}
