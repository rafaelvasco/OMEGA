﻿using System.Runtime.InteropServices;

namespace STB
{
    internal unsafe partial class StbTrueType
	{
		public class stbtt_fontinfo
		{
			public void* userdata;
			public byte* data;
			public int fontstart = 0;
			public int numGlyphs = 0;
			public int loca = 0;
			public int head = 0;
			public int glyf = 0;
			public int hhea = 0;
			public int hmtx = 0;
			public int kern = 0;
			public int gpos = 0;
			public int svg = 0;
			public int index_map = 0;
			public int indexToLocFormat = 0;
			public stbtt__buf cff = new stbtt__buf();
			public stbtt__buf charstrings = new stbtt__buf();
			public stbtt__buf gsubrs = new stbtt__buf();
			public stbtt__buf subrs = new stbtt__buf();
			public stbtt__buf fontdicts = new stbtt__buf();
			public stbtt__buf fdselect = new stbtt__buf();
		}
		[StructLayout(LayoutKind.Sequential)]
		public struct stbtt_kerningentry
		{
			public int glyph1;
			public int glyph2;
			public int advance;
		}

		public static int stbtt__get_svg(stbtt_fontinfo info)
		{
			uint t = 0;
			if ((info.svg) < (0))
			{
				t = (uint)(stbtt__find_table(info.data, (uint)(info.fontstart), "SVG "));
				if ((t) != 0)
				{
					uint offset = (uint)(ttULONG(info.data + t + 2));
					info.svg = (int)(t + offset);
				}
				else
				{
					info.svg = (int)(0);
				}
			}

			return (int)(info.svg);
		}

		public static int stbtt_InitFont_internal(stbtt_fontinfo info, byte* data, int fontstart)
		{
			uint cmap = 0;
			uint t = 0;
			int i = 0;
			int numTables = 0;
			info.data = data;
			info.fontstart = (int)(fontstart);
			info.cff = (stbtt__buf)(stbtt__new_buf((null), (ulong)(0)));
			cmap = (uint)(stbtt__find_table(data, (uint)(fontstart), "cmap"));
			info.loca = (int)(stbtt__find_table(data, (uint)(fontstart), "loca"));
			info.head = (int)(stbtt__find_table(data, (uint)(fontstart), "head"));
			info.glyf = (int)(stbtt__find_table(data, (uint)(fontstart), "glyf"));
			info.hhea = (int)(stbtt__find_table(data, (uint)(fontstart), "hhea"));
			info.hmtx = (int)(stbtt__find_table(data, (uint)(fontstart), "hmtx"));
			info.kern = (int)(stbtt__find_table(data, (uint)(fontstart), "kern"));
			info.gpos = (int)(stbtt__find_table(data, (uint)(fontstart), "GPOS"));
			if ((((cmap == 0) || (info.head == 0)) || (info.hhea == 0)) || (info.hmtx == 0))
				return (int)(0);
			if ((info.glyf) != 0)
			{
				if (info.loca == 0)
					return (int)(0);
			}
			else
			{
				stbtt__buf b = new stbtt__buf();
				stbtt__buf topdict = new stbtt__buf();
				stbtt__buf topdictidx = new stbtt__buf();
				uint cstype = (uint)(2);
				uint charstrings = (uint)(0);
				uint fdarrayoff = (uint)(0);
				uint fdselectoff = (uint)(0);
				uint cff = 0;
				cff = (uint)(stbtt__find_table(data, (uint)(fontstart), "CFF "));
				if (cff == 0)
					return (int)(0);
				info.fontdicts = (stbtt__buf)(stbtt__new_buf((null), (ulong)(0)));
				info.fdselect = (stbtt__buf)(stbtt__new_buf((null), (ulong)(0)));
				info.cff = (stbtt__buf)(stbtt__new_buf(data + cff, (ulong)(512 * 1024 * 1024)));
				b = (stbtt__buf)(info.cff);
				stbtt__buf_skip(&b, (int)(2));
				stbtt__buf_seek(&b, (int)(stbtt__buf_get8(&b)));
				stbtt__cff_get_index(&b);
				topdictidx = (stbtt__buf)(stbtt__cff_get_index(&b));
				topdict = (stbtt__buf)(stbtt__cff_index_get((stbtt__buf)(topdictidx), (int)(0)));
				stbtt__cff_get_index(&b);
				info.gsubrs = (stbtt__buf)(stbtt__cff_get_index(&b));
				stbtt__dict_get_ints(&topdict, (int)(17), (int)(1), &charstrings);
				stbtt__dict_get_ints(&topdict, (int)(0x100 | 6), (int)(1), &cstype);
				stbtt__dict_get_ints(&topdict, (int)(0x100 | 36), (int)(1), &fdarrayoff);
				stbtt__dict_get_ints(&topdict, (int)(0x100 | 37), (int)(1), &fdselectoff);
				info.subrs = (stbtt__buf)(stbtt__get_subrs((stbtt__buf)(b), (stbtt__buf)(topdict)));
				if (cstype != 2)
					return (int)(0);
				if ((charstrings) == (0))
					return (int)(0);
				if ((fdarrayoff) != 0)
				{
					if (fdselectoff == 0)
						return (int)(0);
					stbtt__buf_seek(&b, (int)(fdarrayoff));
					info.fontdicts = (stbtt__buf)(stbtt__cff_get_index(&b));
					info.fdselect = (stbtt__buf)(stbtt__buf_range(&b, (int)(fdselectoff), (int)(b.size - fdselectoff)));
				}
				stbtt__buf_seek(&b, (int)(charstrings));
				info.charstrings = (stbtt__buf)(stbtt__cff_get_index(&b));
			}

			t = (uint)(stbtt__find_table(data, (uint)(fontstart), "maxp"));
			if ((t) != 0)
				info.numGlyphs = (int)(ttUSHORT(data + t + 4));
			else
				info.numGlyphs = (int)(0xffff);
			info.svg = (int)(-1);
			numTables = (int)(ttUSHORT(data + cmap + 2));
			info.index_map = (int)(0);
			for (i = (int)(0); (i) < (numTables); ++i)
			{
				uint encoding_record = (uint)(cmap + 4 + 8 * i);
				switch (ttUSHORT(data + encoding_record))
				{
					case STBTT_PLATFORM_ID_MICROSOFT:
						switch (ttUSHORT(data + encoding_record + 2))
						{
							case STBTT_MS_EID_UNICODE_BMP:
							case STBTT_MS_EID_UNICODE_FULL:
								info.index_map = (int)(cmap + ttULONG(data + encoding_record + 4));
								break;
						}
						break;
					case STBTT_PLATFORM_ID_UNICODE:
						info.index_map = (int)(cmap + ttULONG(data + encoding_record + 4));
						break;
				}
			}
			if ((info.index_map) == (0))
				return (int)(0);
			info.indexToLocFormat = (int)(ttUSHORT(data + info.head + 50));
			return (int)(1);
		}

		public static int stbtt_FindGlyphIndex(stbtt_fontinfo info, int unicode_codepoint)
		{
			byte* data = info.data;
			uint index_map = (uint)(info.index_map);
			ushort format = (ushort)(ttUSHORT(data + index_map + 0));
			if ((format) == (0))
			{
				int bytes = (int)(ttUSHORT(data + index_map + 2));
				if ((unicode_codepoint) < (bytes - 6))
					return (int)(*(data + index_map + 6 + unicode_codepoint));
				return (int)(0);
			}
			else if ((format) == (6))
			{
				uint first = (uint)(ttUSHORT(data + index_map + 6));
				uint count = (uint)(ttUSHORT(data + index_map + 8));
				if ((((uint)(unicode_codepoint)) >= (first)) && (((uint)(unicode_codepoint)) < (first + count)))
					return (int)(ttUSHORT(data + index_map + 10 + (unicode_codepoint - first) * 2));
				return (int)(0);
			}
			else if ((format) == (2))
			{
				return (int)(0);
			}
			else if ((format) == (4))
			{
				ushort segcount = (ushort)(ttUSHORT(data + index_map + 6) >> 1);
				ushort searchRange = (ushort)(ttUSHORT(data + index_map + 8) >> 1);
				ushort entrySelector = (ushort)(ttUSHORT(data + index_map + 10));
				ushort rangeShift = (ushort)(ttUSHORT(data + index_map + 12) >> 1);
				uint endCount = (uint)(index_map + 14);
				uint search = (uint)(endCount);
				if ((unicode_codepoint) > (0xffff))
					return (int)(0);
				if ((unicode_codepoint) >= (ttUSHORT(data + search + rangeShift * 2)))
					search += (uint)(rangeShift * 2);
				search -= (uint)(2);
				while ((entrySelector) != 0)
				{
					ushort end = 0;
					searchRange >>= 1;
					end = (ushort)(ttUSHORT(data + search + searchRange * 2));
					if ((unicode_codepoint) > (end))
						search += (uint)(searchRange * 2);
					--entrySelector;
				}
				search += (uint)(2);
				{
					ushort offset = 0;
					ushort start = 0;
					ushort item = (ushort)((search - endCount) >> 1);
					start = (ushort)(ttUSHORT(data + index_map + 14 + segcount * 2 + 2 + 2 * item));
					if ((unicode_codepoint) < (start))
						return (int)(0);
					offset = (ushort)(ttUSHORT(data + index_map + 14 + segcount * 6 + 2 + 2 * item));
					if ((offset) == (0))
						return (int)((ushort)(unicode_codepoint + ttSHORT(data + index_map + 14 + segcount * 4 + 2 + 2 * item)));
					return (int)(ttUSHORT(data + offset + (unicode_codepoint - start) * 2 + index_map + 14 + segcount * 6 + 2 + 2 * item));
				}
			}
			else if (((format) == (12)) || ((format) == (13)))
			{
				uint ngroups = (uint)(ttULONG(data + index_map + 12));
				int low = 0;
				int high = 0;
				low = (int)(0);
				high = ((int)(ngroups));
				while ((low) < (high))
				{
					int mid = (int)(low + ((high - low) >> 1));
					uint start_char = (uint)(ttULONG(data + index_map + 16 + mid * 12));
					uint end_char = (uint)(ttULONG(data + index_map + 16 + mid * 12 + 4));
					if (((uint)(unicode_codepoint)) < (start_char))
						high = (int)(mid);
					else if (((uint)(unicode_codepoint)) > (end_char))
						low = (int)(mid + 1);
					else
					{
						uint start_glyph = (uint)(ttULONG(data + index_map + 16 + mid * 12 + 8));
						if ((format) == (12))
							return (int)(start_glyph + unicode_codepoint - start_char);
						else
							return (int)(start_glyph);
					}
				}
				return (int)(0);
			}

			return (int)(0);
		}

