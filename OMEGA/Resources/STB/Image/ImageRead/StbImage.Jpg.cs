using System;
namespace STB
{
    internal unsafe partial class StbImage
    {
        #pragma warning disable CA2014

        public static uint[] StbiBmask =
            {0, 1, 3, 7, 15, 31, 63, 127, 255, 511, 1023, 2047, 4095, 8191, 16383, 32767, 65535};

        public static int[] StbiJbias =
            {0, -1, -3, -7, -15, -31, -63, -127, -255, -511, -1023, -2047, -4095, -8191, -16383, -32767};

        public static byte[] StbiJpegDezigzag =
        {
            0, 1, 8, 16, 9, 2, 3, 10, 17, 24, 32, 25, 18, 11, 4, 5, 12, 19, 26, 33, 40, 48, 41, 34, 27, 20, 13, 6, 7,
            14, 21, 28, 35, 42, 49, 56, 57, 50, 43, 36, 29, 22, 15, 23, 30, 37, 44, 51, 58, 59, 52, 45, 38, 31, 39, 46,
            53, 60, 61, 54, 47, 55, 62, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63, 63
        };

        public static int stbi__build_huffman(StbiHuffman h, int* count)
        {
            var i = 0;
            var j = 0;
            var k = 0;
            uint code = 0;
            for (i = 0; i < 16; ++i)
                for (j = 0; j < count[i]; ++j)
                    h.Size[k++] = (byte)(i + 1);
            h.Size[k] = 0;
            code = 0;
            k = 0;
            for (j = 1; j <= 16; ++j)
            {
                h.Delta[j] = (int)(k - code);
                if (h.Size[k] == j)
                {
                    while (h.Size[k] == j)
                        h.Code[k++] = (ushort)code++;
                    if (code - 1 >= 1u << j)
                        return stbi__err("bad code lengths");
                }

                h.Maxcode[j] = code << (16 - j);
                code <<= 1;
            }

            h.Maxcode[j] = 0xffffffff;
            CRuntime.SetArray(h.Fast, (byte)255);
            for (i = 0; i < k; ++i)
            {
                var s = (int)h.Size[i];
                if (s <= 9)
                {
                    var c = h.Code[i] << (9 - s);
                    var m = 1 << (9 - s);
                    for (j = 0; j < m; ++j)
                        h.Fast[c + j] = (byte)i;
                }
            }

            return 1;
        }

        public static void stbi__build_fast_ac(short[] fastAc, StbiHuffman h)
        {
            var i = 0;
            for (i = 0; i < 1 << 9; ++i)
            {
                var fast = h.Fast[i];
                fastAc[i] = 0;
                if (fast < 255)
                {
                    var rs = (int)h.Values[fast];
                    var run = (rs >> 4) & 15;
                    var magbits = rs & 15;
                    var len = (int)h.Size[fast];
                    if (magbits != 0 && len + magbits <= 9)
                    {
                        var k = ((i << len) & ((1 << 9) - 1)) >> (9 - magbits);
                        var m = 1 << (magbits - 1);
                        if (k < m)
                            k += (int)((~0U << magbits) + 1);
                        if (k >= -128 && k <= 127)
                            fastAc[i] = (short)(k * 256 + run * 16 + len + magbits);
                    }
                }
            }
        }

        public static void stbi__grow_buffer_unsafe(StbiJpeg j)
        {
            do
            {
                var b = (uint)(j.Nomore != 0 ? 0 : stbi__get8(j.S));
                if (b == 0xff)
                {
                    var c = (int)stbi__get8(j.S);
                    while (c == 0xff)
                        c = stbi__get8(j.S);
                    if (c != 0)
                    {
                        j.Marker = (byte)c;
                        j.Nomore = 1;
                        return;
                    }
                }

                j.CodeBuffer |= b << (24 - j.CodeBits);
                j.CodeBits += 8;
            } while (j.CodeBits <= 24);
        }

        public static int stbi__jpeg_huff_decode(StbiJpeg j, StbiHuffman h)
        {
            uint temp = 0;
            var c = 0;
            var k = 0;
            if (j.CodeBits < 16)
                stbi__grow_buffer_unsafe(j);
            c = (int)((j.CodeBuffer >> (32 - 9)) & ((1 << 9) - 1));
            k = h.Fast[c];
            if (k < 255)
            {
                var s = (int)h.Size[k];
                if (s > j.CodeBits)
                    return -1;
                j.CodeBuffer <<= s;
                j.CodeBits -= s;
                return h.Values[k];
            }

            temp = j.CodeBuffer >> 16;
            for (k = 9 + 1; ; ++k)
                if (temp < h.Maxcode[k])
                    break;
            if (k == 17)
            {
                j.CodeBits -= 16;
                return -1;
            }

            if (k > j.CodeBits)
                return -1;
            c = (int)(((j.CodeBuffer >> (32 - k)) & StbiBmask[k]) + h.Delta[k]);
            j.CodeBits -= k;
            j.CodeBuffer <<= k;
            return h.Values[c];
        }

        public static int stbi__extend_receive(StbiJpeg j, int n)
        {
            uint k = 0;
            var sgn = 0;
            if (j.CodeBits < n)
                stbi__grow_buffer_unsafe(j);
            sgn = (int)j.CodeBuffer >> 31;
            k = CRuntime._lrotl(j.CodeBuffer, n);
            j.CodeBuffer = k & ~StbiBmask[n];
            k &= StbiBmask[n];
            j.CodeBits -= n;
            return (int)(k + (StbiJbias[n] & ~sgn));
        }

        public static int stbi__jpeg_get_bits(StbiJpeg j, int n)
        {
            uint k = 0;
            if (j.CodeBits < n)
                stbi__grow_buffer_unsafe(j);
            k = CRuntime._lrotl(j.CodeBuffer, n);
            j.CodeBuffer = k & ~StbiBmask[n];
            k &= StbiBmask[n];
            j.CodeBits -= n;
            return (int)k;
        }

        public static int stbi__jpeg_get_bit(StbiJpeg j)
        {
            uint k = 0;
            if (j.CodeBits < 1)
                stbi__grow_buffer_unsafe(j);
            k = j.CodeBuffer;
            j.CodeBuffer <<= 1;
            --j.CodeBits;
            return (int)(k & 0x80000000);
        }

