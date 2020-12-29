
using System;
using System.Runtime.CompilerServices;

namespace OMEGA
{
    public static class MemWordHelpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MakeDWordLittleEndian(Span<byte> buffer, int offset)
        {
            return (buffer[offset + 3] << 0x18) | (buffer[offset + 2] << 0x10) | (buffer[offset + 1] << 8) | buffer[offset];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short MakeWordLittleEndian(Span<byte> buffer, int offset)
        {
            return (short)((buffer[offset + 1] << 8) | buffer[offset]);
        }
    }
}
