using System;

namespace OMEGA
{
    /// <summary>
    /// Represents a dynamically updateable index buffer.
    /// </summary>
    public unsafe class DynamicIndexBuffer : GraphicsResource
    {
        internal DynamicIndexBufferHandle Handle { get; private set; }

        private DynamicIndexBuffer() { }

        public static DynamicIndexBuffer Create(int index_count)
        {
            var index_buffer = new DynamicIndexBuffer
            {
                Handle = GraphicsContext.CreateDynamicIndexBuffer(index_count)
            };

            GraphicsContext.RegisterAllocatedResource(index_buffer);

            return index_buffer;
        }

        public static DynamicIndexBuffer Create(ushort[] indices, BufferFlags flags = BufferFlags.None)
        {
            var index_buffer = new DynamicIndexBuffer
            {
                Handle = GraphicsContext.CreateDynamicIndexBuffer(indices, flags)
            };

            GraphicsContext.RegisterAllocatedResource(index_buffer);

            return index_buffer;
        }


        public void Update(int start_index, ushort[] indices)
        {
            GraphicsContext.UpdateDynamicIndexBuffer(this.Handle, start_index, indices);
        }

        ~DynamicIndexBuffer()
        {
            if (Handle.Valid)
            {
                GraphicsContext.DestroyDynamicIndexBuffer(Handle);
            }

            throw new Exception("Graphics Resource Leak: Dynamic Index Buffer");
        }

        /// <summary>
        /// Releases the index buffer.
        /// </summary>
        protected override void Dispose()
        {
            if (!Handle.Valid)
            {
                return;
            }

            GraphicsContext.DestroyDynamicIndexBuffer(Handle);
        }

    }
}