        public static int stbi__jpeg_decode_block(StbiJpeg j, short* data, StbiHuffman hdc, StbiHuffman hac,
            short[] fac, int b, ushort[] dequant)
        {
            var diff = 0;
            var dc = 0;
            var k = 0;
            var t = 0;
            if (j.CodeBits < 16)
                stbi__grow_buffer_unsafe(j);
            t = stbi__jpeg_huff_decode(j, hdc);
            if (t < 0)
                return stbi__err("bad huffman code");
            CRuntime.Memset(data, 0, (ulong)(64 * sizeof(short)));
            diff = t != 0 ? stbi__extend_receive(j, t) : 0;
            dc = j.ImgComp[b].dc_pred + diff;
            j.ImgComp[b].dc_pred = dc;
            data[0] = (short)(dc * dequant[0]);
            k = 1;
            do
            {
                uint zig = 0;
                var c = 0;
                var r = 0;
                var s = 0;
                if (j.CodeBits < 16)
                    stbi__grow_buffer_unsafe(j);
                c = (int)((j.CodeBuffer >> (32 - 9)) & ((1 << 9) - 1));
                r = fac[c];
                if (r != 0)
                {
                    k += (r >> 4) & 15;
                    s = r & 15;
                    j.CodeBuffer <<= s;
                    j.CodeBits -= s;
                    zig = StbiJpegDezigzag[k++];
                    data[zig] = (short)((r >> 8) * dequant[zig]);
                }
                else
                {
                    var rs = stbi__jpeg_huff_decode(j, hac);
                    if (rs < 0)
                        return stbi__err("bad huffman code");
                    s = rs & 15;
                    r = rs >> 4;
                    if (s == 0)
                    {
                        if (rs != 0xf0)
                            break;
                        k += 16;
                    }
                    else
                    {
                        k += r;
                        zig = StbiJpegDezigzag[k++];
                        data[zig] = (short)(stbi__extend_receive(j, s) * dequant[zig]);
                    }
                }
            } while (k < 64);

            return 1;
        }

        public static int stbi__jpeg_decode_block_prog_dc(StbiJpeg j, short* data, StbiHuffman hdc, int b)
        {
            var diff = 0;
            var dc = 0;
            var t = 0;
            if (j.SpecEnd != 0)
                return stbi__err("can't merge dc and ac");
            if (j.CodeBits < 16)
                stbi__grow_buffer_unsafe(j);
            if (j.SuccHigh == 0)
            {
                CRuntime.Memset(data, 0, (ulong)(64 * sizeof(short)));
                t = stbi__jpeg_huff_decode(j, hdc);
                diff = t != 0 ? stbi__extend_receive(j, t) : 0;
                dc = j.ImgComp[b].dc_pred + diff;
                j.ImgComp[b].dc_pred = dc;
                data[0] = (short)(dc << j.SuccLow);
            }
            else
            {
                if (stbi__jpeg_get_bit(j) != 0)
                    data[0] += (short)(1 << j.SuccLow);
            }

            return 1;
        }

        public static int stbi__jpeg_decode_block_prog_ac(StbiJpeg j, short* data, StbiHuffman hac, short[] fac)
        {
            var k = 0;
            if (j.SpecStart == 0)
                return stbi__err("can't merge dc and ac");
            if (j.SuccHigh == 0)
            {
                var shift = j.SuccLow;
                if (j.EobRun != 0)
                {
                    --j.EobRun;
                    return 1;
                }

                k = j.SpecStart;
                do
                {
                    uint zig = 0;
                    var c = 0;
                    var r = 0;
                    var s = 0;
                    if (j.CodeBits < 16)
                        stbi__grow_buffer_unsafe(j);
                    c = (int)((j.CodeBuffer >> (32 - 9)) & ((1 << 9) - 1));
                    r = fac[c];
                    if (r != 0)
                    {
                        k += (r >> 4) & 15;
                        s = r & 15;
                        j.CodeBuffer <<= s;
                        j.CodeBits -= s;
                        zig = StbiJpegDezigzag[k++];
                        data[zig] = (short)((r >> 8) << shift);
                    }
                    else
                    {
                        var rs = stbi__jpeg_huff_decode(j, hac);
                        if (rs < 0)
                            return stbi__err("bad huffman code");
                        s = rs & 15;
                        r = rs >> 4;
                        if (s == 0)
                        {
                            if (r < 15)
                            {
                                j.EobRun = 1 << r;
                                if (r != 0)
                                    j.EobRun += stbi__jpeg_get_bits(j, r);
                                --j.EobRun;
                                break;
                            }

                            k += 16;
                        }
                        else
                        {
                            k += r;
                            zig = StbiJpegDezigzag[k++];
                            data[zig] = (short)(stbi__extend_receive(j, s) << shift);
                        }
                    }
                } while (k <= j.SpecEnd);
            }
            else
            {
                var bit = (short)(1 << j.SuccLow);
                if (j.EobRun != 0)
                {
                    --j.EobRun;
                    for (k = j.SpecStart; k <= j.SpecEnd; ++k)
                    {
                        var p = &data[StbiJpegDezigzag[k]];
                        if (*p != 0)
                            if (stbi__jpeg_get_bit(j) != 0)
                                if ((*p & bit) == 0)
                                {
                                    if (*p > 0)
                                        *p += bit;
                                    else
                                        *p -= bit;
                                }
                    }
                }
                else
                {
                    k = j.SpecStart;
                    do
                    {
                        var r = 0;
                        var s = 0;
                        var rs = stbi__jpeg_huff_decode(j, hac);
                        if (rs < 0)
                            return stbi__err("bad huffman code");
                        s = rs & 15;
                        r = rs >> 4;
                        if (s == 0)
                        {
                            if (r < 15)
                            {
                                j.EobRun = (1 << r) - 1;
                                if (r != 0)
                                    j.EobRun += stbi__jpeg_get_bits(j, r);
                                r = 64;
                            }
                        }
                        else
                        {
                            if (s != 1)
                                return stbi__err("bad huffman code");
                            if (stbi__jpeg_get_bit(j) != 0)
                                s = bit;
                            else
                                s = -bit;
                        }

                        while (k <= j.SpecEnd)
                        {
                            var p = &data[StbiJpegDezigzag[k++]];
                            if (*p != 0)
                            {
                                if (stbi__jpeg_get_bit(j) != 0)
                                    if ((*p & bit) == 0)
                                    {
                                        if (*p > 0)
                                            *p += bit;
                                        else
                                            *p -= bit;
                                    }
                            }
                            else
                            {
                                if (r == 0)
                                {
                                    *p = (short)s;
                                    break;
                                }

                                --r;
                            }
                        }
                    } while (k <= j.SpecEnd);
                }
            }

            return 1;
        }

