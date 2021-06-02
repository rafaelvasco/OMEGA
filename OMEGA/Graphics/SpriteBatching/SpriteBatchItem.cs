using System;

namespace OMEGA
{
    internal class SpriteBatchItem : IComparable<SpriteBatchItem>
    {
        public Texture2D Texture;
        public float SortKey;

        public Quad Quad;

        public SpriteBatchItem()
        {
            Quad = new Quad();
        }

        public void Set(float x, float y, float dx, float dy, float w, float h, float sin, float cos, Color color,
            Vec2 texCoordTL, Vec2 texCoordBR, float depth)
        {
            Quad.Set(x, y, dx, dy, w, h, sin, cos, color, texCoordTL, texCoordBR, depth);
        }

        public void Set(float x, float y, float w, float h, Color color, Vec2 texCoordTL, Vec2 texCoordBR, float depth)
        {
            Quad.Set(x, y, w, h, color, texCoordTL, texCoordBR, depth);
        }

        public int CompareTo(SpriteBatchItem other)
        {
            return SortKey.CompareTo(other.SortKey);
        }
    }
}