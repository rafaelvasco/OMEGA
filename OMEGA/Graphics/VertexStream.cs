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
        public int VertexCount => m_vertex_count;

        private readonly IndexBuffer m_index_buffer;
        private readonly VertexBuffer m_static_vertex_buffer;
        private readonly DynamicVertexBuffer m_dynamic_vertex_buffer;
        private readonly VertexStreamMode m_stream_mode;
        private readonly Vertex[] m_vertices;
        private readonly int m_max_vertex_count;
        private bool m_vertex_array_changed = true;
        private int m_vertex_count;
        private int m_index_count;

        public VertexStream(int max_vertex_count, ushort[] indices = null, VertexStreamMode stream_mode = VertexStreamMode.Stream)
        {
            if (stream_mode == VertexStreamMode.Static)
            {
                throw new Exception("Invalid VertexStream mode for this constructor: A static VertexStream must be constructed with a pre made vertex array");
            }

            m_stream_mode = stream_mode;

            m_max_vertex_count = max_vertex_count;

            m_vertices = new Vertex[max_vertex_count];

            if (stream_mode == VertexStreamMode.Dynamic)
            {
                m_dynamic_vertex_buffer = DynamicVertexBuffer.Create(m_vertices);
            }

            m_vertex_count = 0;
            m_index_count = 0;

            if (indices != null)
            {
                m_index_buffer = IndexBuffer.Create(indices);
            }
        }


        public VertexStream(Span<Vertex> _vertices, ushort[] indices = null, VertexStreamMode stream_mode = VertexStreamMode.Static)
        {
            m_stream_mode = stream_mode;

            m_max_vertex_count = _vertices.Length;

            m_vertex_count = _vertices.Length;

            m_vertices = new Vertex[_vertices.Length];

            Unsafe.CopyBlockUnaligned(Unsafe.AsPointer(ref m_vertices[0]), Unsafe.AsPointer(ref _vertices[0]), (uint)(_vertices.Length * Unsafe.SizeOf<Vertex>()));

            switch (stream_mode)
            {
                case VertexStreamMode.Static:
                    m_static_vertex_buffer = VertexBuffer.Create(m_vertices);
                    break;
                case VertexStreamMode.Dynamic:
                    m_dynamic_vertex_buffer = DynamicVertexBuffer.Create(m_vertices);
                    break;
            }

            if (indices != null)
            {
                m_index_count = indices.Length;

                m_index_buffer = IndexBuffer.Create(indices);
            }
        }

        public void Reset()
        {
            m_vertex_count = 0;
            m_index_count = 0;
        }

        public bool PutVertices(Span<Vertex> vertices)
        {
            Span<Vertex> next_vertex_fragment = GetNextVertexFragment(vertices.Length);

            if (next_vertex_fragment == null)
            {
                return false;
            }

            fixed (Vertex* vertex_ptr = &MemoryMarshal.GetReference(next_vertex_fragment))
            {
                for (int i = 0; i < vertices.Length; ++i)
                {
                    var vertex = vertices[i];
                    *(vertex_ptr + i) = new Vertex(vertex.X, vertex.Y, vertex.Z, vertex.Col, vertex.Tx, vertex.Ty);
                }
            }

            return true;
        }

        public bool PutQuad(in Quad quad)
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

        private Span<Vertex> GetNextVertexFragment(int vertex_count, int index_count = 0)
        {
            if (m_stream_mode == VertexStreamMode.Static)
            {
                throw new Exception("Trying to update Immutable VertexStream");
            }

            if (m_vertex_count + vertex_count > m_max_vertex_count)
            {
                return null;
            }

            int current_start_index = m_vertex_count;

            m_vertex_count += vertex_count;

            m_index_count += index_count;

            m_vertex_array_changed = true;

            return new Span<Vertex>(m_vertices, current_start_index, vertex_count);
        }

        private Span<Vertex> GetNextQuadFragment()
        {
            return GetNextVertexFragment(4, 6);
        }

        internal void SubmitSpan(int start_vertex_index, int vertex_count, int start_indice_index, int index_count)
        {
            if (index_count > 0)
            {
                GraphicsContext.SetIndexBuffer(m_index_buffer.Handle, start_indice_index, index_count);
            }

            switch (m_stream_mode)
            {
                case VertexStreamMode.Static:
                    GraphicsContext.SetVertexBuffer(0, m_static_vertex_buffer.Handle, start_vertex_index, vertex_count);
                    break;

                case VertexStreamMode.Dynamic:

                    if (m_vertex_array_changed)
                    {
                        GraphicsContext.UpdateDynamicVertexBuffer(m_dynamic_vertex_buffer.Handle, 0, m_vertices);
                        m_vertex_array_changed = false;
                    }

                    GraphicsContext.SetDynamicVertexBuffer(0, m_dynamic_vertex_buffer.Handle, start_vertex_index, vertex_count);
                    break;

                case VertexStreamMode.Stream:
                    var vertices_span = new Span<Vertex>(m_vertices, start_vertex_index, vertex_count);
                    var transient_buffer = TransientVertexBuffer.Create(vertices_span, Vertex.VertexLayout);
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
            if (m_index_buffer != null)
            {
                m_index_buffer.Dispose();
            }

            if (m_static_vertex_buffer != null)
            {
                m_static_vertex_buffer.Dispose();
            }

            if (m_dynamic_vertex_buffer != null)
            {
                m_dynamic_vertex_buffer.Dispose();
            }
        }
    }
}
