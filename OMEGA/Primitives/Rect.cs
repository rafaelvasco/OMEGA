using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OMEGA
{
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct Rect : IEquatable<Rect>
    {
        private static readonly Rect _empty = new Rect(0, 0, 0, 0);
        public static ref readonly Rect Empty => ref _empty;

        public int X1;
        public int Y1;
        public int X2;
        public int Y2;

        public static Rect FromBox(int x, int y, int w, int h)
        {
            return new Rect(x, y, x + w, y + h);
        }

        public Rect(int x1, int y1, int x2, int y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public int Width => X2 - X1;

        public int Height => Y2 - Y1;

        public bool IsEmpty => Width == 0 && Height == 0;

        public bool IsRegular => X2 > X1 && Y2 > Y1;

        public Rect Normalized =>
            new Rect(Calc.Min(X1, X2), Calc.Min(Y1, Y2), Calc.Max(X1, X2), Calc.Max(Y1, Y2));

        public int Area => Math.Abs(Width * Height);

        public Point TopLeft => new Point(X1, Y1);

        public Point TopRight => new Point(X2, Y1);

        public Point BottomLeft => new Point(X1, Y2);

        public Point BottomRight => new Point(X2, Y2);

        public Point Center => new Point(Calc.Abs(X2 - X1) / 2, Calc.Abs(Y2 - Y1) / 2);

        public override bool Equals(object obj)
        {
            return obj is Rect i && Equals(i);
        }

        public bool Equals(Rect other)
        {
            return Equals(ref other);
        }

        public bool Equals(ref Rect other)
        {
            return X1 == other.X1 && Y1 == other.Y1 && X2 == other.X2 && Y2 == other.Y2;
        }

        public bool Contains(Point p)
        {
            return p.X >= X1 && p.X <= X2 && p.Y >= Y1 && p.Y <= Y2;
        }

        public bool Contains(ref Rect rect)
        {
            return X1 <= rect.X1 && X2 >= rect.X2 && Y1 <= rect.Y1 && Y2 >= rect.Y2;
        }

        public bool Contains(Rect rect)
        {
            return Contains(ref rect);
        }

        public bool Intersects(ref Rect other)
        {
            return X1 <= other.X2 &&
                   Y1 <= other.Y2 &&
                   X2 >= other.X1 &&
                   Y2 >= other.Y1;
        }

        public bool Intersects(Rect other)
        {
            return Intersects(ref other);
        }

        public void CopyTo(out Rect other)
        {
            other.X1 = X1;
            other.Y1 = Y1;
            other.X2 = X2;
            other.Y2 = Y2;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + X1;
                hash = hash * 23 + Y1;
                hash = hash * 23 + X2;
                hash = hash * 23 + Y2;
                return hash;
            }
        }

        public Rect Inflated(int x, int y)
        {
            var copy = this;

            copy.X1 -= x;
            copy.X2 += x;
            copy.Y1 -= y;
            copy.Y2 += y;

            return copy;
        }

        public Rect Inflated(int amount)
        {
            return Inflated(amount, amount);
        }

        public override string ToString()
        {
            return $"{X1},{Y1},{Width},{Height}";
        }

        public static bool operator ==(Rect a, Rect b)
        {
            return a.Equals(ref b);
        }

        public static bool operator !=(Rect a, Rect b)
        {
            return !a.Equals(ref b);
        }
    }
}
