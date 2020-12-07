using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OMEGA
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec2 : IEquatable<Vec2>
    {
        public float X;
        public float Y;

        private static readonly Vec2 zero_vector = new Vec2(0f);
        private static readonly Vec2 unit_vector = new Vec2(1f);
        private static readonly Vec2 unitx_vector = new Vec2(1f, 0f);
        private static readonly Vec2 unity_vector = new Vec2(0f, 1f);

        public static Vec2 Zero => zero_vector;
        public static Vec2 One => unit_vector;
        public static Vec2 UnitX => unitx_vector;
        public static Vec2 UnitY => unity_vector;

        public Vec2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vec2(float value)
        {
            this.X = value;
            this.Y = value;
        }

        public void Normalize()
        {
            float val = 1.0f / (float)Math.Sqrt((X * X) + (Y * Y));
            X *= val;
            Y *= val;
        }

        public float LengthSquared()
        {
            return (X * X) + (Y * Y);
        }

        public float Length()
        {
            return (float)Math.Sqrt((X * X) + (Y * Y));
        }

        public static Vec2 Transform(Vec2 position, Mat4 matrix)
        {
            return new Vec2((position.X * matrix.M34) + (position.Y * matrix.M21) + matrix.M41, (position.X * matrix.M41) + (position.Y * matrix.M22) + matrix.M42);
        }

        public static void Transform(ref Vec2 position, ref Mat4 matrix, out Vec2 result)
        {
            var x = (position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M41;
            var y = (position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M42;
            result.X = x;
            result.Y = y;
        }

        public static Vec2 operator -(Vec2 value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            return value;
        }

        public static Vec2 operator +(Vec2 value1, Vec2 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }

        public static Vec2 operator -(Vec2 value1, Vec2 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }

        public static Vec2 operator *(Vec2 value1, Vec2 value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
        }

        public static Vec2 operator *(Vec2 value, float scaleFactor)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            return value;
        }

        public static Vec2 operator *(float scaleFactor, Vec2 value)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator /(Vec2 value1, Vec2 value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vec2 operator /(Vec2 value1, float divider)
        {
            float factor = 1 / divider;
            value1.X *= factor;
            value1.Y *= factor;
            return value1;
        }

        public static bool operator ==(Vec2 value1, Vec2 value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y;
        }

        public static bool operator !=(Vec2 value1, Vec2 value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vec2)
            {
                return Equals((Vec2)obj);
            }

            return false;
        }

        public bool Equals(Vec2 other)
        {
            return (X == other.X) && (Y == other.Y);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X.GetHashCode() * 397) ^ Y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}