        public static void stbi__idct_block(byte* @out, int outStride, short* data)
        {
            var i = 0;
            var val = stackalloc int[64];
            var v = val;
            byte* o;
            var d = data;
            for (i = 0; i < 8; ++i, ++d, ++v)
                if (d[8] == 0 && d[16] == 0 && d[24] == 0 && d[32] == 0 && d[40] == 0 && d[48] == 0 && d[56] == 0)
                {
                    var dcterm = d[0] * 4;
                    v[0] = v[8] = v[16] = v[24] = v[32] = v[40] = v[48] = v[56] = dcterm;
                }
                else
                {
                    var t0 = 0;
                    var t1 = 0;
                    var t2 = 0;
                    var t3 = 0;
                    var p1 = 0;
                    var p2 = 0;
                    var p3 = 0;
                    var p4 = 0;
                    var p5 = 0;
                    var x0 = 0;
                    var x1 = 0;
                    var x2 = 0;
                    var x3 = 0;
                    p2 = d[16];
                    p3 = d[48];
                    p1 = (p2 + p3) * (int)(0.5411961f * 4096 + 0.5);
                    t2 = p1 + p3 * (int)(-1.847759065f * 4096 + 0.5);
                    t3 = p1 + p2 * (int)(0.765366865f * 4096 + 0.5);
                    p2 = d[0];
                    p3 = d[32];
                    t0 = (p2 + p3) * 4096;
                    t1 = (p2 - p3) * 4096;
                    x0 = t0 + t3;
                    x3 = t0 - t3;
                    x1 = t1 + t2;
                    x2 = t1 - t2;
                    t0 = d[56];
                    t1 = d[40];
                    t2 = d[24];
                    t3 = d[8];
                    p3 = t0 + t2;
                    p4 = t1 + t3;
                    p1 = t0 + t3;
                    p2 = t1 + t2;
                    p5 = (p3 + p4) * (int)(1.175875602f * 4096 + 0.5);
                    t0 = t0 * (int)(0.298631336f * 4096 + 0.5);
                    t1 = t1 * (int)(2.053119869f * 4096 + 0.5);
                    t2 = t2 * (int)(3.072711026f * 4096 + 0.5);
                    t3 = t3 * (int)(1.501321110f * 4096 + 0.5);
                    p1 = p5 + p1 * (int)(-0.899976223f * 4096 + 0.5);
                    p2 = p5 + p2 * (int)(-2.562915447f * 4096 + 0.5);
                    p3 = p3 * (int)(-1.961570560f * 4096 + 0.5);
                    p4 = p4 * (int)(-0.390180644f * 4096 + 0.5);
                    t3 += p1 + p4;
                    t2 += p2 + p3;
                    t1 += p2 + p4;
                    t0 += p1 + p3;
                    x0 += 512;
                    x1 += 512;
                    x2 += 512;
                    x3 += 512;
                    v[0] = (x0 + t3) >> 10;
                    v[56] = (x0 - t3) >> 10;
                    v[8] = (x1 + t2) >> 10;
                    v[48] = (x1 - t2) >> 10;
                    v[16] = (x2 + t1) >> 10;
                    v[40] = (x2 - t1) >> 10;
                    v[24] = (x3 + t0) >> 10;
                    v[32] = (x3 - t0) >> 10;
                }

            for (i = 0, v = val, o = @out; i < 8; ++i, v += 8, o += outStride)
            {
                var t0 = 0;
                var t1 = 0;
                var t2 = 0;
                var t3 = 0;
                var p1 = 0;
                var p2 = 0;
                var p3 = 0;
                var p4 = 0;
                var p5 = 0;
                var x0 = 0;
                var x1 = 0;
                var x2 = 0;
                var x3 = 0;
                p2 = v[2];
                p3 = v[6];
                p1 = (p2 + p3) * (int)(0.5411961f * 4096 + 0.5);
                t2 = p1 + p3 * (int)(-1.847759065f * 4096 + 0.5);
                t3 = p1 + p2 * (int)(0.765366865f * 4096 + 0.5);
                p2 = v[0];
                p3 = v[4];
                t0 = (p2 + p3) * 4096;
                t1 = (p2 - p3) * 4096;
                x0 = t0 + t3;
                x3 = t0 - t3;
                x1 = t1 + t2;
                x2 = t1 - t2;
                t0 = v[7];
                t1 = v[5];
                t2 = v[3];
                t3 = v[1];
                p3 = t0 + t2;
                p4 = t1 + t3;
                p1 = t0 + t3;
                p2 = t1 + t2;
                p5 = (p3 + p4) * (int)(1.175875602f * 4096 + 0.5);
                t0 = t0 * (int)(0.298631336f * 4096 + 0.5);
                t1 = t1 * (int)(2.053119869f * 4096 + 0.5);
                t2 = t2 * (int)(3.072711026f * 4096 + 0.5);
                t3 = t3 * (int)(1.501321110f * 4096 + 0.5);
                p1 = p5 + p1 * (int)(-0.899976223f * 4096 + 0.5);
                p2 = p5 + p2 * (int)(-2.562915447f * 4096 + 0.5);
                p3 = p3 * (int)(-1.961570560f * 4096 + 0.5);
                p4 = p4 * (int)(-0.390180644f * 4096 + 0.5);
                t3 += p1 + p4;
                t2 += p2 + p3;
                t1 += p2 + p4;
                t0 += p1 + p3;
                x0 += 65536 + (128 << 17);
                x1 += 65536 + (128 << 17);
                x2 += 65536 + (128 << 17);
                x3 += 65536 + (128 << 17);
                o[0] = stbi__clamp((x0 + t3) >> 17);
                o[7] = stbi__clamp((x0 - t3) >> 17);
                o[1] = stbi__clamp((x1 + t2) >> 17);
                o[6] = stbi__clamp((x1 - t2) >> 17);
                o[2] = stbi__clamp((x2 + t1) >> 17);
                o[5] = stbi__clamp((x2 - t1) >> 17);
                o[3] = stbi__clamp((x3 + t0) >> 17);
                o[4] = stbi__clamp((x3 - t0) >> 17);
            }
        }

        public static byte stbi__get_marker(StbiJpeg j)
        {
            byte x = 0;
            if (j.Marker != 0xff)
            {
                x = j.Marker;
                j.Marker = 0xff;
                return x;
            }

            x = stbi__get8(j.S);
            if (x != 0xff)
                return 0xff;
            while (x == 0xff)
                x = stbi__get8(j.S);
            return x;
        }

        public static void stbi__jpeg_reset(StbiJpeg j)
        {
            j.CodeBits = 0;
            j.CodeBuffer = 0;
            j.Nomore = 0;
            j.ImgComp[0].dc_pred = j.ImgComp[1].dc_pred = j.ImgComp[2].dc_pred = j.ImgComp[3].dc_pred = 0;
            j.Marker = 0xff;
            j.Todo = j.RestartInterval != 0 ? j.RestartInterval : 0x7fffffff;
            j.EobRun = 0;
        }