		public static int stbtt_GetCodepointShape(stbtt_fontinfo info, int unicode_codepoint, stbtt_vertex** vertices)
		{
			return (int)(stbtt_GetGlyphShape(info, (int)(stbtt_FindGlyphIndex(info, (int)(unicode_codepoint))), vertices));
		}

		public static int stbtt__GetGlyfOffset(stbtt_fontinfo info, int glyph_index)
		{
			int g1 = 0;
			int g2 = 0;
			if ((glyph_index) >= (info.numGlyphs))
				return (int)(-1);
			if ((info.indexToLocFormat) >= (2))
				return (int)(-1);
			if ((info.indexToLocFormat) == (0))
			{
				g1 = (int)(info.glyf + ttUSHORT(info.data + info.loca + glyph_index * 2) * 2);
				g2 = (int)(info.glyf + ttUSHORT(info.data + info.loca + glyph_index * 2 + 2) * 2);
			}
			else
			{
				g1 = (int)(info.glyf + ttULONG(info.data + info.loca + glyph_index * 4));
				g2 = (int)(info.glyf + ttULONG(info.data + info.loca + glyph_index * 4 + 4));
			}

			return (int)((g1) == (g2) ? -1 : g1);
		}

		public static int stbtt_GetGlyphBox(stbtt_fontinfo info, int glyph_index, int* x0, int* y0, int* x1, int* y1)
		{
			if ((info.cff.size) != 0)
			{
				stbtt__GetGlyphInfoT2(info, (int)(glyph_index), x0, y0, x1, y1);
			}
			else
			{
				int g = (int)(stbtt__GetGlyfOffset(info, (int)(glyph_index)));
				if ((g) < (0))
					return (int)(0);
				if ((x0) != null)
					*x0 = (int)(ttSHORT(info.data + g + 2));
				if ((y0) != null)
					*y0 = (int)(ttSHORT(info.data + g + 4));
				if ((x1) != null)
					*x1 = (int)(ttSHORT(info.data + g + 6));
				if ((y1) != null)
					*y1 = (int)(ttSHORT(info.data + g + 8));
			}

			return (int)(1);
		}

		public static int stbtt_GetCodepointBox(stbtt_fontinfo info, int codepoint, int* x0, int* y0, int* x1, int* y1)
		{
			return (int)(stbtt_GetGlyphBox(info, (int)(stbtt_FindGlyphIndex(info, (int)(codepoint))), x0, y0, x1, y1));
		}

		public static int stbtt_IsGlyphEmpty(stbtt_fontinfo info, int glyph_index)
		{
			short numberOfContours = 0;
			int g = 0;
			if ((info.cff.size) != 0)
				return (int)((stbtt__GetGlyphInfoT2(info, (int)(glyph_index), (null), (null), (null), (null))) == (0) ? 1 : 0);
			g = (int)(stbtt__GetGlyfOffset(info, (int)(glyph_index)));
			if ((g) < (0))
				return (int)(1);
			numberOfContours = (short)(ttSHORT(info.data + g));
			return (int)((numberOfContours) == (0) ? 1 : 0);
		}

		public static int stbtt__close_shape(stbtt_vertex* vertices, int num_vertices, int was_off, int start_off, int sx, int sy, int scx, int scy, int cx, int cy)
		{
			if ((start_off) != 0)
			{
				if ((was_off) != 0)
					stbtt_setvertex(&vertices[num_vertices++], (byte)(STBTT_vcurve), (int)((cx + scx) >> 1), (int)((cy + scy) >> 1), (int)(cx), (int)(cy));
				stbtt_setvertex(&vertices[num_vertices++], (byte)(STBTT_vcurve), (int)(sx), (int)(sy), (int)(scx), (int)(scy));
			}
			else
			{
				if ((was_off) != 0)
					stbtt_setvertex(&vertices[num_vertices++], (byte)(STBTT_vcurve), (int)(sx), (int)(sy), (int)(cx), (int)(cy));
				else
					stbtt_setvertex(&vertices[num_vertices++], (byte)(STBTT_vline), (int)(sx), (int)(sy), (int)(0), (int)(0));
			}

			return (int)(num_vertices);
		}

		public static int stbtt__GetGlyphShapeTT(stbtt_fontinfo info, int glyph_index, stbtt_vertex** pvertices)
		{
			short numberOfContours = 0;
			byte* endPtsOfContours;
			byte* data = info.data;
			stbtt_vertex* vertices = null;
			int num_vertices = (int)(0);
			int g = (int)(stbtt__GetGlyfOffset(info, (int)(glyph_index)));
			*pvertices = (null);
			if ((g) < (0))
				return (int)(0);
			numberOfContours = (short)(ttSHORT(data + g));
			if ((numberOfContours) > (0))
			{
				byte flags = (byte)(0);
				byte flagcount = 0;
				int ins = 0;
				int i = 0;
				int j = (int)(0);
				int m = 0;
				int n = 0;
				int next_move = 0;
				int was_off = (int)(0);
				int off = 0;
				int start_off = (int)(0);
				int x = 0;
				int y = 0;
				int cx = 0;
				int cy = 0;
				int sx = 0;
				int sy = 0;
				int scx = 0;
				int scy = 0;
				byte* points;
				endPtsOfContours = (data + g + 10);
				ins = (int)(ttUSHORT(data + g + 10 + numberOfContours * 2));
				points = data + g + 10 + numberOfContours * 2 + 2 + ins;
				n = (int)(1 + ttUSHORT(endPtsOfContours + numberOfContours * 2 - 2));
				m = (int)(n + 2 * numberOfContours);
				vertices = (stbtt_vertex*)(CRuntime.Malloc((ulong)(m * sizeof(stbtt_vertex))));
				if ((vertices) == (null))
					return (int)(0);
				next_move = (int)(0);
				flagcount = (byte)(0);
				off = (int)(m - n);
				for (i = (int)(0); (i) < (n); ++i)
				{
					if ((flagcount) == (0))
					{
						flags = (byte)(*points++);
						if ((flags & 8) != 0)
							flagcount = (byte)(*points++);
					}
					else
						--flagcount;
					vertices[off + i].type = (byte)(flags);
				}
				x = (int)(0);
				for (i = (int)(0); (i) < (n); ++i)
				{
					flags = (byte)(vertices[off + i].type);
					if ((flags & 2) != 0)
					{
						short dx = (short)(*points++);
						x += (int)((flags & 16) != 0 ? dx : -dx);
					}
					else
					{
						if ((flags & 16) == 0)
						{
							x = (int)(x + (short)(points[0] * 256 + points[1]));
							points += 2;
						}
					}
					vertices[off + i].x = ((short)(x));
				}
				y = (int)(0);
				for (i = (int)(0); (i) < (n); ++i)
				{
					flags = (byte)(vertices[off + i].type);
					if ((flags & 4) != 0)
					{
						short dy = (short)(*points++);
						y += (int)((flags & 32) != 0 ? dy : -dy);
					}
					else
					{
						if ((flags & 32) == 0)
						{
							y = (int)(y + (short)(points[0] * 256 + points[1]));
							points += 2;
						}
					}
					vertices[off + i].y = ((short)(y));
				}
				num_vertices = (int)(0);
				sx = (int)(sy = (int)(cx = (int)(cy = (int)(scx = (int)(scy = (int)(0))))));
				for (i = (int)(0); (i) < (n); ++i)
				{
					flags = (byte)(vertices[off + i].type);
					x = (int)(vertices[off + i].x);
					y = (int)(vertices[off + i].y);
					if ((next_move) == (i))
					{
						if (i != 0)
							num_vertices = (int)(stbtt__close_shape(vertices, (int)(num_vertices), (int)(was_off), (int)(start_off), (int)(sx), (int)(sy), (int)(scx), (int)(scy), (int)(cx), (int)(cy)));
						start_off = ((flags & 1) != 0 ? 0 : 1);
						if ((start_off) != 0)
						{
							scx = (int)(x);
							scy = (int)(y);
							if ((vertices[off + i + 1].type & 1) == 0)
							{
								sx = (int)((x + (int)(vertices[off + i + 1].x)) >> 1);
								sy = (int)((y + (int)(vertices[off + i + 1].y)) >> 1);
							}
							else
							{
								sx = ((int)(vertices[off + i + 1].x));
								sy = ((int)(vertices[off + i + 1].y));
								++i;
							}
						}
						else
						{
							sx = (int)(x);
							sy = (int)(y);
						}
						stbtt_setvertex(&vertices[num_vertices++], (byte)(STBTT_vmove), (int)(sx), (int)(sy), (int)(0), (int)(0));
						was_off = (int)(0);
						next_move = (int)(1 + ttUSHORT(endPtsOfContours + j * 2));
						++j;
					}
					else
					{
						if ((flags & 1) == 0)
						{
							if ((was_off) != 0)
								stbtt_setvertex(&vertices[num_vertices++], (byte)(STBTT_vcurve), (int)((cx + x) >> 1), (int)((cy + y) >> 1), (int)(cx), (int)(cy));
							cx = (int)(x);
							cy = (int)(y);
							was_off = (int)(1);
						}
						else
						{
							if ((was_off) != 0)
								stbtt_setvertex(&vertices[num_vertices++], (byte)(STBTT_vcurve), (int)(x), (int)(y), (int)(cx), (int)(cy));
							else
								stbtt_setvertex(&vertices[num_vertices++], (byte)(STBTT_vline), (int)(x), (int)(y), (int)(0), (int)(0));
							was_off = (int)(0);
						}
					}
				}
				num_vertices = (int)(stbtt__close_shape(vertices, (int)(num_vertices), (int)(was_off), (int)(start_off), (int)(sx), (int)(sy), (int)(scx), (int)(scy), (int)(cx), (int)(cy)));
			}
			else if ((numberOfContours) < (0))
			{
				int more = (int)(1);
				byte* comp = data + g + 10;
				num_vertices = (int)(0);
				vertices = null;
				while ((more) != 0)
				{
					ushort flags = 0;
					ushort gidx = 0;
					int comp_num_verts = (int)(0);
					int i = 0;
					stbtt_vertex* comp_verts = null;
					stbtt_vertex* tmp = null;
					float* mtx = stackalloc float[6];
					mtx[0] = (float)(1);
					mtx[1] = (float)(0);
					mtx[2] = (float)(0);
					mtx[3] = (float)(1);
					mtx[4] = (float)(0);
					mtx[5] = (float)(0);
					float m = 0;
					float n = 0;
					flags = (ushort)(ttSHORT(comp));
					comp += 2;
					gidx = (ushort)(ttSHORT(comp));
					comp += 2;
					if ((flags & 2) != 0)
					{
						if ((flags & 1) != 0)
						{
							mtx[4] = (float)(ttSHORT(comp));
							comp += 2;
							mtx[5] = (float)(ttSHORT(comp));
							comp += 2;
						}
						else
						{
							mtx[4] = (float)(*(sbyte*)(comp));
							comp += 1;
							mtx[5] = (float)(*(sbyte*)(comp));
							comp += 1;
						}
					}
					else
					{
					}
					if ((flags & (1 << 3)) != 0)
					{
						mtx[0] = (float)(mtx[3] = (float)(ttSHORT(comp) / 16384.0f));
						comp += 2;
						mtx[1] = (float)(mtx[2] = (float)(0));
					}
					else if ((flags & (1 << 6)) != 0)
					{
						mtx[0] = (float)(ttSHORT(comp) / 16384.0f);
						comp += 2;
						mtx[1] = (float)(mtx[2] = (float)(0));
						mtx[3] = (float)(ttSHORT(comp) / 16384.0f);
						comp += 2;
					}
					else if ((flags & (1 << 7)) != 0)
					{
						mtx[0] = (float)(ttSHORT(comp) / 16384.0f);
						comp += 2;
						mtx[1] = (float)(ttSHORT(comp) / 16384.0f);
						comp += 2;
						mtx[2] = (float)(ttSHORT(comp) / 16384.0f);
						comp += 2;
						mtx[3] = (float)(ttSHORT(comp) / 16384.0f);
						comp += 2;
					}
					m = ((float)(CRuntime.Sqrt((double)(mtx[0] * mtx[0] + mtx[1] * mtx[1]))));
					n = ((float)(CRuntime.Sqrt((double)(mtx[2] * mtx[2] + mtx[3] * mtx[3]))));
					comp_num_verts = (int)(stbtt_GetGlyphShape(info, (int)(gidx), &comp_verts));
					if ((comp_num_verts) > (0))
					{
						for (i = (int)(0); (i) < (comp_num_verts); ++i)
						{
							stbtt_vertex* v = &comp_verts[i];
							short x = 0;
							short y = 0;
							x = (short)(v->x);
							y = (short)(v->y);
							v->x = ((short)(m * (mtx[0] * x + mtx[2] * y + mtx[4])));
							v->y = ((short)(n * (mtx[1] * x + mtx[3] * y + mtx[5])));
							x = (short)(v->cx);
							y = (short)(v->cy);
							v->cx = ((short)(m * (mtx[0] * x + mtx[2] * y + mtx[4])));
							v->cy = ((short)(n * (mtx[1] * x + mtx[3] * y + mtx[5])));
						}
						tmp = (stbtt_vertex*)(CRuntime.Malloc((ulong)((num_vertices + comp_num_verts) * sizeof(stbtt_vertex))));
						if (tmp == null)
						{
							if ((vertices) != null)
								CRuntime.Free(vertices);
							if ((comp_verts) != null)
								CRuntime.Free(comp_verts);
							return (int)(0);
						}
						if ((num_vertices) > (0))
							CRuntime.Memcpy(tmp, vertices, (ulong)(num_vertices * sizeof(stbtt_vertex)));
						CRuntime.Memcpy(tmp + num_vertices, comp_verts, (ulong)(comp_num_verts * sizeof(stbtt_vertex)));
						if ((vertices) != null)
							CRuntime.Free(vertices);
						vertices = tmp;
						CRuntime.Free(comp_verts);
						num_vertices += (int)(comp_num_verts);
					}
					more = (int)(flags & (1 << 5));
				}
			}
			else
			{
			}

			*pvertices = vertices;
			return (int)(num_vertices);
		}

