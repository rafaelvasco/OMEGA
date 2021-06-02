using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OMEGA
{
    public static class FontBuilder
    {
        private static readonly Dictionary<int, int> FontSizeTextSizeMap = new()
        {
            { 25, 256},
            { 50, 512 },
            { 100, 1024 },
            { 200, 2048 }
        };

        private const int MaxPermittedTexSize = 8192;

        private static int GetOptimalTextureSize(IReadOnlyCollection<FontFace> faces)
        {
            int mediamGlyphSize = (int)faces.Select(f => f.Size).Average();
            int totalRanges = faces.Sum(f => f.CharRanges.Count);

            foreach (var key in FontSizeTextSizeMap.Keys)
            {
                if (mediamGlyphSize <= key)
                {
                    int size = FontSizeTextSizeMap[key] * totalRanges;

                    size = Calc.Min(size, MaxPermittedTexSize);

                    return size;
                }
            }

            return 4096;
        }

        private static void PopulateFontData(FontData fontData, FontBakerResult result)
        {
            PopulateFontImageData(fontData, result);
            PopulateFontGlyphData(fontData, result);
        }

        private static void PopulateFontImageData(FontData fontData, FontBakerResult result)
        {
            fontData.FontSheet.Width = result.Width;
            fontData.FontSheet.Height = result.Height;

            fontData.FontSheet.Data = new byte[result.Width * result.Height * 4];

            var idx = 0;

            for (int i = 0; i < result.Bitmap.Length; ++i)
            {
                var b = result.Bitmap[i];

                fontData.FontSheet.Data[idx] = b;
                fontData.FontSheet.Data[idx+1] = b;
                fontData.FontSheet.Data[idx+2] = b;
                fontData.FontSheet.Data[idx+3] = b;

                idx += 4;
            }
        }

        private static void PopulateFontGlyphData(FontData fontData, FontBakerResult result)
        {
            var charCount = result.Glyphs.Count;

            var chars = new char[charCount];
            var glyphRects = new SRect[charCount];
            var glyphCroppings = new SRect[charCount];
            var glyphKernings = new SVec3[charCount];

            var index = 0;

            var orderedGlyphKeys = result.Glyphs.Keys.OrderBy(g => g);

            foreach (var charKey in orderedGlyphKeys)
            {
                chars[index] = (char)charKey;

                var glyph = result.Glyphs[charKey];

                var glyphRect = new SRect(glyph.X, glyph.Y, glyph.Width, glyph.Height);

                glyphRects[index] = glyphRect;

                glyphCroppings[index] = new SRect(glyph.XOffset, glyph.YOffset, glyphRect.W, glyphRect.H);

                glyphKernings[index] = new SVec3(0, glyphRect.W, glyph.XAdvance - glyphRect.W);

                ++index;
            }

            fontData.Chars = chars;
            fontData.GlyphRects = glyphRects;
            fontData.GlyphCroppings = glyphCroppings;
            fontData.GlyphKernings = glyphKernings;
        }

        public static FontData Build(FontBuildParams @params)
        {
            var fontBaker = new FontBaker();

            Console.WriteLine($"Compiling Font: {@params.Id}");

            int optimalTextureSize = GetOptimalTextureSize(@params.Faces);

            fontBaker.Begin(optimalTextureSize, optimalTextureSize);

            foreach (var fontFace in @params.Faces)
            {
                Console.WriteLine($"Compiling face with size {fontFace.Size} and char ranges:");
                fontFace.CharRanges.ForEach((cr) =>
                {
                    Console.WriteLine(cr.Name);
                });

                var fontFileDataPath = ResourceLoader.GetFullResourcePath(fontFace.Path);

                var fontFaceBytes = File.ReadAllBytes(fontFileDataPath);

                fontBaker.Add(fontFaceBytes, fontFace.Size, fontFace.CharRanges);
            }

            var fontBakerResult = fontBaker.End();

            var fontData = new FontData()
            {
                Id = @params.Id,
                Spacing = @params.Spacing,
                LineSpacing = @params.LineSpacing,
                DefaultChar = @params.DefaultChar,
                FontSheet = new ImageData()
                {
                    Id = @params.Id + "_texture"
                }
            };

            PopulateFontData(fontData, fontBakerResult);

            return fontData;
        }
    }
}