        public static int stbi__parse_entropy_coded_data(StbiJpeg z)
        {
            stbi__jpeg_reset(z);
            if (z.Progressive == 0)
            {
                if (z.ScanN == 1)
                {
                    var i = 0;
                    var j = 0;
                    var data = stackalloc short[64];
                    var n = z.Order[0];
                    var w = (z.ImgComp[n].x + 7) >> 3;
                    var h = (z.ImgComp[n].y + 7) >> 3;
                    for (j = 0; j < h; ++j)
                        for (i = 0; i < w; ++i)
                        {
                            var ha = z.ImgComp[n].ha;
                            if (stbi__jpeg_decode_block(z, data, z.HuffDc[z.ImgComp[n].hd], z.HuffAc[ha], z.FastAc[ha],
                                    n, z.Dequant[z.ImgComp[n].tq]) == 0)
                                return 0;
                            z.IdctBlockKernel(z.ImgComp[n].data + z.ImgComp[n].w2 * j * 8 + i * 8, z.ImgComp[n].w2,
                                data);
                            if (--z.Todo <= 0)
                            {
                                if (z.CodeBits < 24)
                                    stbi__grow_buffer_unsafe(z);
                                if (!(z.Marker >= 0xd0 && z.Marker <= 0xd7))
                                    return 1;
                                stbi__jpeg_reset(z);
                            }
                        }

                    return 1;
                }
                else
                {
                    var i = 0;
                    var j = 0;
                    var k = 0;
                    var x = 0;
                    var y = 0;
                    var data = stackalloc short[64];
                    for (j = 0; j < z.ImgMcuY; ++j)
                        for (i = 0; i < z.ImgMcuX; ++i)
                        {
                            for (k = 0; k < z.ScanN; ++k)
                            {
                                var n = z.Order[k];
                                for (y = 0; y < z.ImgComp[n].v; ++y)
                                    for (x = 0; x < z.ImgComp[n].h; ++x)
                                    {
                                        var x2 = (i * z.ImgComp[n].h + x) * 8;
                                        var y2 = (j * z.ImgComp[n].v + y) * 8;
                                        var ha = z.ImgComp[n].ha;
                                        if (stbi__jpeg_decode_block(z, data, z.HuffDc[z.ImgComp[n].hd], z.HuffAc[ha],
                                                z.FastAc[ha], n, z.Dequant[z.ImgComp[n].tq]) == 0)
                                            return 0;
                                        z.IdctBlockKernel(z.ImgComp[n].data + z.ImgComp[n].w2 * y2 + x2, z.ImgComp[n].w2,
                                            data);
                                    }
                            }

                            if (--z.Todo <= 0)
                            {
                                if (z.CodeBits < 24)
                                    stbi__grow_buffer_unsafe(z);
                                if (!(z.Marker >= 0xd0 && z.Marker <= 0xd7))
                                    return 1;
                                stbi__jpeg_reset(z);
                            }
                        }

                    return 1;
                }
            }

            if (z.ScanN == 1)
            {
                var i = 0;
                var j = 0;
                var n = z.Order[0];
                var w = (z.ImgComp[n].x + 7) >> 3;
                var h = (z.ImgComp[n].y + 7) >> 3;
                for (j = 0; j < h; ++j)
                    for (i = 0; i < w; ++i)
                    {
                        var data = z.ImgComp[n].coeff + 64 * (i + j * z.ImgComp[n].coeff_w);
                        if (z.SpecStart == 0)
                        {
                            if (stbi__jpeg_decode_block_prog_dc(z, data, z.HuffDc[z.ImgComp[n].hd], n) == 0)
                                return 0;
                        }
                        else
                        {
                            var ha = z.ImgComp[n].ha;
                            if (stbi__jpeg_decode_block_prog_ac(z, data, z.HuffAc[ha], z.FastAc[ha]) == 0)
                                return 0;
                        }

                        if (--z.Todo <= 0)
                        {
                            if (z.CodeBits < 24)
                                stbi__grow_buffer_unsafe(z);
                            if (!(z.Marker >= 0xd0 && z.Marker <= 0xd7))
                                return 1;
                            stbi__jpeg_reset(z);
                        }
                    }

                return 1;
            }
            else
            {
                var i = 0;
                var j = 0;
                var k = 0;
                var x = 0;
                var y = 0;
                for (j = 0; j < z.ImgMcuY; ++j)
                    for (i = 0; i < z.ImgMcuX; ++i)
                    {
                        for (k = 0; k < z.ScanN; ++k)
                        {
                            var n = z.Order[k];
                            for (y = 0; y < z.ImgComp[n].v; ++y)
                                for (x = 0; x < z.ImgComp[n].h; ++x)
                                {
                                    var x2 = i * z.ImgComp[n].h + x;
                                    var y2 = j * z.ImgComp[n].v + y;
                                    var data = z.ImgComp[n].coeff + 64 * (x2 + y2 * z.ImgComp[n].coeff_w);
                                    if (stbi__jpeg_decode_block_prog_dc(z, data, z.HuffDc[z.ImgComp[n].hd], n) == 0)
                                        return 0;
                                }
                        }

                        if (--z.Todo <= 0)
                        {
                            if (z.CodeBits < 24)
                                stbi__grow_buffer_unsafe(z);
                            if (!(z.Marker >= 0xd0 && z.Marker <= 0xd7))
                                return 1;
                            stbi__jpeg_reset(z);
                        }
                    }

                return 1;
            }
        }

        public static void stbi__jpeg_dequantize(short* data, ushort[] dequant)
        {
            var i = 0;
            for (i = 0; i < 64; ++i)
                data[i] *= (short)dequant[i];
        }

        public static void stbi__jpeg_finish(StbiJpeg z)
        {
            if (z.Progressive != 0)
            {
                var i = 0;
                var j = 0;
                var n = 0;
                for (n = 0; n < z.S.ImgN; ++n)
                {
                    var w = (z.ImgComp[n].x + 7) >> 3;
                    var h = (z.ImgComp[n].y + 7) >> 3;
                    for (j = 0; j < h; ++j)
                        for (i = 0; i < w; ++i)
                        {
                            var data = z.ImgComp[n].coeff + 64 * (i + j * z.ImgComp[n].coeff_w);
                            stbi__jpeg_dequantize(data, z.Dequant[z.ImgComp[n].tq]);
                            z.IdctBlockKernel(z.ImgComp[n].data + z.ImgComp[n].w2 * j * 8 + i * 8, z.ImgComp[n].w2,
                                data);
                        }
                }
            }
        }

