using System;

namespace OMEGA
{
    /// <summary>
    /// Represents a static vertex buffer.
    /// </summary>
    public unsafe class VertexBuffer : GraphicsResource
    {

        internal VertexBufferHandle Handle { get; private set; }

        public static VertexBuffer Create<T>(T[] vertices, BufferFlags flags = BufferFlags.None) where T : struct, IVertexType
        {
            var vertex_buffer = new VertexBuffer
            {
                Handle = GraphicsContext.CreateVertexBuffer(vertices, vertices[0].Layout, flags)
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
