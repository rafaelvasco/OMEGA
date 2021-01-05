using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OMEGA
{
    internal enum VertexStreamMode
    {
        Static,
        Dynamic,
        Stream
    }

    internal unsafe class VertexStream : IDisposable
    {
        private const int INITIAL_MAX_BUFFER = 1024;
        private const int MAX_SIZE_BUFFER = sizeof(ushort);

        public int VertexCount => m_vertex_count;

        private readonly IndexBuffer m_static_index_buffer;
        private readonly VertexBuffer m_static_vertex_buffer;
        private readonly DynamicVertexBuffer m_dynamic_vertex_buffer;
        private readonly DynamicIndexBuffer m_dynamic_index_buffer;

        private readonly VertexStreamMode m_stream_mode;
        private Vertex[] m_vertices;
        private ushort[] m_indices;
        private bool m_stream_changed = true;
        private int m_vertex_count;
        private int m_index_count;

        public VertexStream(VertexStreamMode stream_mode = VertexStreamMode.Stream)
        {
            if (stream_mode == VertexStreamMode.Static)
            {
                throw new Exception("Invalid VertexStream mode for this constructor: A static VertexStream must be constructed with a pre made vertex array");
            }

            m_stream_mode = stream_mode;

            m_vertices = new Vertex[INITIAL_MAX_BUFFER];
            m_indices = new ushort[INITIAL_MAX_BUFFER];

            if (stream_mode == VertexStreamMode.Dynamic)
            {
                m_dynamic_vertex_buffer = DynamicVertexBuffer.Create(m_vertices);
                m_dynamic_index_buffer = DynamicIndexBuffer.Create(m_indices);
            }

            m_vertex_count = 0;
            m_index_count = 0;
        }


        public VertexStream(Span<Vertex> vertices, int[] indices)
        {
            m_stream_mode = VertexStreamMode.Static;

            m_vertex_count = vertices.Length;

            m_vertices = new Vertex[vertices.Length];

            Unsafe.CopyBlockUnaligned(Unsafe.AsPointer(ref m_vertices[0]), Unsafe.AsPointer(ref vertices[0]), (uint)(vertices.Length * Unsafe.SizeOf<Vertex>()));

            m_indices = new ushort[indices.Length];

            Unsafe.CopyBlockUnaligned(Unsafe.AsPointer(ref m_indices[0]), Unsafe.AsPointer(ref indices[0]), (uint)(indices.Length * sizeof(ushort)));

            m_index_count = indices.Length;

            m_static_index_buffer = IndexBuffer.Create(m_indices);
            m_static_vertex_buffer = VertexBuffer.Create(m_vertices);
        }

        public void Reset()
        {
            m_vertex_count = 0;
            m_index_count = 0;
        }

        public bool PushTriangle(Vertex v1, Vertex v2, Vertex v3)
        {
            Span<Vertex> next_vertex_fragment = GetNextTriangleFragment();

            if (next_vertex_fragment == null)
            {
                return false;
            }

            fixed (Vertex* vertex_ptr = &MemoryMarshal.GetReference(next_vertex_fragment))
            {
                *(vertex_ptr) = v1;
                *(vertex_ptr + 1) = v2;
                *(vertex_ptr + 2) = v3;
            }

            return true;
        }

        public bool PushQuad(in Quad quad)
        {
            var v0 = quad.V0;
            var v1 = quad.V1;
            var v2 = quad.V2;
            var v3 = quad.V3;

            Span<Vertex> next_quad_fragment = GetNextQuadFragment();

            if (next_quad_fragment == null)
            {
                return false;
            }

            fixed (Vertex* vertex_ptr = &MemoryMarshal.GetReference(next_quad_fragment))
            {
                *(vertex_ptr) = new Vertex(v0.X, v0.Y, v0.Z, v0.Col, v0.Tx, v0.Ty);
                *(vertex_ptr + 1) = new Vertex(v1.X, v1.Y, v1.Z, v1.Col, v1.Tx, v1.Ty);
                *(vertex_ptr + 2) = new Vertex(v2.X, v2.Y, v2.Z, v2.Col, v2.Tx, v2.Ty); ;
                *(vertex_ptr + 3) = new Vertex(v3.X, v3.Y, v3.Z, v3.Col, v3.Tx, v3.Ty);
            }

            return true;
        }

        private Span<Vertex> GetNextTriangleFragment()
        {
            if (m_stream_mode == VertexStreamMode.Static)
            {
                throw new Exception("Trying to update Immutable VertexStream");
            }

            while (m_index_count + 3 >= m_indices.Length)
            {
                if (m_index_count + 3 > MAX_SIZE_BUFFER)
                {
                    return null;
                }

                Array.Resize(ref m_indices, m_indices.Length * 2);
            }

            while (m_vertex_count + 3 >= m_vertices.Length)
            {
                Array.Resize(ref m_vertices, m_vertices.Length * 2);
            }

            ushort base_idx = (ushort)m_vertex_count;
            fixed (ushort* index_ptr = &MemoryMarshal.GetArrayDataReference(m_indices))
            {
                ushort* ptr = index_ptr + m_index_count;
                *(ptr) = base_idx;
                *(ptr + 1) = (ushort)(base_idx + 1);
                *(ptr + 2) = (ushort)(base_idx + 2);
            }

            int current_start_index = m_vertex_count;

            m_vertex_count += 3;

            m_index_count += 3;

            m_stream_changed = true;

            return new Span<Vertex>(m_vertices, current_start_index, 3);
        }

        private Span<Vertex> GetNextQuadFragment()
        {
            if (m_stream_mode == VertexStreamMode.Static)
            {
                throw new Exception("Trying to update Immutable VertexStream");
            }

            while (m_index_count + 6 >= m_indices.Length)
            {
                if (m_index_count + 6 > MAX_SIZE_BUFFER)
                {
                    return null;
                }

                Array.Resize(ref m_indices, m_indices.Length * 2);
            }

            while (m_vertex_count + 4 >= m_vertices.Length)
            {
                Array.Resize(ref m_vertices, m_vertices.Length * 2);
            }

            ushort base_idx = (ushort)m_vertex_count;
            fixed (ushort* index_ptr = &MemoryMarshal.GetArrayDataReference(m_indices))
            {
                ushort* ptr = index_ptr + m_index_count;
                *(ptr) = base_idx;
                *(ptr + 1) = (ushort)(base_idx + 1);
                *(ptr + 2) = (ushort)(base_idx + 2);
                *(ptr + 3) = base_idx;
                *(ptr + 4) = (ushort)(base_idx + 2);
                *(ptr + 5) = (ushort)(base_idx + 3);
            }

            int current_start_index = m_vertex_count;

            m_vertex_count += 4;

            m_index_count += 6;

            m_stream_changed = true;

            return new Span<Vertex>(m_vertices, current_start_index, 4);
        }

        internal void SubmitSpan(int start_vertex_index, int vertex_count, int start_indice_index, int index_count)
        {
            switch (m_stream_mode)
            {
                case VertexStreamMode.Static:
                    GraphicsContext.SetIndexBuffer(m_static_index_buffer.Handle, start_indice_index, index_count);
                    GraphicsContext.SetVertexBuffer(0, m_static_vertex_buffer.Handle, start_vertex_index, vertex_count);
                    break;

                case VertexStreamMode.Dynamic:

                    if (m_stream_changed)
                    {
                        GraphicsContext.UpdateDynamicIndexBuffer(m_dynamic_index_buffer.Handle, start_indice_index, m_indices);
                        GraphicsContext.UpdateDynamicVertexBuffer(m_dynamic_vertex_buffer.Handle, 0, m_vertices);
                        m_stream_changed = false;
                    }

                    GraphicsContext.SetDynamicIndexBuffer(m_dynamic_index_buffer.Handle, start_indice_index, index_count);
                    GraphicsContext.SetDynamicVertexBuffer(0, m_dynamic_vertex_buffer.Handle, start_vertex_index, vertex_count);
                    break;

                case VertexStreamMode.Stream:
                    var vertices_span = new Span<Vertex>(m_vertices, start_vertex_index, vertex_count);
                    var transient_buffer = TransientVertexBuffer.Create(vertices_span, Vertex.VertexLayout);

                    var indices_span = new Span<ushort>(m_indices, start_indice_index, index_count);
                    var transient_idx_buffer = TransientIndexBuffer.Create(indices_span);

                    GraphicsContext.SetTransientIndexBuffer(transient_idx_buffer, 0, index_count);
                    GraphicsContext.SetTransientVertexBuffer(0, transient_buffer, 0, vertex_count);
                    break;
            }
        }

        internal void Submit()
        {
            SubmitSpan(0, m_vertex_count, 0, m_index_count);
        }

        public void Dispose()
        {
            if (m_static_index_buffer != null)
            {
                m_static_index_buffer.Dispose();
            }

            if (m_static_vertex_buffer != null)
            {
                m_static_vertex_buffer.Dispose();
            }

            if (m_dynamic_vertex_buffer != null)
            {
                m_dynamic_vertex_buffer.Dispose();
            }

            if (m_dynamic_index_buffer != null)
            {
                m_dynamic_index_buffer.Dispose();
            }
        }
    }
}