        public static int stbi__process_marker(StbiJpeg z, int m)
        {
            var l = 0;
            switch (m)
            {
                case 0xff:
                    return stbi__err("expected marker");
                case 0xDD:
                    if (stbi__get16be(z.S) != 4)
                        return stbi__err("bad DRI len");
                    z.RestartInterval = stbi__get16be(z.S);
                    return 1;
                case 0xDB:
                    l = stbi__get16be(z.S) - 2;
                    while (l > 0)
                    {
                        var q = (int)stbi__get8(z.S);
                        var p = q >> 4;
                        var sixteen = p != 0 ? 1 : 0;
                        var t = q & 15;
                        var i = 0;
                        if (p != 0 && p != 1)
                            return stbi__err("bad DQT type");
                        if (t > 3)
                            return stbi__err("bad DQT table");
                        for (i = 0; i < 64; ++i)
                            z.Dequant[t][StbiJpegDezigzag[i]] =
                                (ushort)(sixteen != 0 ? stbi__get16be(z.S) : stbi__get8(z.S));
                        l -= sixteen != 0 ? 129 : 65;
                    }

                    return l == 0 ? 1 : 0;
                case 0xC4:
                    l = stbi__get16be(z.S) - 2;
                    while (l > 0)
                    {
                        byte[] v;
                        var sizes = stackalloc int[16];
                        var i = 0;
                        var n = 0;
                        var q = (int)stbi__get8(z.S);
                        var tc = q >> 4;
                        var th = q & 15;
                        if (tc > 1 || th > 3)
                            return stbi__err("bad DHT header");
                        for (i = 0; i < 16; ++i)
                        {
                            sizes[i] = stbi__get8(z.S);
                            n += sizes[i];
                        }

                        l -= 17;
                        if (tc == 0)
                        {
                            if (stbi__build_huffman(z.HuffDc[th], sizes) == 0)
                                return 0;
                            v = z.HuffDc[th].Values;
                        }
                        else
                        {
                            if (stbi__build_huffman(z.HuffAc[th], sizes) == 0)
                                return 0;
                            v = z.HuffAc[th].Values;
                        }

                        for (i = 0; i < n; ++i)
                            v[i] = stbi__get8(z.S);
                        if (tc != 0)
                            stbi__build_fast_ac(z.FastAc[th], z.HuffAc[th]);
                        l -= n;
                    }

                    return l == 0 ? 1 : 0;
            }

            if (m >= 0xE0 && m <= 0xEF || m == 0xFE)
            {
                l = stbi__get16be(z.S);
                if (l < 2)
                {
                    if (m == 0xFE)
                        return stbi__err("bad COM len");
                    return stbi__err("bad APP len");
                }

                l -= 2;
                if (m == 0xE0 && l >= 5)
                {
                    var tag = stackalloc byte[5];
                    tag[0] = (byte)'J';
                    tag[1] = (byte)'F';
                    tag[2] = (byte)'I';
                    tag[3] = (byte)'F';
                    tag[4] = (byte)'\0';
                    var ok = 1;
                    var i = 0;
                    for (i = 0; i < 5; ++i)
                        if (stbi__get8(z.S) != tag[i])
                            ok = 0;
                    l -= 5;
                    if (ok != 0)
                        z.Jfif = 1;
                }
                else if (m == 0xEE && l >= 12)
                {
                    var tag = stackalloc byte[6];
                    tag[0] = (byte)'A';
                    tag[1] = (byte)'d';
                    tag[2] = (byte)'o';
                    tag[3] = (byte)'b';
                    tag[4] = (byte)'e';
                    tag[5] = (byte)'\0';
                    var ok = 1;
                    var i = 0;
                    for (i = 0; i < 6; ++i)
                        if (stbi__get8(z.S) != tag[i])
                            ok = 0;
                    l -= 6;
                    if (ok != 0)
                    {
                        stbi__get8(z.S);
                        stbi__get16be(z.S);
                        stbi__get16be(z.S);
                        z.App14ColorTransform = stbi__get8(z.S);
                        l -= 6;
                    }
                }

                stbi__skip(z.S, l);
                return 1;
            }

            return stbi__err("unknown marker");
        }

        public static int stbi__process_scan_header(StbiJpeg z)
        {
            var i = 0;
            var ls = stbi__get16be(z.S);
            z.ScanN = stbi__get8(z.S);
            if (z.ScanN < 1 || z.ScanN > 4 || z.ScanN > z.S.ImgN)
                return stbi__err("bad SOS component count");
            if (ls != 6 + 2 * z.ScanN)
                return stbi__err("bad SOS len");
            for (i = 0; i < z.ScanN; ++i)
            {
                var id = (int)stbi__get8(z.S);
                var which = 0;
                var q = (int)stbi__get8(z.S);
                for (which = 0; which < z.S.ImgN; ++which)
                    if (z.ImgComp[which].id == id)
                        break;
                if (which == z.S.ImgN)
                    return 0;
                z.ImgComp[which].hd = q >> 4;
                if (z.ImgComp[which].hd > 3)
                    return stbi__err("bad DC huff");
                z.ImgComp[which].ha = q & 15;
                if (z.ImgComp[which].ha > 3)
                    return stbi__err("bad AC huff");
                z.Order[i] = which;
            }

            {
                var aa = 0;
                z.SpecStart = stbi__get8(z.S);
                z.SpecEnd = stbi__get8(z.S);
                aa = stbi__get8(z.S);
                z.SuccHigh = aa >> 4;
                z.SuccLow = aa & 15;
                if (z.Progressive != 0)
                {
                    if (z.SpecStart > 63 || z.SpecEnd > 63 || z.SpecStart > z.SpecEnd || z.SuccHigh > 13 ||
                        z.SuccLow > 13)
                        return stbi__err("bad SOS");
                }
                else
                {
                    if (z.SpecStart != 0)
                        return stbi__err("bad SOS");
                    if (z.SuccHigh != 0 || z.SuccLow != 0)
                        return stbi__err("bad SOS");
                    z.SpecEnd = 63;
                }
            }

            return 1;
        }

        public static int stbi__free_jpeg_components(StbiJpeg z, int ncomp, int why)
        {
            var i = 0;
            for (i = 0; i < ncomp; ++i)
            {
                if (z.ImgComp[i].raw_data != null)
                {
                    CRuntime.Free(z.ImgComp[i].raw_data);
                    z.ImgComp[i].raw_data = null;
                    z.ImgComp[i].data = null;
                }

                if (z.ImgComp[i].raw_coeff != null)
                {
                    CRuntime.Free(z.ImgComp[i].raw_coeff);
                    z.ImgComp[i].raw_coeff = null;
                    z.ImgComp[i].coeff = null;
                }

                if (z.ImgComp[i].linebuf != null)
                {
                    CRuntime.Free(z.ImgComp[i].linebuf);
                    z.ImgComp[i].linebuf = null;
                }
            }

            return why;
        }

