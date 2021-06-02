
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OMEGA
{
    public static partial class ResourceLoader
    {
        public static TextFile LoadTextFile(TextFileData txtData)
        {
            var text_lines = new string[txtData.TextData.Length];

            for (int i = 0; i < txtData.TextData.Length; ++i)
            {
                text_lines[i] = System.Text.Encoding.UTF8.GetString(txtData.TextData[i]);
            }

            var txt_file = new TextFile(text_lines)
            {
                Id = txtData.Id
            };

            return txt_file;
        }

        
    }
}
