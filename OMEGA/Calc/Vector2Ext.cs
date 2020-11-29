using System;
using System.Numerics;

namespace OMEGA
{
    public static class Vector2Ext
    {
        public static void Normalize(this Vector2 vec)
		{
			float val = 1.0f / (float) Math.Sqrt((vec.X * vec.X) + (vec.Y * vec.Y));

			vec.X *= val;
			vec.Y *= val;
		}
    }
}
