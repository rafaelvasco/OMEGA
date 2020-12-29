using System.Collections.Generic;
using System.Text;

namespace OMEGA
{
    public class Font : Resource
    {
        public ref readonly Glyph this[char c]
        {
            get
            {
                if (m_glyphs_map.TryGetValue(c, out var idx))
                {
                    return ref m_glyphs[idx];
                }

                return ref m_empty_glyph;
            }
        }

		internal Font(Texture2D texture, FontData font_data)
        {
            m_texture = texture;

            m_glyphs_map = new Dictionary<char, int>();

            m_glyphs = new Glyph[font_data.Chars.Length];

            var glyph_idx = 0;

            m_font_size = font_data.Size;

            LineSpacing = font_data.LineHeight;

            Spacing = 0;

            for (int i = 0; i < font_data.Chars.Length; ++i)
            {
                var ch = font_data.Chars[i];
                ref var bounds = ref font_data.GlyphRects[i];
                ref var offset = ref font_data.GlyphOffsets[i];
                var x_adv = font_data.GlyphXAdvances[i];

                var glyph = new Glyph() {
                    Character = ch,
                    TextureRect = Rect.FromBox(bounds.X, bounds.Y, bounds.W, bounds.H),
                    Offset = new Vec2(offset.X, offset.Y),
                    XAdvance = x_adv
                };

                m_glyphs_map.Add(ch, glyph_idx);
                m_glyphs[glyph_idx++] = glyph;

                m_empty_glyph = this[' '];
            }
        }

        public Texture2D Texture => m_texture;

		public float LineSpacing { get; set; }

        public int Size => m_font_size;

		public float Spacing { get; set; }

        public Vec2 MeasureString(StringBuilder text, float max_width = -1)
        {
            if (text.Length > 0)
            {
                float current_line_w = 0;
                float current_line_h = LineSpacing;
                float block_w = 0;
                float block_height = 0;
                int length = text.Length;
                List<float> line_heights = new List<float>();

                for (int i = 0; i < length; ++i)
                {
                    char ch = text[i];

                    if (ch == '\n' || ch == '\r')
                    {
                        if (ch == '\n' || i + 1 == length || text[i+1] != '\n')
                        {
                            line_heights.Add(current_line_h);
                            block_w = Calc.Max(block_w, current_line_w);
                            current_line_w = 0;
                            current_line_h = LineSpacing;
                        }
                    }
                    else
                    {
                        var glyph = this[ch];
                        float width = glyph.XAdvance;

                        if (max_width != -1 && current_line_w + width >= max_width)
                        {
                            line_heights.Add(current_line_h);
                            block_w = Calc.Max(block_w, current_line_w);
                            current_line_w = 0;
                            current_line_h = LineSpacing;
                        }

                        current_line_w += width;
                        current_line_h = Calc.Max(current_line_h, glyph.TextureRect.Height + glyph.Offset.Y);

                    }

                }

                // finish off the current line if required
                if (current_line_h != 0)
                {
                    line_heights.Add(current_line_h);
                }

                // reduce any lines other than the last back to the base
                for (int i = 0; i < line_heights.Count - 1; ++i)
                {
                    line_heights[i] = LineSpacing;
                }

                foreach (var line_h in line_heights)
                {
                    block_height += line_h;
                }

                return new Vec2(Calc.Max(current_line_w, block_w), block_height);
            }

            return Vec2.Zero;

        }

        private readonly Texture2D m_texture;

        private readonly Dictionary<char, int> m_glyphs_map;

        private readonly Glyph[] m_glyphs;

        private readonly Glyph m_empty_glyph;

        private readonly int m_font_size;
       
        protected override void FreeUnmanaged()
        {
            m_texture.Dispose();
        }

        public struct Glyph
        {
            public char Character;

            public Rect TextureRect;

            public Vec2 Offset;

            public float XAdvance;

            public Glyph(char ch, Rect bounds, Vec2 offset, float x_advance)
            {
                Character = ch;
                TextureRect = bounds;
                Offset = offset;
                XAdvance = x_advance;
            }
        }

    }
}
