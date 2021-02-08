using System.Text;

namespace OMEGA
{
    public class Text : Drawable
    {
        public string TextValue
        {
            get => m_text.ToString();
            set
            {
                m_text.Clear();
                m_text.Append(value);

                MeasureText();
            }
        }

        public Font Font
        {
            get => m_font;
            set
            {
                m_font = value;

                MeasureText();
            }
        }

        private readonly StringBuilder m_text = new StringBuilder();
        private Font m_font;
        private Color m_color = Color.White;
        private Vec2 m_size;
        private Vec2 m_scale = Vec2.One;

        public override void Move(float dx, float dy)
        {
            X += dx;
            Y += dy;
        }

        public override void SetColor(Color color)
        {
            m_color = color;
        }

        public override void SetPosition(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override void PutOnCenter()
        {
            SetPosition(Engine.Canvas.Width/2 - m_size.X/2, Engine.Canvas.Height/2 - m_size.Y/2);
        }

        public override void SetSize(float w, float h)
        {
            m_scale = new Vec2(w/m_size.X, h/m_size.Y);
        }

        private void MeasureText()
        {
            if (m_font == null || m_text.Length == 0)
            {
                m_size = Vec2.Zero;
            }
            else
            {
                m_size = m_font.MeasureString(m_text);
            }
        }

        public unsafe override void Draw(Canvas canvas)
        {
            if (m_font == null || m_text.Length == 0)
            {
                return;
            }

            var font = m_font;
            var texture = m_font.Texture;
            float line_spacing = font.LineSpacing;
            float cursor_x = X, cursor_y = Y;
            
            for (int i = 0; i < m_text.Length; ++i)
            {
                var ch = m_text[i];

                switch(ch)
                {
                    case '\r':
                        continue;

                    case '\n':
                        cursor_x = X;
                        cursor_y += line_spacing;
                        break;
                   
                    default:
                        ref readonly var glyph = ref font[ch];
                        var quad = new Quad(texture, glyph.TextureRect);

                        float offset_x = glyph.Offset.X;
                        quad.Set(
                            cursor_x + offset_x,
                            cursor_y + glyph.Offset.Y,
                            glyph.TextureRect.Width,
                            glyph.TextureRect.Height
                        );

                        quad.SetColor(m_color);

                        cursor_x += glyph.XAdvance;

                        canvas.DrawQuad(in quad, texture);

                        break;
                }
            }
        }

        
    }
}