		public static stbtt__buf stbtt__cid_get_glyph_subrs(stbtt_fontinfo info, int glyph_index)
		{
			stbtt__buf fdselect = (stbtt__buf)(info.fdselect);
			int nranges = 0;
			int start = 0;
			int end = 0;
			int v = 0;
			int fmt = 0;
			int fdselector = (int)(-1);
			int i = 0;
			stbtt__buf_seek(&fdselect, (int)(0));
			fmt = (int)(stbtt__buf_get8(&fdselect));
			if ((fmt) == (0))
			{
				stbtt__buf_skip(&fdselect, (int)(glyph_index));
				fdselector = (int)(stbtt__buf_get8(&fdselect));
			}
			else if ((fmt) == (3))
			{
				nranges = (int)(stbtt__buf_get((&fdselect), (int)(2)));
				start = (int)(stbtt__buf_get((&fdselect), (int)(2)));
				for (i = (int)(0); (i) < (nranges); i++)
				{
					v = (int)(stbtt__buf_get8(&fdselect));
					end = (int)(stbtt__buf_get((&fdselect), (int)(2)));
					if (((glyph_index) >= (start)) && ((glyph_index) < (end)))
					{
						fdselector = (int)(v);
						break;
					}
					start = (int)(end);
				}
			}

			if ((fdselector) == (-1))
				stbtt__new_buf((null), (ulong)(0));
			return (stbtt__buf)(stbtt__get_subrs((stbtt__buf)(info.cff), (stbtt__buf)(stbtt__cff_index_get((stbtt__buf)(info.fontdicts), (int)(fdselector)))));
		}

