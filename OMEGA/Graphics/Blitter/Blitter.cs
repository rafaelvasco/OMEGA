using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OMEGA
{
    public static unsafe class Blitter
    {
        private static byte[] pixels;
        private static int surface_w;
        private static int surface_h;
        private static bool ready = false;

        public static void Begin(byte[] pixels_array, int w, int h)
        {
            if (ready)
            {
                throw new Exception("Blitter: Dangling Begin Call");
            }


            pixels = pixels_array;
            surface_w = w;
            surface_h = h;
            ready = true;
        }

        public static void Begin(Pixmap pixmap)
        {
            if (ready)
            {
                throw new Exception("Blitter: Dangling Begin Call");
            }

            pixels = pixmap.Data;
            surface_w = pixmap.Width;
            surface_h = pixmap.Height;
            ready = true;
        }

        public static void End()
        {
            pixels = null;
            surface_w = 0;
            surface_h = 0;
            ready = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ClampAndWarp(ref int px, ref int py, int pw, int ph)
        {
            while (px < 0)
            {
                px += pw;
            }

            px %= pw;

            while (py < 0)
            {
                py += ph;
            }

            py %= ph;
        }

        public static void Fill(Color color)
        {
            if (!ready)
            {
                return;
            }

            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            byte a = color.A;

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(pixels))
            {
                var len = pixels.Length - 4;
                for (int i = 0; i <= len; i += 4)
                {
                    *(ptr + i) = b;
                    *(ptr + i + 1) = g;
                    *(ptr + i + 2) = r;
                    *(ptr + i + 3) = a;
                }
            }
        }
        
        public static void Point(int x, int y, Color color)
        {
            if (!ready)
            {
                return;
            }

            int pw = surface_w;
            int ph = surface_h;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            byte a = color.A;

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(pixels))
            {
                ClampAndWarp(ref x, ref y, pw, ph);

                byte* ptr_idx = ptr + (x + y * pw) * 4;

                *(ptr_idx) = b;
                *(ptr_idx + 1) = g;
                *(ptr_idx + 2) = r;
                *(ptr_idx + 3) = a;
            }
        }

        public static void Rect(int x, int y, int w, int h, Color color)
        {
            if (!ready)
            {
                return;
            }

            int pw = surface_w;
            int ph = surface_h;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            byte a = color.A;

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(pixels))
            {
                for (int px = 0; px < w; ++px)
                {
                    for (int py = 0; py < h; ++py)
                    {
                        int blit_x = px + x;
                        int blit_y = py + y;

                        ClampAndWarp(ref blit_x, ref blit_y, pw, ph);

                        byte* ptr_idx = ptr + (blit_x + blit_y * pw) * 4;

                        *(ptr_idx) = b;
                        *(ptr_idx + 1) = g;
                        *(ptr_idx + 2) = r;
                        *(ptr_idx + 3) = a;
                    }
                }
            }
        }

        public static void ColorAdd(byte r, byte g, byte b, byte a)
        {
            if (!ready)
            {
                return;
            }

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(pixels))
            {
                for (int i = 0; i < pixels.Length / 4; ++i)
                {
                    byte* ptr_idx = ptr + (i * 4);

                    var sb = (*(ptr_idx) + b);
                    var sg = (*(ptr_idx + 1) + g);
                    var sr = (*(ptr_idx + 2) + r);
                    var sa = (*(ptr_idx + 3) + a);

                    *(ptr_idx) = (byte)Calc.Clamp(sb, 0, 255);
                    *(ptr_idx + 1) = (byte)Calc.Clamp(sg, 0, 255);
                    *(ptr_idx + 2) = (byte)Calc.Clamp(sr, 0, 255);
                    *(ptr_idx + 3) = (byte)Calc.Clamp(sa, 0, 255);
                }
            }
        }

        public static void ColorMult(float r, float g, float b, float a)
        {
            if (!ready)
            {
                return;
            }

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(pixels))
            {
                for (int i = 0; i < pixels.Length / 4; ++i)
                {
                    byte* ptr_idx = ptr + (i * 4);

                    var sb = (*(ptr_idx) * b);
                    var sg = (*(ptr_idx + 1) * g);
                    var sr = (*(ptr_idx + 2) * r);
                    var sa = (*(ptr_idx + 3) * a);

                    *(ptr_idx) = (byte)Calc.Clamp(sb, 0, 255);
                    *(ptr_idx + 1) = (byte)Calc.Clamp(sg, 0, 255);
                    *(ptr_idx + 2) = (byte)Calc.Clamp(sr, 0, 255);
                    *(ptr_idx + 3) = (byte)Calc.Clamp(sa, 0, 255);
                }
            }
        }

        public static void PixelShift(int shift_x, int shift_y)
        {
            if (!ready)
            {
                return;
            }

            Span<byte> copy = stackalloc byte[pixels.Length];
            Unsafe.CopyBlockUnaligned(ref copy[0], ref pixels[0], (uint)pixels.Length);

            int pw = surface_w;
            int ph = surface_h;

            int px = 0;
            int py = 0;

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(pixels))
            fixed (byte* copy_ptr = &MemoryMarshal.GetReference(copy))
            {
                for (int x = 0; x < pw; ++x)
                {
                    px = x - shift_x + pw;
                    while (px < 0)
                    {
                        px += pw;
                    }
                    px %= pw;
                    for (int y = 0; y < ph; ++y)
                    {
                        py = y - shift_y + ph;
                        while (py < 0)
                        {
                            py += ph;
                        }
                        py %= ph;

                        int old_idx = (px + py * pw) * 4;
                        int new_idx = (x + y * pw) * 4;

                        byte* ptr_idx = ptr + new_idx;
                        byte* copy_idx = copy_ptr + old_idx;

                        *(ptr_idx) = *(copy_idx);
                        *(ptr_idx + 1) = *(copy_idx + 1);
                        *(ptr_idx + 2) = *(copy_idx + 2);
                        *(ptr_idx + 3) = *(copy_idx + 3);
                    }
                }
            }
        }

        public static void Blit(
            Pixmap pixmap,
            int x, 
            int y,
            Rect region = default,
            int w=0, 
            int h=0
        )
        {
            if (!ready)
            {
                return;
            }

            if (region.IsEmpty)
            {
                region = OMEGA.Rect.FromBox(0, 0, pixmap.Width, pixmap.Height);
            }

            if (w == 0)
            {
                w = region.Width;
            }

            if (h == 0)
            {
                h = region.Height;
            }

            var factor_w = w / region.Width;
            var factor_h = h / region.Height;

            int pw = surface_w;
            int ph = surface_h;

            int paste_w = pixmap.Width;

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(pixels))
            fixed (byte* paste = &MemoryMarshal.GetArrayDataReference(pixmap.Data))
            {
                for (int px = 0; px < w; ++px)
                {
                    for (int py = 0; py < h; ++py)
                    {
                        int blit_x = px + x;
                        int blit_y = py + y;

                        ClampAndWarp(ref blit_x, ref blit_y, pw, ph);

                        byte* ptr_idx = ptr + (blit_x + blit_y * pw) * 4;
                        byte* src_idx = paste + ( region.X1 + (px / factor_w) + (region.Y1 + (py / factor_h) ) * paste_w) * 4;

                        *(ptr_idx) = *(src_idx);
                        *(ptr_idx + 1) = *(src_idx + 1);
                        *(ptr_idx + 2) = *(src_idx + 2);
                        *(ptr_idx + 3) = *(src_idx + 3);
                    }
                }
            }
        } 

        #region FILTERS

        /// <summary>
        /// Applies a Shadow effect on the Pixmap
        /// </summary>
        /// <param name="dist">The distance of the shadow. Can't be smaller then the shadow's size.</param>
        /// <param name="size">The Size/Blur Level of the shadow. Can't be higher than 10.</param>
        /// <param name="color">Shadow color</param>
        public static void DropShadow(int dist, int size, Color color)
        {
            size = Calc.Clamp(size, 0, 10);

            if (dist < size)
            {
                dist = size;
            }


        }

        #endregion


    }
}
