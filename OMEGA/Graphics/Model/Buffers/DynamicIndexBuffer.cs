using static Bgfx.Bgfx;

namespace OMEGA
{
    /// <summary>
    /// Represents a dynamically updateable index buffer.
    /// </summary>
    public class DynamicIndexBuffer : GraphicsResource
    {
        internal DynamicIndexBufferHandle Handle { get; private init; }

        private DynamicIndexBuffer() { }

        public static DynamicIndexBuffer Create(int indexCount)
        {
            var index_buffer = new DynamicIndexBuffer
            {
                Handle = GraphicsContext.CreateDynamicIndexBuffer(indexCount)
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


        public void Update(int startIndex, ushort[] indices)
        {
            GraphicsContext.UpdateDynamicIndexBuffer(this.Handle, startIndex, indices);
        }

        /// <summary>
        /// Releases the index buffer.
        /// </summary>
        protected override void Free()
        {
            base.Free();

            if (Handle.Valid)
            {
                GraphicsContext.DestroyDynamicIndexBuffer(Handle);
            }
        }
    }
}