		public static int stbtt__run_charstring(stbtt_fontinfo info, int glyph_index, stbtt__csctx* c)
		{
			int in_header = (int)(1);
			int maskbits = (int)(0);
			int subr_stack_height = (int)(0);
			int sp = (int)(0);
			int v = 0;
			int i = 0;
			int b0 = 0;
			int has_subrs = (int)(0);
			int clear_stack = 0;
			float* s = stackalloc float[48];
			stbtt__buf* subr_stack = stackalloc stbtt__buf[10];
			stbtt__buf subrs = (stbtt__buf)(info.subrs);
			stbtt__buf b = new stbtt__buf();
			float f = 0;
			b = (stbtt__buf)(stbtt__cff_index_get((stbtt__buf)(info.charstrings), (int)(glyph_index)));
			while ((b.cursor) < (b.size))
			{
				i = (int)(0);
				clear_stack = (int)(1);
				b0 = (int)(stbtt__buf_get8(&b));
				switch (b0)
				{
					case 0x13:
					case 0x14:
						if ((in_header) != 0)
							maskbits += (int)(sp / 2);
						in_header = (int)(0);
						stbtt__buf_skip(&b, (int)((maskbits + 7) / 8));
						break;
					case 0x01:
					case 0x03:
					case 0x12:
					case 0x17:
						maskbits += (int)(sp / 2);
						break;
					case 0x15:
						in_header = (int)(0);
						if ((sp) < (2))
							return (int)(0);
						stbtt__csctx_rmove_to(c, (float)(s[sp - 2]), (float)(s[sp - 1]));
						break;
					case 0x04:
						in_header = (int)(0);
						if ((sp) < (1))
							return (int)(0);
						stbtt__csctx_rmove_to(c, (float)(0), (float)(s[sp - 1]));
						break;
					case 0x16:
						in_header = (int)(0);
						if ((sp) < (1))
							return (int)(0);
						stbtt__csctx_rmove_to(c, (float)(s[sp - 1]), (float)(0));
						break;
					case 0x05:
						if ((sp) < (2))
							return (int)(0);
						for (; (i + 1) < (sp); i += (int)(2))
						{
							stbtt__csctx_rline_to(c, (float)(s[i]), (float)(s[i + 1]));
						}
						break;
					case 0x07:
					case 0x06:
						if ((sp) < (1))
							return (int)(0);
						int goto_vlineto = (int)((b0) == (0x07) ? 1 : 0);
						for (; ; )
						{
							if ((goto_vlineto) == (0))
							{
								if ((i) >= (sp))
									break;
								stbtt__csctx_rline_to(c, (float)(s[i]), (float)(0));
								i++;
							}
							goto_vlineto = (int)(0);
							if ((i) >= (sp))
								break;
							stbtt__csctx_rline_to(c, (float)(0), (float)(s[i]));
							i++;
						}
						break;
					case 0x1F:
					case 0x1E:
						if ((sp) < (4))
							return (int)(0);
						int goto_hvcurveto = (int)((b0) == (0x1F) ? 1 : 0);
						for (; ; )
						{
							if ((goto_hvcurveto) == (0))
							{
								if ((i + 3) >= (sp))
									break;
								stbtt__csctx_rccurve_to(c, (float)(0), (float)(s[i]), (float)(s[i + 1]), (float)(s[i + 2]), (float)(s[i + 3]), (float)(((sp - i) == (5)) ? s[i + 4] : 0.0f));
								i += (int)(4);
							}
							goto_hvcurveto = (int)(0);
							if ((i + 3) >= (sp))
								break;
							stbtt__csctx_rccurve_to(c, (float)(s[i]), (float)(0), (float)(s[i + 1]), (float)(s[i + 2]), (float)(((sp - i) == (5)) ? s[i + 4] : 0.0f), (float)(s[i + 3]));
							i += (int)(4);
						}
						break;
					case 0x08:
						if ((sp) < (6))
							return (int)(0);
						for (; (i + 5) < (sp); i += (int)(6))
						{
							stbtt__csctx_rccurve_to(c, (float)(s[i]), (float)(s[i + 1]), (float)(s[i + 2]), (float)(s[i + 3]), (float)(s[i + 4]), (float)(s[i + 5]));
						}
						break;
					case 0x18:
						if ((sp) < (8))
							return (int)(0);
						for (; (i + 5) < (sp - 2); i += (int)(6))
						{
							stbtt__csctx_rccurve_to(c, (float)(s[i]), (float)(s[i + 1]), (float)(s[i + 2]), (float)(s[i + 3]), (float)(s[i + 4]), (float)(s[i + 5]));
						}
						if ((i + 1) >= (sp))
							return (int)(0);
						stbtt__csctx_rline_to(c, (float)(s[i]), (float)(s[i + 1]));
						break;
					case 0x19:
						if ((sp) < (8))
							return (int)(0);
						for (; (i + 1) < (sp - 6); i += (int)(2))
						{
							stbtt__csctx_rline_to(c, (float)(s[i]), (float)(s[i + 1]));
						}
						if ((i + 5) >= (sp))
							return (int)(0);
						stbtt__csctx_rccurve_to(c, (float)(s[i]), (float)(s[i + 1]), (float)(s[i + 2]), (float)(s[i + 3]), (float)(s[i + 4]), (float)(s[i + 5]));
						break;
					case 0x1A:
					case 0x1B:
						if ((sp) < (4))
							return (int)(0);
						f = (float)(0.0);
						if ((sp & 1) != 0)
						{
							f = (float)(s[i]);
							i++;
						}
						for (; (i + 3) < (sp); i += (int)(4))
						{
							if ((b0) == (0x1B))
								stbtt__csctx_rccurve_to(c, (float)(s[i]), (float)(f), (float)(s[i + 1]), (float)(s[i + 2]), (float)(s[i + 3]), (float)(0.0));
							else
								stbtt__csctx_rccurve_to(c, (float)(f), (float)(s[i]), (float)(s[i + 1]), (float)(s[i + 2]), (float)(0.0), (float)(s[i + 3]));
							f = (float)(0.0);
						}
						break;
					case 0x0A:
					case 0x1D:
						if ((b0) == (0x0A))
						{
							if (has_subrs == 0)
							{
								if ((info.fdselect.size) != 0)
									subrs = (stbtt__buf)(stbtt__cid_get_glyph_subrs(info, (int)(glyph_index)));
								has_subrs = (int)(1);
							}
						}
						if ((sp) < (1))
							return (int)(0);
						v = ((int)(s[--sp]));
						if ((subr_stack_height) >= (10))
							return (int)(0);
						subr_stack[subr_stack_height++] = (stbtt__buf)(b);
						b = (stbtt__buf)(stbtt__get_subr((stbtt__buf)((b0) == (0x0A) ? subrs : info.gsubrs), (int)(v)));
						if ((b.size) == (0))
							return (int)(0);
						b.cursor = (int)(0);
						clear_stack = (int)(0);
						break;
					case 0x0B:
						if (subr_stack_height <= 0)
							return (int)(0);
						b = (stbtt__buf)(subr_stack[--subr_stack_height]);
						clear_stack = (int)(0);
						break;
					case 0x0E:
						stbtt__csctx_close_shape(c);
						return (int)(1);
					case 0x0C:
						{
							float dx1 = 0;
							float dx2 = 0;
							float dx3 = 0;
							float dx4 = 0;
							float dx5 = 0;
							float dx6 = 0;
							float dy1 = 0;
							float dy2 = 0;
							float dy3 = 0;
							float dy4 = 0;
							float dy5 = 0;
							float dy6 = 0;
							float dx = 0;
							float dy = 0;
							int b1 = (int)(stbtt__buf_get8(&b));
							switch (b1)
							{
								case 0x22:
									if ((sp) < (7))
										return (int)(0);
									dx1 = (float)(s[0]);
									dx2 = (float)(s[1]);
									dy2 = (float)(s[2]);
									dx3 = (float)(s[3]);
									dx4 = (float)(s[4]);
									dx5 = (float)(s[5]);
									dx6 = (float)(s[6]);
									stbtt__csctx_rccurve_to(c, (float)(dx1), (float)(0), (float)(dx2), (float)(dy2), (float)(dx3), (float)(0));
									stbtt__csctx_rccurve_to(c, (float)(dx4), (float)(0), (float)(dx5), (float)(-dy2), (float)(dx6), (float)(0));
									break;
								case 0x23:
									if ((sp) < (13))
										return (int)(0);
									dx1 = (float)(s[0]);
									dy1 = (float)(s[1]);
									dx2 = (float)(s[2]);
									dy2 = (float)(s[3]);
									dx3 = (float)(s[4]);
									dy3 = (float)(s[5]);
									dx4 = (float)(s[6]);
									dy4 = (float)(s[7]);
									dx5 = (float)(s[8]);
									dy5 = (float)(s[9]);
									dx6 = (float)(s[10]);
									dy6 = (float)(s[11]);
									stbtt__csctx_rccurve_to(c, (float)(dx1), (float)(dy1), (float)(dx2), (float)(dy2), (float)(dx3), (float)(dy3));
									stbtt__csctx_rccurve_to(c, (float)(dx4), (float)(dy4), (float)(dx5), (float)(dy5), (float)(dx6), (float)(dy6));
									break;
								case 0x24:
									if ((sp) < (9))
										return (int)(0);
									dx1 = (float)(s[0]);
									dy1 = (float)(s[1]);
									dx2 = (float)(s[2]);
									dy2 = (float)(s[3]);
									dx3 = (float)(s[4]);
									dx4 = (float)(s[5]);
									dx5 = (float)(s[6]);
									dy5 = (float)(s[7]);
									dx6 = (float)(s[8]);
									stbtt__csctx_rccurve_to(c, (float)(dx1), (float)(dy1), (float)(dx2), (float)(dy2), (float)(dx3), (float)(0));
									stbtt__csctx_rccurve_to(c, (float)(dx4), (float)(0), (float)(dx5), (float)(dy5), (float)(dx6), (float)(-(dy1 + dy2 + dy5)));
									break;
								case 0x25:
									if ((sp) < (11))
										return (int)(0);
									dx1 = (float)(s[0]);
									dy1 = (float)(s[1]);
									dx2 = (float)(s[2]);
									dy2 = (float)(s[3]);
									dx3 = (float)(s[4]);
									dy3 = (float)(s[5]);
									dx4 = (float)(s[6]);
									dy4 = (float)(s[7]);
									dx5 = (float)(s[8]);
									dy5 = (float)(s[9]);
									dx6 = (float)(dy6 = (float)(s[10]));
									dx = (float)(dx1 + dx2 + dx3 + dx4 + dx5);
									dy = (float)(dy1 + dy2 + dy3 + dy4 + dy5);
									if ((CRuntime.Fabs((double)(dx))) > (CRuntime.Fabs((double)(dy))))
										dy6 = (float)(-dy);
									else
										dx6 = (float)(-dx);
									stbtt__csctx_rccurve_to(c, (float)(dx1), (float)(dy1), (float)(dx2), (float)(dy2), (float)(dx3), (float)(dy3));
									stbtt__csctx_rccurve_to(c, (float)(dx4), (float)(dy4), (float)(dx5), (float)(dy5), (float)(dx6), (float)(dy6));
									break;
								default:
									return (int)(0);
							}
						}
						break;
					default:
						if (((b0 != 255) && (b0 != 28)) && (((b0) < (32)) || ((b0) > (254))))
							return (int)(0);
						if ((b0) == (255))
						{
							f = (float)((float)((int)(stbtt__buf_get((&b), (int)(4)))) / 0x10000);
						}
						else
						{
							stbtt__buf_skip(&b, (int)(-1));
							f = ((float)((short)(stbtt__cff_int(&b))));
						}
						if ((sp) >= (48))
							return (int)(0);
						s[sp++] = (float)(f);
						clear_stack = (int)(0);
						break;
				}
				if ((clear_stack) != 0)
					sp = (int)(0);
			}
			return (int)(0);
		}

		public static int stbtt__GetGlyphShapeT2(stbtt_fontinfo info, int glyph_index, stbtt_vertex** pvertices)
		{
			stbtt__csctx count_ctx = new stbtt__csctx();
			count_ctx.bounds = (int)(1);
			stbtt__csctx output_ctx = new stbtt__csctx();
			if ((stbtt__run_charstring(info, (int)(glyph_index), &count_ctx)) != 0)
			{
				*pvertices = (stbtt_vertex*)(CRuntime.Malloc((ulong)(count_ctx.num_vertices * sizeof(stbtt_vertex))));
				output_ctx.pvertices = *pvertices;
				if ((stbtt__run_charstring(info, (int)(glyph_index), &output_ctx)) != 0)
				{
					return (int)(output_ctx.num_vertices);
				}
			}

			*pvertices = (null);
			return (int)(0);
		}

		public static int stbtt__GetGlyphInfoT2(stbtt_fontinfo info, int glyph_index, int* x0, int* y0, int* x1, int* y1)
		{
			stbtt__csctx c = new stbtt__csctx();
			c.bounds = (int)(1);
			int r = (int)(stbtt__run_charstring(info, (int)(glyph_index), &c));
			if ((x0) != null)
				*x0 = (int)((r) != 0 ? c.min_x : 0);
			if ((y0) != null)
				*y0 = (int)((r) != 0 ? c.min_y : 0);
			if ((x1) != null)
				*x1 = (int)((r) != 0 ? c.max_x : 0);
			if ((y1) != null)
				*y1 = (int)((r) != 0 ? c.max_y : 0);
			return (int)((r) != 0 ? c.num_vertices : 0);
		}

