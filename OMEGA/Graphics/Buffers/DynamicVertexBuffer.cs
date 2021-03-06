﻿using System;

namespace OMEGA
{
    /// <summary>
    /// Represents a dynamically updateable vertex buffer.
    /// </summary>
    public unsafe class DynamicVertexBuffer : GraphicsResource
    {
        internal DynamicVertexBufferHandle Handle { get; private set; }

        private DynamicVertexBuffer() { }

        public static DynamicVertexBuffer Create(int vertex_count, VertexLayout layout, BufferFlags flags = BufferFlags.None)
        {
            var vertex_buffer = new DynamicVertexBuffer
            {
                Handle = GraphicsContext.CreateDynamicVertexBuffer(vertex_count, layout, flags)
            };

            GraphicsContext.RegisterAllocatedResource(vertex_buffer);

            return vertex_buffer;
        }

        public static DynamicVertexBuffer Create(Vertex[] vertices, VertexLayout layout, BufferFlags flags = BufferFlags.None)
        {
            var vertex_buffer = new DynamicVertexBuffer
            {
                Handle = GraphicsContext.CreateDynamicVertexBuffer(vertices, layout, flags)
            };

            GraphicsContext.RegisterAllocatedResource(vertex_buffer);

            return vertex_buffer;
        }


        public void Update(int start_vertex, Vertex[] vertices)
        {
            GraphicsContext.UpdateDynamicVertexBuffer(this.Handle, start_vertex, vertices);
        }

        ~DynamicVertexBuffer()
        {
            if (Handle.Valid)
            {
                GraphicsContext.DestroyDynamicVertexBuffer(Handle);
            }

            throw new Exception("Graphics Resource Leak: Dynamic Vertex Buffer");
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

            GraphicsContext.DestroyDynamicVertexBuffer(Handle);
        }

    }
}
