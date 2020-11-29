
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OMEGA
{
    public static partial class ResourceLoader
    {
        public static TextFile LoadTextFile(TextFileData txt_data)
        {
            var text_lines = new string[txt_data.TextData.Length];

            for (int i = 0; i < txt_data.TextData.Length; ++i)
            {
                text_lines[i] = System.Text.Encoding.UTF8.GetString(txt_data.TextData[i]);
            }

            var txt_file = new TextFile(text_lines)
            {
                Id = txt_data.Id
            };

            return txt_file;
        }

        public static TextFile LoadTextFile(string path)
        {
            var txt_data = LoadTextFileData(path);

            return LoadTextFile(txt_data);
        }

        public static TextFileData LoadTextFileData(string relative_path)
        {
            var text = File.ReadAllLines(GetFullResourcePath(relative_path));

            var id = Path.GetFileNameWithoutExtension(relative_path);

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
