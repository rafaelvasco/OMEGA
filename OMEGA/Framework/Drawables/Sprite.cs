using System.Runtime.CompilerServices;

namespace OMEGA
{
    public class Sprite : Drawable
    {
        public Texture2D Texture => m_texture;

        public Sprite(Texture2D texture) : this(texture, RectF.FromBox(0f, 0f, texture.Width, texture.Height))
        {
        }

        public Sprite(TextureAtlas atlas) : this(atlas.Texture, atlas[0])
        {

        }

        public Sprite(Texture2D texture, RectF src_rect)
        {
            m_texture = texture;
            m_quad = new Quad(texture, src_rect);
            Width = m_quad.Width;
            Height = m_quad.Height;

            m_origin_x = 0.5f;
            m_origin_y = 0.5f;

            m_flip_x = false;

            m_flip_y = false;
        }


        private bool m_flip_x;
        private bool m_flip_y;
        private float m_origin_x;
        private float m_origin_y;
        private Color m_color = Color.White;

        private Texture2D m_texture;
        private BlendMode m_blend_mode = BlendMode.Alpha;
        private Quad m_quad;

        public void SetTextureRegion(RectF rect, bool reset_size)
        {
            if (reset_size)
            {
                Width = rect.Width;
                Height = rect.Height;
            }

            m_quad = new Quad(m_texture, rect, RectF.FromBox(0, 0, Width, Height));

        }

        public void SetTextureRegion(Texture2D texture, RectF region, bool reset_size)
        {
            m_texture = texture;
            SetTextureRegion(region, reset_size);
        }

        public void SetAtlasFrame(TextureAtlas atlas, int frame)
        {
            SetTextureRegion(atlas.Texture, atlas[frame], false);
        }

        public void SetAtlasFrame(TextureAtlas atlas, string frame)
        {
            SetTextureRegion(atlas.Texture, atlas[frame], false);
        }

        public override void SetPosition(float x, float y)
        {
            X = x;
            Y = y;
            UpdateQuadGeometry();
        }

        public override void Move(float dx, float dy)
        {
            X += dx;
            Y += dy;
            UpdateQuadGeometry();
        }

        public override void SetSize(float w, float h)
        {
            Width = w;
            Height = h;

            UpdateQuadGeometry();
        }

        public override void SetColor(Color color)
        {
            if (color == m_color)
            {
                return;
            }

            m_color = color;

            m_quad.V0.Col = color;
            m_quad.V1.Col = color;
            m_quad.V2.Col = color;
            m_quad.V3.Col = color;
        }

        public void FlipHorizontal(bool flip)
        {
            SetFlip(flip, m_flip_y);
        }

        public void FlipVertical(bool flip)
        {
            SetFlip(m_flip_x, flip);
        }

        public void SetFlip(bool flip_h, bool flip_v)
        {
            if (m_flip_x == flip_h && m_flip_y == flip_v)
            {
                return;
            }

            m_flip_x = flip_h;
            m_flip_y = flip_v;

            float tx, ty;

            if (flip_h != m_flip_x)
            {
                tx = m_quad.V0.Tx;
                m_quad.V0.Tx = m_quad.V1.Tx;
                m_quad.V1.Tx = tx;

                tx = m_quad.V3.Tx;
                m_quad.V3.Tx = m_quad.V2.Tx;
                m_quad.V2.Tx = tx;
            }

            if (flip_v != m_flip_y)
            {
                ty = m_quad.V0.Ty;
                m_quad.V0.Ty = m_quad.V3.Ty;
                m_quad.V3.Ty = ty;

                ty = m_quad.V1.Ty;
                m_quad.V1.Ty = m_quad.V2.Ty;
                m_quad.V2.Ty = ty;
            }
        }

        public void SetOrigin(float x, float y)
        {
            m_origin_x = x;
            m_origin_y = y;
        }

        public void SetBlend(BlendMode mode)
        {
            m_blend_mode = mode;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateQuadGeometry()
        {
            m_quad.Set(X, Y, Width, Height);
        }

        public override void Draw(Canvas canvas)
        {
            canvas.BlendMode = m_blend_mode;
            

            var origin_dx = m_origin_x * Width;
            var origin_dy = m_origin_y * Height;

            var draw_q = m_quad;

            draw_q.Set(X - origin_dx, Y - origin_dy, Width, Height);

            canvas.DrawQuad(in draw_q, m_texture);

        }

        
    }
}
