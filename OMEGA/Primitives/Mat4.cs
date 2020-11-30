using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OMEGA
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Mat4
    {
        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M21;
        public float M22;
        public float M23;
        public float M24;
        public float M31;
        public float M32;
        public float M33;
        public float M34;
        public float M41;
        public float M42;
        public float M43;
        public float M44;

        public static Mat4 Identity => new Mat4(1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1);
        
        public Mat4(float m0, float m1, float m2, float m3, float m4, float m5, float m6, float m7, float m8, float m9, float m10, float m11, float m12, float m13, float m14, float m15)
        {
            M11 = m0;
            M12 = m1;
            M13 = m2;
            M14 = m3;
            M21 = m4;
            M22 = m5;
            M23 = m6;
            M24 = m7;
            M31 = m8;
            M32 = m9;
            M33 = m10;
            M34 = m11;
            M41 = m12;
            M42 = m13;
            M43 = m14;
            M44 = m15;
        }

        public static void ZeroSet(ref Mat4 mat)
        {
            unsafe
            {
                Unsafe.InitBlockUnaligned(Unsafe.AsPointer(ref mat.M11), 0, sizeof(float) * 16);
            }
        } 
    }
}
