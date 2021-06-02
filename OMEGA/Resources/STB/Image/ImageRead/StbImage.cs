using System;
using System.IO;
using System.Runtime.InteropServices;

namespace STB
{
    internal static unsafe partial class StbImage
    {
        public static string LastError;

        public const int STBI_ZFAST_BITS = 9;

        public delegate void IdctBlockKernel(byte* output, int outStride, short* data);

        public delegate void YCbCrToRgbKernel(
            byte* output, byte* y, byte* pcb, byte* pcr, int count, int step);

        public delegate byte* Resampler(byte* a, byte* b, byte* c, int d, int e);

        public static string StbiGFailureReason;
        public static int StbiVerticallyFlipOnLoad;

        public class StbiContext
        {
            private readonly Stream _stream;

            public byte[] TempBuffer;
            public int ImgN = 0;
            public int ImgOutN = 0;
            public uint ImgX = 0;
            public uint ImgY = 0;

            public StbiContext(Stream stream)
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");

                _stream = stream;
            }

            public Stream Stream
            {
                get
                {
                    return _stream;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ImgComp
        {
            public int id;
            public int h, v;
            public int tq;
            public int hd, ha;
            public int dc_pred;

            public int x, y, w2, h2;
            public byte* data;
            public void* raw_data;
            public void* raw_coeff;
            public byte* linebuf;
            public short* coeff; // progressive only
            public int coeff_w, coeff_h; // number of 8x8 coefficient blocks
        }

        public class StbiJpeg
        {
            public readonly ushort[][] Dequant;

            public readonly short[][] FastAc;
            public readonly StbiHuffman[] HuffAc = new StbiHuffman[4];
            public readonly StbiHuffman[] HuffDc = new StbiHuffman[4];
            public int App14ColorTransform; // Adobe APP14 tag
            public int CodeBits; // number of valid bits

            public uint CodeBuffer; // jpeg entropy-coded buffer
            public int EobRun;

            // kernels
            public IdctBlockKernel IdctBlockKernel;

            // definition of jpeg image component
            public ImgComp[] ImgComp = new ImgComp[4];

            // sizes for components, interleaved MCUs
            public int ImgHMax, ImgVMax;
            public int ImgMcuW, ImgMcuH;
            public int ImgMcuX, ImgMcuY;
            public int Jfif;
            public byte Marker; // marker seen while filling entropy buffer
            public int Nomore; // flag if we saw a marker so must stop
            public int[] Order = new int[4];

            public int Progressive;
            public Resampler ResampleRowHv2Kernel;
            public int RestartInterval, Todo;
            public int Rgb;
            public StbiContext S;

            public int ScanN;
            public int SpecEnd;
            public int SpecStart;
            public int SuccHigh;
            public int SuccLow;
            public YCbCrToRgbKernel YCbCrToRgbKernel;

            public StbiJpeg()
            {
                for (var i = 0; i < 4; ++i)
                {
                    HuffAc[i] = new StbiHuffman();
                    HuffDc[i] = new StbiHuffman();
                }

                for (var i = 0; i < ImgComp.Length; ++i)
                    ImgComp[i] = new ImgComp();

                FastAc = new short[4][];
                for (var i = 0; i < FastAc.Length; ++i)
                    FastAc[i] = new short[1 << STBI_ZFAST_BITS];

                Dequant = new ushort[4][];
                for (var i = 0; i < Dequant.Length; ++i)
                    Dequant[i] = new ushort[64];
            }
        }

        public class StbiResample
        {
            public int Hs;
            public byte* Line0;
            public byte* Line1;
            public Resampler Resample;
            public int Vs;
            public int WLores;
            public int Ypos;
            public int Ystep;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct StbiGifLzw
        {
            public short prefix;
            public byte first;
            public byte suffix;
        }

        public class StbiGif : IDisposable
        {
            public byte* Out;
            public byte* Background;
            public int Bgindex;
            public StbiGifLzw* Codes = (StbiGifLzw*)stbi__malloc(8192 * sizeof(StbiGifLzw));
            public byte* ColorTable;
            public int CurX;
            public int CurY;
            public int Delay;
            public int Eflags;
            public int Flags;
            public int H;
            public byte* History;
            public int Lflags;
            public int LineSize;
            public byte* Lpal;
            public int MaxX;
            public int MaxY;
            public byte* Pal;
            public int Parse;
            public int Ratio;
            public int StartX;
            public int StartY;
            public int Step;
            public int Transparent;
            public int W;

            public StbiGif()
            {
                Pal = (byte*)stbi__malloc(256 * 4 * sizeof(byte));
                Lpal = (byte*)stbi__malloc(256 * 4 * sizeof(byte));
            }

            public void Dispose()
            {
                if (Pal != null)
                {
                    CRuntime.Free(Pal);
                    Pal = null;
                }

                if (Lpal != null)
                {
                    CRuntime.Free(Lpal);
                    Lpal = null;
                }

                if (Codes != null)
                {
                    CRuntime.Free(Codes);
                    Codes = null;
                }
            }

            ~StbiGif()
            {
                Dispose();
            }
        }

        private static void* stbi__malloc(int size)
        {
            return CRuntime.Malloc((ulong)size);
        }

        private static void* stbi__malloc(ulong size)
        {
            return stbi__malloc((int)size);
        }

        private static int stbi__err(string str)
        {
            LastError = str;
            return 0;
        }

        public static void stbi__gif_parse_colortable(StbiContext s, byte* pal, int numEntries, int transp)
        {
            int i;
            for (i = 0; i < numEntries; ++i)
            {
                pal[i * 4 + 2] = stbi__get8(s);
                pal[i * 4 + 1] = stbi__get8(s);
                pal[i * 4] = stbi__get8(s);
                pal[i * 4 + 3] = (byte)(transp == i ? 0 : 255);
            }
        }

        public static byte stbi__get8(StbiContext s)
        {
            var b = s.Stream.ReadByte();
            if (b == -1)
            {
                return 0;
            }

            return (byte)b;
        }

        public static void stbi__skip(StbiContext s, int skip)
        {
            s.Stream.Seek(skip, SeekOrigin.Current);
        }

        public static void stbi__rewind(StbiContext s)
        {
            s.Stream.Seek(0, SeekOrigin.Begin);
        }

        public static int stbi__at_eof(StbiContext s)
        {
            return s.Stream.Position == s.Stream.Length ? 1 : 0;
        }

        public static int stbi__getn(StbiContext s, byte* buf, int size)
        {
            if (s.TempBuffer == null ||
                s.TempBuffer.Length < size)
                s.TempBuffer = new byte[size * 2];

            var result = s.Stream.Read(s.TempBuffer, 0, size);
            Marshal.Copy(s.TempBuffer, 0, new IntPtr(buf), result);

            return result;
        }
    }
}