		public static int stbtt_GetGlyphShape(stbtt_fontinfo info, int glyph_index, stbtt_vertex** pvertices)
		{
			if (info.cff.size == 0)
				return (int)(stbtt__GetGlyphShapeTT(info, (int)(glyph_index), pvertices));
			else
				return (int)(stbtt__GetGlyphShapeT2(info, (int)(glyph_index), pvertices));
		}

		public static void stbtt_GetGlyphHMetrics(stbtt_fontinfo info, int glyph_index, int* advanceWidth, int* leftSideBearing)
		{
			ushort numOfLongHorMetrics = (ushort)(ttUSHORT(info.data + info.hhea + 34));
			if ((glyph_index) < (numOfLongHorMetrics))
			{
				if ((advanceWidth) != null)
					*advanceWidth = (int)(ttSHORT(info.data + info.hmtx + 4 * glyph_index));
				if ((leftSideBearing) != null)
					*leftSideBearing = (int)(ttSHORT(info.data + info.hmtx + 4 * glyph_index + 2));
			}
			else
			{
				if ((advanceWidth) != null)
					*advanceWidth = (int)(ttSHORT(info.data + info.hmtx + 4 * (numOfLongHorMetrics - 1)));
				if ((leftSideBearing) != null)
					*leftSideBearing = (int)(ttSHORT(info.data + info.hmtx + 4 * numOfLongHorMetrics + 2 * (glyph_index - numOfLongHorMetrics)));
			}

		}

		public static int stbtt_GetKerningTableLength(stbtt_fontinfo info)
		{
			byte* data = info.data + info.kern;
			if (info.kern == 0)
				return (int)(0);
			if ((ttUSHORT(data + 2)) < (1))
				return (int)(0);
			if (ttUSHORT(data + 8) != 1)
				return (int)(0);
			return (int)(ttUSHORT(data + 10));
		}

		public static int stbtt_GetKerningTable(stbtt_fontinfo info, stbtt_kerningentry* table, int table_length)
		{
			byte* data = info.data + info.kern;
			int k = 0;
			int length = 0;
			if (info.kern == 0)
				return (int)(0);
			if ((ttUSHORT(data + 2)) < (1))
				return (int)(0);
			if (ttUSHORT(data + 8) != 1)
				return (int)(0);
			length = (int)(ttUSHORT(data + 10));
			if ((table_length) < (length))
				length = (int)(table_length);
			for (k = (int)(0); (k) < (length); k++)
			{
				table[k].glyph1 = (int)(ttUSHORT(data + 18 + (k * 6)));
				table[k].glyph2 = (int)(ttUSHORT(data + 20 + (k * 6)));
				table[k].advance = (int)(ttSHORT(data + 22 + (k * 6)));
			}
			return (int)(length);
		}

		public static int stbtt__GetGlyphKernInfoAdvance(stbtt_fontinfo info, int glyph1, int glyph2)
		{
			byte* data = info.data + info.kern;
			uint needle = 0;
			uint straw = 0;
			int l = 0;
			int r = 0;
			int m = 0;
			if (info.kern == 0)
				return (int)(0);
			if ((ttUSHORT(data + 2)) < (1))
				return (int)(0);
			if (ttUSHORT(data + 8) != 1)
				return (int)(0);
			l = (int)(0);
			r = (int)(ttUSHORT(data + 10) - 1);
			needle = (uint)(glyph1 << 16 | glyph2);
			while (l <= r)
			{
				m = (int)((l + r) >> 1);
				straw = (uint)(ttULONG(data + 18 + (m * 6)));
				if ((needle) < (straw))
					r = (int)(m - 1);
				else if ((needle) > (straw))
					l = (int)(m + 1);
				else
					return (int)(ttSHORT(data + 22 + (m * 6)));
			}
			return (int)(0);
		}

		public static int stbtt__GetCoverageIndex(byte* coverageTable, int glyph)
		{
			ushort coverageFormat = (ushort)(ttUSHORT(coverageTable));
			switch (coverageFormat)
			{
				case 1:
					{
						ushort glyphCount = (ushort)(ttUSHORT(coverageTable + 2));
						int l = (int)(0);
						int r = (int)(glyphCount - 1);
						int m = 0;
						int straw = 0;
						int needle = (int)(glyph);
						while (l <= r)
						{
							byte* glyphArray = coverageTable + 4;
							ushort glyphID = 0;
							m = (int)((l + r) >> 1);
							glyphID = (ushort)(ttUSHORT(glyphArray + 2 * m));
							straw = (int)(glyphID);
							if ((needle) < (straw))
								r = (int)(m - 1);
							else if ((needle) > (straw))
								l = (int)(m + 1);
							else
							{
								return (int)(m);
							}
						}
					}
					break;
				case 2:
					{
						ushort rangeCount = (ushort)(ttUSHORT(coverageTable + 2));
						byte* rangeArray = coverageTable + 4;
						int l = (int)(0);
						int r = (int)(rangeCount - 1);
						int m = 0;
						int strawStart = 0;
						int strawEnd = 0;
						int needle = (int)(glyph);
						while (l <= r)
						{
							byte* rangeRecord;
							m = (int)((l + r) >> 1);
							rangeRecord = rangeArray + 6 * m;
							strawStart = (int)(ttUSHORT(rangeRecord));
							strawEnd = (int)(ttUSHORT(rangeRecord + 2));
							if ((needle) < (strawStart))
								r = (int)(m - 1);
							else if ((needle) > (strawEnd))
								l = (int)(m + 1);
							else
							{
								ushort startCoverageIndex = (ushort)(ttUSHORT(rangeRecord + 4));
								return (int)(startCoverageIndex + glyph - strawStart);
							}
						}
					}
					break;
				default:
					{
					}
					break;
			}

			return (int)(-1);
		}

		public static int stbtt__GetGlyphClass(byte* classDefTable, int glyph)
		{
			ushort classDefFormat = (ushort)(ttUSHORT(classDefTable));
			switch (classDefFormat)
			{
				case 1:
					{
						ushort startGlyphID = (ushort)(ttUSHORT(classDefTable + 2));
						ushort glyphCount = (ushort)(ttUSHORT(classDefTable + 4));
						byte* classDef1ValueArray = classDefTable + 6;
						if (((glyph) >= (startGlyphID)) && ((glyph) < (startGlyphID + glyphCount)))
							return (int)(ttUSHORT(classDef1ValueArray + 2 * (glyph - startGlyphID)));
						classDefTable = classDef1ValueArray + 2 * glyphCount;
					}
					break;
				case 2:
					{
						ushort classRangeCount = (ushort)(ttUSHORT(classDefTable + 2));
						byte* classRangeRecords = classDefTable + 4;
						int l = (int)(0);
						int r = (int)(classRangeCount - 1);
						int m = 0;
						int strawStart = 0;
						int strawEnd = 0;
						int needle = (int)(glyph);
						while (l <= r)
						{
							byte* classRangeRecord;
							m = (int)((l + r) >> 1);
							classRangeRecord = classRangeRecords + 6 * m;
							strawStart = (int)(ttUSHORT(classRangeRecord));
							strawEnd = (int)(ttUSHORT(classRangeRecord + 2));
							if ((needle) < (strawStart))
								r = (int)(m - 1);
							else if ((needle) > (strawEnd))
								l = (int)(m + 1);
							else
								return (int)(ttUSHORT(classRangeRecord + 4));
						}
						classDefTable = classRangeRecords + 6 * classRangeCount;
					}
					break;
				default:
					{
					}
					break;
			}

			return (int)(-1);
		}

