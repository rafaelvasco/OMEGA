﻿using System;
using System.Collections.Generic;
using System.Linq;
using STB;

namespace OMEGA
{
    public unsafe class FontBaker
    {
        private byte[] _bitmap;
        private StbTrueType.stbtt_pack_context _context;
        private Dictionary<int, GlyphInfo> _glyphs;
        private int _bitmapWidth, _bitmapHeight;

        public void Begin(int width, int height)
        {
            _bitmapWidth = width;
            _bitmapHeight = height;
            _bitmap = new byte[width * height];
            _context = new StbTrueType.stbtt_pack_context();
            

            fixed (byte* pixelsPtr = _bitmap)
            {
                StbTrueType.stbtt_PackBegin(_context, pixelsPtr, width, height, width, 1, null);
            }

            _glyphs = new Dictionary<int, GlyphInfo>();
        }

        public void Add(byte[] ttf, float fontPixelHeight,
            List<CharRange> charRanges)
        {
            if (ttf == null || ttf.Length == 0)
                throw new ArgumentNullException(nameof(ttf));

            if (fontPixelHeight <= 0)
                throw new ArgumentOutOfRangeException(nameof(fontPixelHeight));

            if (charRanges == null)
                throw new ArgumentNullException(nameof(charRanges));

            if (!charRanges.Any())
                throw new ArgumentException("charRanges must have a least one value.");

            fixed (byte* ttfPtr = ttf)
            {
                var fontInfo = new StbTrueType.stbtt_fontinfo();

                if (StbTrueType.stbtt_InitFont(fontInfo, ttfPtr, 0) == 0)
                    throw new Exception("Failed to init font.");

                var scaleFactor = StbTrueType.stbtt_ScaleForPixelHeight(fontInfo, fontPixelHeight);

                int ascent, descent, lineGap;
                StbTrueType.stbtt_GetFontVMetrics(fontInfo, &ascent, &descent, &lineGap);

                foreach (var range in charRanges)
                {
                    if (range.Start > range.End)
                        continue;

                    var cd = new StbTrueType.stbtt_packedchar[range.End - range.Start + 1];
                    fixed (StbTrueType.stbtt_packedchar* chardataPtr = cd)
                    {
                        StbTrueType.stbtt_PackFontRange(_context, ttfPtr, 0, fontPixelHeight,
                            range.Start,
                            range.End - range.Start + 1,
                            chardataPtr);
                    }

                    for (var i = 0; i < cd.Length; ++i)
                    {
                        var yOff = cd[i].yoff;
                        yOff += ascent * scaleFactor;

                        var glyphInfo = new GlyphInfo
                        {
                            X = cd[i].x0,
                            Y = cd[i].y0,
                            Width = cd[i].x1 - cd[i].x0,
                            Height = cd[i].y1 - cd[i].y0,
                            XOffset = (int)cd[i].xoff,
                            YOffset = (int)Math.Round(yOff),
                            XAdvance = (int)Math.Round(cd[i].xadvance)
                        };

                        _glyphs[i + range.Start] = glyphInfo;
                    }
                }
            }

        }

        public FontBakerResult End()
        {
            return new(_glyphs, _bitmap, _bitmapWidth, _bitmapHeight);
        }

	}
}
