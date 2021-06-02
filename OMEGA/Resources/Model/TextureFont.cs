using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OMEGA
{
    public class TextureFont : Resource
    {
        internal TextureFont(Texture2D texture, FontData fontData)
        {
            _texture = texture;

            LineSpacing = fontData.LineSpacing;

            Spacing = fontData.Spacing;

            Glyphs = new Glyph[fontData.Chars.Length];

            var kernings = fontData.GlyphKernings;
            var texRects = fontData.GlyphRects;
            var croppings = fontData.GlyphCroppings;
            var chars = fontData.Chars;

            var regions = new Stack<CharRegion>();

            for (int i = 0; i < chars.Length; ++i)
            {
                Glyphs[i] = new Glyph()
                {
                    TextureRect = texRects[i],
                    Cropping = croppings[i],
                    Character = chars[i],
                    LeftSideBearing = kernings[i].X,
                    Width = kernings[i].Y,
                    RightSideBearing = kernings[i].Z,
                    WidthKerning = kernings[i].X + kernings[i].Y + kernings[i].Z
                };

                if (regions.Count == 0 || chars[i] > (regions.Peek().End + 1))
                {
                    // New Region

                    regions.Push(new CharRegion(chars[i], i));
                }
                else if (chars[i] == (regions.Peek().End + 1))
                {
                    // Add char in current region

                    var currentRegion = regions.Pop();

                    currentRegion.End++;
                    regions.Push(currentRegion);
                }
                else
                {
                    throw new InvalidOperationException(
                        "Invalid TextureFont. Character map must be in ascending order.");
                }
            }

            _regions = regions.ToArray();

            Array.Reverse(_regions);

            DefaultCharacter = fontData.DefaultChar;
        }

        public Texture2D Texture => _texture;

        public Glyph[] Glyphs { get; }

        public float LineSpacing { get; set; }

        public float Spacing { get; set; }

        public char? DefaultCharacter
        {
            get => _defaultChar;
            set
            {
                // Get the default glyph index here once.
                if (value.HasValue)
                {
                    if (!TryGetGlyphIndex(value.Value, out _defaultGlyphIndex))
                        throw new ArgumentException(Errors.UNRESOLVABLE_CHARACTER);
                }
                else
                    _defaultGlyphIndex = -1;

                _defaultChar = value;
            }
        }


        public Vec2 MeasureString(string text)
        {
            var source = new CharSource(text);
            MeasureString(ref source, out var size);
            return size;
        }

        public Vec2 MeasureString(StringBuilder text)
        {
            var source = new CharSource(text);
            MeasureString(ref source, out var size);
            return size;
        }

        internal unsafe void MeasureString(ref CharSource text, out Vec2 size)
        {
            if (text.Length == 0)
            {
                size = Vec2.Zero;
                return;
            }

            var width = 0.0f;

            var maxGlyphHeight = 0;

            var offset = Vec2.Zero;

            var firstGlyphOfLine = true;

            var lines = 1;

            fixed (Glyph* pGlyphs = Glyphs)
            {
                for (int i = 0; i < text.Length; ++i)
                {
                    var c = text[i];

                    if (c == '\r')
                    {
                        continue;
                    }

                    if (c == '\n')
                    {
                        lines += 1;

                        offset.X = 0;
                        offset.Y += LineSpacing;
                        firstGlyphOfLine = true;
                        continue;
                    }

                    var currentGlyphIndex = GetGlyphIndexOrDefault(c);

                    Debug.Assert(currentGlyphIndex >= 0 && currentGlyphIndex < Glyphs.Length,
                        "currentGlyphIndex was outside array bounds.");

                    var pCurrentGlyph = pGlyphs + currentGlyphIndex;

                    if (firstGlyphOfLine)
                    {
                        offset.X = Calc.Max(pCurrentGlyph->LeftSideBearing, 0);
                        firstGlyphOfLine = false;
                    }
                    else
                    {
                        offset.X += Spacing + pCurrentGlyph->LeftSideBearing;
                    }

                    offset.X += pCurrentGlyph->Width;

                    var proposedWidth = offset.X;

                    if (i < text.Length - 1 && text[i+1] != '\n')
                    {
                        proposedWidth += Calc.Max(pCurrentGlyph->RightSideBearing, 0);
                    }

                    if (proposedWidth > width)
                    {
                        width = proposedWidth;
                    }

                    offset.X += pCurrentGlyph->RightSideBearing;

                    if (pCurrentGlyph->Cropping.Height > maxGlyphHeight)
                    {
                        maxGlyphHeight = pCurrentGlyph->Cropping.Height;
                    }
                }

                size.X = width;
                size.Y = (maxGlyphHeight * lines) + ((LineSpacing-maxGlyphHeight) * (lines-1));
            }
        }

        internal int GetGlyphIndexOrDefault(char c)
        {
            if (TryGetGlyphIndex(c, out var glyphIdx)) return glyphIdx;

            if (_defaultGlyphIndex == -1)
            {
                throw new Exception(Errors.TEXT_CONTAINS_UNRESOLVABLE_CHARACTERS);
            }

            return _defaultGlyphIndex;
        }

        private readonly Texture2D _texture;

        private readonly CharRegion[] _regions;

        private char? _defaultChar;

        private int _defaultGlyphIndex = -1;


        protected override void FreeUnmanaged()
        {
            _texture.Dispose();
        }

        private unsafe bool TryGetGlyphIndex(char c, out int index)
        {
            // Do a binary search on char regions

            fixed (CharRegion* pRegions = _regions)
            {
                int regionIdx = -1;
                var left = 0;
                var right = _regions.Length - 1;

                while (left <= right)
                {
                    var mid = (left + right) / 2;

                    Debug.Assert(mid >= 0 && mid < _regions.Length, "Index was outside of array bounds");

                    if (pRegions[mid].End < c)
                    {
                        left = mid + 1;
                    }
                    else if (pRegions[mid].Start > c)
                    {
                        right = mid - 1;
                    }
                    else
                    {
                        regionIdx = mid;
                        break;
                    }
                }

                if (regionIdx == -1)
                {
                    index = -1;
                    return false;
                }

                index = pRegions[regionIdx].StartIndex + (c - pRegions[regionIdx].Start);
            }

            return true;
        }

        public struct Glyph
        {
            public char Character;

            public Rect TextureRect;

            public Rect Cropping;

            public float LeftSideBearing;

            public float RightSideBearing;

            public float Width;

            public float WidthKerning;

            public static readonly Glyph Empty = new();

            public override string ToString()
            {
                return "CharacterIndex=" + Character + ", Glyph=" + TextureRect + ", Cropping=" + Cropping +
                       ", Kerning=" + LeftSideBearing + "," + Width + "," + RightSideBearing;
            }
        }

        public struct CharRegion
        {
            public char Start;
            public char End;
            public int StartIndex;

            public CharRegion(char start, int startIndex)
            {
                Start = start;
                End = start;
                StartIndex = startIndex;
            }
        }

        internal static class Errors
        {
            public const string TEXT_CONTAINS_UNRESOLVABLE_CHARACTERS =
                "Text contains characters that cannot be resolved by this SpriteFont.";

            public const string UNRESOLVABLE_CHARACTER =
                "Character cannot be resolved by this SpriteFont.";
        }

        internal readonly struct CharSource
        {
            public readonly int Length;

            public char this[int index] => _string?[index] ?? _builder[index];

            private readonly string _string;

            private readonly StringBuilder _builder;

            public CharSource(string str)
            {
                _string = str;
                _builder = null;
                Length = str.Length;
            }

            public CharSource(StringBuilder builder)
            {
                _builder = builder;
                _string = null;
                Length = builder.Length;
            }
        }
    }
}