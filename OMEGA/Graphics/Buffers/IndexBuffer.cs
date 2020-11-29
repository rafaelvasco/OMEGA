using System;

namespace OMEGA
{
    public class IndexBuffer : GraphicsResource
    {
        internal IndexBufferHandle Handle {get; private set;}

        public static IndexBuffer Create(ushort[] indices, BufferFlags flags = BufferFlags.None)
        {
            var index_buffer = new IndexBuffer();

            if (indices != null)
            {
                index_buffer.Handle = GraphicsContext.CreateIndexBuffer(indices, flags);
            }
            else
            {
                index_buffer.Handle = default;
            }

            GraphicsContext.RegisterAllocatedResource(index_buffer);
            
            return index_buffer;
        }

        private IndexBuffer(){}

        ~IndexBuffer()
        {
            if (Handle.Valid)
            {
                GraphicsContext.DestroyIndexBuffer(Handle);
            }

            throw new Exception("Graphics Resource Leak: Index Buffer");
        }



        protected override void Dispose()
        {
            if (!Handle.Valid)
            {
                return;
            }

            GC.SuppressFinalize(this);
            GraphicsContext.DestroyIndexBuffer(Handle);
        }
    }
}
