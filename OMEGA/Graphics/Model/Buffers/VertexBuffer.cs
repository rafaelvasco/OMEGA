using System;
using static Bgfx.Bgfx;

namespace OMEGA
{
    /// <summary>
    /// Represents a static vertex buffer.
    /// </summary>
    public unsafe class VertexBuffer : GraphicsResource
    {

        internal VertexBufferHandle Handle { get; private set; }

        public static VertexBuffer Create(VertexPositionColorTexture[] vertices, VertexLayout layout, BufferFlags flags = BufferFlags.None)
        {
            var vertex_buffer = new VertexBuffer
            {
                Handle = GraphicsContext.CreateVertexBuffer(vertices, layout, flags)
            };

            GraphicsContext.RegisterAllocatedResource(vertex_buffer);

            return vertex_buffer;
        }

        private VertexBuffer() { }


        /// <summary>
        /// Releases the vertex buffer.
        /// </summary>
        protected override void Free()
        {
            base.Free();

            if (Handle.Valid)
            {
                GraphicsContext.DestroyVertexBuffer(Handle);
            }
        }
    }
}