        public static int stbi__process_frame_header(StbiJpeg z, int scan)
        {
            var s = z.S;
            var lf = 0;
            var p = 0;
            var i = 0;
            var q = 0;
            var h_max = 1;
            var v_max = 1;
            var c = 0;
            lf = stbi__get16be(s);
            if (lf < 11)
                return stbi__err("bad SOF len");
            p = stbi__get8(s);
            if (p != 8)
                return stbi__err("only 8-bit");
            s.ImgY = (uint)stbi__get16be(s);
            if (s.ImgY == 0)
                return stbi__err("no header height");
            s.ImgX = (uint)stbi__get16be(s);
            if (s.ImgX == 0)
                return stbi__err("0 width");
            c = stbi__get8(s);
            if (c != 3 && c != 1 && c != 4)
                return stbi__err("bad component count");
            s.ImgN = c;
            for (i = 0; i < c; ++i)
            {
                z.ImgComp[i].data = null;
                z.ImgComp[i].linebuf = null;
            }

            if (lf != 8 + 3 * s.ImgN)
                return stbi__err("bad SOF len");
            z.Rgb = 0;
            for (i = 0; i < s.ImgN; ++i)
            {
                var rgb = stackalloc byte[3];
                rgb[0] = (byte)'R';
                rgb[1] = (byte)'G';
                rgb[2] = (byte)'B';
                z.ImgComp[i].id = stbi__get8(s);
                if (s.ImgN == 3 && z.ImgComp[i].id == rgb[i])
                    ++z.Rgb;
                q = stbi__get8(s);
                z.ImgComp[i].h = q >> 4;
                if (z.ImgComp[i].h == 0 || z.ImgComp[i].h > 4)
                    return stbi__err("bad H");
                z.ImgComp[i].v = q & 15;
                if (z.ImgComp[i].v == 0 || z.ImgComp[i].v > 4)
                    return stbi__err("bad V");
                z.ImgComp[i].tq = stbi__get8(s);
                if (z.ImgComp[i].tq > 3)
                    return stbi__err("bad TQ");
            }

            if (scan != STBI_SCAN_LOAD)
                return 1;
            if (stbi__mad3sizes_valid((int)s.ImgX, (int)s.ImgY, s.ImgN, 0) == 0)
                return stbi__err("too large");
            for (i = 0; i < s.ImgN; ++i)
            {
                if (z.ImgComp[i].h > h_max)
                    h_max = z.ImgComp[i].h;
                if (z.ImgComp[i].v > v_max)
                    v_max = z.ImgComp[i].v;
            }

            z.ImgHMax = h_max;
            z.ImgVMax = v_max;
            z.ImgMcuW = h_max * 8;
            z.ImgMcuH = v_max * 8;
            z.ImgMcuX = (int)((s.ImgX + z.ImgMcuW - 1) / z.ImgMcuW);
            z.ImgMcuY = (int)((s.ImgY + z.ImgMcuH - 1) / z.ImgMcuH);
            for (i = 0; i < s.ImgN; ++i)
            {
                z.ImgComp[i].x = (int)((s.ImgX * z.ImgComp[i].h + h_max - 1) / h_max);
                z.ImgComp[i].y = (int)((s.ImgY * z.ImgComp[i].v + v_max - 1) / v_max);
                z.ImgComp[i].w2 = z.ImgMcuX * z.ImgComp[i].h * 8;
                z.ImgComp[i].h2 = z.ImgMcuY * z.ImgComp[i].v * 8;
                z.ImgComp[i].coeff = null;
                z.ImgComp[i].raw_coeff = null;
                z.ImgComp[i].linebuf = null;
                z.ImgComp[i].raw_data = stbi__malloc_mad2(z.ImgComp[i].w2, z.ImgComp[i].h2, 15);
                if (z.ImgComp[i].raw_data == null)
                    return stbi__free_jpeg_components(z, i + 1, stbi__err("outofmem"));
                z.ImgComp[i].data = (byte*)(((long)z.ImgComp[i].raw_data + 15) & ~15);
                if (z.Progressive != 0)
                {
                    z.ImgComp[i].coeff_w = z.ImgComp[i].w2 / 8;
                    z.ImgComp[i].coeff_h = z.ImgComp[i].h2 / 8;
                    z.ImgComp[i].raw_coeff = stbi__malloc_mad3(z.ImgComp[i].w2, z.ImgComp[i].h2, sizeof(short), 15);
                    if (z.ImgComp[i].raw_coeff == null)
                        return stbi__free_jpeg_components(z, i + 1, stbi__err("outofmem"));
                    z.ImgComp[i].coeff = (short*)(((long)z.ImgComp[i].raw_coeff + 15) & ~15);
                }
            }

            return 1;
        }

        public static int stbi__decode_jpeg_header(StbiJpeg z, int scan)
        {
            var m = 0;
            z.Jfif = 0;
            z.App14ColorTransform = -1;
            z.Marker = 0xff;
            m = stbi__get_marker(z);
            if (!(m == 0xd8))
                return stbi__err("no SOI");
            if (scan == STBI_SCAN_TYPE)
                return 1;
            m = stbi__get_marker(z);
            while (!(m == 0xc0 || m == 0xc1 || m == 0xc2))
            {
                if (stbi__process_marker(z, m) == 0)
                    return 0;
                m = stbi__get_marker(z);
                while (m == 0xff)
                {
                    if (stbi__at_eof(z.S) != 0)
                        return stbi__err("no SOF");
                    m = stbi__get_marker(z);
                }
            }

            z.Progressive = m == 0xc2 ? 1 : 0;
            if (stbi__process_frame_header(z, scan) == 0)
                return 0;
            return 1;
        }

        public static int stbi__decode_jpeg_image(StbiJpeg j)
        {
            var m = 0;
            for (m = 0; m < 4; m++)
            {
                j.ImgComp[m].raw_data = null;
                j.ImgComp[m].raw_coeff = null;
            }

            j.RestartInterval = 0;
            if (stbi__decode_jpeg_header(j, STBI_SCAN_LOAD) == 0)
                return 0;
            m = stbi__get_marker(j);
            while (!(m == 0xd9))
            {
                if (m == 0xda)
                {
                    if (stbi__process_scan_header(j) == 0)
                        return 0;
                    if (stbi__parse_entropy_coded_data(j) == 0)
                        return 0;
                    if (j.Marker == 0xff)
                        while (stbi__at_eof(j.S) == 0)
                        {
                            var x = (int)stbi__get8(j.S);
                            if (x == 255)
                            {
                                j.Marker = stbi__get8(j.S);
                                break;
                            }
                        }
                }
                else if (m == 0xdc)
                {
                    var ld = stbi__get16be(j.S);
                    var nl = (uint)stbi__get16be(j.S);
                    if (ld != 4)
                        return stbi__err("bad DNL len");
                    if (nl != j.S.ImgY)
                        return stbi__err("bad DNL height");
                }
                else
                {
                    if (stbi__process_marker(j, m) == 0)
                        return 0;
                }

                m = stbi__get_marker(j);
            }

            if (j.Progressive != 0)
                stbi__jpeg_finish(j);
            return 1;
        }

        public static byte* resample_row_1(byte* @out, byte* inNear, byte* inFar, int w, int hs)
        {
            return inNear;
        }

        public static byte* stbi__resample_row_v_2(byte* @out, byte* inNear, byte* inFar, int w, int hs)
        {
            var i = 0;
            for (i = 0; i < w; ++i)
                @out[i] = (byte)((3 * inNear[i] + inFar[i] + 2) >> 2);
            return @out;
        }

