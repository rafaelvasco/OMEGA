
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

        
    }
}
