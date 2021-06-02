using static Bgfx.Bgfx;

namespace OMEGA
{
    /// <summary>
    /// Represents a dynamically updateable vertex buffer.
    /// </summary>
    public unsafe class DynamicVertexBuffer : GraphicsResource
    {
        internal DynamicVertexBufferHandle Handle { get; private set; }

        private DynamicVertexBuffer() { }

        public static DynamicVertexBuffer Create(int vertexCount, VertexLayout layout, BufferFlags flags = BufferFlags.None)
        {
            var vertex_buffer = new DynamicVertexBuffer
            {
                Handle = GraphicsContext.CreateDynamicVertexBuffer(vertexCount, layout, flags)
            };

            GraphicsContext.RegisterAllocatedResource(vertex_buffer);

            return vertex_buffer;
        }

        public static DynamicVertexBuffer Create(VertexPositionColorTexture[] vertices, VertexLayout layout, BufferFlags flags = BufferFlags.None)
        {
            var vertex_buffer = new DynamicVertexBuffer
            {
                Handle = GraphicsContext.CreateDynamicVertexBuffer(vertices, layout, flags)
            };

            GraphicsContext.RegisterAllocatedResource(vertex_buffer);

            return vertex_buffer;
        }


        public void Update(int startVertex, VertexPositionColorTexture[] vertices)
        {
            GraphicsContext.UpdateDynamicVertexBuffer(this.Handle, startVertex, vertices);
        }

        /// <summary>
        /// Releases the vertex buffer.
        /// </summary>
        protected override void Free()
        {
            base.Free();

            if (Handle.Valid)
            {
                GraphicsContext.DestroyDynamicVertexBuffer(Handle);
            }

            
        }

    }
}
