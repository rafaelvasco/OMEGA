using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OMEGA
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Mat4
    {
        public float M0;
        public float M1;
        public float M2;
        public float M3;
        public float M4;
        public float M5;
        public float M6;
        public float M7;
        public float M8;
        public float M9;
        public float M10;
        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M15;

        public static Mat4 Identity => new Mat4(1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1);
        
        public Mat4(float m0, float m1, float m2, float m3, float m4, float m5, float m6, float m7, float m8, float m9, float m10, float m11, float m12, float m13, float m14, float m15)
        {
            M0 = m0;
            M1 = m1;
            M2 = m2;
            M3 = m3;
            M4 = m4;
            M5 = m5;
            M6 = m6;
            M7 = m7;
            M8 = m8;
            M9 = m9;
            M10 = m10;
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M14 = m14;
            M15 = m15;
        }

        public static void ZeroSet(ref Mat4 mat)
        {
            unsafe
            {
                Unsafe.InitBlockUnaligned(Unsafe.AsPointer(ref mat.M0), 0, sizeof(float) * 16);
            }
        } 
    }
}
