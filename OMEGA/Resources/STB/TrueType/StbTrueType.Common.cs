﻿using System.Runtime.InteropServices;
// ReSharper disable StackAllocInsideLoop

namespace STB
{
	internal unsafe partial class StbTrueType
	{
		public const int STBTT_vmove = 1;
		public const int STBTT_vline = 2;
		public const int STBTT_vcurve = 3;
		public const int STBTT_vcubic = 4;
		public const int STBTT_PLATFORM_ID_UNICODE = 0;
		public const int STBTT_PLATFORM_ID_MAC = 1;
		public const int STBTT_PLATFORM_ID_ISO = 2;
		public const int STBTT_PLATFORM_ID_MICROSOFT = 3;
		public const int STBTT_UNICODE_EID_UNICODE_1_0 = 0;
		public const int STBTT_UNICODE_EID_UNICODE_1_1 = 1;
		public const int STBTT_UNICODE_EID_ISO_10646 = 2;
		public const int STBTT_UNICODE_EID_UNICODE_2_0_BMP = 3;
		public const int STBTT_UNICODE_EID_UNICODE_2_0_FULL = 4;
		public const int STBTT_MS_EID_SYMBOL = 0;
		public const int STBTT_MS_EID_UNICODE_BMP = 1;
		public const int STBTT_MS_EID_SHIFTJIS = 2;
		public const int STBTT_MS_EID_UNICODE_FULL = 10;
		public const int STBTT_MAC_EID_ROMAN = 0;
		public const int STBTT_MAC_EID_ARABIC = 4;
		public const int STBTT_MAC_EID_JAPANESE = 1;
		public const int STBTT_MAC_EID_HEBREW = 5;
		public const int STBTT_MAC_EID_CHINESE_TRAD = 2;
		public const int STBTT_MAC_EID_GREEK = 6;
		public const int STBTT_MAC_EID_KOREAN = 3;
		public const int STBTT_MAC_EID_RUSSIAN = 7;
		public const int STBTT_MS_LANG_ENGLISH = 0x0409;
		public const int STBTT_MS_LANG_ITALIAN = 0x0410;
		public const int STBTT_MS_LANG_CHINESE = 0x0804;
		public const int STBTT_MS_LANG_JAPANESE = 0x0411;
		public const int STBTT_MS_LANG_DUTCH = 0x0413;
		public const int STBTT_MS_LANG_KOREAN = 0x0412;
		public const int STBTT_MS_LANG_FRENCH = 0x040c;
		public const int STBTT_MS_LANG_RUSSIAN = 0x0419;
		public const int STBTT_MS_LANG_GERMAN = 0x0407;
		public const int STBTT_MS_LANG_SPANISH = 0x0409;
		public const int STBTT_MS_LANG_HEBREW = 0x040d;
		public const int STBTT_MS_LANG_SWEDISH = 0x041D;
		public const int STBTT_MAC_LANG_ENGLISH = 0;
		public const int STBTT_MAC_LANG_JAPANESE = 11;
		public const int STBTT_MAC_LANG_ARABIC = 12;
		public const int STBTT_MAC_LANG_KOREAN = 23;
		public const int STBTT_MAC_LANG_DUTCH = 4;
		public const int STBTT_MAC_LANG_RUSSIAN = 32;
		public const int STBTT_MAC_LANG_FRENCH = 1;
		public const int STBTT_MAC_LANG_SPANISH = 6;
		public const int STBTT_MAC_LANG_GERMAN = 2;
		public const int STBTT_MAC_LANG_SWEDISH = 5;
		public const int STBTT_MAC_LANG_HEBREW = 10;
		public const int STBTT_MAC_LANG_CHINESE_SIMPLIFIED = 33;
		public const int STBTT_MAC_LANG_ITALIAN = 3;
		public const int STBTT_MAC_LANG_CHINESE_TRAD = 19;

