using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OMEGA
{
    public static unsafe class Blitter
    {
        private const int AUX_BUFFER_SIZE = 1024 * 1024 * 4;
        private const int AUX_BUFFER_STACK_SIZE = 2;
        private static byte[] m_pixels;
        private static byte[][] m_aux_buffers = new byte[AUX_BUFFER_STACK_SIZE][];
        private static Texture2D m_texture_target;
        private static bool m_blitter_dirty;
        private static int m_aux_buffers_idx = 0;
        private static int m_surface_w;
        private static int m_surface_h;
        private static bool m_ready = false;

        public static void Begin(byte[] pixels_array, int w, int h)
        {
            if (m_ready)
            {
                throw new Exception("Blitter: Dangling Begin Call");
            }


            m_pixels = pixels_array;
            m_surface_w = w;
            m_surface_h = h;
            m_ready = true;
        }

        public static void Begin(Pixmap pixmap)
        {
            if (m_ready)
            {
                throw new Exception("Blitter: Dangling Begin Call");
            }

            m_pixels = pixmap.Data;
            m_surface_w = pixmap.Width;
            m_surface_h = pixmap.Height;
            m_ready = true;
        }

        public static void Begin(Texture2D texture)
        {
            if (texture.Pixmap == null)
            {
                throw new Exception("This texture can't be modified with Blitter.");
            }

            m_texture_target = texture;

            m_pixels = texture.Pixmap.Data;
            m_surface_w = texture.Pixmap.Width;
            m_surface_h = texture.Pixmap.Height;
            m_ready = true;
        }

        public static void End()
        {
            m_pixels = null;
            m_surface_w = 0;
            m_surface_h = 0;
            m_ready = false;

            if (m_texture_target != null && m_blitter_dirty)
            {
                m_texture_target.ReloadPixels();
                m_texture_target = null;
                m_blitter_dirty = false;
            }
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
            if (!m_ready)
            {
                return;
            }

            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            byte a = color.A;

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(m_pixels))
            {
                var len = m_pixels.Length - 4;
                for (int i = 0; i <= len; i += 4)
                {
                    *(ptr + i) = b;
                    *(ptr + i + 1) = g;
                    *(ptr + i + 2) = r;
                    *(ptr + i + 3) = a;
                }
            }

            m_blitter_dirty = true;
        }
        
        public static void Point(int x, int y, Color color)
        {
            if (!m_ready)
            {
                return;
            }

            int pw = m_surface_w;
            int ph = m_surface_h;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            byte a = color.A;

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(m_pixels))
            {
                ClampAndWarp(ref x, ref y, pw, ph);

                byte* ptr_idx = ptr + (x + y * pw) * 4;

                *(ptr_idx) = b;
                *(ptr_idx + 1) = g;
                *(ptr_idx + 2) = r;
                *(ptr_idx + 3) = a;
            }

            m_blitter_dirty = true;
        }

        public static void Rect(int x, int y, int w, int h, Color color)
        {
            if (!m_ready)
            {
                return;
            }

            int pw = m_surface_w;
            int ph = m_surface_h;
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            byte a = color.A;

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(m_pixels))
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

            m_blitter_dirty = true;
        }

        public static void ColorAdd(byte r, byte g, byte b, byte a)
        {
            if (!m_ready)
            {
                return;
            }

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(m_pixels))
            {
                for (int i = 0; i < m_pixels.Length / 4; ++i)
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

            m_blitter_dirty = true;
        }

        public static void ColorMult(float r, float g, float b, float a)
        {
            if (!m_ready)
            {
                return;
            }

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(m_pixels))
            {
                for (int i = 0; i < m_pixels.Length / 4; ++i)
                {
                    byte* ptr_idx = ptr + (i * 4);

                    if (*(ptr_idx + 3) == 0)
                    {
                        continue;
                    }

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

            m_blitter_dirty = true;
        }

        public static void PixelShift(int shift_x, int shift_y)
        {
            if (!m_ready)
            {
                return;
            }

            Span<byte> copy = GetCopy(m_pixels.Length);

            int pw = m_surface_w;
            int ph = m_surface_h;

            int px = 0;
            int py = 0;

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(m_pixels))
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

            m_blitter_dirty = true;
        }

        public static void Blit(
            Span<byte> paste_pixels,
            int paste_pixels_w,
            int paste_pixels_h,
            int x,
            int y,
            Rect region = default,
            int w = 0,
            int h = 0
        )
        {
            if (!m_ready)
            {
                return;
            }

            if (region.IsEmpty)
            {
                region = OMEGA.Rect.FromBox(0, 0, paste_pixels_w, paste_pixels_h);
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

            int pw = m_surface_w;
            int ph = m_surface_h;

            int paste_w = paste_pixels_w;

            fixed (byte* ptr = &MemoryMarshal.GetArrayDataReference(m_pixels))
            fixed (byte* paste = &MemoryMarshal.GetReference(paste_pixels))
            {
                for (int px = 0; px < w; ++px)
                {
                    for (int py = 0; py < h; ++py)
                    {
                        byte* src_idx = paste + (region.X1 + (px / factor_w) + (region.Y1 + (py / factor_h)) * paste_w) * 4;

                        if (*(src_idx + 3) == 0)
                        {
                            continue;
                        }

                        int blit_x = px + x;
                        int blit_y = py + y;

                        ClampAndWarp(ref blit_x, ref blit_y, pw, ph);

                        byte* ptr_idx = ptr + (blit_x + blit_y * pw) * 4;
                        

                        *(ptr_idx) = *(src_idx);
                        *(ptr_idx + 1) = *(src_idx + 1);
                        *(ptr_idx + 2) = *(src_idx + 2);
                        *(ptr_idx + 3) = *(src_idx + 3);
                    }
                }
            }

            m_blitter_dirty = true;
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
            Blit(pixmap.Data, pixmap.Width, pixmap.Height, x, y, region, w, h);
        } 

        #region FILTERS

        /// <summary>
        /// Applies a Shadow effect on the Pixmap
        /// </summary>
        /// <param name="dist">The distance of the shadow. Can't be smaller then the shadow's size.</param>
        /// <param name="size">The Size/Blur Level of the shadow. Can't be higher than 10.</param>
        /// <param name="color">Shadow color</param>
        public static void DropShadow(int offset_x, int offset_y, Color color)
        {

            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            byte a = color.A;

            Span<byte> copy = GetCopy(m_pixels.Length);

            PixelShift(offset_x, offset_y);
            ColorAdd(255, 255, 255, 0);

            ColorMult(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);

            Blit(copy, m_surface_w, m_surface_h, 0, 0);
        }

        private static Span<byte> GetCopy(int length)
        {
            if (length > AUX_BUFFER_SIZE)
            {
                throw new Exception($"Blitter GetCopy: Overflow. Length {length} is bigger than max {AUX_BUFFER_SIZE}");
            }

            if (m_aux_buffers[m_aux_buffers_idx] == null)
            {
                m_aux_buffers[m_aux_buffers_idx] = new byte[AUX_BUFFER_SIZE];
            }

            Unsafe.CopyBlockUnaligned(ref m_aux_buffers[m_aux_buffers_idx][0], ref m_pixels[0], (uint)length);

            var result = new Span<byte>(m_aux_buffers[m_aux_buffers_idx], 0, length);

            m_aux_buffers_idx++;

            if (m_aux_buffers_idx > AUX_BUFFER_STACK_SIZE - 1)
            {
                m_aux_buffers_idx = 0;
            }

            return result;
        }

        #endregion


    }
}
