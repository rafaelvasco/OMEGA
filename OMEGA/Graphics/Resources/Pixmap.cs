using STB;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OMEGA
{
    public class Pixmap : Resource
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public byte[] Data => pixel_data;

        public int SizeBytes { get; }

        public int Stride => Width * 4;

        private byte[] pixel_data;

        internal Pixmap(byte[] src_data, int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.SizeBytes = src_data.Length;
            this.pixel_data = new byte[src_data.Length];
            Unsafe.CopyBlockUnaligned(ref pixel_data[0], ref src_data[0], (uint)SizeBytes);

            ConvertToEngineRepresentation();
        }

        internal Pixmap(int width, int height, Color fill_color = default)
        {
            this.Width = width;
            this.Height = height;
            this.SizeBytes = width * height;

            int length = width * height * 4;

            pixel_data = new byte[length];

            Blitter.Begin(this);

            Blitter.Fill(fill_color);

            Blitter.End();
        }

        public static Pixmap Create(int width, int height, Color fill_color)
        {
            var pixmap = new Pixmap(width, height, fill_color);

            int id = Engine.Content.RegisterRuntimeLoaded(pixmap);

            pixmap.Id = $"Pixmap({id}) [{width},{height}]";

            return pixmap;
        }

        protected override void FreeManaged()
        {
            pixel_data = null;
        }

        public void SaveToFile(string path)
        {
            using var stream = File.OpenWrite(path);

            var image_writer = new ImageWriter();
            image_writer.WritePng(pixel_data, Width, Height, ColorComponents.RedGreenBlueAlpha, stream);
        }

        private unsafe void ConvertToExportRepresentation()
        {
            var pd = pixel_data;

            fixed (byte* p = &MemoryMarshal.GetArrayDataReference(pd))
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

        private unsafe void ConvertToEngineRepresentation(bool premultiply_alpha = false)
        {
            var pd = pixel_data;

            fixed (byte* p = &MemoryMarshal.GetArrayDataReference(pd))
            {
                var len = pd.Length - 4;
                for (int i = 0; i <= len; i += 4)
                {
                    byte r = *(p + i);
                    byte g = *(p + i + 1);
                    byte b = *(p + i + 2);
                    byte a = *(p + i + 3);

                    if (!premultiply_alpha)
                    {
                        *(p + i) = b;
                        *(p + i + 1) = g;
                        *(p + i + 2) = r;
                    }
                    else
                    {
                        *(p + i) = (byte)((b * a) / 255);
                        *(p + i + 1) = (byte)((g * a) / 255);
                        *(p + i + 2) = (byte)(r * a / 255);
                    }

                    *(p + i + 3) = a;
                }
            }
        }
    }
}