		public static int stbtt__GetGlyphGPOSInfoAdvance(stbtt_fontinfo info, int glyph1, int glyph2)
		{
			ushort lookupListOffset = 0;
			byte* lookupList;
			ushort lookupCount = 0;
			byte* data;
			int i = 0;
			if (info.gpos == 0)
				return (int)(0);
			data = info.data + info.gpos;
			if (ttUSHORT(data + 0) != 1)
				return (int)(0);
			if (ttUSHORT(data + 2) != 0)
				return (int)(0);
			lookupListOffset = (ushort)(ttUSHORT(data + 8));
			lookupList = data + lookupListOffset;
			lookupCount = (ushort)(ttUSHORT(lookupList));
			for (i = (int)(0); (i) < (lookupCount); ++i)
			{
				ushort lookupOffset = (ushort)(ttUSHORT(lookupList + 2 + 2 * i));
				byte* lookupTable = lookupList + lookupOffset;
				ushort lookupType = (ushort)(ttUSHORT(lookupTable));
				ushort subTableCount = (ushort)(ttUSHORT(lookupTable + 4));
				byte* subTableOffsets = lookupTable + 6;
				switch (lookupType)
				{
					case 2:
						{
							int sti = 0;
							for (sti = (int)(0); (sti) < (subTableCount); sti++)
							{
								ushort subtableOffset = (ushort)(ttUSHORT(subTableOffsets + 2 * sti));
								byte* table = lookupTable + subtableOffset;
								ushort posFormat = (ushort)(ttUSHORT(table));
								ushort coverageOffset = (ushort)(ttUSHORT(table + 2));
								int coverageIndex = (int)(stbtt__GetCoverageIndex(table + coverageOffset, (int)(glyph1)));
								if ((coverageIndex) == (-1))
									continue;
								switch (posFormat)
								{
									case 1:
										{
											int l = 0;
											int r = 0;
											int m = 0;
											int straw = 0;
											int needle = 0;
											ushort valueFormat1 = (ushort)(ttUSHORT(table + 4));
											ushort valueFormat2 = (ushort)(ttUSHORT(table + 6));
											int valueRecordPairSizeInBytes = (int)(2);
											ushort pairSetCount = (ushort)(ttUSHORT(table + 8));
											ushort pairPosOffset = (ushort)(ttUSHORT(table + 10 + 2 * coverageIndex));
											byte* pairValueTable = table + pairPosOffset;
											ushort pairValueCount = (ushort)(ttUSHORT(pairValueTable));
											byte* pairValueArray = pairValueTable + 2;
											if (valueFormat1 != 4)
												return (int)(0);
											if (valueFormat2 != 0)
												return (int)(0);
											needle = (int)(glyph2);
											r = (int)(pairValueCount - 1);
											l = (int)(0);
											while (l <= r)
											{
												ushort secondGlyph = 0;
												byte* pairValue;
												m = (int)((l + r) >> 1);
												pairValue = pairValueArray + (2 + valueRecordPairSizeInBytes) * m;
												secondGlyph = (ushort)(ttUSHORT(pairValue));
												straw = (int)(secondGlyph);
												if ((needle) < (straw))
													r = (int)(m - 1);
												else if ((needle) > (straw))
													l = (int)(m + 1);
												else
												{
													short xAdvance = (short)(ttSHORT(pairValue + 2));
													return (int)(xAdvance);
												}
											}
										}
										break;
									case 2:
										{
											ushort valueFormat1 = (ushort)(ttUSHORT(table + 4));
											ushort valueFormat2 = (ushort)(ttUSHORT(table + 6));
											ushort classDef1Offset = (ushort)(ttUSHORT(table + 8));
											ushort classDef2Offset = (ushort)(ttUSHORT(table + 10));
											int glyph1class = (int)(stbtt__GetGlyphClass(table + classDef1Offset, (int)(glyph1)));
											int glyph2class = (int)(stbtt__GetGlyphClass(table + classDef2Offset, (int)(glyph2)));
											ushort class1Count = (ushort)(ttUSHORT(table + 12));
											ushort class2Count = (ushort)(ttUSHORT(table + 14));
											if (valueFormat1 != 4)
												return (int)(0);
											if (valueFormat2 != 0)
												return (int)(0);
											if (((((glyph1class) >= (0)) && ((glyph1class) < (class1Count))) && ((glyph2class) >= (0))) && ((glyph2class) < (class2Count)))
											{
												byte* class1Records = table + 16;
												byte* class2Records = class1Records + 2 * (glyph1class * class2Count);
												short xAdvance = (short)(ttSHORT(class2Records + 2 * glyph2class));
												return (int)(xAdvance);
											}
										}
										break;
									default:
										{
											break;
										}
								}
							}
							break;
						}
					default:
						break;
				}
			}
			return (int)(0);
		}

		public static int stbtt_GetGlyphKernAdvance(stbtt_fontinfo info, int g1, int g2)
		{
			int xAdvance = (int)(0);
			if ((info.gpos) != 0)
				xAdvance += (int)(stbtt__GetGlyphGPOSInfoAdvance(info, (int)(g1), (int)(g2)));
			else if ((info.kern) != 0)
				xAdvance += (int)(stbtt__GetGlyphKernInfoAdvance(info, (int)(g1), (int)(g2)));
			return (int)(xAdvance);
		}

		public static int stbtt_GetCodepointKernAdvance(stbtt_fontinfo info, int ch1, int ch2)
		{
			if ((info.kern == 0) && (info.gpos == 0))
				return (int)(0);
			return (int)(stbtt_GetGlyphKernAdvance(info, (int)(stbtt_FindGlyphIndex(info, (int)(ch1))), (int)(stbtt_FindGlyphIndex(info, (int)(ch2)))));
		}

		public static void stbtt_GetCodepointHMetrics(stbtt_fontinfo info, int codepoint, int* advanceWidth, int* leftSideBearing)
		{
			stbtt_GetGlyphHMetrics(info, (int)(stbtt_FindGlyphIndex(info, (int)(codepoint))), advanceWidth, leftSideBearing);
		}

		public static void stbtt_GetFontVMetrics(stbtt_fontinfo info, int* ascent, int* descent, int* lineGap)
		{
			if ((ascent) != null)
				*ascent = (int)(ttSHORT(info.data + info.hhea + 4));
			if ((descent) != null)
				*descent = (int)(ttSHORT(info.data + info.hhea + 6));
			if ((lineGap) != null)
				*lineGap = (int)(ttSHORT(info.data + info.hhea + 8));
		}

		public static int stbtt_GetFontVMetricsOS2(stbtt_fontinfo info, int* typoAscent, int* typoDescent, int* typoLineGap)
		{
			int tab = (int)(stbtt__find_table(info.data, (uint)(info.fontstart), "OS/2"));
			if (tab == 0)
				return (int)(0);
			if ((typoAscent) != null)
				*typoAscent = (int)(ttSHORT(info.data + tab + 68));
			if ((typoDescent) != null)
				*typoDescent = (int)(ttSHORT(info.data + tab + 70));
			if ((typoLineGap) != null)
				*typoLineGap = (int)(ttSHORT(info.data + tab + 72));
			return (int)(1);
		}

		public static void stbtt_GetFontBoundingBox(stbtt_fontinfo info, int* x0, int* y0, int* x1, int* y1)
		{
			*x0 = (int)(ttSHORT(info.data + info.head + 36));
			*y0 = (int)(ttSHORT(info.data + info.head + 38));
			*x1 = (int)(ttSHORT(info.data + info.head + 40));
			*y1 = (int)(ttSHORT(info.data + info.head + 42));
		}

		public static float stbtt_ScaleForPixelHeight(stbtt_fontinfo info, float height)
		{
			int fheight = (int)(ttSHORT(info.data + info.hhea + 4) - ttSHORT(info.data + info.hhea + 6));
			return (float)(height / fheight);
		}

		public static float stbtt_ScaleForMappingEmToPixels(stbtt_fontinfo info, float pixels)
		{
			int unitsPerEm = (int)(ttUSHORT(info.data + info.head + 18));
			return (float)(pixels / unitsPerEm);
		}

		public static void stbtt_FreeShape(stbtt_fontinfo info, stbtt_vertex* v)
		{
			CRuntime.Free(v);
		}

		public static byte* stbtt_FindSVGDoc(stbtt_fontinfo info, int gl)
		{
			int i = 0;
			byte* data = info.data;
			byte* svg_doc_list = data + stbtt__get_svg(info);
			int numEntries = (int)(ttUSHORT(svg_doc_list));
			byte* svg_docs = svg_doc_list + 2;
			for (i = (int)(0); (i) < (numEntries); i++)
			{
				byte* svg_doc = svg_docs + (12 * i);
				if (((gl) >= (ttUSHORT(svg_doc))) && (gl <= ttUSHORT(svg_doc + 2)))
					return svg_doc;
			}
			return null;
		}

		public static int stbtt_GetGlyphSVG(stbtt_fontinfo info, int gl, sbyte** svg)
		{
			byte* data = info.data;
			byte* svg_doc;
			if ((info.svg) == (0))
				return (int)(0);
			svg_doc = stbtt_FindSVGDoc(info, (int)(gl));
			if (svg_doc != (null))
			{
				*svg = (sbyte*)(data) + info.svg + ttULONG(svg_doc + 4);
				return (int)(ttULONG(svg_doc + 8));
			}
			else
			{
				return (int)(0);
			}

		}

		public static int stbtt_GetCodepointSVG(stbtt_fontinfo info, int unicode_codepoint, sbyte** svg)
		{
			return (int)(stbtt_GetGlyphSVG(info, (int)(stbtt_FindGlyphIndex(info, (int)(unicode_codepoint))), svg));
		}

		public static void stbtt_GetGlyphBitmapBoxSubpixel(stbtt_fontinfo font, int glyph, float scale_x, float scale_y, float shift_x, float shift_y, int* ix0, int* iy0, int* ix1, int* iy1)
		{
			int x0 = (int)(0);
			int y0 = (int)(0);
			int x1 = 0;
			int y1 = 0;
			if (stbtt_GetGlyphBox(font, (int)(glyph), &x0, &y0, &x1, &y1) == 0)
			{
				if ((ix0) != null)
					*ix0 = (int)(0);
				if ((iy0) != null)
					*iy0 = (int)(0);
				if ((ix1) != null)
					*ix1 = (int)(0);
				if ((iy1) != null)
					*iy1 = (int)(0);
			}
			else
			{
				if ((ix0) != null)
					*ix0 = ((int)(CRuntime.Floor((double)(x0 * scale_x + shift_x))));
				if ((iy0) != null)
					*iy0 = ((int)(CRuntime.Floor((double)(-y1 * scale_y + shift_y))));
				if ((ix1) != null)
					*ix1 = ((int)(CRuntime.Ceil((double)(x1 * scale_x + shift_x))));
				if ((iy1) != null)
					*iy1 = ((int)(CRuntime.Ceil((double)(-y0 * scale_y + shift_y))));
			}

		}

		public static void stbtt_GetGlyphBitmapBox(stbtt_fontinfo font, int glyph, float scale_x, float scale_y, int* ix0, int* iy0, int* ix1, int* iy1)
		{
			stbtt_GetGlyphBitmapBoxSubpixel(font, (int)(glyph), (float)(scale_x), (float)(scale_y), (float)(0.0f), (float)(0.0f), ix0, iy0, ix1, iy1);
		}

		public static void stbtt_GetCodepointBitmapBoxSubpixel(stbtt_fontinfo font, int codepoint, float scale_x, float scale_y, float shift_x, float shift_y, int* ix0, int* iy0, int* ix1, int* iy1)
		{
			stbtt_GetGlyphBitmapBoxSubpixel(font, (int)(stbtt_FindGlyphIndex(font, (int)(codepoint))), (float)(scale_x), (float)(scale_y), (float)(shift_x), (float)(shift_y), ix0, iy0, ix1, iy1);
		}

