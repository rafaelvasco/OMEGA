using System.Runtime.InteropServices;

namespace OMEGA
{
    public struct Quad
    {
        public VertexPositionColorTexture TopLeft;
        public VertexPositionColorTexture TopRight;
        public VertexPositionColorTexture BottomRight;
        public VertexPositionColorTexture BottomLeft;

        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Quad));

        public float Width => Calc.Abs(TopRight.X - TopLeft.X);
        public float Height => Calc.Abs(BottomRight.Y - TopRight.Y);

        public Quad(RectF rect)
        {
            this.TopLeft = default;
            this.TopRight = default;
            this.BottomRight = default;
            this.BottomLeft = default;

            TopLeft.X = rect.X1;
            TopLeft.Y = rect.Y1;
            TopLeft.Col = 0xFFFFFFFF;
            TopRight.X = rect.X2;
            TopRight.Y = rect.Y1;
            TopRight.Col = 0xFFFFFFFF;
            BottomRight.X = rect.X2;
            BottomRight.Y = rect.Y2;
            BottomRight.Col = 0xFFFFFFFF;
            BottomLeft.X = rect.X1;
            BottomLeft.Y = rect.Y2;
            BottomLeft.Col = 0xFFFFFFFF;
        }

        public void SetColor(Color color)
        {
            TopLeft.Col = color;
            TopRight.Col = color;
            BottomRight.Col = color;
            BottomLeft.Col = color;
        }

        public void SetColors(Color colorTopLeft, Color colorTopRight, Color colorBottomLeft, Color colorBottomRight)
        {
            TopLeft.Col = colorTopLeft;
            TopRight.Col = colorTopRight;
            BottomRight.Col = colorBottomRight;
            BottomLeft.Col = colorBottomLeft;
        }

        public void Set(float x, float y, float dx, float dy, float w, float h, float sin, float cos, Color color,
            Vec2 texCoordTL, Vec2 texCoordBR, float depth)
        {
            TopLeft.X = x + dx * cos - dy * sin;
            TopLeft.Y = y + dx * sin + dy * cos;
            TopLeft.Z = depth;
            TopLeft.Col = color;
            TopLeft.Tx = texCoordTL.X;
            TopLeft.Ty = texCoordTL.Y;

            TopRight.X = x + (dx + w) * cos - dy * sin;
            TopRight.Y = y + (dx + w) * sin + dy * cos;
            TopRight.Z = depth;
            TopRight.Col = color;
            TopRight.Tx = texCoordBR.X;
            TopRight.Ty = texCoordTL.Y;

            BottomLeft.X = x + dx * cos - (dy + h) * sin;
            BottomLeft.Y = y + dx * sin + (dy + h) * cos;
            BottomLeft.Z = depth;
            BottomLeft.Col = color;
            BottomLeft.Tx = texCoordTL.X;
            BottomLeft.Ty = texCoordBR.Y;

            BottomRight.X = x + (dx + w) * cos - (dy + h) * sin;
            BottomRight.Y = y + (dx + w) * sin + (dy + h) * cos;
            BottomRight.Z = depth;
            BottomRight.Col = color;
            BottomRight.Tx = texCoordBR.X;
            BottomRight.Ty = texCoordBR.Y;
        }

        public void Set(float x, float y, float w, float h, Color color, Vec2 texCoordTL, Vec2 texCoordBR, float depth)
        {
            TopLeft.X = x;
            TopLeft.Y = y;
            TopLeft.Z = depth;
            TopLeft.Col = color;
            TopLeft.Tx = texCoordTL.X;
            TopLeft.Ty = texCoordTL.Y;

            TopRight.X = x + w;
            TopRight.Y = y;
            TopRight.Z = depth;
            TopRight.Col = color;
            TopRight.Tx = texCoordBR.X;
            TopRight.Ty = texCoordTL.Y;

            BottomLeft.X = x;
            BottomLeft.Y = y + h;
            BottomLeft.Z = depth;
            BottomLeft.Col = color;
            BottomLeft.Tx = texCoordTL.X;
            BottomLeft.Ty = texCoordBR.Y;

            BottomRight.X = x + w;
            BottomRight.Y = y + h;
            BottomRight.Z = depth;
            BottomRight.Col = color;
            BottomRight.Tx = texCoordBR.X;
            BottomRight.Ty = texCoordBR.Y;
        }

        public Quad(Texture2D texture, RectF srcRect = default, RectF destRect = default)
        {
            this.TopLeft = default;
            this.TopRight = default;
            this.BottomRight = default;
            this.BottomLeft = default;

            if (texture == null) return;

            float ax, ay, bx, by;

            float dest_x1, dest_y1, dest_x2, dest_y2;

            if (srcRect.IsEmpty)
            {
                srcRect = RectF.FromBox(0, 0, texture.Width, texture.Height);

                ax = 0;
                ay = 0;
                bx = 1;
                by = 1;
            }
            else
            {
                float inv_tex_w = 1.0f / texture.Width;
                float inv_tex_h = 1.0f / texture.Height;

                ax = srcRect.X1 * inv_tex_w;
                ay = srcRect.Y1 * inv_tex_h;
                bx = srcRect.X2 * inv_tex_w;
                by = srcRect.Y2 * inv_tex_h;
            }

            if (destRect.IsEmpty)
            {
                dest_x1 = 0;
                dest_y1 = 0;
                dest_x2 = srcRect.Width;
                dest_y2 = srcRect.Height;
            }
            else
            {
                dest_x1 = destRect.X1;
                dest_y1 = destRect.Y1;
                dest_x2 = destRect.X2;
                dest_y2 = destRect.Y2;
            }

            this.TopLeft.X = dest_x1;
            this.TopLeft.Y = dest_y1;
            this.TopLeft.Tx = ax;
            this.TopLeft.Ty = ay;
            this.TopLeft.Col = 0xFFFFFFFF;

            this.TopRight.X = dest_x2;
            this.TopRight.Y = dest_y1;
            this.TopRight.Tx = bx;
            this.TopRight.Ty = ay;
            this.TopRight.Col = 0xFFFFFFFF;

            this.BottomRight.X = dest_x2;
            this.BottomRight.Y = dest_y2;
            this.BottomRight.Tx = bx;
            this.BottomRight.Ty = by;
            this.BottomRight.Col = 0xFFFFFFFF;

            this.BottomLeft.X = dest_x1;
            this.BottomLeft.Y = dest_y2;
            this.BottomLeft.Tx = ax;
            this.BottomLeft.Ty = by;
            this.BottomLeft.Col = 0xFFFFFFFF;
        }

        public override string ToString()
        {
            return $"{TopLeft};{TopRight};{BottomRight};{BottomLeft}";
        }

        public RectF GetRegionRect(Texture2D texture)
        {
            return new RectF(TopLeft.Tx * texture.Width, TopLeft.Ty * texture.Height, BottomRight.X * texture.Width, BottomRight.Y * texture.Height);
        }
    }
}
