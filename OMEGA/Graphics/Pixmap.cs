using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OMEGA
{
    public class Pixmap
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public byte[] Data => pixel_data;

        public int SizeBytes { get; }

        public int Stride => Width * 4;

        private byte[] pixel_data;

        public Pixmap(byte[] src_data, int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.SizeBytes = src_data.Length;
            this.pixel_data = new byte[src_data.Length];
            Unsafe.CopyBlockUnaligned(ref pixel_data[0], ref src_data[0], (uint)SizeBytes);

            SwizzleToBGRA();
        }

        public Pixmap(int width, int height, Color color)
        {
            this.Width = width;
            this.Height = height;
            this.SizeBytes = width * height;

            int length = width * height * 4;

            pixel_data = new byte[length];

            Fill(color);

            SwizzleToBGRA();
        }

        public unsafe void BlitColors(Color[] colors)
        {
            if (colors.Length > this.Width * this.Height)
            {
                return;
            }

            fixed (byte* ptr = &MemoryMarshal.GetReference<byte>(pixel_data))
            {
                int col_idx = 0;
                var len = pixel_data.Length - 4;
                for (int i = 0; i <= len; i += 4)
                {
                    var col = colors[col_idx++];

                    *(ptr + i) = col.B;
                    *(ptr + i + 1) = col.G;
                    *(ptr + i + 2) = col.R;
                    *(ptr + i + 3) = col.A;
                }
            }
        }

        public unsafe void Fill(Color color)
        {
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            byte a = color.A;

            fixed (byte* ptr = &MemoryMarshal.GetReference<byte>(pixel_data))
            {
                var len = pixel_data.Length - 4;
                for (int i = 0; i <= len; i += 4)
                {
                    *(ptr + i) = b;
                    *(ptr + i + 1) = g;
                    *(ptr + i + 2) = r;
                    *(ptr + i + 3) = a;
                }
            }
        }

        private unsafe void SwizzleToRGBA()
        {
            var pd = pixel_data;

            fixed (byte* p = &MemoryMarshal.GetReference<byte>(pd))
            {
                var len = pd.Length - 4;
                for (int i = 0; i <= len; i += 4)
                {
                    byte b = *(p + i);
                    byte g = *(p + i + 1);
                    byte r = *(p + i + 2);
                    byte a = *(p + i + 3);

                    *(p + i) = r;
                    *(p + i + 1) = g;
                    *(p + i + 2) = b;
                    *(p + i + 3) = a;
                }
            }
        }

        private unsafe void SwizzleToBGRA()
        {
            var pd = pixel_data;

            fixed (byte* p = &MemoryMarshal.GetReference<byte>(pd))
            {
                var len = pd.Length - 4;
                for (int i = 0; i <= len; i += 4)
                {
                    byte r = *(p + i);
                    byte g = *(p + i + 1);
                    byte b = *(p + i + 2);
                    byte a = *(p + i + 3);

                    *(p + i) = b;
                    *(p + i + 1) = g;
                    *(p + i + 2) = r;
                    *(p + i + 3) = a;
                }
            }
        }
    }
}
