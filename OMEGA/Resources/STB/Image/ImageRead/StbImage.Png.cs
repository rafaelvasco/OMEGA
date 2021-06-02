using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace STB
{
	#pragma warning disable CA2014

    internal unsafe partial class StbImage
	{
		public const int STBI_F_NONE = 0;
		public const int STBI_F_SUB = 1;
		public const int STBI_F_UP = 2;
		public const int STBI_F_AVG = 3;
		public const int STBI_F_PAETH = 4;
		public const int STBI_F_AVG_FIRST = 5;
		public const int STBI_F_PAETH_FIRST = 6;

		public static byte[] PngSig = { 137, 80, 78, 71, 13, 10, 26, 10 };

		public static byte[] FirstRowFilter =
			{STBI_F_NONE, STBI_F_SUB, STBI_F_NONE, STBI_F_AVG_FIRST, STBI_F_PAETH_FIRST};

		public static byte[] StbiDepthScaleTable = { 0, 0xff, 0x55, 0, 0x11, 0, 0, 0, 0x01 };

		public static StbiPngchunk stbi__get_chunk_header(StbiContext s)
		{
			var c = new StbiPngchunk();
			c.length = stbi__get32be(s);
			c.type = stbi__get32be(s);
			return c;
		}

		public static int stbi__check_png_header(StbiContext s)
		{
			var i = 0;
			for (i = 0; i < 8; ++i)
				if (stbi__get8(s) != PngSig[i])
					return stbi__err("bad png sig");
			return 1;
		}

		public static int stbi__create_png_image_raw(StbiPng a, byte* raw, uint rawLen, int outN, uint x, uint y,
			int depth, int color)
		{
			var bytes = depth == 16 ? 2 : 1;
			var s = a.S;
			uint i = 0;
			uint j = 0;
			var stride = (uint)(x * outN * bytes);
			uint img_len = 0;
			uint img_width_bytes = 0;
			var k = 0;
			var img_n = s.ImgN;
			var output_bytes = outN * bytes;
			var filter_bytes = img_n * bytes;
			var width = (int)x;
			a.Out = (byte*)stbi__malloc_mad3((int)x, (int)y, output_bytes, 0);
			if (a.Out == null)
				return stbi__err("outofmem");
			if (stbi__mad3sizes_valid(img_n, (int)x, depth, 7) == 0)
				return stbi__err("too large");
			img_width_bytes = (uint)((img_n * x * depth + 7) >> 3);
			img_len = (img_width_bytes + 1) * y;
			if (rawLen < img_len)
				return stbi__err("not enough pixels");
			for (j = (uint)0; j < y; ++j)
			{
				var cur = a.Out + stride * j;
				byte* prior;
				var filter = (int)*raw++;
				if (filter > 4)
					return stbi__err("invalid filter");
				if (depth < 8)
				{
					cur += x * outN - img_width_bytes;
					filter_bytes = 1;
					width = (int)img_width_bytes;
				}

				prior = cur - stride;
				if (j == 0)
					filter = FirstRowFilter[filter];
				for (k = 0; k < filter_bytes; ++k)
					switch (filter)
					{
						case STBI_F_NONE:
							cur[k] = raw[k];
							break;
						case STBI_F_SUB:
							cur[k] = raw[k];
							break;
						case STBI_F_UP:
							cur[k] = (byte)((raw[k] + prior[k]) & 255);
							break;
						case STBI_F_AVG:
							cur[k] = (byte)((raw[k] + (prior[k] >> 1)) & 255);
							break;
						case STBI_F_PAETH:
							cur[k] = (byte)((raw[k] + stbi__paeth(0, prior[k], 0)) & 255);
							break;
						case STBI_F_AVG_FIRST:
							cur[k] = raw[k];
							break;
						case STBI_F_PAETH_FIRST:
							cur[k] = raw[k];
							break;
					}

				if (depth == 8)
				{
					if (img_n != outN)
						cur[img_n] = 255;
					raw += img_n;
					cur += outN;
					prior += outN;
				}
				else if (depth == 16)
				{
					if (img_n != outN)
					{
						cur[filter_bytes] = 255;
						cur[filter_bytes + 1] = 255;
					}

					raw += filter_bytes;
					cur += output_bytes;
					prior += output_bytes;
				}
				else
				{
					raw += 1;
					cur += 1;
					prior += 1;
				}

				if (depth < 8 || img_n == outN)
				{
					var nk = (width - 1) * filter_bytes;
					switch (filter)
					{
						case STBI_F_NONE:
							CRuntime.Memcpy(cur, raw, (ulong)nk);
							break;
						case STBI_F_SUB:
							for (k = 0; k < nk; ++k)
								cur[k] = (byte)((raw[k] + cur[k - filter_bytes]) & 255);
							break;
						case STBI_F_UP:
							for (k = 0; k < nk; ++k)
								cur[k] = (byte)((raw[k] + prior[k]) & 255);
							break;
						case STBI_F_AVG:
							for (k = 0; k < nk; ++k)
								cur[k] = (byte)((raw[k] + ((prior[k] + cur[k - filter_bytes]) >> 1)) & 255);
							break;
						case STBI_F_PAETH:
							for (k = 0; k < nk; ++k)
								cur[k] = (byte)((raw[k] + stbi__paeth(cur[k - filter_bytes], prior[k],
													  prior[k - filter_bytes])) & 255);
							break;
						case STBI_F_AVG_FIRST:
							for (k = 0; k < nk; ++k)
								cur[k] = (byte)((raw[k] + (cur[k - filter_bytes] >> 1)) & 255);
							break;
						case STBI_F_PAETH_FIRST:
							for (k = 0; k < nk; ++k)
								cur[k] = (byte)((raw[k] + stbi__paeth(cur[k - filter_bytes], 0, 0)) & 255);
							break;
					}

					raw += nk;
				}
				else
				{
					switch (filter)
					{
						case STBI_F_NONE:
							for (i = x - 1;
								i >= 1;
								--i, cur[filter_bytes] = (byte)255, raw += filter_bytes, cur += output_bytes, prior +=
									output_bytes)
								for (k = 0; k < filter_bytes; ++k)
									cur[k] = raw[k];
							break;
						case STBI_F_SUB:
							for (i = x - 1;
								i >= 1;
								--i, cur[filter_bytes] = (byte)255, raw += filter_bytes, cur += output_bytes, prior +=
									output_bytes)
								for (k = 0; k < filter_bytes; ++k)
									cur[k] = (byte)((raw[k] + cur[k - output_bytes]) & 255);
							break;
						case STBI_F_UP:
							for (i = x - 1;
								i >= 1;
								--i, cur[filter_bytes] = (byte)255, raw += filter_bytes, cur += output_bytes, prior +=
									output_bytes)
								for (k = 0; k < filter_bytes; ++k)
									cur[k] = (byte)((raw[k] + prior[k]) & 255);
							break;
						case STBI_F_AVG:
							for (i = x - 1;
								i >= 1;
								--i, cur[filter_bytes] = (byte)255, raw += filter_bytes, cur += output_bytes, prior +=
									output_bytes)
								for (k = 0; k < filter_bytes; ++k)
									cur[k] = (byte)((raw[k] + ((prior[k] + cur[k - output_bytes]) >> 1)) & 255);
							break;
						case STBI_F_PAETH:
							for (i = x - 1;
								i >= 1;
								--i, cur[filter_bytes] = (byte)255, raw += filter_bytes, cur += output_bytes, prior +=
									output_bytes)
								for (k = 0; k < filter_bytes; ++k)
									cur[k] = (byte)((raw[k] + stbi__paeth(cur[k - output_bytes], prior[k],
														  prior[k - output_bytes])) & 255);
							break;
						case STBI_F_AVG_FIRST:
							for (i = x - 1;
								i >= 1;
								--i, cur[filter_bytes] = (byte)255, raw += filter_bytes, cur += output_bytes, prior +=
									output_bytes)
								for (k = 0; k < filter_bytes; ++k)
									cur[k] = (byte)((raw[k] + (cur[k - output_bytes] >> 1)) & 255);
							break;
						case STBI_F_PAETH_FIRST:
							for (i = x - 1;
								i >= 1;
								--i, cur[filter_bytes] = (byte)255, raw += filter_bytes, cur += output_bytes, prior +=
									output_bytes)
								for (k = 0; k < filter_bytes; ++k)
									cur[k] = (byte)((raw[k] + stbi__paeth(cur[k - output_bytes], 0, 0)) & 255);
							break;
					}

					if (depth == 16)
					{
						cur = a.Out + stride * j;
						for (i = (uint)0; i < x; ++i, cur += output_bytes)
							cur[filter_bytes + 1] = 255;
					}
				}
			}

			if (depth < 8)
			{
				for (j = (uint)0; j < y; ++j)
				{
					var cur = a.Out + stride * j;
					var in_ = a.Out + stride * j + x * outN - img_width_bytes;
					var scale = (byte)(color == 0 ? StbiDepthScaleTable[depth] : 1);
					if (depth == 4)
					{
						for (k = (int)(x * img_n); k >= 2; k -= 2, ++in_)
						{
							*cur++ = (byte)(scale * (*in_ >> 4));
							*cur++ = (byte)(scale * (*in_ & 0x0f));
						}

						if (k > 0)
							*cur++ = (byte)(scale * (*in_ >> 4));
					}
					else if (depth == 2)
					{
						for (k = (int)(x * img_n); k >= 4; k -= 4, ++in_)
						{
							*cur++ = (byte)(scale * (*in_ >> 6));
							*cur++ = (byte)(scale * ((*in_ >> 4) & 0x03));
							*cur++ = (byte)(scale * ((*in_ >> 2) & 0x03));
							*cur++ = (byte)(scale * (*in_ & 0x03));
						}

						if (k > 0)
							*cur++ = (byte)(scale * (*in_ >> 6));
						if (k > 1)
							*cur++ = (byte)(scale * ((*in_ >> 4) & 0x03));
						if (k > 2)
							*cur++ = (byte)(scale * ((*in_ >> 2) & 0x03));
					}
					else if (depth == 1)
					{
						for (k = (int)(x * img_n); k >= 8; k -= 8, ++in_)
						{
							*cur++ = (byte)(scale * (*in_ >> 7));
							*cur++ = (byte)(scale * ((*in_ >> 6) & 0x01));
							*cur++ = (byte)(scale * ((*in_ >> 5) & 0x01));
							*cur++ = (byte)(scale * ((*in_ >> 4) & 0x01));
							*cur++ = (byte)(scale * ((*in_ >> 3) & 0x01));
							*cur++ = (byte)(scale * ((*in_ >> 2) & 0x01));
							*cur++ = (byte)(scale * ((*in_ >> 1) & 0x01));
							*cur++ = (byte)(scale * (*in_ & 0x01));
						}

						if (k > 0)
							*cur++ = (byte)(scale * (*in_ >> 7));
						if (k > 1)
							*cur++ = (byte)(scale * ((*in_ >> 6) & 0x01));
						if (k > 2)
							*cur++ = (byte)(scale * ((*in_ >> 5) & 0x01));
						if (k > 3)
							*cur++ = (byte)(scale * ((*in_ >> 4) & 0x01));
						if (k > 4)
							*cur++ = (byte)(scale * ((*in_ >> 3) & 0x01));
						if (k > 5)
							*cur++ = (byte)(scale * ((*in_ >> 2) & 0x01));
						if (k > 6)
							*cur++ = (byte)(scale * ((*in_ >> 1) & 0x01));
					}

					if (img_n != outN)
					{
						var q = 0;
						cur = a.Out + stride * j;
						if (img_n == 1)
							for (q = (int)(x - 1); q >= 0; --q)
							{
								cur[q * 2 + 1] = 255;
								cur[q * 2 + 0] = cur[q];
							}
						else
							for (q = (int)(x - 1); q >= 0; --q)
							{
								cur[q * 4 + 3] = 255;
								cur[q * 4 + 2] = cur[q * 3 + 2];
								cur[q * 4 + 1] = cur[q * 3 + 1];
								cur[q * 4 + 0] = cur[q * 3 + 0];
							}
					}
				}
			}
			else if (depth == 16)
			{
				var cur = a.Out;
				var cur16 = (ushort*)cur;
				for (i = (uint)0; i < x * y * outN; ++i, cur16++, cur += 2)
					*cur16 = (ushort)((cur[0] << 8) | cur[1]);
			}

			return 1;
		}

		public static int stbi__create_png_image(StbiPng a, byte* imageData, uint imageDataLen, int outN,
			int depth, int color, int interlaced)
		{
			var bytes = depth == 16 ? 2 : 1;
			var out_bytes = outN * bytes;
			byte* final;
			var p = 0;
			if (interlaced == 0)
				return stbi__create_png_image_raw(a, imageData, imageDataLen, outN, a.S.ImgX, a.S.ImgY, depth,
					color);
			final = (byte*)stbi__malloc_mad3((int)a.S.ImgX, (int)a.S.ImgY, out_bytes, 0);
			for (p = 0; p < 7; ++p)
			{
				var xorig = stackalloc int[7];
				xorig[0] = 0;
				xorig[1] = 4;
				xorig[2] = 0;
				xorig[3] = 2;
				xorig[4] = 0;
				xorig[5] = 1;
				xorig[6] = 0;
				var yorig = stackalloc int[7];
				yorig[0] = 0;
				yorig[1] = 0;
				yorig[2] = 4;
				yorig[3] = 0;
				yorig[4] = 2;
				yorig[5] = 0;
				yorig[6] = 1;
				var xspc = stackalloc int[7];
				xspc[0] = 8;
				xspc[1] = 8;
				xspc[2] = 4;
				xspc[3] = 4;
				xspc[4] = 2;
				xspc[5] = 2;
				xspc[6] = 1;
				var yspc = stackalloc int[7];
				yspc[0] = 8;
				yspc[1] = 8;
				yspc[2] = 8;
				yspc[3] = 4;
				yspc[4] = 4;
				yspc[5] = 2;
				yspc[6] = 2;
				var i = 0;
				var j = 0;
				var x = 0;
				var y = 0;
				x = (int)((a.S.ImgX - xorig[p] + xspc[p] - 1) / xspc[p]);
				y = (int)((a.S.ImgY - yorig[p] + yspc[p] - 1) / yspc[p]);
				if (x != 0 && y != 0)
				{
					var img_len = (uint)((((a.S.ImgN * x * depth + 7) >> 3) + 1) * y);
					if (stbi__create_png_image_raw(a, imageData, imageDataLen, outN, (uint)x, (uint)y, depth,
							color) == 0)
					{
						CRuntime.Free(final);
						return 0;
					}

					for (j = 0; j < y; ++j)
						for (i = 0; i < x; ++i)
						{
							var out_y = j * yspc[p] + yorig[p];
							var out_x = i * xspc[p] + xorig[p];
							CRuntime.Memcpy(final + out_y * a.S.ImgX * out_bytes + out_x * out_bytes,
								a.Out + (j * x + i) * out_bytes, (ulong)out_bytes);
						}

					CRuntime.Free(a.Out);
					imageData += img_len;
					imageDataLen -= img_len;
				}
			}

			a.Out = final;
			return 1;
		}

		public static int stbi__compute_transparency(StbiPng z, byte* tc, int outN)
		{
			var s = z.S;
			uint i = 0;
			var pixel_count = s.ImgX * s.ImgY;
			var p = z.Out;
			if (outN == 2)
				for (i = (uint)0; i < pixel_count; ++i)
				{
					p[1] = (byte)(p[0] == tc[0] ? 0 : 255);
					p += 2;
				}
			else
				for (i = (uint)0; i < pixel_count; ++i)
				{
					if (p[0] == tc[0] && p[1] == tc[1] && p[2] == tc[2])
						p[3] = 0;
					p += 4;
				}

			return 1;
		}

		public static int stbi__compute_transparency16(StbiPng z, ushort* tc, int outN)
		{
			var s = z.S;
			uint i = 0;
			var pixel_count = s.ImgX * s.ImgY;
			var p = (ushort*)z.Out;
			if (outN == 2)
				for (i = (uint)0; i < pixel_count; ++i)
				{
					p[1] = (ushort)(p[0] == tc[0] ? 0 : 65535);
					p += 2;
				}
			else
				for (i = (uint)0; i < pixel_count; ++i)
				{
					if (p[0] == tc[0] && p[1] == tc[1] && p[2] == tc[2])
						p[3] = 0;
					p += 4;
				}

			return 1;
		}

		public static int stbi__expand_png_palette(StbiPng a, byte* palette, int len, int palImgN)
		{
			uint i = 0;
			var pixel_count = a.S.ImgX * a.S.ImgY;
			byte* p;
			byte* temp_out;
			var orig = a.Out;
			p = (byte*)stbi__malloc_mad2((int)pixel_count, palImgN, 0);
			if (p == null)
				return stbi__err("outofmem");
			temp_out = p;
			if (palImgN == 3)
				for (i = (uint)0; i < pixel_count; ++i)
				{
					var n = orig[i] * 4;
					p[0] = palette[n];
					p[1] = palette[n + 1];
					p[2] = palette[n + 2];
					p += 3;
				}
			else
				for (i = (uint)0; i < pixel_count; ++i)
				{
					var n = orig[i] * 4;
					p[0] = palette[n];
					p[1] = palette[n + 1];
					p[2] = palette[n + 2];
					p[3] = palette[n + 3];
					p += 4;
				}

			CRuntime.Free(a.Out);
			a.Out = temp_out;
			return 1;
		}

		public static void stbi__de_iphone(StbiPng z)
		{
			var s = z.S;
			uint i = 0;
			var pixel_count = s.ImgX * s.ImgY;
			var p = z.Out;
			if (s.ImgOutN == 3)
			{
				for (i = (uint)0; i < pixel_count; ++i)
				{
					var t = p[0];
					p[0] = p[2];
					p[2] = t;
					p += 3;
				}
			}
			else
			{
				if (StbiUnpremultiplyOnLoad != 0)
					for (i = (uint)0; i < pixel_count; ++i)
					{
						var a = p[3];
						var t = p[0];
						if (a != 0)
						{
							var half = (byte)(a / 2);
							p[0] = (byte)((p[2] * 255 + half) / a);
							p[1] = (byte)((p[1] * 255 + half) / a);
							p[2] = (byte)((t * 255 + half) / a);
						}
						else
						{
							p[0] = p[2];
							p[2] = t;
						}

						p += 4;
					}
				else
					for (i = (uint)0; i < pixel_count; ++i)
					{
						var t = p[0];
						p[0] = p[2];
						p[2] = t;
						p += 4;
					}
			}
		}

		public static int stbi__parse_png_file(StbiPng z, int scan, int reqComp)
		{
			var palette = stackalloc byte[1024];
			var pal_img_n = (byte)0;
			var has_trans = (byte)0;
			var tc = stackalloc byte[3];
			tc[0] = 0;

			var tc16 = stackalloc ushort[3];
			var ioff = (uint)0;
			var idata_limit = (uint)0;
			uint i = 0;
			var pal_len = (uint)0;
			var first = 1;
			var k = 0;
			var interlace = 0;
			var color = 0;
			var is_iphone = 0;
			var s = z.S;
			z.Expanded = null;
			z.Idata = null;
			z.Out = null;
			if (stbi__check_png_header(s) == 0)
				return 0;
			if (scan == STBI_SCAN_TYPE)
				return 1;
			for (; ; )
			{
				var c = stbi__get_chunk_header(s);
				switch (c.type)
				{
					case ((uint)'C' << 24) + ((uint)'g' << 16) + ((uint)'B' << 8) + 'I':
						is_iphone = 1;
						stbi__skip(s, (int)c.length);
						break;
					case ((uint)'I' << 24) + ((uint)'H' << 16) + ((uint)'D' << 8) + 'R':
					{
						var comp = 0;
						var filter = 0;
						if (first == 0)
							return stbi__err("multiple IHDR");
						first = 0;
						if (c.length != 13)
							return stbi__err("bad IHDR len");
						s.ImgX = stbi__get32be(s);
						if (s.ImgX > 1 << 24)
							return stbi__err("too large");
						s.ImgY = stbi__get32be(s);
						if (s.ImgY > 1 << 24)
							return stbi__err("too large");
						z.Depth = stbi__get8(s);
						if (z.Depth != 1 && z.Depth != 2 && z.Depth != 4 && z.Depth != 8 && z.Depth != 16)
							return stbi__err("1/2/4/8/16-bit only");
						color = stbi__get8(s);
						if (color > 6)
							return stbi__err("bad ctype");
						if (color == 3 && z.Depth == 16)
							return stbi__err("bad ctype");
						if (color == 3)
							pal_img_n = 3;
						else if ((color & 1) != 0)
							return stbi__err("bad ctype");
						comp = stbi__get8(s);
						if (comp != 0)
							return stbi__err("bad comp method");
						filter = stbi__get8(s);
						if (filter != 0)
							return stbi__err("bad filter method");
						interlace = stbi__get8(s);
						if (interlace > 1)
							return stbi__err("bad interlace method");
						if (s.ImgX == 0 || s.ImgY == 0)
							return stbi__err("0-pixel image");
						if (pal_img_n == 0)
						{
							s.ImgN = ((color & 2) != 0 ? 3 : 1) + ((color & 4) != 0 ? 1 : 0);
							if ((1 << 30) / s.ImgX / s.ImgN < s.ImgY)
								return stbi__err("too large");
							if (scan == STBI_SCAN_HEADER)
								return 1;
						}
						else
						{
							s.ImgN = 1;
							if ((1 << 30) / s.ImgX / 4 < s.ImgY)
								return stbi__err("too large");
						}

						break;
					}
					case ((uint)'P' << 24) + ((uint)'L' << 16) + ((uint)'T' << 8) + 'E':
					{
						if (first != 0)
							return stbi__err("first not IHDR");
						if (c.length > 256 * 3)
							return stbi__err("invalid PLTE");
						pal_len = c.length / 3;
						if (pal_len * 3 != c.length)
							return stbi__err("invalid PLTE");
						for (i = (uint)0; i < pal_len; ++i)
						{
							palette[i * 4 + 0] = stbi__get8(s);
							palette[i * 4 + 1] = stbi__get8(s);
							palette[i * 4 + 2] = stbi__get8(s);
							palette[i * 4 + 3] = 255;
						}

						break;
					}
					case ((uint)'t' << 24) + ((uint)'R' << 16) + ((uint)'N' << 8) + 'S':
					{
						if (first != 0)
							return stbi__err("first not IHDR");
						if (z.Idata != null)
							return stbi__err("tRNS after IDAT");
						if (pal_img_n != 0)
						{
							if (scan == STBI_SCAN_HEADER)
							{
								s.ImgN = 4;
								return 1;
							}

							if (pal_len == 0)
								return stbi__err("tRNS before PLTE");
							if (c.length > pal_len)
								return stbi__err("bad tRNS len");
							pal_img_n = 4;
							for (i = (uint)0; i < c.length; ++i)
								palette[i * 4 + 3] = stbi__get8(s);
						}
						else
						{
							if ((s.ImgN & 1) == 0)
								return stbi__err("tRNS with alpha");
							if (c.length != (uint)s.ImgN * 2)
								return stbi__err("bad tRNS len");
							has_trans = 1;
							if (z.Depth == 16)
								for (k = 0; k < s.ImgN; ++k)
									tc16[k] = (ushort)stbi__get16be(s);
							else
								for (k = 0; k < s.ImgN; ++k)
									tc[k] = (byte)((byte)(stbi__get16be(s) & 255) * StbiDepthScaleTable[z.Depth]);
						}

						break;
					}
					case ((uint)'I' << 24) + ((uint)'D' << 16) + ((uint)'A' << 8) + 'T':
					{
						if (first != 0)
							return stbi__err("first not IHDR");
						if (pal_img_n != 0 && pal_len == 0)
							return stbi__err("no PLTE");
						if (scan == STBI_SCAN_HEADER)
						{
							s.ImgN = pal_img_n;
							return 1;
						}

						if ((int)(ioff + c.length) < (int)ioff)
							return 0;
						if (ioff + c.length > idata_limit)
						{
							var idata_limit_old = idata_limit;
							byte* p;
							if (idata_limit == 0)
								idata_limit = c.length > 4096 ? c.length : 4096;
							while (ioff + c.length > idata_limit)
								idata_limit *= 2;
							p = (byte*)CRuntime.Realloc(z.Idata, (ulong)idata_limit);
							if (p == null)
								return stbi__err("outofmem");
							z.Idata = p;
						}

						if (stbi__getn(s, z.Idata + ioff, (int)c.length) == 0)
							return stbi__err("outofdata");
						ioff += c.length;
						break;
					}
					case ((uint)'I' << 24) + ((uint)'E' << 16) + ((uint)'N' << 8) + 'D':
					{
						uint raw_len = 0;
						uint bpl = 0;
						if (first != 0)
							return stbi__err("first not IHDR");
						if (scan != STBI_SCAN_LOAD)
							return 1;
						if (z.Idata == null)
							return stbi__err("no IDAT");
						bpl = (uint)((s.ImgX * z.Depth + 7) / 8);
						raw_len = (uint)(bpl * s.ImgY * s.ImgN + s.ImgY);
						z.Expanded = (byte*)stbi_zlib_decode_malloc_guesssize_headerflag((sbyte*)z.Idata, (int)ioff,
							(int)raw_len, (int*)&raw_len, is_iphone != 0 ? 0 : 1);
						if (z.Expanded == null)
							return 0;
						CRuntime.Free(z.Idata);
						z.Idata = null;
						if (reqComp == s.ImgN + 1 && reqComp != 3 && pal_img_n == 0 || has_trans != 0)
							s.ImgOutN = s.ImgN + 1;
						else
							s.ImgOutN = s.ImgN;
						if (stbi__create_png_image(z, z.Expanded, raw_len, s.ImgOutN, z.Depth, color, interlace) == 0)
							return 0;
						if (has_trans != 0)
						{
							if (z.Depth == 16)
							{
								if (stbi__compute_transparency16(z, tc16, s.ImgOutN) == 0)
									return 0;
							}
							else
							{
								if (stbi__compute_transparency(z, tc, s.ImgOutN) == 0)
									return 0;
							}
						}

						if (is_iphone != 0 && StbiDeIphoneFlag != 0 && s.ImgOutN > 2)
							stbi__de_iphone(z);
						if (pal_img_n != 0)
						{
							s.ImgN = pal_img_n;
							s.ImgOutN = pal_img_n;
							if (reqComp >= 3)
								s.ImgOutN = reqComp;
							if (stbi__expand_png_palette(z, palette, (int)pal_len, s.ImgOutN) == 0)
								return 0;
						}
						else if (has_trans != 0)
						{
							++s.ImgN;
						}

						CRuntime.Free(z.Expanded);
						z.Expanded = null;
						return 1;
					}
					default:
						if (first != 0)
							return stbi__err("first not IHDR");
						if ((c.type & (1 << 29)) == 0)
						{
							var invalid_chunk = c.type + " PNG chunk not known";
							return stbi__err(invalid_chunk);
						}

						stbi__skip(s, (int)c.length);
						break;
				}

				stbi__get32be(s);
			}
		}

		public static void* stbi__do_png(StbiPng p, int* x, int* y, int* n, int reqComp, StbiResultInfo* ri)
		{
			void* result = null;
			if (reqComp < 0 || reqComp > 4)
				return (byte*)(ulong)(stbi__err("bad req_comp") != 0 ? (byte*)null : null);
			if (stbi__parse_png_file(p, STBI_SCAN_LOAD, reqComp) != 0)
			{
				if (p.Depth < 8)
					ri->bits_per_channel = 8;
				else
					ri->bits_per_channel = p.Depth;
				result = p.Out;
				p.Out = null;
				if (reqComp != 0 && reqComp != p.S.ImgOutN)
				{
					if (ri->bits_per_channel == 8)
						result = stbi__convert_format((byte*)result, p.S.ImgOutN, reqComp, p.S.ImgX, p.S.ImgY);
					else
						result = stbi__convert_format16((ushort*)result, p.S.ImgOutN, reqComp, p.S.ImgX,
							p.S.ImgY);
					p.S.ImgOutN = reqComp;
					if (result == null)
						return result;
				}

				*x = (int)p.S.ImgX;
				*y = (int)p.S.ImgY;
				if (n != null)
					*n = p.S.ImgN;
			}

			CRuntime.Free(p.Out);
			p.Out = null;
			CRuntime.Free(p.Expanded);
			p.Expanded = null;
			CRuntime.Free(p.Idata);
			p.Idata = null;
			return result;
		}

		public static void* stbi__png_load(StbiContext s, int* x, int* y, int* comp, int reqComp,
			StbiResultInfo* ri)
		{
			var p = new StbiPng();
			p.S = s;
			return stbi__do_png(p, x, y, comp, reqComp, ri);
		}

		public static int stbi__png_test(StbiContext s)
		{
			var r = 0;
			r = stbi__check_png_header(s);
			stbi__rewind(s);
			return r;
		}

		public static int stbi__png_info_raw(StbiPng p, int* x, int* y, int* comp)
		{
			if (stbi__parse_png_file(p, STBI_SCAN_HEADER, 0) == 0)
			{
				stbi__rewind(p.S);
				return 0;
			}

			if (x != null)
				*x = (int)p.S.ImgX;
			if (y != null)
				*y = (int)p.S.ImgY;
			if (comp != null)
				*comp = p.S.ImgN;
			return 1;
		}

		public static int stbi__png_info(StbiContext s, int* x, int* y, int* comp)
		{
			var p = new StbiPng();
			p.S = s;
			return stbi__png_info_raw(p, x, y, comp);
		}

		public static int stbi__png_is16(StbiContext s)
		{
			var p = new StbiPng();
			p.S = s;
			if (stbi__png_info_raw(p, null, null, null) == 0)
				return 0;
			if (p.Depth != 16)
			{
				stbi__rewind(p.S);
				return 0;
			}

			return 1;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct StbiPngchunk
		{
			public uint length;
			public uint type;
		}

		public class StbiPng
		{
			public byte* Out;
			public int Depth;
			public byte* Expanded;
			public byte* Idata;
			public StbiContext S;
		}
	}

	#pragma warning restore CA2014
}
