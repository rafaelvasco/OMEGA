using STB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace OMEGA
{
    public unsafe class FontCompileParams
    {
        public string FontName;
        public string FontFilePath;
        public int FontSize;
        public int PaddingUp;
        public int PaddingDown;
        public int PaddingLeft;
        public int PaddingRight;
        public int SpacingH;
        public int SpacingV;
        public int TexWidth;
        public int TexHeight;
        public int CharRangeLevel;
        public CharRange[] CharRanges;
    }

    public class FontCompileResult 
    {
        public byte[] FontImageData;
        public int FontImageWidth;
        public int FontImageHeight;
        public byte[] FontDescrData;

        public FontCompileResult(byte[] image, int image_w, int image_h, byte[] desc)
        {
            FontImageData = image;
            FontDescrData = desc;
            FontImageWidth = image_w;
            FontImageHeight = image_h;
        }
    }

    public static class FontCompiler
    {

        private const string COMPILER_PATH = "fontbm.exe";
        private const string FONT_FILE_PARAM = " --font-file ";
        private const string FONT_SIZE_PARAM = " --font-size ";
        private const string PADDING_DOWN_PARAM = " --padding-down ";
        private const string PADDING_UP_PARAM = " --padding-up ";
        private const string PADDING_LEFT_PARAM = " --padding-left ";
        private const string PADDING_RIGHT_PARAM = " --padding-right ";
        private const string SPACING_H_PARAM = " --spacing-horiz ";
        private const string SPACING_V_PARAM = " --spacing-vert ";
        private const string TEX_WIDTH_PARAM = " --texture-width ";
        private const string TEX_HEIGHT_PARAM = " --texture-height ";
        private const string CHAR_RANGES_PARAM = " --chars ";
        private const string OUTPUT_FORMAT_PARAM = " --data-format ";
        private const string OUTPUT_PARAM = " --output ";
        private const string OUTPUT_FORMAT = "bin";
        private const string FONT_DESCR_EXT = ".fnt";
        private const string FONT_IMAGE_EXT = ".png";
        private const string FONT_IMAGE_SUFFIX = "_0";

        private static readonly Dictionary<int, int> m_font_size_tex_size_map = new Dictionary<int, int>()
        {
            { 50, 256 },
            { 100, 512 },
            { 200, 1024 }
        };

        private static readonly Dictionary<string, CharRange> m_char_range_map = new Dictionary<string, CharRange>() 
        {
            { "Latin", CharRange.Latin },
            { "LatinSupplement", CharRange.LatinSupplement }
        };

        private static int GetOptimalTextureSize(int glyph_size, int range_count)
        {
            foreach (var key in m_font_size_tex_size_map.Keys)
            {
                if (glyph_size <= key)
                {
                    return m_font_size_tex_size_map[key] * range_count;
                }
            }

            int last_resort = 2048 * range_count;

            if (last_resort > 4096)
            {
                last_resort = 4096;
            }

            return last_resort;
        }

        public static FontCompileResult Compile(FontCompileParams @params)
        {
            var process_info = new ProcessStartInfo
            {
                UseShellExecute = false,
                FileName = COMPILER_PATH
            };

            var font_file_copy_path = Path.Combine(Path.GetTempPath(), Path.GetFileName(@params.FontFilePath));
            var output_font_image_path = Path.Combine(Path.GetTempPath(), @params.FontName + FONT_IMAGE_SUFFIX + FONT_IMAGE_EXT);
            var output_font_descr_path = Path.Combine(Path.GetTempPath(), @params.FontName + FONT_DESCR_EXT);

            File.Delete(font_file_copy_path);
            File.Delete(output_font_image_path);
            File.Delete(output_font_descr_path);

            File.Copy(@params.FontFilePath, font_file_copy_path);

            @params.FontFilePath = font_file_copy_path;

            if (@params.CharRangeLevel == 0)
            {
                @params.CharRanges = new CharRange[1];
                @params.CharRanges[0] = CharRange.Latin;
            }
            else if (@params.CharRangeLevel == 1)
            {
                @params.CharRanges = new CharRange[2];
                @params.CharRanges[0] = CharRange.Latin;
                @params.CharRanges[1] = CharRange.LatinSupplement;
            }

            int tex_size = GetOptimalTextureSize(@params.FontSize, @params.CharRanges.Length);

            @params.TexWidth = tex_size;
            @params.TexHeight = tex_size;

            process_info.Arguments = BuildArgs(@params);
            process_info.WorkingDirectory = Path.GetTempPath();

            var proc = Process.Start(process_info);

            proc?.WaitForExit();

            var output = proc?.ExitCode ?? -1;

            string result = string.Empty;

            if (output != 0 && output != -1)
            {
                using var reader = proc?.StandardError;
                result = reader?.ReadToEnd();
            }

            if (result.Length > 0)
            {
                throw new Exception($"Error while building font: {result}");
            }

            try
            {
                using var font_image_stream = File.OpenRead(output_font_image_path);
                var font_image = ImageResult.FromStream(font_image_stream, ColorComponents.RedGreenBlueAlpha);

                byte[] font_desc_bytes = File.ReadAllBytes(output_font_descr_path);

                return new FontCompileResult(font_image.Data, @params.TexWidth, @params.TexHeight, font_desc_bytes); 

            }
            catch (Exception e)
            {
                throw new Exception($"Error while processing font compile output: {e.Message}");
            }
        }

        private static string BuildArgs(FontCompileParams @params)
        {
            static string BuildCharRangesParamValue(FontCompileParams @params)
            {
                var ranges = @params.CharRanges;

                var buffer = new StringBuilder();

                for (int i = 0; i < ranges.Length; ++i)
                {
                    if (i > 0)
                    {
                        buffer.Append(',');
                    }

                    var range = ranges[i];

                    buffer.Append(range.Start);
                    buffer.Append('-');
                    buffer.Append(range.End);
                }

                return buffer.ToString();
            }

            var args = new StringBuilder();

            args.Append(FONT_FILE_PARAM);
            args.Append(@params.FontFilePath);

            args.Append(FONT_SIZE_PARAM);
            args.Append(@params.FontSize);

            args.Append(SPACING_H_PARAM);
            args.Append(@params.SpacingH);

            args.Append(SPACING_V_PARAM);
            args.Append(@params.SpacingV);

            args.Append(PADDING_DOWN_PARAM);
            args.Append(@params.PaddingDown);

            args.Append(PADDING_UP_PARAM);
            args.Append(@params.PaddingUp);

            args.Append(PADDING_LEFT_PARAM);
            args.Append(@params.PaddingLeft);

            args.Append(PADDING_RIGHT_PARAM);
            args.Append(@params.PaddingRight);

            args.Append(TEX_WIDTH_PARAM);
            args.Append(@params.TexWidth);

            args.Append(TEX_HEIGHT_PARAM);
            args.Append(@params.TexHeight);

            args.Append(CHAR_RANGES_PARAM);
            args.Append(BuildCharRangesParamValue(@params));

            args.Append(OUTPUT_FORMAT_PARAM);
            args.Append(OUTPUT_FORMAT);

            args.Append(OUTPUT_PARAM);
            args.Append(@params.FontName);

            return args.ToString();
        }
    }
}
