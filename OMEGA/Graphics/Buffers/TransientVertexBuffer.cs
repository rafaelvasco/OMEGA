using System;
using System.Runtime.CompilerServices;

namespace OMEGA
{
    public unsafe struct TransientVertexBuffer {
        
        readonly IntPtr data;
        int size;
        int startVertex;
        ushort stride;
        readonly ushort handle;
        ushort decl;

        /// <summary>
        /// A pointer that can be filled with vertex data.
        /// </summary>
        public IntPtr Data { get { return data; } }

        /// <summary>
        /// The size of the buffer.
        /// </summary>
        public int Count { get { return size; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransientVertexBuffer"/> struct.
        /// </summary>
        /// <param name="vertex_count">The number of vertices that fit in the buffer.</param>
        /// <param name="layout">The layout of the vertex data.</param>
        private TransientVertexBuffer (int vertex_count, VertexLayout layout) {
            GraphicsContext.AllocTransientVertexBuffer(out this, vertex_count, ref layout);
        }

        public static TransientVertexBuffer Create<T>(Span<T> vertices, VertexLayout layout)
        {
            var transient_vtx_buffer = new TransientVertexBuffer(vertices.Length, layout);

            var data_size = (uint)(vertices.Length * Unsafe.SizeOf<T>());

            Unsafe.CopyBlock((void*)transient_vtx_buffer.data, Unsafe.AsPointer(ref vertices[0]), data_size);

            return transient_vtx_buffer;
        }

        /// <summary>
        /// Gets the available space in the global transient vertex buffer.
        /// </summary>
        /// <param name="count">The number of vertices required.</param>
        /// <param name="layout">The layout of each vertex.</param>
        /// <returns>The number of available vertices.</returns>
        public static int GetAvailableSpace (int count, VertexLayout layout) {
            return (int)GraphicsContext.GetAvailableTransientVertexBuffers(count, layout);
        }
    }
}
