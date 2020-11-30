using System;
using System.Runtime.InteropServices;

namespace OMEGA
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec4 : IEquatable<Vec4>
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public static Vec4 Zero
        {
            get { return zero_vector; }
        }

        public static Vec4 One
        {
            get { return one_vector; }
        }

        public static Vec4 UnitX
        {
            get { return unitx_vector; }
        }

        public static Vec4 UnitY
        {
            get { return unity_vector; }
        }

        public static Vec4 UnitZ
        {
            get { return unitz_vector; }
        }

        public static Vec4 UnitW
        {
            get { return unitw_vector; }
        }

        private static readonly Vec4 zero_vector = new Vec4();
        private static readonly Vec4 one_vector = new Vec4(1f, 1f, 1f, 1f);
        private static readonly Vec4 unitx_vector = new Vec4(1f, 0f, 0f, 0f);
        private static readonly Vec4 unity_vector = new Vec4(0f, 1f, 0f, 0f);
        private static readonly Vec4 unitz_vector = new Vec4(0f, 0f, 1f, 0f);
        private static readonly Vec4 unitw_vector = new Vec4(0f, 0f, 0f, 1f);

        public Vec4(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public Vec4(Vec2 value, float z, float w)
        {
            this.X = value.X;
            this.Y = value.Y;
            this.Z = z;
            this.W = w;
        }

        public Vec4(Vec3 value, float w)
        {
            this.X = value.X;
            this.Y = value.Y;
            this.Z = value.Z;
            this.W = w;
        }

        public Vec4(float value)
        {
            this.X = value;
            this.Y = value;
            this.Z = value;
            this.W = value;
        }

        public float Length()
        {
            return (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));
        }

        public float LengthSquared()
        {
            return (X * X) + (Y * Y) + (Z * Z) + (W * W);
        }

        public void Normalize()
        {
            float factor = (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));
            factor = 1f / factor;
            X *= factor;
            Y *= factor;
            Z *= factor;
            W *= factor;
        }

        public static Vec4 Normalize(Vec4 value)
        {
            float factor = (float)Math.Sqrt((value.X * value.X) + (value.Y * value.Y) + (value.Z * value.Z) + (value.W * value.W));
            factor = 1f / factor;
            return new Vec4(value.X*factor,value.Y*factor,value.Z*factor,value.W*factor);
        }

        public static void Normalize(ref Vec4 value, out Vec4 result)
        {
            float factor = (float)Math.Sqrt((value.X * value.X) + (value.Y * value.Y) + (value.Z * value.Z) + (value.W * value.W));
            factor = 1f / factor;
            result.W = value.W * factor;
            result.X = value.X * factor;
            result.Y = value.Y * factor;
            result.Z = value.Z * factor;
        }

        public override string ToString()
        {
            return "{X:" + X + " Y:" + Y + " Z:" + Z + " W:" + W + "}";
        }

        public static Vec4 Clamp(Vec4 value1, Vec4 min, Vec4 max)
        {
            return new Vec4(
                Calc.Clamp(value1.X, min.X, max.X),
                Calc.Clamp(value1.Y, min.Y, max.Y),
                Calc.Clamp(value1.Z, min.Z, max.Z),
                Calc.Clamp(value1.W, min.W, max.W));
        }

        public static void Clamp(ref Vec4 value1, ref Vec4 min, ref Vec4 max, out Vec4 result)
        {
            result.X = Calc.Clamp(value1.X, min.X, max.X);
            result.Y = Calc.Clamp(value1.Y, min.Y, max.Y);
            result.Z = Calc.Clamp(value1.Z, min.Z, max.Z);
            result.W = Calc.Clamp(value1.W, min.W, max.W);
        }

        public static Vec4 operator +(Vec4 value1, Vec4 value2)
        {
            value1.W += value2.W;
            value1.X += value2.X;
            value1.Y += value2.Y;
            value1.Z += value2.Z;
            return value1;
        }

        public static Vec4 operator -(Vec4 value)
        {
            return new Vec4(-value.X, -value.Y, -value.Z, -value.W);
        }

        public static Vec4 operator -(Vec4 value1, Vec4 value2)
        {
            value1.W -= value2.W;
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            value1.Z -= value2.Z;
            return value1;
        }

        public static Vec4 operator *(Vec4 value1, Vec4 value2)
        {
            value1.W *= value2.W;
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            value1.Z *= value2.Z;
            return value1;
        }

        public static Vec4 operator *(Vec4 value, float scaleFactor)
        {
            value.W *= scaleFactor;
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            value.Z *= scaleFactor;
            return value;
        }

        public static Vec4 operator *(float scaleFactor, Vec4 value)
        {
            value.W *= scaleFactor;
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            value.Z *= scaleFactor;
            return value;
        }

        public static Vec4 operator /(Vec4 value1, Vec4 value2)
        {
            value1.W /= value2.W;
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            value1.Z /= value2.Z;
            return value1;
        }

        public static Vec4 operator /(Vec4 value1, float divider)
        {
            float factor = 1f / divider;
            value1.W *= factor;
            value1.X *= factor;
            value1.Y *= factor;
            value1.Z *= factor;
            return value1;
        }

        public static bool operator ==(Vec4 value1, Vec4 value2)
        {
            return value1.W == value2.W
                && value1.X == value2.X
                && value1.Y == value2.Y
                && value1.Z == value2.Z;
        }

        public static bool operator !=(Vec4 value1, Vec4 value2)
        {
            return !(value1 == value2);
        }

        public override bool Equals(object obj)
        {
            return (obj is Vec4) ? this == (Vec4)obj : false;
        }

        public bool Equals(Vec4 other)
        {
            return this.W == other.W
                && this.X == other.X
                && this.Y == other.Y
                && this.Z == other.Z;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = W.GetHashCode();
                hashCode = (hashCode * 397) ^ X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                return hashCode;
            }
        }
    }
}
