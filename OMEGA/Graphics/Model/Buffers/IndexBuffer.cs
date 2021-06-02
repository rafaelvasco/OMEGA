using System;
using static Bgfx.Bgfx;

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


        protected override void Free()
        {
            base.Free();

            if (Handle.Valid)
            {
                GraphicsContext.DestroyIndexBuffer(Handle);
            }
        }
    }
}
