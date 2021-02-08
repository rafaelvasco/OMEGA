using System;

namespace OMEGA
{
    /// <summary>
    /// Represents a static vertex buffer.
    /// </summary>
    public unsafe class VertexBuffer : GraphicsResource
    {

        internal VertexBufferHandle Handle { get; private set; }

        public static VertexBuffer Create(Vertex[] vertices, VertexLayout layout, BufferFlags flags = BufferFlags.None)
        {
            var vertex_buffer = new VertexBuffer
            {
                Handle = GraphicsContext.CreateVertexBuffer(vertices, layout, flags)
            };

            GraphicsContext.RegisterAllocatedResource(vertex_buffer);

            return vertex_buffer;
        }

        private VertexBuffer() { }

        ~VertexBuffer()
        {
            if (Handle.Valid)
            {
                GraphicsContext.DestroyVertexBuffer(Handle);
            }

            throw new Exception("Graphics Resource Leak: Vertex Buffer");
        }

        /// <summary>
        /// Releases the vertex buffer.
        /// </summary>
        protected override void Dispose()
        {
            if (!Handle.Valid)
            {
                return;
            }

            GC.SuppressFinalize(this);
            GraphicsContext.DestroyVertexBuffer(Handle);
        }
    }
}
