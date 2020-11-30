using System;
using System.Runtime.CompilerServices;

namespace OMEGA
{
    public static partial class Calc
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Rcp(float a)
        {
            return 1.0f / a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Trunc(float a)
        {
            return (int)a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Fract(float a)
        {
            return a - Trunc(a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Vec3Normalize(Vec3 vec)
        {
            float inv_len = 1.0f / Vec3Length(vec);
            Vec3 result = Vec3Mul(vec, inv_len);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Vec3Length(Vec3 v)
        {
            return (float)Math.Sqrt(Vec3Dot(v, v));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Vec3Dot(Vec3 a, Vec3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Vec3Mul(Vec3 a, Vec3 b)
        {
            return new Vec3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Vec3Mul(Vec3 v, float s)
        {
            return new Vec3(v.X * s, v.Y * s, v.Z * s);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Vec3Sub(Vec3 a, Vec3 b)
        {
            return new Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Vec3Sub(Vec3 v, float s)
        {
            return new Vec3(v.X - s, v.Y - s, v.Z - s);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Vec3Div(Vec3 a, Vec3 b)
        {
            return Vec3Mul(a, Vec3Rcp(b));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Vec3Cross(Vec3 a, Vec3 b)
        {
            return new Vec3(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Vec3Div(Vec3 v, float s)
        {
            return new Vec3(v.X - s, v.Y - s, v.Z - s);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec3 Vec3Rcp(Vec3 v)
        {
            return new Vec3(1.0f / v.X, 1.0f / v.Y, 1.0f / v.Z);
        }

        // ==================================================================

        private static void MatProjXYWH(ref Mat4 result, float x, float y, float w, float h, float near, float far)
        {
            float diff = far - near;
            float aa = far / diff;
            float bb = near * aa;

            Mat4.ZeroSet(ref result);

            result.M11 = w;
            result.M22 = h;
            result.M31 = x;
            result.M32 = y;
            result.M33 = -aa;
            result.M34 = -1.0f;
            result.M43 = -bb;
        }

        public static void MatProj(ref Mat4 result, float fov, float aspect, float near, float far)
        {
            float height = (float)(1.0f / Math.Tan(ToRadians(fov) * 0.5f));
            float width = height * 1.0f / aspect;
            MatProjXYWH(ref result, 0.0f, 0.0f, width, height, near, far);
        }

        public static void MatOrtho(ref Mat4 result, float left, float right, float bottom, float top, float near, float far, float offset = 0f)
        {
            Mat4.ZeroSet(ref result);

            result.M11 = 2.0f/(right - left);
            result.M22 = 2.0f/(top - bottom);
            result.M33 = 1.0f / (near - far);
            result.M41 = (left + right)/(left - right) + offset;
            result.M42 = (top + bottom)/(bottom - top);
            result.M43 = near/(near-far);
            result.M44 = 1.0f;
        }

        public static void MatLookAt(ref Mat4 result, Vec3 eye, Vec3 at)
        {
            MatLookAt(ref result, eye, at, new Vec3(0f, 1f, 0f));
        }

        public static void MatLookAt(ref Mat4 result, Vec3 eye, Vec3 at, Vec3 up)
        {
            Vec3 view = Vec3Normalize(
                Vec3Sub(eye, at)
            );

            Vec3 uxv = Vec3Cross(up, view);
            Vec3 right = Vec3Normalize(uxv);
            up = Vec3Cross(view, right);

            Mat4.ZeroSet(ref result);

            result.M11 = right.X;
            result.M12 = up.X;
            result.M13 = view.X;

            result.M21 = right.Y;
            result.M22 = up.Y;
            result.M23 = view.Y;

            result.M31 = right.Z;
            result.M32 = up.Z;
            result.M33 = view.Z;

            result.M41 = -Vec3Dot(right, eye);
            result.M42 = -Vec3Dot(up, eye);
            result.M43 = -Vec3Dot(view, eye);
            result.M44 = 1.0f;
        }

        public static void MatRotateXY(ref Mat4 result, float rx, float ry)
        {
            float sx = Sin(rx);
            float cx = Cos(rx);
            float sy = Sin(ry);
            float cy = Cos(ry);

            Mat4.ZeroSet(ref result);

            result.M11 = cy;
            result.M13 = sy;
            result.M21 = sx*sy;
            result.M22 = cx;
            result.M23 = -sx*cy;
            result.M31 = -cx*sy;
            result.M32 = sx;
            result.M33 = cx * cy;
            result.M44 = 1.0f;
        }
    }
}
