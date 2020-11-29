using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OMEGA
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec3
    {
        public float X;
        public float Y;
        public float Z;

        public Vec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static void ZeroSet(ref Vec3 vec)
        {
            unsafe
            {
                Unsafe.InitBlockUnaligned(Unsafe.AsPointer(ref vec.X), 0, sizeof(float) * 3);
            }
        } 
    }
}
