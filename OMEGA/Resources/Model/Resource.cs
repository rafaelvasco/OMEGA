﻿using System;

namespace OMEGA
{
    public abstract class Resource : IDisposable
    {
        private bool _disposedValue;

        public string Id {get; internal set;}

        private void InternalFree(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    FreeManaged();
                }

                FreeUnmanaged();

                _disposedValue = true;
            }
        }

        protected virtual void FreeManaged() {}

        protected virtual void FreeUnmanaged() {}

        ~Resource()
        {
            InternalFree(disposing: false);
            throw new Exception("Resource Leak");
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            InternalFree(disposing: true);
        }
    }
}
