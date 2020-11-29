
using System;
using System.IO;
using System.Text;

namespace OMEGA
{
    public static partial class ResourceLoader
    {
        private const string FNT_HEADER_TAG = "[BTFONT]";
        private const string FNT_CHAR_TAG = "Char=";

        public static Font LoadFont(FontData font_data)
        {
            var texture = LoadTexture(font_data.FontSheet);
            var glyphs = new Quad[font_data.GlyphRects.Length];

            for (int i = 0; i < font_data.GlyphRects.Length; ++i)
            {
                var glyph_rect = font_data.GlyphRects[i];

                glyphs[i] = new Quad(texture, RectF.FromBox(glyph_rect.X1, glyph_rect.Y1, glyph_rect.Width, glyph_rect.Height));
            }

            var font = new Font(texture, glyphs, font_data.PreSpacings, font_data.PostSpacings) { Id = font_data.Id };


            return font;
        }


        public static FontData LoadFontData(string id, string relative_descr_path, string relative_image_path)
        {
            var sheet_data = LoadImageData(id, relative_image_path);

            var glyphs = new Rect[255];
            var pre_spacings = new float[255];
            var post_spacings = new float[255];

            using (var descr_stream = File.OpenRead(GetFullResourcePath(relative_descr_path)))
            {
                using var reader = new StreamReader(descr_stream, Encoding.UTF8);

                string line;
                var idx = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length == 0)
                    {
                        continue;
                    }

                    if (idx == 0 && !line.Equals(FNT_HEADER_TAG))
                    {
                        throw new Exception("Invalid Font Description File.");
                    }

                    if (line.StartsWith(FNT_CHAR_TAG))
                    {
                        string char_def_str = line.Split('=')[1];

                        string[] char_def_attrs = char_def_str.Split(',');

                        if (char_def_attrs.Length != 7)
                        {
                            throw new Exception(
                                $"Invalid Font Description File: Invalid Char Definition at line: {line + 1}");
                        }

                        int ch_idx = int.Parse(char_def_attrs[0]);

                        if (ch_idx < 0 || ch_idx > 255)
                        {
                            throw new Exception("Invalid Font Description File: Character Id out of range");
                        }

                        int letter_reg_x = int.Parse(char_def_attrs[1]);
                        int letter_reg_y = int.Parse(char_def_attrs[2]);
                        int letter_reg_w = int.Parse(char_def_attrs[3]);
                        int letter_reg_h = int.Parse(char_def_attrs[4]);
                        int letter_pre_spac = int.Parse(char_def_attrs[5]);
                        int letter_post_spac = int.Parse(char_def_attrs[6]);

                        glyphs[ch_idx] = Rect.FromBox(letter_reg_x, letter_reg_y, letter_reg_w,
                            letter_reg_h);

                        pre_spacings[ch_idx] = letter_pre_spac;
                        post_spacings[ch_idx] = letter_post_spac;
                    }

                    idx++;
                }
            }

            var font_data = new FontData()
            {
                FontSheet = sheet_data,
                GlyphRects = glyphs,
                Id = id,
                PreSpacings = pre_spacings,
                PostSpacings = post_spacings
            };

            return font_data;
        }

    }
}
