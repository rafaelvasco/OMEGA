namespace OMEGA
{
    public class TriangleShape : Shape
    {
        private Vec2 m_a_center_dist;
        private Vec2 m_b_center_dist;
        private Vec2 m_c_center_dist;
        private readonly float m_orig_w;
        private readonly float m_orig_h;

        public TriangleShape(float xa, float ya, float xb, float yb, float xc, float yc)
        {
            m_vertices = new Vertex[3];

            X = (xa + xb + xc)/3;
            Y = (ya + yb + yc)/3;

            float min_x = Calc.Min(xa, xb, xc);
            float min_y = Calc.Min(ya, yb, yc);
            float max_x = Calc.Max(xa, xb, xc);
            float max_y = Calc.Max(ya, yb, yc);

            Width = max_x - min_x;
            Height = max_y - min_y;

            var vertices = m_vertices;

            vertices[0].X = xa;
            vertices[0].Y = ya;
            vertices[0].Col = 0xFFFFFFFF;
            vertices[1].X = xb;
            vertices[1].Y = yb;
            vertices[1].Col = 0xFFFFFFFF;
            vertices[2].X = xc;
            vertices[2].Y = yc;
            vertices[2].Col = 0xFFFFFFFF;

            m_a_center_dist = new Vec2(xa - X, ya - Y);
            m_b_center_dist = new Vec2(xb - X, yb - Y);
            m_c_center_dist = new Vec2(xc - X, yc - Y);

            m_orig_w = Width;
            m_orig_h = Height;

        }

        public override void SetTexture(Texture2D texture)
        {
            base.SetTexture(texture);
            
            var vertices = m_vertices;

            vertices[0].Tx = 0.5f;
            vertices[0].Ty = 0f;
            vertices[1].Tx = 0f;
            vertices[1].Ty = 1f;
            vertices[2].Tx = 1f;
            vertices[2].Ty = 1f;
        }

        public override void Move(float dx, float dy)
        {
            var vertices = m_vertices;

            X += dx;
            Y += dy;

            vertices[0].X += dx;
            vertices[0].Y += dy;
            vertices[1].X += dx;
            vertices[1].Y += dy;
            vertices[2].X += dx;
            vertices[2].Y += dy;
        }

        public override void SetColor(Color color)
        {
            var vertices = m_vertices;

            uint col = color;

            vertices[0].Col = col;
            vertices[1].Col = col;
            vertices[2].Col = col;
            vertices[3].Col = col;
            vertices[4].Col = col;
            vertices[5].Col = col;

        }

        public override void SetPosition(float x, float y)
        {
            X = x;
            Y = y;

            var vertices = m_vertices;

            float scale_x = Width / m_orig_w;
            float scale_y = Height / m_orig_h;

            vertices[0].X = x + m_a_center_dist.X * scale_x;
            vertices[0].Y = y + m_a_center_dist.Y * scale_y;
            vertices[1].X = x + m_b_center_dist.X * scale_x;
            vertices[1].Y = y + m_b_center_dist.Y * scale_y;
            vertices[2].X = x + m_c_center_dist.X * scale_x;
            vertices[2].Y = y + m_c_center_dist.Y * scale_y;
        }

        public override void SetSize(float w, float h)
        {

            float scale_x = w / Width;
            float scale_y = h / Height;

            var vertices = m_vertices;

            vertices[0].X *= scale_x;
            vertices[0].Y *= scale_y;
            vertices[1].X *= scale_x;
            vertices[1].Y *= scale_y;
            vertices[2].X *= scale_x;
            vertices[2].Y *= scale_y;

            float new_center_x = (vertices[0].X + vertices[1].X + vertices[2].X) / 3;
            float new_center_y = (vertices[0].Y + vertices[1].Y + vertices[2].Y) / 3;

            float dx = new_center_x - X;
            float dy = new_center_y - Y;

            Move(-dx, -dy);

            Width = w;
            Height = h;

        }
    }

    public class RectShape : Shape
    {
        public RectShape(float x, float y, float w, float h)
        {
            m_vertices = new Vertex[6];

            var vertices = m_vertices;

            vertices[0].Col = 0xFFFFFFFF;
            vertices[1].Col = 0xFFFFFFFF;
            vertices[2].Col = 0xFFFFFFFF;
            vertices[3].Col = 0xFFFFFFFF;
            vertices[4].Col = 0xFFFFFFFF;
            vertices[5].Col = 0xFFFFFFFF;

            SetSize(w, h);
            SetPosition(x, y);
        }

        public override void SetTexture(Texture2D texture)
        {
            base.SetTexture(texture);

            var vertices = m_vertices;

            vertices[0].Tx = 0f;
            vertices[0].Ty = 0f;
            vertices[1].Tx = 1f;
            vertices[1].Ty = 0f;
            vertices[2].Tx = 1f;
            vertices[2].Ty = 1f;
            vertices[3].Tx = 0f;
            vertices[3].Ty = 0f;
            vertices[4].Tx = 1f;
            vertices[4].Ty = 1f;
            vertices[5].Tx = 0f;
            vertices[5].Ty = 1f;
        }

        public override void Move(float dx, float dy)
        {
            var vertices = m_vertices;

            X += dx;
            Y += dy;

            vertices[0].X += dx;
            vertices[0].Y += dy;
            vertices[1].X += dx;
            vertices[1].Y += dy;
            vertices[2].X += dx;
            vertices[2].Y += dy;
            vertices[3].X += dx;
            vertices[3].Y += dy;
            vertices[4].X += dx;
            vertices[4].Y += dy;
            vertices[5].X += dx;
            vertices[5].Y += dy;

        }

        public override void SetColor(Color color)
        {
            var vertices = m_vertices;

            uint col = color;

            vertices[0].Col = col;
            vertices[1].Col = col;
            vertices[2].Col = col;
            vertices[3].Col = col;
            vertices[4].Col = col;
            vertices[5].Col = col;
        }

        public override void SetPosition(float x, float y)
        {
            var vertices = m_vertices;

            X = x;
            Y = y;

            var w = Width;
            var h = Height;

            var x1 = x;
            var y1 = y;
            var x2 = x + w;
            var y2 = y + h;

            vertices[0].X = x1;
            vertices[0].Y = y1;
            vertices[1].X = x2;
            vertices[1].Y = y1;
            vertices[2].X = x2;
            vertices[2].Y = y2;
            vertices[3].X = x1;
            vertices[3].Y = y1;
            vertices[4].X = x2;
            vertices[4].Y = y2;
            vertices[5].X = x1;
            vertices[5].Y = y2;
        }

        public override void SetSize(float w, float h)
        {
            Width = w;
            Height = h;

            var vertices = m_vertices;

            var x2 = X + w;
            var y2 = Y + h;

            vertices[1].X = x2;
            vertices[2].X = x2;
            vertices[2].Y = y2;
            vertices[3].Y = y2;
            vertices[4].X = x2;
            vertices[4].Y = y2;
            vertices[5].Y = y2;

        }

    }


    public abstract class Shape : Drawable
    {
        protected Texture2D m_texture;

        protected Vertex[] m_vertices;

        public virtual void SetTexture(Texture2D texture)
        {
            m_texture = texture;
        }

        public override void Draw(Canvas canvas)
        {
            canvas.DrawVertices(m_vertices, m_texture);
        }
    }
}
