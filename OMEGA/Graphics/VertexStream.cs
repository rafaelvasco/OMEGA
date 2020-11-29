using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OMEGA
{
    public enum VertexStreamMode
    {
        Static,
        Dynamic,
        Stream
    }

    public unsafe class VertexStream<T> where T : struct, IVertexType
    {
        public int VertexCount => vertex_count;

        private readonly IndexBuffer index_buffer;
        private readonly VertexBuffer static_vertex_buffer;
        private readonly DynamicVertexBuffer dynamic_vertex_buffer;
        private readonly VertexLayout vertex_layout;
        private readonly VertexStreamMode stream_mode;
        private readonly T[] vertices;
        private readonly int max_vertex_count;
        private bool vertex_array_changed = true;
        private int vertex_count;
        private int index_count;

        public VertexStream(int max_vertex_count, ushort[] indices, VertexStreamMode stream_mode = VertexStreamMode.Stream)
        {
            if (stream_mode == VertexStreamMode.Static)
            {
                throw new Exception("Invalid VertexStream mode for this constructor: A static VertexStream must be constructed with a pre made vertex array");
            }

            this.stream_mode = stream_mode;

            this.max_vertex_count = max_vertex_count;

            vertices = new T[max_vertex_count];

            if (stream_mode == VertexStreamMode.Dynamic)
            {
                dynamic_vertex_buffer = DynamicVertexBuffer.Create(vertices);
            }

            vertex_layout = vertices[0].Layout;
            vertex_count = 0;
            index_count = 0;
            index_buffer = IndexBuffer.Create(indices);
        }

        public VertexStream(Span<T> _vertices, ushort[] indices = null, VertexStreamMode stream_mode = VertexStreamMode.Static)
        {
            this.stream_mode = stream_mode;

            max_vertex_count = _vertices.Length;

            vertex_count = _vertices.Length;

            vertex_layout = _vertices[0].Layout;

            vertices = new T[_vertices.Length];

            Unsafe.CopyBlockUnaligned(Unsafe.AsPointer(ref vertices[0]), Unsafe.AsPointer(ref _vertices[0]), (uint)(_vertices.Length * Unsafe.SizeOf<T>()));

            switch (stream_mode)
            {
                case VertexStreamMode.Static:
                    static_vertex_buffer = VertexBuffer.Create(vertices);
                    break;
                case VertexStreamMode.Dynamic:
                    dynamic_vertex_buffer = DynamicVertexBuffer.Create(vertices);
                    break;
            }

            if (indices != null)
            {
                index_count = indices.Length;

                index_buffer = IndexBuffer.Create(indices);
            }
        }

        public static VertexStream<VertexPositionColorTexture> FromQuad(Quad quad, VertexStreamMode stream_mode = VertexStreamMode.Static)
        {
            return new VertexStream<VertexPositionColorTexture>(
                new Span<VertexPositionColorTexture>(Unsafe.AsPointer(ref quad.V0), 4),
                new ushort[] {0, 1, 2, 0, 2, 3},
                stream_mode
            );
        }

        public void Reset()
        {
            vertex_count = 0;
            index_count = 0;
        }

        public Span<T> GetNextVertexFragment(int frag_vertex_count)
        {
            if (stream_mode == VertexStreamMode.Static)
            {
                throw new Exception("Trying to update Immutable VertexStream");
            }

            if (vertex_count + frag_vertex_count > max_vertex_count)
            {
                return null;
            }

            int current_start_index = vertex_count;

            vertex_count += frag_vertex_count;

            index_count = vertex_count / 4 * 6;

            vertex_array_changed = true;

            return new Span<T>(vertices, current_start_index, frag_vertex_count);
        }

        internal void SubmitSpan(int start_vertex_index, int vertex_count, int start_indice_index, int index_count)
        {
            if (index_count > 0)
            {
                GraphicsContext.SetIndexBuffer(index_buffer.Handle, start_indice_index, index_count);
            }

            switch (stream_mode)
            {
                case VertexStreamMode.Static:
                    GraphicsContext.SetVertexBuffer(0, static_vertex_buffer.Handle, start_vertex_index, vertex_count);
                    break;

                case VertexStreamMode.Dynamic:

                    if (vertex_array_changed)
                    {
                        GraphicsContext.UpdateDynamicVertexBuffer(dynamic_vertex_buffer.Handle, 0, vertices);
                        vertex_array_changed = false;
                    }
                    
                    GraphicsContext.SetDynamicVertexBuffer(0, dynamic_vertex_buffer.Handle, start_vertex_index, vertex_count);
                    break;

                case VertexStreamMode.Stream:
                    var vertices_span = new Span<T>(vertices, start_vertex_index, vertex_count);
                    var transient_buffer = TransientVertexBuffer.Create(vertices_span, vertex_layout);
                    GraphicsContext.SetTransientVertexBuffer(0, transient_buffer, 0, vertex_count);
                    break;
            }
        }

        internal void Submit()
        {
            SubmitSpan(0, vertex_count, 0, index_count);
        }

    }
}
