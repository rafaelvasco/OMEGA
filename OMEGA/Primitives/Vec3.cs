using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OMEGA
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec3 : IEquatable<Vec3>
    {
        public float X;
        public float Y;
        public float Z;

        private static readonly Vec3 zero_vector = new Vec3(0f);
        private static readonly Vec3 unit_vector = new Vec3(1f);
        private static readonly Vec3 unitx_vector = new Vec3(1f, 0f, 0f);
        private static readonly Vec3 unity_vector = new Vec3(0f, 1f, 0f);
        private static readonly Vec3 unitz_vector = new Vec3(0f, 0f, 1f);
        private static readonly Vec3 up = new Vec3(0f, 1f, 0f);
        private static readonly Vec3 down = new Vec3(0f, -1f, 0f);
        private static readonly Vec3 right = new Vec3(1f, 0f, 0f);
        private static readonly Vec3 left = new Vec3(-1f, 0f, 0f);
        private static readonly Vec3 forward = new Vec3(0f, 0f, -1f);
        private static readonly Vec3 backward = new Vec3(0f, 0f, 1f);

        public static Vec3 Zero => zero_vector;
        public static Vec3 One => unit_vector;
        public static Vec3 UnitX => unitx_vector;
        public static Vec3 UnitY => unity_vector;
        public static Vec3 UnitZ => unitz_vector;
        public static Vec3 Up => up;
        public static Vec3 Down => down;
        public static Vec3 Left => left;
        public static Vec3 Right => right;
        public static Vec3 Forward => forward;
        public static Vec3 Backward => backward;


        public Vec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vec3(float value)
        {
            this.X = value;
            this.Y = value;
            this.Z = value;
        }

        public Vec3(Vec2 value, float z)
        {
            this.X = value.X;
            this.Y = value.Y;
            this.Z = z;
        }

        public static Vec3 Clamp(Vec3 value1, Vec3 min, Vec3 max)
        {
            return new Vec3(
                Calc.Clamp(value1.X, min.X, max.X),
                Calc.Clamp(value1.Y, min.Y, max.Y),
                Calc.Clamp(value1.Z, min.Z, max.Z));
        }

        public static void Clamp(ref Vec3 value1, ref Vec3 min, ref Vec3 max, out Vec3 result)
        {
            result.X = Calc.Clamp(value1.X, min.X, max.X);
            result.Y = Calc.Clamp(value1.Y, min.Y, max.Y);
            result.Z = Calc.Clamp(value1.Z, min.Z, max.Z);
        }

        public static float Distance(Vec3 value1, Vec3 value2)
        {
            float result;
            DistanceSquared(ref value1, ref value2, out result);
            return (float)Math.Sqrt(result);
        }

        public static void Distance(ref Vec3 value1, ref Vec3 value2, out float result)
        {
            DistanceSquared(ref value1, ref value2, out result);
            result = (float)Math.Sqrt(result);
        }

        public static float DistanceSquared(Vec3 value1, Vec3 value2)
        {
            return (value1.X - value2.X) * (value1.X - value2.X) +
                    (value1.Y - value2.Y) * (value1.Y - value2.Y) +
                    (value1.Z - value2.Z) * (value1.Z - value2.Z);
        }

        public static void DistanceSquared(ref Vec3 value1, ref Vec3 value2, out float result)
        {
            result = (value1.X - value2.X) * (value1.X - value2.X) +
                     (value1.Y - value2.Y) * (value1.Y - value2.Y) +
                     (value1.Z - value2.Z) * (value1.Z - value2.Z);
        }

        public void Normalize()
        {
            float factor = (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
            factor = 1f / factor;
            X *= factor;
            Y *= factor;
            Z *= factor;
        }

        public static Vec3 Normalize(Vec3 value)
        {
            float factor = (float)Math.Sqrt((value.X * value.X) + (value.Y * value.Y) + (value.Z * value.Z));
            factor = 1f / factor;
            return new Vec3(value.X * factor, value.Y * factor, value.Z * factor);
        }

        public static void Normalize(ref Vec3 value, out Vec3 result)
        {
            float factor = (float)Math.Sqrt((value.X * value.X) + (value.Y * value.Y) + (value.Z * value.Z));
            factor = 1f / factor;
            result.X = value.X * factor;
            result.Y = value.Y * factor;
            result.Z = value.Z * factor;
        }

        public float LengthSquared()
        {
            return (X * X) + (Y * Y) + (Z * Z);
        }

        public float Length()
        {
            return (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        }

        public static Vec3 Transform(Vec3 position, Mat4 matrix)
        {
            Transform(ref position, ref matrix, out position);
            return position;
        }

        public static void Transform(ref Vec3 position, ref Mat4 matrix, out Vec3 result)
        {
            var x = (position.X * matrix.M11) + (position.Y * matrix.M21) + (position.Z * matrix.M31) + matrix.M41;
            var y = (position.X * matrix.M12) + (position.Y * matrix.M22) + (position.Z * matrix.M32) + matrix.M42;
            var z = (position.X * matrix.M13) + (position.Y * matrix.M23) + (position.Z * matrix.M33) + matrix.M43;
            result.X = x;
            result.Y = y;
            result.Z = z;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(32);
            sb.Append("{X:");
            sb.Append(this.X);
            sb.Append(" Y:");
            sb.Append(this.Y);
            sb.Append(" Z:");
            sb.Append(this.Z);
            sb.Append("}");
            return sb.ToString();
        }

        public static Vec3 operator +(Vec3 value1, Vec3 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            value1.Z += value2.Z;
            return value1;
        }

        public static Vec3 operator -(Vec3 value)
        {
            value = new Vec3(-value.X, -value.Y, -value.Z);
            return value;
        }

        public static Vec3 operator -(Vec3 value1, Vec3 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            value1.Z -= value2.Z;
            return value1;
        }

        public static Vec3 operator *(Vec3 value1, Vec3 value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            value1.Z *= value2.Z;
            return value1;
        }

        public static Vec3 operator *(Vec3 value, float scaleFactor)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            value.Z *= scaleFactor;
            return value;
        }

        public static Vec3 operator *(float scaleFactor, Vec3 value)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            value.Z *= scaleFactor;
            return value;
        }

        public static Vec3 operator /(Vec3 value1, Vec3 value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            value1.Z /= value2.Z;
            return value1;
        }

        public static Vec3 operator /(Vec3 value1, float divider)
        {
            float factor = 1 / divider;
            value1.X *= factor;
            value1.Y *= factor;
            value1.Z *= factor;
            return value1;
        }

        public static bool operator ==(Vec3 value1, Vec3 value2)
        {
            return value1.X == value2.X
                && value1.Y == value2.Y
                && value1.Z == value2.Z;
        }

        public static bool operator !=(Vec3 value1, Vec3 value2)
        {
            return !(value1 == value2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vec3))
                return false;

            var other = (Vec3)obj;
            return X == other.X &&
                    Y == other.Y &&
                    Z == other.Z;
        }

        public bool Equals(Vec3 other)
        {
            return X == other.X &&
                    Y == other.Y &&
                    Z == other.Z;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }
    }
}