		[StructLayout(LayoutKind.Sequential)]
		public struct stbtt_bakedchar
		{
			public ushort x0;
			public ushort y0;
			public ushort x1;
			public ushort y1;
			public float xoff;
			public float yoff;
			public float xadvance;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct stbtt_aligned_quad
		{
			public float x0;
			public float y0;
			public float s0;
			public float t0;
			public float x1;
			public float y1;
			public float s1;
			public float t1;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct stbtt_packedchar
		{
			public ushort x0;
			public ushort y0;
			public ushort x1;
			public ushort y1;
			public float xoff;
			public float yoff;
			public float xadvance;
			public float xoff2;
			public float yoff2;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct stbtt_vertex
		{
			public short x;
			public short y;
			public short cx;
			public short cy;
			public short cx1;
			public short cy1;
			public byte type;
			public byte padding;
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct stbtt__point
		{
			public float x;
			public float y;
		}

		public static ushort ttUSHORT(byte* p)
		{
			return (ushort)(p[0] * 256 + p[1]);
		}

		public static short ttSHORT(byte* p)
		{
			return (short)(p[0] * 256 + p[1]);
		}

		public static uint ttULONG(byte* p)
		{
			return (uint)((p[0] << 24) + (p[1] << 16) + (p[2] << 8) + p[3]);
		}

		public static int ttLONG(byte* p)
		{
			return (p[0] << 24) + (p[1] << 16) + (p[2] << 8) + p[3];
		}

		public static int stbtt__isfont(byte* font)
		{
			if (((((((font)[0]) == ('1')) && (((font)[1]) == (0))) && (((font)[2]) == (0))) && (((font)[3]) == (0))))
				return 1;
			if (((((((font)[0]) == ("typ1"[0])) && (((font)[1]) == ("typ1"[1]))) && (((font)[2]) == ("typ1"[2]))) && (((font)[3]) == ("typ1"[3]))))
				return 1;
			if (((((((font)[0]) == ("OTTO"[0])) && (((font)[1]) == ("OTTO"[1]))) && (((font)[2]) == ("OTTO"[2]))) && (((font)[3]) == ("OTTO"[3]))))
				return 1;
			if (((((((font)[0]) == (0)) && (((font)[1]) == (1))) && (((font)[2]) == (0))) && (((font)[3]) == (0))))
				return 1;
			if (((((((font)[0]) == ("true"[0])) && (((font)[1]) == ("true"[1]))) && (((font)[2]) == ("true"[2]))) && (((font)[3]) == ("true"[3]))))
				return 1;
			return 0;
		}

		public static int stbtt_GetFontOffsetForIndex_internal(byte* font_collection, int index)
		{
			if ((stbtt__isfont(font_collection)) != 0)
				return (index) == (0) ? 0 : -1;
			if (((((((font_collection)[0]) == ("ttcf"[0])) && (((font_collection)[1]) == ("ttcf"[1]))) && (((font_collection)[2]) == ("ttcf"[2]))) && (((font_collection)[3]) == ("ttcf"[3]))))
			{
				if (((ttULONG(font_collection + 4)) == (0x00010000)) || ((ttULONG(font_collection + 4)) == (0x00020000)))
				{
					int n = ttLONG(font_collection + 8);
					if ((index) >= (n))
						return -1;
					return (int)(ttULONG(font_collection + 12 + index * 4));
				}
			}

			return -1;
		}

		public static int stbtt_GetNumberOfFonts_internal(byte* font_collection)
		{
			if ((stbtt__isfont(font_collection)) != 0)
				return 1;
			if (((((((font_collection)[0]) == ("ttcf"[0])) && (((font_collection)[1]) == ("ttcf"[1]))) && (((font_collection)[2]) == ("ttcf"[2]))) && (((font_collection)[3]) == ("ttcf"[3]))))
			{
				if (((ttULONG(font_collection + 4)) == (0x00010000)) || ((ttULONG(font_collection + 4)) == (0x00020000)))
				{
					return ttLONG(font_collection + 8);
				}
			}

			return 0;
		}

		public static void stbtt_setvertex(stbtt_vertex* v, byte type, int x, int y, int cx, int cy)
		{
			v->type = type;
			v->x = ((short)(x));
			v->y = ((short)(y));
			v->cx = ((short)(cx));
			v->cy = ((short)(cy));
		}

		public static void stbtt__add_point(stbtt__point* points, int n, float x, float y)
		{
			if (points == null)
				return;
			points[n].x = x;
			points[n].y = y;
		}

		public static int stbtt__tesselate_curve(stbtt__point* points, int* num_points, float x0, float y0, float x1, float y1, float x2, float y2, float objspace_flatness_squared, int n)
		{
			float mx = (x0 + 2 * x1 + x2) / 4;
			float my = (y0 + 2 * y1 + y2) / 4;
			float dx = (x0 + x2) / 2 - mx;
			float dy = (y0 + y2) / 2 - my;
			if ((n) > (16))
				return 1;
			if ((dx * dx + dy * dy) > (objspace_flatness_squared))
			{
				stbtt__tesselate_curve(points, num_points, x0, y0, (x0 + x1) / 2.0f, (y0 + y1) / 2.0f, mx, my, objspace_flatness_squared, n + 1);
				stbtt__tesselate_curve(points, num_points, mx, my, (x1 + x2) / 2.0f, (y1 + y2) / 2.0f, x2, y2, objspace_flatness_squared, n + 1);
			}
			else
			{
				stbtt__add_point(points, *num_points, x2, y2);
				*num_points = *num_points + 1;
			}

			return 1;
		}

		public static void stbtt__tesselate_cubic(stbtt__point* points, int* num_points, float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3, float objspace_flatness_squared, int n)
		{
			float dx0 = x1 - x0;
			float dy0 = y1 - y0;
			float dx1 = x2 - x1;
			float dy1 = y2 - y1;
			float dx2 = x3 - x2;
			float dy2 = y3 - y2;
			float dx = x3 - x0;
			float dy = y3 - y0;
			float longlen = (float)(CRuntime.Sqrt(dx0 * dx0 + dy0 * dy0) + CRuntime.Sqrt(dx1 * dx1 + dy1 * dy1) + CRuntime.Sqrt(dx2 * dx2 + dy2 * dy2));
			float shortlen = (float)(CRuntime.Sqrt(dx * dx + dy * dy));
			float flatness_squared = longlen * longlen - shortlen * shortlen;
			if ((n) > (16))
				return;
			if ((flatness_squared) > (objspace_flatness_squared))
			{
				float x01 = (x0 + x1) / 2;
				float y01 = (y0 + y1) / 2;
				float x12 = (x1 + x2) / 2;
				float y12 = (y1 + y2) / 2;
				float x23 = (x2 + x3) / 2;
				float y23 = (y2 + y3) / 2;
				float xa = (x01 + x12) / 2;
				float ya = (y01 + y12) / 2;
				float xb = (x12 + x23) / 2;
				float yb = (y12 + y23) / 2;
				float mx = (xa + xb) / 2;
				float my = (ya + yb) / 2;
				stbtt__tesselate_cubic(points, num_points, x0, y0, x01, y01, xa, ya, mx, my, objspace_flatness_squared, n + 1);
				stbtt__tesselate_cubic(points, num_points, mx, my, xb, yb, x23, y23, x3, y3, objspace_flatness_squared, n + 1);
			}
			else
			{
				stbtt__add_point(points, *num_points, x3, y3);
				*num_points = *num_points + 1;
			}

		}

		public static stbtt__point* stbtt_FlattenCurves(stbtt_vertex* vertices, int num_verts, float objspace_flatness, int** contour_lengths, int* num_contours, void* userdata)
		{
			stbtt__point* points = null;
			int num_points = 0;
			float objspace_flatness_squared = objspace_flatness * objspace_flatness;
			int i = 0;
			int n = 0;
			int start = 0;
			int pass = 0;
			for (i = 0; (i) < (num_verts); ++i)
			{
				if ((vertices[i].type) == (STBTT_vmove))
					++n;
			}
			*num_contours = n;
			if ((n) == (0))
				return null;
			*contour_lengths = (int*)(CRuntime.Malloc((ulong)(sizeof(int) * n)));
			if ((*contour_lengths) == (null))
			{
				*num_contours = 0;
				return null;
			}

			for (pass = 0; (pass) < (2); ++pass)
			{
				float x = 0;
				float y = 0;
				if ((pass) == (1))
				{
					points = (stbtt__point*)(CRuntime.Malloc((ulong)(num_points * sizeof(stbtt__point))));
					if ((points) == (null))
						goto error;
				}
				num_points = 0;
				n = -1;
				for (i = 0; (i) < (num_verts); ++i)
				{
					switch (vertices[i].type)
					{
						case STBTT_vmove:
							if ((n) >= (0))
								(*contour_lengths)[n] = num_points - start;
							++n;
							start = num_points;
							x = vertices[i].x;
							y = vertices[i].y;
							stbtt__add_point(points, num_points++, x, y);
							break;
						case STBTT_vline:
							x = vertices[i].x;
							y = vertices[i].y;
							stbtt__add_point(points, num_points++, x, y);
							break;
						case STBTT_vcurve:
							stbtt__tesselate_curve(points, &num_points, x, y, vertices[i].cx, vertices[i].cy, vertices[i].x, vertices[i].y, objspace_flatness_squared, 0);
							x = vertices[i].x;
							y = vertices[i].y;
							break;
						case STBTT_vcubic:
							stbtt__tesselate_cubic(points, &num_points, x, y, vertices[i].cx, vertices[i].cy, vertices[i].cx1, vertices[i].cy1, vertices[i].x, vertices[i].y, objspace_flatness_squared, 0);
							x = vertices[i].x;
							y = vertices[i].y;
							break;
					}
				} (*contour_lengths)[n] = num_points - start;
			}
			return points;
		error:
			;
			CRuntime.Free(points);
			CRuntime.Free(*contour_lengths);
			*contour_lengths = null;
			*num_contours = 0;
			return (null);
		}

		public static void stbtt_FreeBitmap(byte* bitmap, void* userdata)
		{
			CRuntime.Free(bitmap);
		}

		public static int stbtt_BakeFontBitmap_internal(byte* data, int offset, float pixel_height, byte* pixels, int pw, int ph, int first_char, int num_chars, stbtt_bakedchar* chardata)
		{
			float scale = 0;
			int x = 0;
			int y = 0;
			int bottom_y = 0;
			int i = 0;
			stbtt_fontinfo f = new stbtt_fontinfo();
			f.userdata = (null);
			if (stbtt_InitFont(f, data, offset) == 0)
				return -1;
			CRuntime.Memset(pixels, 0, (ulong)(pw * ph));
			x = y = 1;
			bottom_y = 1;
			scale = stbtt_ScaleForPixelHeight(f, pixel_height);
			for (i = 0; (i) < (num_chars); ++i)
			{
				int advance = 0;
				int lsb = 0;
				int x0 = 0;
				int y0 = 0;
				int x1 = 0;
				int y1 = 0;
				int gw = 0;
				int gh = 0;
				int g = stbtt_FindGlyphIndex(f, first_char + i);
				stbtt_GetGlyphHMetrics(f, g, &advance, &lsb);
				stbtt_GetGlyphBitmapBox(f, g, scale, scale, &x0, &y0, &x1, &y1);
				gw = x1 - x0;
				gh = y1 - y0;
				if ((x + gw + 1) >= (pw))
				{
					y = bottom_y;
					x = 1;
				}
				if ((y + gh + 1) >= (ph))
					return -i;
				stbtt_MakeGlyphBitmap(f, pixels + x + y * pw, gw, gh, pw, scale, scale, g);
				chardata[i].x0 = (ushort)((short)(x));
				chardata[i].y0 = (ushort)((short)(y));
				chardata[i].x1 = (ushort)((short)(x + gw));
				chardata[i].y1 = (ushort)((short)(y + gh));
				chardata[i].xadvance = scale * advance;
				chardata[i].xoff = x0;
				chardata[i].yoff = y0;
				x = x + gw + 1;
				if ((y + gh + 1) > (bottom_y))
					bottom_y = y + gh + 1;
			}
			return bottom_y;
		}

		public static void stbtt_GetBakedQuad(stbtt_bakedchar* chardata, int pw, int ph, int char_index, float* xpos, float* ypos, stbtt_aligned_quad* q, int opengl_fillrule)
		{
			float d3d_bias = (opengl_fillrule) != 0 ? 0 : -0.5f;
			float ipw = 1.0f / pw;
			float iph = 1.0f / ph;
			stbtt_bakedchar* b = chardata + char_index;
			int round_x = ((int)(CRuntime.Floor((*xpos + b->xoff) + 0.5f)));
			int round_y = ((int)(CRuntime.Floor((*ypos + b->yoff) + 0.5f)));
			q->x0 = round_x + d3d_bias;
			q->y0 = round_y + d3d_bias;
			q->x1 = round_x + b->x1 - b->x0 + d3d_bias;
			q->y1 = round_y + b->y1 - b->y0 + d3d_bias;
			q->s0 = b->x0 * ipw;
			q->t0 = b->y0 * iph;
			q->s1 = b->x1 * ipw;
			q->t1 = b->y1 * iph;
			*xpos += b->xadvance;
		}

		public static void stbtt__h_prefilter(byte* pixels, int w, int h, int stride_in_bytes, uint kernel_width)
		{
			byte* buffer = stackalloc byte[8];
			int safe_w = (int)(w - kernel_width);
			int j = 0;
			CRuntime.Memset(buffer, 0, (ulong)(8));
			for (j = 0; (j) < (h); ++j)
			{
				int i = 0;
				uint total = 0;
				CRuntime.Memset(buffer, 0, (ulong)(kernel_width));
				total = 0;
				switch (kernel_width)
				{
					case 2:
						for (i = 0; i <= safe_w; ++i)
						{
							total += (uint)(pixels[i] - buffer[i & (8 - 1)]);
							buffer[(i + kernel_width) & (8 - 1)] = pixels[i];
							pixels[i] = ((byte)(total / 2));
						}
						break;
					case 3:
						for (i = 0; i <= safe_w; ++i)
						{
							total += (uint)(pixels[i] - buffer[i & (8 - 1)]);
							buffer[(i + kernel_width) & (8 - 1)] = pixels[i];
							pixels[i] = ((byte)(total / 3));
						}
						break;
					case 4:
						for (i = 0; i <= safe_w; ++i)
						{
							total += (uint)(pixels[i] - buffer[i & (8 - 1)]);
							buffer[(i + kernel_width) & (8 - 1)] = pixels[i];
							pixels[i] = ((byte)(total / 4));
						}
						break;
					case 5:
						for (i = 0; i <= safe_w; ++i)
						{
							total += (uint)(pixels[i] - buffer[i & (8 - 1)]);
							buffer[(i + kernel_width) & (8 - 1)] = pixels[i];
							pixels[i] = ((byte)(total / 5));
						}
						break;
					default:
						for (i = 0; i <= safe_w; ++i)
						{
							total += (uint)(pixels[i] - buffer[i & (8 - 1)]);
							buffer[(i + kernel_width) & (8 - 1)] = pixels[i];
							pixels[i] = ((byte)(total / kernel_width));
						}
						break;
				}
				for (; (i) < (w); ++i)
				{
					total -= buffer[i & (8 - 1)];
					pixels[i] = ((byte)(total / kernel_width));
				}
				pixels += stride_in_bytes;
			}
		}

		public static void stbtt__v_prefilter(byte* pixels, int w, int h, int stride_in_bytes, uint kernel_width)
		{
			byte* buffer = stackalloc byte[8];
			int safe_h = (int)(h - kernel_width);
			int j = 0;
			CRuntime.Memset(buffer, 0, (ulong)(8));
			for (j = 0; (j) < (w); ++j)
			{
				int i = 0;
				uint total = 0;
				CRuntime.Memset(buffer, 0, (ulong)(kernel_width));
				total = 0;
				switch (kernel_width)
				{
					case 2:
						for (i = 0; i <= safe_h; ++i)
						{
							total += (uint)(pixels[i * stride_in_bytes] - buffer[i & (8 - 1)]);
							buffer[(i + kernel_width) & (8 - 1)] = pixels[i * stride_in_bytes];
							pixels[i * stride_in_bytes] = ((byte)(total / 2));
						}
						break;
					case 3:
						for (i = 0; i <= safe_h; ++i)
						{
							total += (uint)(pixels[i * stride_in_bytes] - buffer[i & (8 - 1)]);
							buffer[(i + kernel_width) & (8 - 1)] = pixels[i * stride_in_bytes];
							pixels[i * stride_in_bytes] = ((byte)(total / 3));
						}
						break;
					case 4:
						for (i = 0; i <= safe_h; ++i)
						{
							total += (uint)(pixels[i * stride_in_bytes] - buffer[i & (8 - 1)]);
							buffer[(i + kernel_width) & (8 - 1)] = pixels[i * stride_in_bytes];
							pixels[i * stride_in_bytes] = ((byte)(total / 4));
						}
						break;
					case 5:
						for (i = 0; i <= safe_h; ++i)
						{
							total += (uint)(pixels[i * stride_in_bytes] - buffer[i & (8 - 1)]);
							buffer[(i + kernel_width) & (8 - 1)] = pixels[i * stride_in_bytes];
							pixels[i * stride_in_bytes] = ((byte)(total / 5));
						}
						break;
					default:
						for (i = 0; i <= safe_h; ++i)
						{
							total += (uint)(pixels[i * stride_in_bytes] - buffer[i & (8 - 1)]);
							buffer[(i + kernel_width) & (8 - 1)] = pixels[i * stride_in_bytes];
							pixels[i * stride_in_bytes] = ((byte)(total / kernel_width));
						}
						break;
				}
				for (; (i) < (h); ++i)
				{
					total -= buffer[i & (8 - 1)];
					pixels[i * stride_in_bytes] = ((byte)(total / kernel_width));
				}
				pixels += 1;
			}
		}

		public static float stbtt__oversample_shift(int oversample)
		{
			if (oversample == 0)
				return 0.0f;
			return -(oversample - 1) / (2.0f * oversample);
		}

		public static void stbtt_GetScaledFontVMetrics(byte* fontdata, int index, float size, float* ascent, float* descent, float* lineGap)
		{
			int i_ascent = 0;
			int i_descent = 0;
			int i_lineGap = 0;
			float scale = 0;
			stbtt_fontinfo info = new stbtt_fontinfo();
			stbtt_InitFont(info, fontdata, stbtt_GetFontOffsetForIndex(fontdata, index));
			scale = (size) > (0) ? stbtt_ScaleForPixelHeight(info, size) : stbtt_ScaleForMappingEmToPixels(info, -size);
			stbtt_GetFontVMetrics(info, &i_ascent, &i_descent, &i_lineGap);
			*ascent = i_ascent * scale;
			*descent = i_descent * scale;
			*lineGap = i_lineGap * scale;
		}

		public static void stbtt_GetPackedQuad(stbtt_packedchar* chardata, int pw, int ph, int char_index, float* xpos, float* ypos, stbtt_aligned_quad* q, int align_to_integer)
		{
			float ipw = 1.0f / pw;
			float iph = 1.0f / ph;
			stbtt_packedchar* b = chardata + char_index;
			if ((align_to_integer) != 0)
			{
				float x = (int)(CRuntime.Floor((*xpos + b->xoff) + 0.5f));
				float y = (int)(CRuntime.Floor((*ypos + b->yoff) + 0.5f));
				q->x0 = x;
				q->y0 = y;
				q->x1 = x + b->xoff2 - b->xoff;
				q->y1 = y + b->yoff2 - b->yoff;
			}
			else
			{
				q->x0 = *xpos + b->xoff;
				q->y0 = *ypos + b->yoff;
				q->x1 = *xpos + b->xoff2;
				q->y1 = *ypos + b->yoff2;
			}

			q->s0 = b->x0 * ipw;
			q->t0 = b->y0 * iph;
			q->s1 = b->x1 * ipw;
			q->t1 = b->y1 * iph;
			*xpos += b->xadvance;
		}

		public static int stbtt__ray_intersect_bezier(float* orig, float* ray, float* q0, float* q1, float* q2, float* hits)
		{
			float q0perp = q0[1] * ray[0] - q0[0] * ray[1];
			float q1perp = q1[1] * ray[0] - q1[0] * ray[1];
			float q2perp = q2[1] * ray[0] - q2[0] * ray[1];
			float roperp = orig[1] * ray[0] - orig[0] * ray[1];
			float a = q0perp - 2 * q1perp + q2perp;
			float b = q1perp - q0perp;
			float c = q0perp - roperp;
			float s0 = 0;
			float s1 = 0;
			int num_s = 0;
			if (a != 0.0)
			{
				float discr = b * b - a * c;
				if ((discr) > (0.0))
				{
					float rcpna = -1 / a;
					float d = (float)(CRuntime.Sqrt(discr));
					s0 = (b + d) * rcpna;
					s1 = (b - d) * rcpna;
					if (((s0) >= (0.0)) && (s0 <= 1.0))
						num_s = 1;
					if ((((d) > (0.0)) && ((s1) >= (0.0))) && (s1 <= 1.0))
					{
						if ((num_s) == (0))
							s0 = s1;
						++num_s;
					}
				}
			}
			else
			{
				s0 = c / (-2 * b);
				if (((s0) >= (0.0)) && (s0 <= 1.0))
					num_s = 1;
			}

			if ((num_s) == (0))
				return 0;
			else
			{
				float rcp_len2 = 1 / (ray[0] * ray[0] + ray[1] * ray[1]);
				float rayn_x = ray[0] * rcp_len2;
				float rayn_y = ray[1] * rcp_len2;
				float q0d = q0[0] * rayn_x + q0[1] * rayn_y;
				float q1d = q1[0] * rayn_x + q1[1] * rayn_y;
				float q2d = q2[0] * rayn_x + q2[1] * rayn_y;
				float rod = orig[0] * rayn_x + orig[1] * rayn_y;
				float q10d = q1d - q0d;
				float q20d = q2d - q0d;
				float q0rd = q0d - rod;
				hits[0] = q0rd + s0 * (2.0f - 2.0f * s0) * q10d + s0 * s0 * q20d;
				hits[1] = a * s0 + b;
				if ((num_s) > (1))
				{
					hits[2] = q0rd + s1 * (2.0f - 2.0f * s1) * q10d + s1 * s1 * q20d;
					hits[3] = a * s1 + b;
					return 2;
				}
				else
				{
					return 1;
				}
			}

		}

		public static int equal(float* a, float* b)
		{
			return ((a[0] == b[0]) && (a[1] == b[1])) ? 1 : 0;
		}

		public static int stbtt__compute_crossings_x(float x, float y, int nverts, stbtt_vertex* verts)
		{
			int i = 0;
			float* orig = stackalloc float[2];
			float* ray = stackalloc float[2];
			ray[0] = 1;
			ray[1] = 0;

			float y_frac = 0;
			int winding = 0;
			orig[0] = x;
			orig[1] = y;
			y_frac = ((float)(CRuntime.Fmod(y, 1.0f)));
			if ((y_frac) < (0.01f))
				y += 0.01f;
			else if ((y_frac) > (0.99f))
				y -= 0.01f;
			orig[1] = y;
			for (i = 0; (i) < (nverts); ++i)
			{
				if ((verts[i].type) == (STBTT_vline))
				{
					int x0 = verts[i - 1].x;
					int y0 = verts[i - 1].y;
					int x1 = verts[i].x;
					int y1 = verts[i].y;
					if ((((y) > ((y0) < (y1) ? (y0) : (y1))) && ((y) < ((y0) < (y1) ? (y1) : (y0)))) && ((x) > ((x0) < (x1) ? (x0) : (x1))))
					{
						float x_inter = (y - y0) / (y1 - y0) * (x1 - x0) + x0;
						if ((x_inter) < (x))
							winding += ((y0) < (y1)) ? 1 : -1;
					}
				}
				if ((verts[i].type) == (STBTT_vcurve))
				{
					int x0 = verts[i - 1].x;
					int y0 = verts[i - 1].y;
					int x1 = verts[i].cx;
					int y1 = verts[i].cy;
					int x2 = verts[i].x;
					int y2 = verts[i].y;
					int ax = (x0) < ((x1) < (x2) ? (x1) : (x2)) ? (x0) : ((x1) < (x2) ? (x1) : (x2));
					int ay = (y0) < ((y1) < (y2) ? (y1) : (y2)) ? (y0) : ((y1) < (y2) ? (y1) : (y2));
					int by = (y0) < ((y1) < (y2) ? (y2) : (y1)) ? ((y1) < (y2) ? (y2) : (y1)) : (y0);
					if ((((y) > (ay)) && ((y) < (by))) && ((x) > (ax)))
					{
						float* q0 = stackalloc float[2];
						float* q1 = stackalloc float[2];
						float* q2 = stackalloc float[2];
						float* hits = stackalloc float[4];
						q0[0] = x0;
						q0[1] = y0;
						q1[0] = x1;
						q1[1] = y1;
						q2[0] = x2;
						q2[1] = y2;
						if (((equal(q0, q1)) != 0) || ((equal(q1, q2)) != 0))
						{
							x0 = verts[i - 1].x;
							y0 = verts[i - 1].y;
							x1 = verts[i].x;
							y1 = verts[i].y;
							if ((((y) > ((y0) < (y1) ? (y0) : (y1))) && ((y) < ((y0) < (y1) ? (y1) : (y0)))) && ((x) > ((x0) < (x1) ? (x0) : (x1))))
							{
								float x_inter = (y - y0) / (y1 - y0) * (x1 - x0) + x0;
								if ((x_inter) < (x))
									winding += ((y0) < (y1)) ? 1 : -1;
							}
						}
						else
						{
							int num_hits = stbtt__ray_intersect_bezier(orig, ray, q0, q1, q2, hits);
							if ((num_hits) >= (1))
								if ((hits[0]) < (0))
									winding += (hits[1]) < (0) ? -1 : 1;
							if ((num_hits) >= (2))
								if ((hits[2]) < (0))
									winding += (hits[3]) < (0) ? -1 : 1;
						}
					}
				}
			}
			return winding;
		}

		public static float stbtt__cuberoot(float x)
		{
			if ((x) < (0))
				return -(float)(CRuntime.Pow(-x, 1.0f / 3.0f));
			else
				return (float)(CRuntime.Pow(x, 1.0f / 3.0f));
		}

		public static int stbtt__solve_cubic(float a, float b, float c, float* r)
		{
			float s = -a / 3;
			float p = b - a * a / 3;
			float q = a * (2 * a * a - 9 * b) / 27 + c;
			float p3 = p * p * p;
			float d = q * q + 4 * p3 / 27;
			if ((d) >= (0))
			{
				float z = (float)(CRuntime.Sqrt(d));
				float u = (-q + z) / 2;
				float v = (-q - z) / 2;
				u = stbtt__cuberoot(u);
				v = stbtt__cuberoot(v);
				r[0] = s + u + v;
				return 1;
			}
			else
			{
				float u = (float)(CRuntime.Sqrt(-p / 3));
				float v = (float)(CRuntime.Acos(-CRuntime.Sqrt(-27 / p3) * q / 2)) / 3;
				float m = (float)(CRuntime.Cos(v));
				float n = (float)(CRuntime.Cos(v - 3.141592 / 2)) * 1.732050808f;
				r[0] = s + u * 2 * m;
				r[1] = s - u * (m + n);
				r[2] = s - u * (m - n);
				return 3;
			}

		}

		public static void stbtt_FreeSDF(byte* bitmap, void* userdata)
		{
			CRuntime.Free(bitmap);
		}

		public static int stbtt__CompareUTF8toUTF16_bigendian_prefix(byte* s1, int len1, byte* s2, int len2)
		{
			int i = 0;
			while ((len2) != 0)
			{
				ushort ch = (ushort)(s2[0] * 256 + s2[1]);
				if ((ch) < (0x80))
				{
					if ((i) >= (len1))
						return -1;
					if (s1[i++] != ch)
						return -1;
				}
				else if ((ch) < (0x800))
				{
					if ((i + 1) >= (len1))
						return -1;
					if (s1[i++] != 0xc0 + (ch >> 6))
						return -1;
					if (s1[i++] != 0x80 + (ch & 0x3f))
						return -1;
				}
				else if (((ch) >= (0xd800)) && ((ch) < (0xdc00)))
				{
					uint c = 0;
					ushort ch2 = (ushort)(s2[2] * 256 + s2[3]);
					if ((i + 3) >= (len1))
						return -1;
					c = (uint)(((ch - 0xd800) << 10) + (ch2 - 0xdc00) + 0x10000);
					if (s1[i++] != 0xf0 + (c >> 18))
						return -1;
					if (s1[i++] != 0x80 + ((c >> 12) & 0x3f))
						return -1;
					if (s1[i++] != 0x80 + ((c >> 6) & 0x3f))
						return -1;
					if (s1[i++] != 0x80 + ((c) & 0x3f))
						return -1;
					s2 += 2;
					len2 -= 2;
				}
				else if (((ch) >= (0xdc00)) && ((ch) < (0xe000)))
				{
					return -1;
				}
				else
				{
					if ((i + 2) >= (len1))
						return -1;
					if (s1[i++] != 0xe0 + (ch >> 12))
						return -1;
					if (s1[i++] != 0x80 + ((ch >> 6) & 0x3f))
						return -1;
					if (s1[i++] != 0x80 + ((ch) & 0x3f))
						return -1;
				}
				s2 += 2;
				len2 -= 2;
			}
			return i;
		}

		public static int stbtt_CompareUTF8toUTF16_bigendian_internal(sbyte* s1, int len1, sbyte* s2, int len2)
		{
			return (len1) == (stbtt__CompareUTF8toUTF16_bigendian_prefix((byte*)(s1), len1, (byte*)(s2), len2)) ? 1 : 0;
		}

		public static int stbtt__matchpair(byte* fc, uint nm, byte* name, int nlen, int target_id, int next_id)
		{
			int i = 0;
			int count = ttUSHORT(fc + nm + 2);
			int stringOffset = (int)(nm + ttUSHORT(fc + nm + 4));
			for (i = 0; (i) < (count); ++i)
			{
				uint loc = (uint)(nm + 6 + 12 * i);
				int id = ttUSHORT(fc + loc + 6);
				if ((id) == (target_id))
				{
					int platform = ttUSHORT(fc + loc + 0);
					int encoding = ttUSHORT(fc + loc + 2);
					int language = ttUSHORT(fc + loc + 4);
					if ((((platform) == (0)) || (((platform) == (3)) && ((encoding) == (1)))) || (((platform) == (3)) && ((encoding) == (10))))
					{
						int slen = ttUSHORT(fc + loc + 8);
						int off = ttUSHORT(fc + loc + 10);
						int matchlen = stbtt__CompareUTF8toUTF16_bigendian_prefix(name, nlen, fc + stringOffset + off, slen);
						if ((matchlen) >= (0))
						{
							if ((((((i + 1) < (count)) && ((ttUSHORT(fc + loc + 12 + 6)) == (next_id))) && ((ttUSHORT(fc + loc + 12)) == (platform))) && ((ttUSHORT(fc + loc + 12 + 2)) == (encoding))) && ((ttUSHORT(fc + loc + 12 + 4)) == (language)))
							{
								slen = ttUSHORT(fc + loc + 12 + 8);
								off = ttUSHORT(fc + loc + 12 + 10);
								if ((slen) == (0))
								{
									if ((matchlen) == (nlen))
										return 1;
								}
								else if (((matchlen) < (nlen)) && ((name[matchlen]) == (' ')))
								{
									++matchlen;
									if ((stbtt_CompareUTF8toUTF16_bigendian_internal((sbyte*)(name + matchlen), nlen - matchlen, (sbyte*)(fc + stringOffset + off), slen)) != 0)
										return 1;
								}
							}
							else
							{
								if ((matchlen) == (nlen))
									return 1;
							}
						}
					}
				}
			}
			return 0;
		}

		public static int stbtt__matches(byte* fc, uint offset, byte* name, int flags)
		{
			int nlen = (int)(CRuntime.Strlen((sbyte*)(name)));
			uint nm = 0;
			uint hd = 0;
			if (stbtt__isfont(fc + offset) == 0)
				return 0;
			if ((flags) != 0)
			{
				hd = stbtt__find_table(fc, offset, "head");
				if ((ttUSHORT(fc + hd + 44) & 7) != (flags & 7))
					return 0;
			}

			nm = stbtt__find_table(fc, offset, "name");
			if (nm == 0)
				return 0;
			if ((flags) != 0)
			{
				if ((stbtt__matchpair(fc, nm, name, nlen, 16, -1)) != 0)
					return 1;
				if ((stbtt__matchpair(fc, nm, name, nlen, 1, -1)) != 0)
					return 1;
				if ((stbtt__matchpair(fc, nm, name, nlen, 3, -1)) != 0)
					return 1;
			}
			else
			{
				if ((stbtt__matchpair(fc, nm, name, nlen, 16, 17)) != 0)
					return 1;
				if ((stbtt__matchpair(fc, nm, name, nlen, 1, 2)) != 0)
					return 1;
				if ((stbtt__matchpair(fc, nm, name, nlen, 3, -1)) != 0)
					return 1;
			}

			return 0;
		}

		public static int stbtt_FindMatchingFont_internal(byte* font_collection, sbyte* name_utf8, int flags)
		{
			int i = 0;
			for (i = 0; ; ++i)
			{
				int off = stbtt_GetFontOffsetForIndex(font_collection, i);
				if ((off) < (0))
					return off;
				if ((stbtt__matches(font_collection, (uint)(off), (byte*)(name_utf8), flags)) != 0)
					return off;
			}
		}

		public static int stbtt_BakeFontBitmap(byte* data, int offset, float pixel_height, byte* pixels, int pw, int ph, int first_char, int num_chars, stbtt_bakedchar* chardata)
		{
			return stbtt_BakeFontBitmap_internal(data, offset, pixel_height, pixels, pw, ph, first_char, num_chars, chardata);
		}

		public static int stbtt_GetFontOffsetForIndex(byte* data, int index)
		{
			return stbtt_GetFontOffsetForIndex_internal(data, index);
		}

		public static int stbtt_GetNumberOfFonts(byte* data)
		{
			return stbtt_GetNumberOfFonts_internal(data);
		}

		public static int stbtt_FindMatchingFont(byte* fontdata, sbyte* name, int flags)
		{
			return stbtt_FindMatchingFont_internal(fontdata, name, flags);
		}

		public static int stbtt_CompareUTF8toUTF16_bigendian(sbyte* s1, int len1, sbyte* s2, int len2)
		{
			return stbtt_CompareUTF8toUTF16_bigendian_internal(s1, len1, s2, len2);
		}
	}
}