		public static void stbtt_GetCodepointBitmapBox(stbtt_fontinfo font, int codepoint, float scale_x, float scale_y, int* ix0, int* iy0, int* ix1, int* iy1)
		{
			stbtt_GetCodepointBitmapBoxSubpixel(font, (int)(codepoint), (float)(scale_x), (float)(scale_y), (float)(0.0f), (float)(0.0f), ix0, iy0, ix1, iy1);
		}

		public static byte* stbtt_GetGlyphBitmapSubpixel(stbtt_fontinfo info, float scale_x, float scale_y, float shift_x, float shift_y, int glyph, int* width, int* height, int* xoff, int* yoff)
		{
			int ix0 = 0;
			int iy0 = 0;
			int ix1 = 0;
			int iy1 = 0;
			stbtt__bitmap gbm = new stbtt__bitmap();
			stbtt_vertex* vertices;
			int num_verts = (int)(stbtt_GetGlyphShape(info, (int)(glyph), &vertices));
			if ((scale_x) == (0))
				scale_x = (float)(scale_y);
			if ((scale_y) == (0))
			{
				if ((scale_x) == (0))
				{
					CRuntime.Free(vertices);
					return (null);
				}
				scale_y = (float)(scale_x);
			}

			stbtt_GetGlyphBitmapBoxSubpixel(info, (int)(glyph), (float)(scale_x), (float)(scale_y), (float)(shift_x), (float)(shift_y), &ix0, &iy0, &ix1, &iy1);
			gbm.w = (int)(ix1 - ix0);
			gbm.h = (int)(iy1 - iy0);
			gbm.pixels = (null);
			if ((width) != null)
				*width = (int)(gbm.w);
			if ((height) != null)
				*height = (int)(gbm.h);
			if ((xoff) != null)
				*xoff = (int)(ix0);
			if ((yoff) != null)
				*yoff = (int)(iy0);
			if (((gbm.w) != 0) && ((gbm.h) != 0))
			{
				gbm.pixels = (byte*)(CRuntime.Malloc((ulong)(gbm.w * gbm.h)));
				if ((gbm.pixels) != null)
				{
					gbm.stride = (int)(gbm.w);
					stbtt_Rasterize(&gbm, (float)(0.35f), vertices, (int)(num_verts), (float)(scale_x), (float)(scale_y), (float)(shift_x), (float)(shift_y), (int)(ix0), (int)(iy0), (int)(1), info.userdata);
				}
			}

			CRuntime.Free(vertices);
			return gbm.pixels;
		}

		public static byte* stbtt_GetGlyphBitmap(stbtt_fontinfo info, float scale_x, float scale_y, int glyph, int* width, int* height, int* xoff, int* yoff)
		{
			return stbtt_GetGlyphBitmapSubpixel(info, (float)(scale_x), (float)(scale_y), (float)(0.0f), (float)(0.0f), (int)(glyph), width, height, xoff, yoff);
		}

		public static void stbtt_MakeGlyphBitmapSubpixel(stbtt_fontinfo info, byte* output, int out_w, int out_h, int out_stride, float scale_x, float scale_y, float shift_x, float shift_y, int glyph)
		{
			int ix0 = 0;
			int iy0 = 0;
			stbtt_vertex* vertices;
			int num_verts = (int)(stbtt_GetGlyphShape(info, (int)(glyph), &vertices));
			stbtt__bitmap gbm = new stbtt__bitmap();
			stbtt_GetGlyphBitmapBoxSubpixel(info, (int)(glyph), (float)(scale_x), (float)(scale_y), (float)(shift_x), (float)(shift_y), &ix0, &iy0, null, null);
			gbm.pixels = output;
			gbm.w = (int)(out_w);
			gbm.h = (int)(out_h);
			gbm.stride = (int)(out_stride);
			if (((gbm.w) != 0) && ((gbm.h) != 0))
				stbtt_Rasterize(&gbm, (float)(0.35f), vertices, (int)(num_verts), (float)(scale_x), (float)(scale_y), (float)(shift_x), (float)(shift_y), (int)(ix0), (int)(iy0), (int)(1), info.userdata);
			CRuntime.Free(vertices);
		}

		public static void stbtt_MakeGlyphBitmap(stbtt_fontinfo info, byte* output, int out_w, int out_h, int out_stride, float scale_x, float scale_y, int glyph)
		{
			stbtt_MakeGlyphBitmapSubpixel(info, output, (int)(out_w), (int)(out_h), (int)(out_stride), (float)(scale_x), (float)(scale_y), (float)(0.0f), (float)(0.0f), (int)(glyph));
		}

		public static byte* stbtt_GetCodepointBitmapSubpixel(stbtt_fontinfo info, float scale_x, float scale_y, float shift_x, float shift_y, int codepoint, int* width, int* height, int* xoff, int* yoff)
		{
			return stbtt_GetGlyphBitmapSubpixel(info, (float)(scale_x), (float)(scale_y), (float)(shift_x), (float)(shift_y), (int)(stbtt_FindGlyphIndex(info, (int)(codepoint))), width, height, xoff, yoff);
		}

		public static void stbtt_MakeCodepointBitmapSubpixelPrefilter(stbtt_fontinfo info, byte* output, int out_w, int out_h, int out_stride, float scale_x, float scale_y, float shift_x, float shift_y, int oversample_x, int oversample_y, float* sub_x, float* sub_y, int codepoint)
		{
			stbtt_MakeGlyphBitmapSubpixelPrefilter(info, output, (int)(out_w), (int)(out_h), (int)(out_stride), (float)(scale_x), (float)(scale_y), (float)(shift_x), (float)(shift_y), (int)(oversample_x), (int)(oversample_y), sub_x, sub_y, (int)(stbtt_FindGlyphIndex(info, (int)(codepoint))));
		}

		public static void stbtt_MakeCodepointBitmapSubpixel(stbtt_fontinfo info, byte* output, int out_w, int out_h, int out_stride, float scale_x, float scale_y, float shift_x, float shift_y, int codepoint)
		{
			stbtt_MakeGlyphBitmapSubpixel(info, output, (int)(out_w), (int)(out_h), (int)(out_stride), (float)(scale_x), (float)(scale_y), (float)(shift_x), (float)(shift_y), (int)(stbtt_FindGlyphIndex(info, (int)(codepoint))));
		}

		public static byte* stbtt_GetCodepointBitmap(stbtt_fontinfo info, float scale_x, float scale_y, int codepoint, int* width, int* height, int* xoff, int* yoff)
		{
			return stbtt_GetCodepointBitmapSubpixel(info, (float)(scale_x), (float)(scale_y), (float)(0.0f), (float)(0.0f), (int)(codepoint), width, height, xoff, yoff);
		}

		public static void stbtt_MakeCodepointBitmap(stbtt_fontinfo info, byte* output, int out_w, int out_h, int out_stride, float scale_x, float scale_y, int codepoint)
		{
			stbtt_MakeCodepointBitmapSubpixel(info, output, (int)(out_w), (int)(out_h), (int)(out_stride), (float)(scale_x), (float)(scale_y), (float)(0.0f), (float)(0.0f), (int)(codepoint));
		}

		public static void stbtt_MakeGlyphBitmapSubpixelPrefilter(stbtt_fontinfo info, byte* output, int out_w, int out_h, int out_stride, float scale_x, float scale_y, float shift_x, float shift_y, int prefilter_x, int prefilter_y, float* sub_x, float* sub_y, int glyph)
		{
			stbtt_MakeGlyphBitmapSubpixel(info, output, (int)(out_w - (prefilter_x - 1)), (int)(out_h - (prefilter_y - 1)), (int)(out_stride), (float)(scale_x), (float)(scale_y), (float)(shift_x), (float)(shift_y), (int)(glyph));
			if ((prefilter_x) > (1))
				stbtt__h_prefilter(output, (int)(out_w), (int)(out_h), (int)(out_stride), (uint)(prefilter_x));
			if ((prefilter_y) > (1))
				stbtt__v_prefilter(output, (int)(out_w), (int)(out_h), (int)(out_stride), (uint)(prefilter_y));
			*sub_x = (float)(stbtt__oversample_shift((int)(prefilter_x)));
			*sub_y = (float)(stbtt__oversample_shift((int)(prefilter_y)));
		}

