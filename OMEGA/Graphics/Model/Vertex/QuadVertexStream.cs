using System;
using System.Runtime.CompilerServices;

namespace OMEGA
{
    public unsafe class QuadVertexStream : GraphicsResource
    {
        private const int MaxSizeBuffer = sizeof(ushort);

        public int VertexCount => _vertexCount;

        public int VertexCapacity => _vertices.Length;

        public int IndexCapacity => _indices.Length;

        private readonly VertexLayout VertexLayout = VertexPositionColorTexture.VertexLayout;

        private VertexPositionColorTexture[] _vertices;
        private ushort[] _indices;
        
        private int _vertexCount;
        private int _indexCount;


        public QuadVertexStream(int quadCount)
        {
            UpdateBuffers(quadCount);

            _vertexCount = 0;
            _indexCount = 0;

            GraphicsContext.RegisterAllocatedResource(this);
        }

        public void UpdateBuffers(int quadCount)
        {
            int neededCapacity = 6 * quadCount;

            if (_indices != null && neededCapacity < _indices.Length)
            {
                return;
            }

            ushort[] newIndices = new ushort[6 * quadCount];

            int start = 0;

            if (_indices != null)
            {
                Unsafe.CopyBlockUnaligned(Unsafe.AsPointer(ref newIndices[0]), Unsafe.AsPointer(ref _indices), (uint)_indices.Length * sizeof(ushort));
                start = _indices.Length / 6;
            }

            fixed (ushort* indexFixedPtr = newIndices)
            {
                var indexPtr = indexFixedPtr + (start * 6);

                for (var i = start; i < quadCount; i++, indexPtr += 6)
                {
                    // Triangle 1
                    *(indexPtr + 0) = (ushort)(i * 4 + 0); // Quad TopLeft
                    *(indexPtr + 1) = (ushort)(i * 4 + 1); // Quad TopRight
                    *(indexPtr + 2) = (ushort)(i * 4 + 2); // Quad BottomRight
                    // Triangle 2
                    *(indexPtr + 3) = (ushort)(i * 4 + 0); // Quad TopLeft
                    *(indexPtr + 4) = (ushort)(i * 4 + 2); // Quad BottomRight
                    *(indexPtr + 5) = (ushort)(i * 4 + 3); // Quad BottomLeft
                }
            }

            _indices = newIndices;

            _vertices = new VertexPositionColorTexture[4 * quadCount];
        }

        public void Reset()
        {
            _vertexCount = 0;
            _indexCount = 0;
        }

        public void Push(in Quad quad)
        {
            var topLeft = quad.TopLeft;
            var topRight = quad.TopRight;
            var bottomRight = quad.BottomRight;
            var bottomLeft = quad.BottomLeft;

            fixed (VertexPositionColorTexture* vertexFixedPtr = _vertices)
            {
                var vertexPtr = vertexFixedPtr + _vertexCount;

                *(vertexPtr) = topLeft;
                *(vertexPtr + 1) = topRight;
                *(vertexPtr + 2) = bottomRight;
                *(vertexPtr + 3) = bottomLeft;

                unchecked
                {
                    _vertexCount += 4;
                    _indexCount += 6;
                }
            }
        }

        
        internal void SubmitSpan(int startVertexIndex, int vertexCount, int startIndiceIndex, int indexCount)
        {
            var vertices_span = new Span<VertexPositionColorTexture>(_vertices, startVertexIndex, vertexCount);
            var transient_buffer = GraphicsContext.CreateTransientVertexBuffer(vertices_span, VertexLayout);

            var indices_span = new Span<ushort>(_indices, startIndiceIndex, indexCount);
            var transient_idx_buffer = GraphicsContext.CreateTransientIndexBuffer(indices_span);


            GraphicsContext.SetTransientIndexBuffer(transient_idx_buffer, 0, indexCount);
            GraphicsContext.SetTransientVertexBuffer(0, transient_buffer, 0, vertexCount);
        }

        internal void Submit()
        {
            SubmitSpan(0, _vertexCount, 0, _indexCount);
        }
    }
}
