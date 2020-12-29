using System;
using System.IO;

namespace OMEGA
{
    internal static class FontDescrParser
    {
        public static void ParseAndFillData(FontData font_data, byte[] font_descr_content)
        {
            using var stream = new MemoryStream(font_descr_content);

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var buffer = new byte[1024];

            stream.Read(buffer, 0, 4);

            if (buffer[0] != 66 || buffer[1] != 77 || buffer[2] != 70)
            {
                throw new InvalidDataException("Source steam does not contain BMFont data.");
            }

            if (buffer[3] != 3)
            {
                throw new InvalidDataException("Only BMFont version 3 format data is supported.");
            }

            // Following the first four bytes is a series of blocks with information. Each block starts with a one byte block type identifier, followed by a 4 byte integer that gives the size of the block, not including the block type identifier and the size value.
            while (stream.Read(buffer, 0, 5) != 0)
            {
                byte block_type;
                int block_size;

                block_type = buffer[0];

                block_size = MemWordHelpers.MakeDWordLittleEndian(buffer, 1);
                if (block_size > buffer.Length)
                {
                    buffer = new byte[block_size];
                }

                if (stream.Read(buffer, 0, block_size) != block_size)
                {
                    throw new InvalidDataException("Failed to read enough data to fill block.");
                }

                switch (block_type)
                {
                    case 1: // Block type 1: info
                        LoadInfoBlock(buffer, font_data);
                        break;

                    case 2: // Block type 2: common
                        LoadCommonBlock(buffer, font_data);
                        break;

                    case 3: // Block type 3: pages

                        break;
                    case 4: // Block type 4: chars
                        LoadCharsBlock(buffer, block_size, font_data);
                        break;

                    case 5: // Block type 5: kerning pairs
                        //LoadKerningsBlock(buffer, block_size);
                        break;

                    default: throw new InvalidDataException("Block type " + block_type + " is not a valid BMFont block");
                }
            }
        }


        private static void LoadInfoBlock(byte[] buffer, FontData font_data)
        {
            font_data.Size = Calc.Abs(MemWordHelpers.MakeWordLittleEndian(buffer, 0));
        }

        private static void LoadCommonBlock(byte[] buffer, FontData font_data)
        {
            font_data.LineHeight = MemWordHelpers.MakeWordLittleEndian(buffer, 0);
        }

        private static void LoadCharsBlock(byte[] buffer, int block_size, FontData font_data)
        {
            const int char_line_block_size = 20;
            int char_count = block_size/char_line_block_size;

            font_data.Chars = new char[char_count];
            font_data.GlyphOffsets = new SVec2[char_count];
            font_data.GlyphRects = new SRect[char_count];
            font_data.GlyphXAdvances = new float[char_count];

            for (int i = 0; i < char_count; ++i)
            {
                int start = i * char_line_block_size;

                int char_index = MemWordHelpers.MakeDWordLittleEndian(buffer, start);

                font_data.Chars[i] = char_index >= 0 ? (char)char_index : '\0';
                font_data.GlyphRects[i] = new SRect(
                    MemWordHelpers.MakeWordLittleEndian(buffer, start + 4),
                    MemWordHelpers.MakeWordLittleEndian(buffer, start + 6),
                    MemWordHelpers.MakeWordLittleEndian(buffer, start + 8),
                    MemWordHelpers.MakeWordLittleEndian(buffer, start + 10)
                );
                font_data.GlyphOffsets[i] = new SVec2(
                    MemWordHelpers.MakeWordLittleEndian(buffer, start + 12),
                    MemWordHelpers.MakeWordLittleEndian(buffer, start + 14)
                );
                font_data.GlyphXAdvances[i] = MemWordHelpers.MakeWordLittleEndian(buffer, start + 16);
            }
        }
    }
}