		public static byte* stbtt_GetGlyphSDF(stbtt_fontinfo info, float scale, int glyph, int padding, byte onedge_value, float pixel_dist_scale, int* width, int* height, int* xoff, int* yoff)
		{
			float scale_x = (float)(scale);
			float scale_y = (float)(scale);
			int ix0 = 0;
			int iy0 = 0;
			int ix1 = 0;
			int iy1 = 0;
			int w = 0;
			int h = 0;
			byte* data;
			if ((scale) == (0))
				return (null);
			stbtt_GetGlyphBitmapBoxSubpixel(info, (int)(glyph), (float)(scale), (float)(scale), (float)(0.0f), (float)(0.0f), &ix0, &iy0, &ix1, &iy1);
			if (((ix0) == (ix1)) || ((iy0) == (iy1)))
				return (null);
			ix0 -= (int)(padding);
			iy0 -= (int)(padding);
			ix1 += (int)(padding);
			iy1 += (int)(padding);
			w = (int)(ix1 - ix0);
			h = (int)(iy1 - iy0);
			if ((width) != null)
				*width = (int)(w);
			if ((height) != null)
				*height = (int)(h);
			if ((xoff) != null)
				*xoff = (int)(ix0);
			if ((yoff) != null)
				*yoff = (int)(iy0);
			scale_y = (float)(-scale_y);
			{
				int x = 0;
				int y = 0;
				int i = 0;
				int j = 0;
				float* precompute;
				stbtt_vertex* verts;
				int num_verts = (int)(stbtt_GetGlyphShape(info, (int)(glyph), &verts));
				data = (byte*)(CRuntime.Malloc((ulong)(w * h)));
				precompute = (float*)(CRuntime.Malloc((ulong)(num_verts * sizeof(float))));
				for (i = (int)(0), j = (int)(num_verts - 1); (i) < (num_verts); j = (int)(i++))
				{
					if ((verts[i].type) == (STBTT_vline))
					{
						float x0 = (float)(verts[i].x * scale_x);
						float y0 = (float)(verts[i].y * scale_y);
						float x1 = (float)(verts[j].x * scale_x);
						float y1 = (float)(verts[j].y * scale_y);
						float dist = (float)(CRuntime.Sqrt((double)((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0))));
						precompute[i] = (float)(((dist) == (0)) ? 0.0f : 1.0f / dist);
					}
					else if ((verts[i].type) == (STBTT_vcurve))
					{
						float x2 = (float)(verts[j].x * scale_x);
						float y2 = (float)(verts[j].y * scale_y);
						float x1 = (float)(verts[i].cx * scale_x);
						float y1 = (float)(verts[i].cy * scale_y);
						float x0 = (float)(verts[i].x * scale_x);
						float y0 = (float)(verts[i].y * scale_y);
						float bx = (float)(x0 - 2 * x1 + x2);
						float by = (float)(y0 - 2 * y1 + y2);
						float len2 = (float)(bx * bx + by * by);
						if (len2 != 0.0f)
							precompute[i] = (float)(1.0f / (bx * bx + by * by));
						else
							precompute[i] = (float)(0.0f);
					}
					else
						precompute[i] = (float)(0.0f);
				}
				for (y = (int)(iy0); (y) < (iy1); ++y)
				{
					for (x = (int)(ix0); (x) < (ix1); ++x)
					{
						float val = 0;
						float min_dist = (float)(999999.0f);
						float sx = (float)((float)(x) + 0.5f);
						float sy = (float)((float)(y) + 0.5f);
						float x_gspace = (float)(sx / scale_x);
						float y_gspace = (float)(sy / scale_y);
						int winding = (int)(stbtt__compute_crossings_x((float)(x_gspace), (float)(y_gspace), (int)(num_verts), verts));
						for (i = (int)(0); (i) < (num_verts); ++i)
						{
							float x0 = (float)(verts[i].x * scale_x);
							float y0 = (float)(verts[i].y * scale_y);
							float dist2 = (float)((x0 - sx) * (x0 - sx) + (y0 - sy) * (y0 - sy));
							if ((dist2) < (min_dist * min_dist))
								min_dist = ((float)(CRuntime.Sqrt((double)(dist2))));
							if ((verts[i].type) == (STBTT_vline))
							{
								float x1 = (float)(verts[i - 1].x * scale_x);
								float y1 = (float)(verts[i - 1].y * scale_y);
								float dist = (float)((float)(CRuntime.Fabs((double)((x1 - x0) * (y0 - sy) - (y1 - y0) * (x0 - sx)))) * precompute[i]);
								if ((dist) < (min_dist))
								{
									float dx = (float)(x1 - x0);
									float dy = (float)(y1 - y0);
									float px = (float)(x0 - sx);
									float py = (float)(y0 - sy);
									float t = (float)(-(px * dx + py * dy) / (dx * dx + dy * dy));
									if (((t) >= (0.0f)) && (t <= 1.0f))
										min_dist = (float)(dist);
								}
							}
							else if ((verts[i].type) == (STBTT_vcurve))
							{
								float x2 = (float)(verts[i - 1].x * scale_x);
								float y2 = (float)(verts[i - 1].y * scale_y);
								float x1 = (float)(verts[i].cx * scale_x);
								float y1 = (float)(verts[i].cy * scale_y);
								float box_x0 = (float)(((x0) < (x1) ? (x0) : (x1)) < (x2) ? ((x0) < (x1) ? (x0) : (x1)) : (x2));
								float box_y0 = (float)(((y0) < (y1) ? (y0) : (y1)) < (y2) ? ((y0) < (y1) ? (y0) : (y1)) : (y2));
								float box_x1 = (float)(((x0) < (x1) ? (x1) : (x0)) < (x2) ? (x2) : ((x0) < (x1) ? (x1) : (x0)));
								float box_y1 = (float)(((y0) < (y1) ? (y1) : (y0)) < (y2) ? (y2) : ((y0) < (y1) ? (y1) : (y0)));
								if (((((sx) > (box_x0 - min_dist)) && ((sx) < (box_x1 + min_dist))) && ((sy) > (box_y0 - min_dist))) && ((sy) < (box_y1 + min_dist)))
								{
									int num = (int)(0);
									float ax = (float)(x1 - x0);
									float ay = (float)(y1 - y0);
									float bx = (float)(x0 - 2 * x1 + x2);
									float by = (float)(y0 - 2 * y1 + y2);
									float mx = (float)(x0 - sx);
									float my = (float)(y0 - sy);
									float* res = stackalloc float[3];
									float px = 0;
									float py = 0;
									float t = 0;
									float it = 0;
									float a_inv = (float)(precompute[i]);
									if ((a_inv) == (0.0))
									{
										float a = (float)(3 * (ax * bx + ay * by));
										float b = (float)(2 * (ax * ax + ay * ay) + (mx * bx + my * by));
										float c = (float)(mx * ax + my * ay);
										if ((a) == (0.0))
										{
											if (b != 0.0)
											{
												res[num++] = (float)(-c / b);
											}
										}
										else
										{
											float discriminant = (float)(b * b - 4 * a * c);
											if ((discriminant) < (0))
												num = (int)(0);
											else
											{
												float root = (float)(CRuntime.Sqrt((double)(discriminant)));
												res[0] = (float)((-b - root) / (2 * a));
												res[1] = (float)((-b + root) / (2 * a));
												num = (int)(2);
											}
										}
									}
									else
									{
										float b = (float)(3 * (ax * bx + ay * by) * a_inv);
										float c = (float)((2 * (ax * ax + ay * ay) + (mx * bx + my * by)) * a_inv);
										float d = (float)((mx * ax + my * ay) * a_inv);
										num = (int)(stbtt__solve_cubic((float)(b), (float)(c), (float)(d), res));
									}
									if ((((num) >= (1)) && ((res[0]) >= (0.0f))) && (res[0] <= 1.0f))
									{
										t = (float)(res[0]);
										it = (float)(1.0f - t);
										px = (float)(it * it * x0 + 2 * t * it * x1 + t * t * x2);
										py = (float)(it * it * y0 + 2 * t * it * y1 + t * t * y2);
										dist2 = (float)((px - sx) * (px - sx) + (py - sy) * (py - sy));
										if ((dist2) < (min_dist * min_dist))
											min_dist = ((float)(CRuntime.Sqrt((double)(dist2))));
									}
									if ((((num) >= (2)) && ((res[1]) >= (0.0f))) && (res[1] <= 1.0f))
									{
										t = (float)(res[1]);
										it = (float)(1.0f - t);
										px = (float)(it * it * x0 + 2 * t * it * x1 + t * t * x2);
										py = (float)(it * it * y0 + 2 * t * it * y1 + t * t * y2);
										dist2 = (float)((px - sx) * (px - sx) + (py - sy) * (py - sy));
										if ((dist2) < (min_dist * min_dist))
											min_dist = ((float)(CRuntime.Sqrt((double)(dist2))));
									}
									if ((((num) >= (3)) && ((res[2]) >= (0.0f))) && (res[2] <= 1.0f))
									{
										t = (float)(res[2]);
										it = (float)(1.0f - t);
										px = (float)(it * it * x0 + 2 * t * it * x1 + t * t * x2);
										py = (float)(it * it * y0 + 2 * t * it * y1 + t * t * y2);
										dist2 = (float)((px - sx) * (px - sx) + (py - sy) * (py - sy));
										if ((dist2) < (min_dist * min_dist))
											min_dist = ((float)(CRuntime.Sqrt((double)(dist2))));
									}
								}
							}
						}
						if ((winding) == (0))
							min_dist = (float)(-min_dist);
						val = (float)(onedge_value + pixel_dist_scale * min_dist);
						if ((val) < (0))
							val = (float)(0);
						else if ((val) > (255))
							val = (float)(255);
						data[(y - iy0) * w + (x - ix0)] = ((byte)(val));
					}
				}
				CRuntime.Free(precompute);
				CRuntime.Free(verts);
			}

			return data;
		}

		public static byte* stbtt_GetCodepointSDF(stbtt_fontinfo info, float scale, int codepoint, int padding, byte onedge_value, float pixel_dist_scale, int* width, int* height, int* xoff, int* yoff)
		{
			return stbtt_GetGlyphSDF(info, (float)(scale), (int)(stbtt_FindGlyphIndex(info, (int)(codepoint))), (int)(padding), (byte)(onedge_value), (float)(pixel_dist_scale), width, height, xoff, yoff);
		}

		public static sbyte* stbtt_GetFontNameString(stbtt_fontinfo font, int* length, int platformID, int encodingID, int languageID, int nameID)
		{
			int i = 0;
			int count = 0;
			int stringOffset = 0;
			byte* fc = font.data;
			uint offset = (uint)(font.fontstart);
			uint nm = (uint)(stbtt__find_table(fc, (uint)(offset), "name"));
			if (nm == 0)
				return (null);
			count = (int)(ttUSHORT(fc + nm + 2));
			stringOffset = (int)(nm + ttUSHORT(fc + nm + 4));
			for (i = (int)(0); (i) < (count); ++i)
			{
				uint loc = (uint)(nm + 6 + 12 * i);
				if (((((platformID) == (ttUSHORT(fc + loc + 0))) && ((encodingID) == (ttUSHORT(fc + loc + 2)))) && ((languageID) == (ttUSHORT(fc + loc + 4)))) && ((nameID) == (ttUSHORT(fc + loc + 6))))
				{
					*length = (int)(ttUSHORT(fc + loc + 8));
					return (sbyte*)(fc + stringOffset + ttUSHORT(fc + loc + 10));
				}
			}
			return (null);
		}

		public static int stbtt_InitFont(stbtt_fontinfo info, byte* data, int offset)
		{
			return (int)(stbtt_InitFont_internal(info, data, (int)(offset)));
		}
	}
}
