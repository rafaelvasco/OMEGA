using System;
using System.Runtime.CompilerServices;

namespace OMEGA
{
    public unsafe struct TransientIndexBuffer
    {

        readonly IntPtr data;
        int size;
        int startIndex;
        readonly ushort handle;

        /// <summary>
        /// A pointer that can be filled with index data.
        /// </summary>
        public IntPtr Data { get { return data; } }

        /// <summary>
        /// The size of the buffer.
        /// </summary>
        public int Count { get { return size; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransientIndexBuffer"/> struct.
        /// </summary>
        /// <param name="indexCount">The number of 16-bit indices that fit in the buffer.</param>
        private TransientIndexBuffer(int indexCount)
        {
            GraphicsContext.AllocTransientIndexBuffer(out this, indexCount);
        }

        public static TransientIndexBuffer Create(Span<ushort> indices)
        {
            var transient_idx_buffer = new TransientIndexBuffer(indices.Length);

            var data_size = (uint)(indices.Length * Unsafe.SizeOf<ushort>());

            Unsafe.CopyBlock((void*)transient_idx_buffer.data, Unsafe.AsPointer(ref indices[0]), data_size);

            return transient_idx_buffer;
        }

        /// <summary>
        /// Gets the available space in the global transient index buffer.
        /// </summary>
        /// <param name="count">The number of 16-bit indices required.</param>
        /// <returns>The number of available indices.</returns>
        public static int GetAvailableSpace(int count)
        {
            return GraphicsContext.GetAvailableTransientIndexBuffers(count);
        }
    }
}
