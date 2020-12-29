using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OMEGA
{
    public static class TextBuilder
    {
        public static TextFileData Build(string id, string relative_path)
        {
            var text = File.ReadAllLines(ResourceLoader.GetFullResourcePath(relative_path));

            var text_file_data = new TextFileData()
            {
                Id = id,
            };

            text = text
                .Where(t => t.Length > 0)
                .ToArray();

            text_file_data.TextData = new byte[text.Length][];

            for (int i = 0; i < text.Length; ++i)
            {
                var line = text[i];

                var bytes = System.Text.Encoding.UTF8.GetBytes(line);

                text_file_data.TextData[i] = new byte[bytes.Length];

                Unsafe.CopyBlockUnaligned(ref text_file_data.TextData[i][0], ref bytes[0], (uint)bytes.Length);
            }

            return text_file_data;
        }
    }
}