        public static byte* stbi__resample_row_h_2(byte* @out, byte* inNear, byte* inFar, int w, int hs)
        {
            var i = 0;
            var input = inNear;
            if (w == 1)
            {
                @out[0] = @out[1] = input[0];
                return @out;
            }

            @out[0] = input[0];
            @out[1] = (byte)((input[0] * 3 + input[1] + 2) >> 2);
            for (i = 1; i < w - 1; ++i)
            {
                var n = 3 * input[i] + 2;
                @out[i * 2 + 0] = (byte)((n + input[i - 1]) >> 2);
                @out[i * 2 + 1] = (byte)((n + input[i + 1]) >> 2);
            }

            @out[i * 2 + 0] = (byte)((input[w - 2] * 3 + input[w - 1] + 2) >> 2);
            @out[i * 2 + 1] = input[w - 1];
            return @out;
        }

        public static byte* stbi__resample_row_hv_2(byte* @out, byte* inNear, byte* inFar, int w, int hs)
        {
            var i = 0;
            var t0 = 0;
            var t1 = 0;
            if (w == 1)
            {
                @out[0] = @out[1] = (byte)((3 * inNear[0] + inFar[0] + 2) >> 2);
                return @out;
            }

            t1 = 3 * inNear[0] + inFar[0];
            @out[0] = (byte)((t1 + 2) >> 2);
            for (i = 1; i < w; ++i)
            {
                t0 = t1;
                t1 = 3 * inNear[i] + inFar[i];
                @out[i * 2 - 1] = (byte)((3 * t0 + t1 + 8) >> 4);
                @out[i * 2] = (byte)((3 * t1 + t0 + 8) >> 4);
            }

            @out[w * 2 - 1] = (byte)((t1 + 2) >> 2);
            return @out;
        }

        public static byte* stbi__resample_row_generic(byte* @out, byte* inNear, byte* inFar, int w, int hs)
        {
            var i = 0;
            var j = 0;
            for (i = 0; i < w; ++i)
                for (j = 0; j < hs; ++j)
                    @out[i * hs + j] = inNear[i];
            return @out;
        }

        public static void stbi__YCbCr_to_RGB_row(byte* @out, byte* y, byte* pcb, byte* pcr, int count, int step)
        {
            var i = 0;
            for (i = 0; i < count; ++i)
            {
                var y_fixed = (y[i] << 20) + (1 << 19);
                var r = 0;
                var g = 0;
                var b = 0;
                var cr = pcr[i] - 128;
                var cb = pcb[i] - 128;
                r = y_fixed + cr * ((int)(1.40200f * 4096.0f + 0.5f) << 8);
                g = (int)(y_fixed + cr * -((int)(0.71414f * 4096.0f + 0.5f) << 8) +
                           ((cb * -((int)(0.34414f * 4096.0f + 0.5f) << 8)) & 0xffff0000));
                b = y_fixed + cb * ((int)(1.77200f * 4096.0f + 0.5f) << 8);
                r >>= 20;
                g >>= 20;
                b >>= 20;
                if ((uint)r > 255)
                {
                    if (r < 0)
                        r = 0;
                    else
                        r = 255;
                }

                if ((uint)g > 255)
                {
                    if (g < 0)
                        g = 0;
                    else
                        g = 255;
                }

                if ((uint)b > 255)
                {
                    if (b < 0)
                        b = 0;
                    else
                        b = 255;
                }

                @out[0] = (byte)r;
                @out[1] = (byte)g;
                @out[2] = (byte)b;
                @out[3] = 255;
                @out += step;
            }
        }

        public static void stbi__setup_jpeg(StbiJpeg j)
        {
            j.IdctBlockKernel = stbi__idct_block;
            j.YCbCrToRgbKernel = stbi__YCbCr_to_RGB_row;
            j.ResampleRowHv2Kernel = stbi__resample_row_hv_2;
        }

        public static void stbi__cleanup_jpeg(StbiJpeg j)
        {
            stbi__free_jpeg_components(j, j.S.ImgN, 0);
        }

