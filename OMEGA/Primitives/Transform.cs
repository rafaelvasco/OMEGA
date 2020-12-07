using System;
using System.Runtime.InteropServices;

namespace OMEGA
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Transform : IEquatable<Transform>
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

        public static Transform Identity => new Transform(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

        private Transform(
            float a0, float a1, float a2, float a3,
            float a4, float a5, float a6, float a7,
            float a8, float a9, float a10, float a11,
            float a12, float a13, float a14, float a15
        )
        {
            M0 = a0;
            M1 = a1;
            M2 = a2;
            M3 = a3;
            M4 = a4;
            M5 = a5;
            M6 = a6;
            M7 = a7;
            M8 = a8;
            M9 = a9;
            M10 = a10;
            M11 = a11;
            M12 = a12;
            M13 = a13;
            M14 = a14;
            M15 = a15;
        }

        public Transform(
            float a00, float a01, float a02,
            float a10, float a11, float a12,
            float a20, float a21, float a22
        )
        {
            M0 = a00;
            M1 = a10;
            M2 = 0f;
            M3 = a20;
            M4 = a01;
            M5 = a11;
            M6 = 0f;
            M7 = a21;
            M8 = 0f;
            M9 = 0f;
            M10 = -1f;
            M11 = 0f;
            M12 = a02;
            M13 = a12;
            M14 = 0f;
            M15 = a22;

        }

        public Transform GetInverse()
        {
            float determinant =
                M0 * (M15 * M5 - M7 * M13) -
                M1 * (M15 * M4 - M7 * M12) +
                M3 * (M13 * M4 - M5 * M12);

            if (determinant != 0f)
            {
                return new Transform(
                    (M15 * M5 - M7 * M13) / determinant,
                    -(M15 * M4 - M7 * M12) / determinant,
                    (M13 * M4 - M5 * M12) / determinant,
                    -(M15 * M1 - M3 * M13) / determinant,
                    (M15 * M0 - M3 * M12) / determinant,
                    -(M13 * M0 - M1 * M12) / determinant,
                    (M7 * M1 - M3 * M5) / determinant,
                    -(M7 * M0 - M3 * M4) / determinant,
                    (M5 * M0 - M1 * M4) / determinant
                );
            }
            else
            {
                return Identity;
            }
        }

        Vec2 TransformPoint(float x, float y)
        {
            return new Vec2(
                M0 * x + M4 * y + M12,
                M1 * x + M5 * y + M13
            );
        }

        Vec2 TransformPoint(Vec2 point)
        {
            return TransformPoint(point.X, point.Y);
        }

        RectF TransformRect(RectF rect)
        {
            // Transform the 4 corners of the rect
            Span<Vec2> points = stackalloc Vec2[4];

            points[0] = TransformPoint(rect.X1, rect.Y1);
            points[1] = TransformPoint(rect.X1, rect.Y2);
            points[2] = TransformPoint(rect.X2, rect.Y1);
            points[3] = TransformPoint(rect.X2, rect.Y2);

            // Compute the bounding rect of the transformed points
            float left = points[0].X;
            float top = points[0].Y;
            float right = points[0].X;
            float bottom = points[0].Y;

            for (int i = 1; i < 4; ++i)
            {
                if (points[i].X < left)
                {
                    left = points[i].X;
                }
                else if (points[i].X > right)
                {
                    right = points[i].X;
                }
                if (points[i].Y < top)
                {
                    top = points[i].Y;
                }
                else if (points[i].Y > bottom)
                {
                    bottom = points[i].Y;
                }
            }

            return new RectF(left, top, right, bottom);

        }

        public Transform Combine(Transform transform)
        {
            return new Transform(
                M0 * transform.M0  + M4 * transform.M1  + M12 * transform.M3,
                M0 * transform.M4  + M4 * transform.M5  + M12 * transform.M7,
                M0 * transform.M12 + M4 * transform.M13 + M12 * transform.M15,
                M1 * transform.M0  + M5 * transform.M1  + M13 * transform.M3,
                M1 * transform.M4  + M5 * transform.M5  + M13 * transform.M7,
                M1 * transform.M12 + M5 * transform.M13 + M13 * transform.M15,
                M3 * transform.M0  + M7 * transform.M1  + M15 * transform.M3,
                M3 * transform.M4  + M7 * transform.M5  + M15 * transform.M7,
                M3 * transform.M12 + M7 * transform.M13 + M15 * transform.M15  
            );
        }

        public Transform Translate(float x, float y)
        {
            var translation = new Transform(1f, 0f, x, 0f, 1f, y, 0f, 0f, 1f);

            return Combine(translation);
        }

        public Transform Translate(Vec2 offset)
        {
            return Translate(offset.X, offset.Y);
        }

        public Transform Rotate(float angle)
        {
            float rad = Calc.ToRadians(angle);
            float cos = Calc.Cos(rad);
            float sin = Calc.Sin(rad);

            var rotation = new Transform(cos, -sin, 0f, sin, cos, 0f, 0f, 0f, 1f);

            return Combine(rotation);
        }
           
        public Transform Rotate(float angle, float center_x, float center_y)
        {
            float rad = Calc.ToRadians(angle);
            float cos = Calc.Cos(rad);
            float sin = Calc.Sin(rad);

            var rotation = new Transform(
                cos, -sin, center_x * (1 - cos) + center_y * sin,
                sin, cos, center_y * (1 - cos) - center_x * sin,
                0f, 0f, 1f
            );

            return Combine(rotation);
        }


        public Transform Rotate(float angle, Vec2 center)
        {
            return Rotate(angle, center.X, center.Y);
        }

        public Transform Scale(float scale_x, float scale_y)
        {
            var scaling = new Transform(
                scale_x, 0f, 0f,
                0, scale_y, 0f,
                0f, 0f, 1f
            );

            return Combine(scaling);
        }

        public Transform Scale(float scale_x, float scale_y, float center_x, float center_y)
        {
            var scaling = new Transform(
                scale_x, 0f,      center_x * (1 - scale_x),
                0f,      scale_y, center_y * (1 - scale_y),
                0f,      0f,      1f
            );

            return Combine(scaling);
        }

        public Transform Scale(Vec2 factors)
        {
            return Scale(factors.X, factors.Y);
        }
           
        public Transform Scale(Vec2 factors, Vec2 center)
        {
            return Scale(factors.X, factors.Y, center.X, center.Y);
        }


        public static Transform operator * (Transform left, Transform right)
        {
            return left.Combine(right);
        }

        public static Vec2 operator * (Transform left, Vec2 right)
        {
            return left.TransformPoint(right);
        }
       
        public static bool operator == (Transform left, Transform right)
        {
            return (
                left.M0 == right.M0 &&    
                left.M1 == right.M1 &&
                left.M2 == right.M2 &&
                left.M3 == right.M3 &&
                left.M4 == right.M4 &&
                left.M5 == right.M5 &&
                left.M6 == right.M6 &&
                left.M7 == right.M7 &&
                left.M8 == right.M8 &&
                left.M9 == right.M9 &&
                left.M10 == right.M10 &&
                left.M11 == right.M11 &&
                left.M12 == right.M12 &&
                left.M13 == right.M13 &&
                left.M14 == right.M14 &&
                left.M15 == right.M15
            );
        }

        public static bool operator != (Transform left, Transform right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Transform transform)
            {
                return Equals(transform);
            }

            return false;
        }

        public bool Equals(Transform other)
        {
             return (
                M0 == other.M0 &&    
                M1 == other.M1 &&
                M2 == other.M2 &&
                M3 == other.M3 &&
                M4 == other.M4 &&
                M5 == other.M5 &&
                M6 == other.M6 &&
                M7 == other.M7 &&
                M8 == other.M8 &&
                M9 == other.M9 &&
                M10 == other.M10 &&
                M11 == other.M11 &&
                M12 == other.M12 &&
                M13 == other.M13 &&
                M14 == other.M14 &&
                M15 == other.M15
            );
        }

        public override int GetHashCode()
        {
            return M0.GetHashCode() + M1.GetHashCode() + M2.GetHashCode() + M3.GetHashCode() +
                   M4.GetHashCode() + M5.GetHashCode() + M6.GetHashCode() + M7.GetHashCode() +
                   M8.GetHashCode() + M9.GetHashCode() + M10.GetHashCode() + M11.GetHashCode() +
                   M12.GetHashCode() + M13.GetHashCode() + M14.GetHashCode() + M15.GetHashCode();
        }
    }
}
