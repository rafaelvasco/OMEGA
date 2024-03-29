﻿using System.Runtime.InteropServices;

namespace STB
{
	internal unsafe partial class StbTrueType
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct stbtt_pack_range
		{
			public float font_size;
			public int first_unicode_codepoint_in_range;
			public int* array_of_unicode_codepoints;
			public int num_chars;
			public stbtt_packedchar* chardata_for_range;
			public byte h_oversample;
			public byte v_oversample;
		}
		public class stbtt_pack_context
		{
			public void* user_allocator_context;
			public void* pack_info;
			public int width = 0;
			public int height = 0;
			public int stride_in_bytes = 0;
			public int padding = 0;
			public int skip_missing = 0;
			public uint h_oversample = 0;
			public uint v_oversample = 0;
			public byte* pixels;
			public void* nodes;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct stbrp_context
		{
			public int width;
			public int height;
			public int x;
			public int y;
			public int bottom_y;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct stbrp_node
		{
			public byte x;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct stbrp_rect
		{
			public int x;
			public int y;
			public int id;
			public int w;
			public int h;
			public int was_packed;
		}

		public static void stbrp_init_target(stbrp_context* con, int pw, int ph, stbrp_node* nodes, int num_nodes)
		{
			con->width = (int)(pw);
			con->height = (int)(ph);
			con->x = (int)(0);
			con->y = (int)(0);
			con->bottom_y = (int)(0);
		}

		public static void stbrp_pack_rects(stbrp_context* con, stbrp_rect* rects, int num_rects)
		{
			int i = 0;
			for (i = (int)(0); (i) < (num_rects); ++i)
			{
				if ((con->x + rects[i].w) > (con->width))
				{
					con->x = (int)(0);
					con->y = (int)(con->bottom_y);
				}
				if ((con->y + rects[i].h) > (con->height))
					break;
				rects[i].x = (int)(con->x);
				rects[i].y = (int)(con->y);
				rects[i].was_packed = (int)(1);
				con->x += (int)(rects[i].w);
				if ((con->y + rects[i].h) > (con->bottom_y))
					con->bottom_y = (int)(con->y + rects[i].h);
			}
			for (; (i) < (num_rects); ++i)
			{
				rects[i].was_packed = (int)(0);
			}
		}

		public static int stbtt_PackBegin(stbtt_pack_context spc, byte* pixels, int pw, int ph, int stride_in_bytes, int padding, void* alloc_context)
		{
			stbrp_context* context = (stbrp_context*)(CRuntime.Malloc((ulong)(sizeof(stbrp_context))));
			int num_nodes = (int)(pw - padding);
			stbrp_node* nodes = (stbrp_node*)(CRuntime.Malloc((ulong)(sizeof(stbrp_node) * num_nodes)));
			if (((context) == (null)) || ((nodes) == (null)))
			{
				if (context != (null))
					CRuntime.Free(context);
				if (nodes != (null))
					CRuntime.Free(nodes);
				return (int)(0);
			}

			spc.user_allocator_context = alloc_context;
			spc.width = (int)(pw);
			spc.height = (int)(ph);
			spc.pixels = pixels;
			spc.pack_info = context;
			spc.nodes = nodes;
			spc.padding = (int)(padding);
			spc.stride_in_bytes = (int)(stride_in_bytes != 0 ? stride_in_bytes : pw);
			spc.h_oversample = (uint)(1);
			spc.v_oversample = (uint)(1);
			spc.skip_missing = (int)(0);
			stbrp_init_target(context, (int)(pw - padding), (int)(ph - padding), nodes, (int)(num_nodes));
			if ((pixels) != null)
				CRuntime.Memset(pixels, (int)(0), (ulong)(pw * ph));
			return (int)(1);
		}

		public static void stbtt_PackEnd(stbtt_pack_context spc)
		{
			CRuntime.Free(spc.nodes);
			CRuntime.Free(spc.pack_info);
		}

		public static void stbtt_PackSetOversampling(stbtt_pack_context spc, uint h_oversample, uint v_oversample)
		{
			if (h_oversample <= 8)
				spc.h_oversample = (uint)(h_oversample);
			if (v_oversample <= 8)
				spc.v_oversample = (uint)(v_oversample);
		}

		public static void stbtt_PackSetSkipMissingCodepoints(stbtt_pack_context spc, int skip)
		{
			spc.skip_missing = (int)(skip);
		}

		public static int stbtt_PackFontRangesGatherRects(stbtt_pack_context spc, stbtt_fontinfo info, stbtt_pack_range* ranges, int num_ranges, stbrp_rect* rects)
		{
			int i = 0;
			int j = 0;
			int k = 0;
			int missing_glyph_added = (int)(0);
			k = (int)(0);
			for (i = (int)(0); (i) < (num_ranges); ++i)
			{
				float fh = (float)(ranges[i].font_size);
				float scale = (float)((fh) > (0) ? stbtt_ScaleForPixelHeight(info, (float)(fh)) : stbtt_ScaleForMappingEmToPixels(info, (float)(-fh)));
				ranges[i].h_oversample = ((byte)(spc.h_oversample));
				ranges[i].v_oversample = ((byte)(spc.v_oversample));
				for (j = (int)(0); (j) < (ranges[i].num_chars); ++j)
				{
					int x0 = 0;
					int y0 = 0;
					int x1 = 0;
					int y1 = 0;
					int codepoint = (int)((ranges[i].array_of_unicode_codepoints) == (null) ? ranges[i].first_unicode_codepoint_in_range + j : ranges[i].array_of_unicode_codepoints[j]);
					int glyph = (int)(stbtt_FindGlyphIndex(info, (int)(codepoint)));
					if (((glyph) == (0)) && (((spc.skip_missing) != 0) || ((missing_glyph_added) != 0)))
					{
						rects[k].w = (int)(rects[k].h = (int)(0));
					}
					else
					{
						stbtt_GetGlyphBitmapBoxSubpixel(info, (int)(glyph), (float)(scale * spc.h_oversample), (float)(scale * spc.v_oversample), (float)(0), (float)(0), &x0, &y0, &x1, &y1);
						rects[k].w = ((int)(x1 - x0 + spc.padding + spc.h_oversample - 1));
						rects[k].h = ((int)(y1 - y0 + spc.padding + spc.v_oversample - 1));
						if ((glyph) == (0))
							missing_glyph_added = (int)(1);
					}
					++k;
				}
			}
			return (int)(k);
		}

		public static int stbtt_PackFontRangesRenderIntoRects(stbtt_pack_context spc, stbtt_fontinfo info, stbtt_pack_range* ranges, int num_ranges, stbrp_rect* rects)
		{
			int i = 0;
			int j = 0;
			int k = 0;
			int missing_glyph = (int)(-1);
			int return_value = (int)(1);
			int old_h_over = (int)(spc.h_oversample);
			int old_v_over = (int)(spc.v_oversample);
			k = (int)(0);
			for (i = (int)(0); (i) < (num_ranges); ++i)
			{
				float fh = (float)(ranges[i].font_size);
				float scale = (float)((fh) > (0) ? stbtt_ScaleForPixelHeight(info, (float)(fh)) : stbtt_ScaleForMappingEmToPixels(info, (float)(-fh)));
				float recip_h = 0;
				float recip_v = 0;
				float sub_x = 0;
				float sub_y = 0;
				spc.h_oversample = (uint)(ranges[i].h_oversample);
				spc.v_oversample = (uint)(ranges[i].v_oversample);
				recip_h = (float)(1.0f / spc.h_oversample);
				recip_v = (float)(1.0f / spc.v_oversample);
				sub_x = (float)(stbtt__oversample_shift((int)(spc.h_oversample)));
				sub_y = (float)(stbtt__oversample_shift((int)(spc.v_oversample)));
				for (j = (int)(0); (j) < (ranges[i].num_chars); ++j)
				{
					stbrp_rect* r = &rects[k];
					if ((((r->was_packed) != 0) && (r->w != 0)) && (r->h != 0))
					{
						stbtt_packedchar* bc = &ranges[i].chardata_for_range[j];
						int advance = 0;
						int lsb = 0;
						int x0 = 0;
						int y0 = 0;
						int x1 = 0;
						int y1 = 0;
						int codepoint = (int)((ranges[i].array_of_unicode_codepoints) == (null) ? ranges[i].first_unicode_codepoint_in_range + j : ranges[i].array_of_unicode_codepoints[j]);
						int glyph = (int)(stbtt_FindGlyphIndex(info, (int)(codepoint)));
						int pad = (int)(spc.padding);
						r->x += (int)(pad);
						r->y += (int)(pad);
						r->w -= (int)(pad);
						r->h -= (int)(pad);
						stbtt_GetGlyphHMetrics(info, (int)(glyph), &advance, &lsb);
						stbtt_GetGlyphBitmapBox(info, (int)(glyph), (float)(scale * spc.h_oversample), (float)(scale * spc.v_oversample), &x0, &y0, &x1, &y1);
						stbtt_MakeGlyphBitmapSubpixel(info, spc.pixels + r->x + r->y * spc.stride_in_bytes, (int)(r->w - spc.h_oversample + 1), (int)(r->h - spc.v_oversample + 1), (int)(spc.stride_in_bytes), (float)(scale * spc.h_oversample), (float)(scale * spc.v_oversample), (float)(0), (float)(0), (int)(glyph));
						if ((spc.h_oversample) > (1))
							stbtt__h_prefilter(spc.pixels + r->x + r->y * spc.stride_in_bytes, (int)(r->w), (int)(r->h), (int)(spc.stride_in_bytes), (uint)(spc.h_oversample));
						if ((spc.v_oversample) > (1))
							stbtt__v_prefilter(spc.pixels + r->x + r->y * spc.stride_in_bytes, (int)(r->w), (int)(r->h), (int)(spc.stride_in_bytes), (uint)(spc.v_oversample));
						bc->x0 = (ushort)((short)(r->x));
						bc->y0 = (ushort)((short)(r->y));
						bc->x1 = (ushort)((short)(r->x + r->w));
						bc->y1 = (ushort)((short)(r->y + r->h));
						bc->xadvance = (float)(scale * advance);
						bc->xoff = (float)((float)(x0) * recip_h + sub_x);
						bc->yoff = (float)((float)(y0) * recip_v + sub_y);
						bc->xoff2 = (float)((x0 + r->w) * recip_h + sub_x);
						bc->yoff2 = (float)((y0 + r->h) * recip_v + sub_y);
						if ((glyph) == (0))
							missing_glyph = (int)(j);
					}
					else if ((spc.skip_missing) != 0)
					{
						return_value = (int)(0);
					}
					else if (((((r->was_packed) != 0) && ((r->w) == (0))) && ((r->h) == (0))) && ((missing_glyph) >= (0)))
					{
						ranges[i].chardata_for_range[j] = (stbtt_packedchar)(ranges[i].chardata_for_range[missing_glyph]);
					}
					else
					{
						return_value = (int)(0);
					}
					++k;
				}
			}
			spc.h_oversample = (uint)(old_h_over);
			spc.v_oversample = (uint)(old_v_over);
			return (int)(return_value);
		}

		public static void stbtt_PackFontRangesPackRects(stbtt_pack_context spc, stbrp_rect* rects, int num_rects)
		{
			stbrp_pack_rects((stbrp_context*)(spc.pack_info), rects, (int)(num_rects));
		}

		public static int stbtt_PackFontRanges(stbtt_pack_context spc, byte* fontdata, int font_index, stbtt_pack_range* ranges, int num_ranges)
		{
			stbtt_fontinfo info = new stbtt_fontinfo();
			int i = 0;
			int j = 0;
			int n = 0;
			int return_value = (int)(1);
			stbrp_rect* rects;
			for (i = (int)(0); (i) < (num_ranges); ++i)
			{
				for (j = (int)(0); (j) < (ranges[i].num_chars); ++j)
				{
					ranges[i].chardata_for_range[j].x0 = (ushort)(ranges[i].chardata_for_range[j].y0 = (ushort)(ranges[i].chardata_for_range[j].x1 = (ushort)(ranges[i].chardata_for_range[j].y1 = (ushort)(0))));
				}
			}
			n = (int)(0);
			for (i = (int)(0); (i) < (num_ranges); ++i)
			{
				n += (int)(ranges[i].num_chars);
			}
			rects = (stbrp_rect*)(CRuntime.Malloc((ulong)(sizeof(stbrp_rect) * n)));
			if ((rects) == (null))
				return (int)(0);
			info.userdata = spc.user_allocator_context;
			stbtt_InitFont(info, fontdata, (int)(stbtt_GetFontOffsetForIndex(fontdata, (int)(font_index))));
			n = (int)(stbtt_PackFontRangesGatherRects(spc, info, ranges, (int)(num_ranges), rects));
			stbtt_PackFontRangesPackRects(spc, rects, (int)(n));
			return_value = (int)(stbtt_PackFontRangesRenderIntoRects(spc, info, ranges, (int)(num_ranges), rects));
			CRuntime.Free(rects);
			return (int)(return_value);
		}

		public static int stbtt_PackFontRange(stbtt_pack_context spc, byte* fontdata, int font_index, float font_size, int first_unicode_codepoint_in_range, int num_chars_in_range, stbtt_packedchar* chardata_for_range)
		{
			stbtt_pack_range range = new stbtt_pack_range();
			range.first_unicode_codepoint_in_range = (int)(first_unicode_codepoint_in_range);
			range.array_of_unicode_codepoints = (null);
			range.num_chars = (int)(num_chars_in_range);
			range.chardata_for_range = chardata_for_range;
			range.font_size = (float)(font_size);
			return (int)(stbtt_PackFontRanges(spc, fontdata, (int)(font_index), &range, (int)(1)));
		}
	}
}