        public static byte* load_jpeg_image(StbiJpeg z, int* outX, int* outY, int* comp, int reqComp)
        {
            var n = 0;
            var decode_n = 0;
            var is_rgb = 0;
            z.S.ImgN = 0;
            if (reqComp < 0 || reqComp > 4)
                return (byte*)(ulong)(stbi__err("bad req_comp") != 0 ? (byte*)null : null);
            if (stbi__decode_jpeg_image(z) == 0)
            {
                stbi__cleanup_jpeg(z);
                return null;
            }

            n = reqComp != 0 ? reqComp : z.S.ImgN >= 3 ? 3 : 1;
            is_rgb = z.S.ImgN == 3 && (z.Rgb == 3 || z.App14ColorTransform == 0 && z.Jfif == 0) ? 1 : 0;
            if (z.S.ImgN == 3 && n < 3 && is_rgb == 0)
                decode_n = 1;
            else
                decode_n = z.S.ImgN;
            {
                var k = 0;
                uint i = 0;
                uint j = 0;
                byte* output;
                var coutput = stackalloc byte*[4];
                coutput[0] = null;
                coutput[1] = null;
                coutput[2] = null;
                coutput[3] = null;
                var res_comp = new StbiResample[4];
                for (var kkk = 0; kkk < res_comp.Length; ++kkk)
                    res_comp[kkk] = new StbiResample();
                for (k = 0; k < decode_n; ++k)
                {
                    var r = res_comp[k];
                    z.ImgComp[k].linebuf = (byte*)stbi__malloc(z.S.ImgX + 3);
                    if (z.ImgComp[k].linebuf == null)
                    {
                        stbi__cleanup_jpeg(z);
                        return (byte*)(ulong)(stbi__err("outofmem") != 0 ? (byte*)null : null);
                    }

                    r.Hs = z.ImgHMax / z.ImgComp[k].h;
                    r.Vs = z.ImgVMax / z.ImgComp[k].v;
                    r.Ystep = r.Vs >> 1;
                    r.WLores = (int)((z.S.ImgX + r.Hs - 1) / r.Hs);
                    r.Ypos = 0;
                    r.Line0 = r.Line1 = z.ImgComp[k].data;
                    if (r.Hs == 1 && r.Vs == 1)
                        r.Resample = resample_row_1;
                    else if (r.Hs == 1 && r.Vs == 2)
                        r.Resample = stbi__resample_row_v_2;
                    else if (r.Hs == 2 && r.Vs == 1)
                        r.Resample = stbi__resample_row_h_2;
                    else if (r.Hs == 2 && r.Vs == 2)
                        r.Resample = z.ResampleRowHv2Kernel;
                    else
                        r.Resample = stbi__resample_row_generic;
                }

                output = (byte*)stbi__malloc_mad3(n, (int)z.S.ImgX, (int)z.S.ImgY, 1);
                if (output == null)
                {
                    stbi__cleanup_jpeg(z);
                    return (byte*)(ulong)(stbi__err("outofmem") != 0 ? (byte*)null : null);
                }

                for (j = (uint)0; j < z.S.ImgY; ++j)
                {
                    var out_ = output + n * z.S.ImgX * j;
                    for (k = 0; k < decode_n; ++k)
                    {
                        var r = res_comp[k];
                        var y_bot = r.Ystep >= r.Vs >> 1 ? 1 : 0;
                        coutput[k] = r.Resample(z.ImgComp[k].linebuf, y_bot != 0 ? r.Line1 : r.Line0,
                            y_bot != 0 ? r.Line0 : r.Line1, r.WLores, r.Hs);
                        if (++r.Ystep >= r.Vs)
                        {
                            r.Ystep = 0;
                            r.Line0 = r.Line1;
                            if (++r.Ypos < z.ImgComp[k].y)
                                r.Line1 += z.ImgComp[k].w2;
                        }
                    }

                    if (n >= 3)
                    {
                        var y = coutput[0];
                        if (z.S.ImgN == 3)
                        {
                            if (is_rgb != 0)
                                for (i = (uint)0; i < z.S.ImgX; ++i)
                                {
                                    out_[0] = y[i];
                                    out_[1] = coutput[1][i];
                                    out_[2] = coutput[2][i];
                                    out_[3] = 255;
                                    out_ += n;
                                }
                            else
                                z.YCbCrToRgbKernel(out_, y, coutput[1], coutput[2], (int)z.S.ImgX, n);
                        }
                        else if (z.S.ImgN == 4)
                        {
                            if (z.App14ColorTransform == 0)
                            {
                                for (i = (uint)0; i < z.S.ImgX; ++i)
                                {
                                    var m = coutput[3][i];
                                    out_[0] = stbi__blinn_8x8(coutput[0][i], m);
                                    out_[1] = stbi__blinn_8x8(coutput[1][i], m);
                                    out_[2] = stbi__blinn_8x8(coutput[2][i], m);
                                    out_[3] = 255;
                                    out_ += n;
                                }
                            }
                            else if (z.App14ColorTransform == 2)
                            {
                                z.YCbCrToRgbKernel(out_, y, coutput[1], coutput[2], (int)z.S.ImgX, n);
                                for (i = (uint)0; i < z.S.ImgX; ++i)
                                {
                                    var m = coutput[3][i];
                                    out_[0] = stbi__blinn_8x8((byte)(255 - out_[0]), m);
                                    out_[1] = stbi__blinn_8x8((byte)(255 - out_[1]), m);
                                    out_[2] = stbi__blinn_8x8((byte)(255 - out_[2]), m);
                                    out_ += n;
                                }
                            }
                            else
                            {
                                z.YCbCrToRgbKernel(out_, y, coutput[1], coutput[2], (int)z.S.ImgX, n);
                            }
                        }
                        else
                        {
                            for (i = (uint)0; i < z.S.ImgX; ++i)
                            {
                                out_[0] = out_[1] = out_[2] = y[i];
                                out_[3] = 255;
                                out_ += n;
                            }
                        }
                    }
                    else
                    {
                        if (is_rgb != 0)
                        {
                            if (n == 1)
                                for (i = (uint)0; i < z.S.ImgX; ++i)
                                    *out_++ = stbi__compute_y(coutput[0][i], coutput[1][i], coutput[2][i]);
                            else
                                for (i = (uint)0; i < z.S.ImgX; ++i, out_ += 2)
                                {
                                    out_[0] = stbi__compute_y(coutput[0][i], coutput[1][i], coutput[2][i]);
                                    out_[1] = 255;
                                }
                        }
                        else if (z.S.ImgN == 4 && z.App14ColorTransform == 0)
                        {
                            for (i = (uint)0; i < z.S.ImgX; ++i)
                            {
                                var m = coutput[3][i];
                                var r = stbi__blinn_8x8(coutput[0][i], m);
                                var g = stbi__blinn_8x8(coutput[1][i], m);
                                var b = stbi__blinn_8x8(coutput[2][i], m);
                                out_[0] = stbi__compute_y(r, g, b);
                                out_[1] = 255;
                                out_ += n;
                            }
                        }
                        else if (z.S.ImgN == 4 && z.App14ColorTransform == 2)
                        {
                            for (i = (uint)0; i < z.S.ImgX; ++i)
                            {
                                out_[0] = stbi__blinn_8x8((byte)(255 - coutput[0][i]), coutput[3][i]);
                                out_[1] = 255;
                                out_ += n;
                            }
                        }
                        else
                        {
                            var y = coutput[0];
                            if (n == 1)
                                for (i = (uint)0; i < z.S.ImgX; ++i)
                                    out_[i] = y[i];
                            else
                                for (i = (uint)0; i < z.S.ImgX; ++i)
                                {
                                    *out_++ = y[i];
                                    *out_++ = 255;
                                }
                        }
                    }
                }

                stbi__cleanup_jpeg(z);
                *outX = (int)z.S.ImgX;
                *outY = (int)z.S.ImgY;
                if (comp != null)
                    *comp = z.S.ImgN >= 3 ? 3 : 1;
                return output;
            }
        }

        public static void* stbi__jpeg_load(StbiContext s, int* x, int* y, int* comp, int reqComp,
            StbiResultInfo* ri)
        {
            byte* result;
            var j = new StbiJpeg();
            j.S = s;
            stbi__setup_jpeg(j);
            result = load_jpeg_image(j, x, y, comp, reqComp);

            return result;
        }

        public static int stbi__jpeg_test(StbiContext s)
        {
            var r = 0;
            var j = new StbiJpeg();
            j.S = s;
            stbi__setup_jpeg(j);
            r = stbi__decode_jpeg_header(j, STBI_SCAN_TYPE);
            stbi__rewind(s);

            return r;
        }

        public static int stbi__jpeg_info_raw(StbiJpeg j, int* x, int* y, int* comp)
        {
            if (stbi__decode_jpeg_header(j, STBI_SCAN_HEADER) == 0)
            {
                stbi__rewind(j.S);
                return 0;
            }

            if (x != null)
                *x = (int)j.S.ImgX;
            if (y != null)
                *y = (int)j.S.ImgY;
            if (comp != null)
                *comp = j.S.ImgN >= 3 ? 3 : 1;
            return 1;
        }

        public static int stbi__jpeg_info(StbiContext s, int* x, int* y, int* comp)
        {
            var result = 0;
            var j = new StbiJpeg();
            j.S = s;
            result = stbi__jpeg_info_raw(j, x, y, comp);

            return result;
        }

        public class StbiHuffman
        {
            public ushort[] Code = new ushort[256];
            public int[] Delta = new int[17];
            public byte[] Fast = new byte[1 << 9];
            public uint[] Maxcode = new uint[18];
            public byte[] Size = new byte[257];
            public byte[] Values = new byte[256];
        }
    }

    #pragma warning restore CA2014
}
