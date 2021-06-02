namespace STB
{
    internal unsafe partial class StbImage
    {
        public static int stbi__gif_test_raw(StbiContext s)
        {
            var sz = 0;
            if (stbi__get8(s) != 'G' || stbi__get8(s) != 'I' || stbi__get8(s) != 'F' || stbi__get8(s) != '8')
                return 0;
            sz = stbi__get8(s);
            if (sz != '9' && sz != '7')
                return 0;
            if (stbi__get8(s) != 'a')
                return 0;
            return 1;
        }

        public static int stbi__gif_test(StbiContext s)
        {
            var r = stbi__gif_test_raw(s);
            stbi__rewind(s);
            return r;
        }

        public static int stbi__gif_header(StbiContext s, StbiGif g, int* comp, int isInfo)
        {
            byte version = 0;
            if (stbi__get8(s) != 'G' || stbi__get8(s) != 'I' || stbi__get8(s) != 'F' || stbi__get8(s) != '8')
                return stbi__err("not GIF");
            version = stbi__get8(s);
            if (version != '7' && version != '9')
                return stbi__err("not GIF");
            if (stbi__get8(s) != 'a')
                return stbi__err("not GIF");
            StbiGFailureReason = "";
            g.W = stbi__get16le(s);
            g.H = stbi__get16le(s);
            g.Flags = stbi__get8(s);
            g.Bgindex = stbi__get8(s);
            g.Ratio = stbi__get8(s);
            g.Transparent = -1;
            if (comp != null)
                *comp = 4;
            if (isInfo != 0)
                return 1;
            if ((g.Flags & 0x80) != 0)
                stbi__gif_parse_colortable(s, g.Pal, 2 << (g.Flags & 7), -1);
            return 1;
        }

        public static int stbi__gif_info_raw(StbiContext s, int* x, int* y, int* comp)
        {
            var g = new StbiGif();
            if (stbi__gif_header(s, g, comp, 1) == 0)
            {
                stbi__rewind(s);
                return 0;
            }

            if (x != null)
                *x = g.W;
            if (y != null)
                *y = g.H;

            return 1;
        }

        public static void stbi__out_gif_code(StbiGif g, ushort code)
        {
            byte* p;
            byte* c;
            var idx = 0;
            if (g.Codes[code].prefix >= 0)
                stbi__out_gif_code(g, (ushort)g.Codes[code].prefix);
            if (g.CurY >= g.MaxY)
                return;
            idx = g.CurX + g.CurY;
            p = &g.Out[idx];
            g.History[idx / 4] = 1;
            c = &g.ColorTable[g.Codes[code].suffix * 4];
            if (c[3] > 128)
            {
                p[0] = c[2];
                p[1] = c[1];
                p[2] = c[0];
                p[3] = c[3];
            }

            g.CurX += 4;
            if (g.CurX >= g.MaxX)
            {
                g.CurX = g.StartX;
                g.CurY += g.Step;
                while (g.CurY >= g.MaxY && g.Parse > 0)
                {
                    g.Step = (1 << g.Parse) * g.LineSize;
                    g.CurY = g.StartY + (g.Step >> 1);
                    --g.Parse;
                }
            }
        }

        public static byte* stbi__process_gif_raster(StbiContext s, StbiGif g)
        {
            byte lzw_cs = 0;
            var len = 0;
            var init_code = 0;
            uint first = 0;
            var codesize = 0;
            var codemask = 0;
            var avail = 0;
            var oldcode = 0;
            var bits = 0;
            var valid_bits = 0;
            var clear = 0;
            StbiGifLzw* p;
            lzw_cs = stbi__get8(s);
            if (lzw_cs > 12)
                return null;
            clear = 1 << lzw_cs;
            first = 1;
            codesize = lzw_cs + 1;
            codemask = (1 << codesize) - 1;
            bits = 0;
            valid_bits = 0;
            for (init_code = 0; init_code < clear; init_code++)
            {
                g.Codes[init_code].prefix = -1;
                g.Codes[init_code].first = (byte)init_code;
                g.Codes[init_code].suffix = (byte)init_code;
            }

            avail = clear + 2;
            oldcode = -1;
            len = 0;
            for (; ; )
                if (valid_bits < codesize)
                {
                    if (len == 0)
                    {
                        len = stbi__get8(s);
                        if (len == 0)
                            return g.Out;
                    }

                    --len;
                    bits |= stbi__get8(s) << valid_bits;
                    valid_bits += 8;
                }
                else
                {
                    var code = bits & codemask;
                    bits >>= codesize;
                    valid_bits -= codesize;
                    if (code == clear)
                    {
                        codesize = lzw_cs + 1;
                        codemask = (1 << codesize) - 1;
                        avail = clear + 2;
                        oldcode = -1;
                        first = 0;
                    }
                    else if (code == clear + 1)
                    {
                        stbi__skip(s, len);
                        while ((len = stbi__get8(s)) > 0)
                            stbi__skip(s, len);
                        return g.Out;
                    }
                    else if (code <= avail)
                    {
                        if (first != 0)
                            return (byte*)(ulong)(stbi__err("no clear code") != 0 ? (byte*)null : null);
                        if (oldcode >= 0)
                        {
                            p = g.Codes + avail++;
                            if (avail > 8192)
                                return (byte*)(ulong)(stbi__err("too many codes") != 0 ? (byte*)null : null);
                            p->prefix = (short)oldcode;
                            p->first = g.Codes[oldcode].first;
                            p->suffix = code == avail ? p->first : g.Codes[code].first;
                        }
                        else if (code == avail)
                        {
                            return (byte*)(ulong)(stbi__err("illegal code in raster") != 0 ? (byte*)null : null);
                        }

                        stbi__out_gif_code(g, (ushort)code);
                        if ((avail & codemask) == 0 && avail <= 0x0FFF)
                        {
                            codesize++;
                            codemask = (1 << codesize) - 1;
                        }

                        oldcode = code;
                    }
                    else
                    {
                        return (byte*)(ulong)(stbi__err("illegal code in raster") != 0 ? (byte*)null : null);
                    }
                }
        }

        public static byte* stbi__gif_load_next(StbiContext s, StbiGif g, int* comp, int reqComp, byte* twoBack)
        {
            var dispose = 0;
            var first_frame = 0;
            var pi = 0;
            var pcount = 0;
            first_frame = 0;
            if (g.Out == null)
            {
                if (stbi__gif_header(s, g, comp, 0) == 0)
                    return null;
                if (stbi__mad3sizes_valid(4, g.W, g.H, 0) == 0)
                    return (byte*)(ulong)(stbi__err("too large") != 0 ? (byte*)null : null);
                pcount = g.W * g.H;
                g.Out = (byte*)stbi__malloc((ulong)(4 * pcount));
                g.Background = (byte*)stbi__malloc((ulong)(4 * pcount));
                g.History = (byte*)stbi__malloc((ulong)pcount);
                if (g.Out == null || g.Background == null || g.History == null)
                    return (byte*)(ulong)(stbi__err("outofmem") != 0 ? (byte*)null : null);
                CRuntime.Memset(g.Out, 0x00, (ulong)(4 * pcount));
                CRuntime.Memset(g.Background, 0x00, (ulong)(4 * pcount));
                CRuntime.Memset(g.History, 0x00, (ulong)pcount);
                first_frame = 1;
            }
            else
            {
                dispose = (g.Eflags & 0x1C) >> 2;
                pcount = g.W * g.H;
                if (dispose == 3 && twoBack == null)
                    dispose = 2;
                if (dispose == 3)
                {
                    for (pi = 0; pi < pcount; ++pi)
                        if (g.History[pi] != 0)
                            CRuntime.Memcpy(&g.Out[pi * 4], &twoBack[pi * 4], (ulong)4);
                }
                else if (dispose == 2)
                {
                    for (pi = 0; pi < pcount; ++pi)
                        if (g.History[pi] != 0)
                            CRuntime.Memcpy(&g.Out[pi * 4], &g.Background[pi * 4], (ulong)4);
                }

                CRuntime.Memcpy(g.Background, g.Out, (ulong)(4 * g.W * g.H));
            }

            CRuntime.Memset(g.History, 0x00, (ulong)(g.W * g.H));
            for (; ; )
            {
                var tag = (int)stbi__get8(s);
                switch (tag)
                {
                    case 0x2C:
                        {
                            var x = 0;
                            var y = 0;
                            var w = 0;
                            var h = 0;
                            byte* o;
                            x = stbi__get16le(s);
                            y = stbi__get16le(s);
                            w = stbi__get16le(s);
                            h = stbi__get16le(s);
                            if (x + w > g.W || y + h > g.H)
                                return (byte*)(ulong)(stbi__err("bad Image Descriptor") != 0 ? (byte*)null : null);
                            g.LineSize = g.W * 4;
                            g.StartX = x * 4;
                            g.StartY = y * g.LineSize;
                            g.MaxX = g.StartX + w * 4;
                            g.MaxY = g.StartY + h * g.LineSize;
                            g.CurX = g.StartX;
                            g.CurY = g.StartY;
                            if (w == 0)
                                g.CurY = g.MaxY;
                            g.Lflags = stbi__get8(s);
                            if ((g.Lflags & 0x40) != 0)
                            {
                                g.Step = 8 * g.LineSize;
                                g.Parse = 3;
                            }
                            else
                            {
                                g.Step = g.LineSize;
                                g.Parse = 0;
                            }

                            if ((g.Lflags & 0x80) != 0)
                            {
                                stbi__gif_parse_colortable(s, g.Lpal, 2 << (g.Lflags & 7),
                                    (g.Eflags & 0x01) != 0 ? g.Transparent : -1);
                                g.ColorTable = g.Lpal;
                            }
                            else if ((g.Flags & 0x80) != 0)
                            {
                                g.ColorTable = g.Pal;
                            }
                            else
                            {
                                return (byte*)(ulong)(stbi__err("missing color table") != 0 ? (byte*)null : null);
                            }

                            o = stbi__process_gif_raster(s, g);
                            if (o == null)
                                return null;
                            pcount = g.W * g.H;
                            if (first_frame != 0 && g.Bgindex > 0)
                                for (pi = 0; pi < pcount; ++pi)
                                    if (g.History[pi] == 0)
                                    {
                                        g.Pal[g.Bgindex * 4 + 3] = 255;
                                        CRuntime.Memcpy(&g.Out[pi * 4], &g.Pal[g.Bgindex], (ulong)4);
                                    }

                            return o;
                        }
                    case 0x21:
                        {
                            var len = 0;
                            var ext = (int)stbi__get8(s);
                            if (ext == 0xF9)
                            {
                                len = stbi__get8(s);
                                if (len == 4)
                                {
                                    g.Eflags = stbi__get8(s);
                                    g.Delay = 10 * stbi__get16le(s);
                                    if (g.Transparent >= 0)
                                        g.Pal[g.Transparent * 4 + 3] = 255;
                                    if ((g.Eflags & 0x01) != 0)
                                    {
                                        g.Transparent = stbi__get8(s);
                                        if (g.Transparent >= 0)
                                            g.Pal[g.Transparent * 4 + 3] = 0;
                                    }
                                    else
                                    {
                                        stbi__skip(s, 1);
                                        g.Transparent = -1;
                                    }
                                }
                                else
                                {
                                    stbi__skip(s, len);
                                    break;
                                }
                            }

                            while ((len = stbi__get8(s)) != 0)
                                stbi__skip(s, len);
                            break;
                        }
                    case 0x3B:
                        return null;
                    default:
                        return (byte*)(ulong)(stbi__err("unknown code") != 0 ? (byte*)null : null);
                }
            }
        }

        public static void* stbi__load_gif_main(StbiContext s, int** delays, int* x, int* y, int* z, int* comp,
            int reqComp)
        {
            if (stbi__gif_test(s) != 0)
            {
                var layers = 0;
                byte* u = null;
                byte* out_ = null;
                byte* two_back = null;
                var g = new StbiGif();
                var stride = 0;
                if (delays != null)
                    *delays = null;
                do
                {
                    u = stbi__gif_load_next(s, g, comp, reqComp, two_back);
                    if (u != null)
                    {
                        *x = g.W;
                        *y = g.H;
                        ++layers;
                        stride = g.W * g.H * 4;
                        if (out_ != null)
                        {
                            out_ = (byte*)CRuntime.Realloc(out_, (ulong)(layers * stride));
                            if (delays != null)
                                *delays = (int*)CRuntime.Realloc(*delays, (ulong)(sizeof(int) * layers));
                        }
                        else
                        {
                            out_ = (byte*)stbi__malloc((ulong)(layers * stride));
                            if (delays != null)
                                *delays = (int*)stbi__malloc((ulong)(layers * sizeof(int)));
                        }

                        CRuntime.Memcpy(out_ + (layers - 1) * stride, u, (ulong)stride);
                        if (layers >= 2)
                            two_back = out_ - 2 * stride;
                        if (delays != null)
                            (*delays)[layers - 1U] = g.Delay;
                    }
                } while (u != null);

                CRuntime.Free(g.Out);
                CRuntime.Free(g.History);
                CRuntime.Free(g.Background);
                if (reqComp != 0 && reqComp != 4)
                    out_ = stbi__convert_format(out_, 4, reqComp, (uint)(layers * g.W), (uint)g.H);
                *z = layers;
                return out_;
            }

            return (byte*)(ulong)(stbi__err("not GIF") != 0 ? (byte*)null : null);
        }

        public static void* stbi__gif_load(StbiContext s, int* x, int* y, int* comp, int reqComp,
            StbiResultInfo* ri)
        {
            byte* u = null;
            var g = new StbiGif();

            u = stbi__gif_load_next(s, g, comp, reqComp, null);
            if (u != null)
            {
                *x = g.W;
                *y = g.H;
                if (reqComp != 0 && reqComp != 4)
                    u = stbi__convert_format(u, 4, reqComp, (uint)g.W, (uint)g.H);
            }
            else if (g.Out != null)
            {
                CRuntime.Free(g.Out);
            }

            CRuntime.Free(g.History);
            CRuntime.Free(g.Background);
            return u;
        }

        public static int stbi__gif_info(StbiContext s, int* x, int* y, int* comp)
        {
            return stbi__gif_info_raw(s, x, y, comp);
        }
    }
}
