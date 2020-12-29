using System;
using System.IO;

namespace OMEGA
{
    public static class FontBuilder
    {
        public static FontData Build(string id, string relative_path, int size, string[] char_ranges)
        {
            var compile_params = new FontCompileParams() {
                CharRangesInput = char_ranges,
                FontFilePath = ResourceLoader.GetFullResourcePath(relative_path),
                FontName = id,
                FontSize = size,
                SpacingH = 1,
                SpacingV = 1
            };     

            Console.WriteLine($"Compiling Font: {id}, Size: {size}");

            var compile_result = FontCompiler.Compile(compile_params);

            var font_data = new FontData()
            {
                Id = id,
                FontSheet = new TextureData()
                {
                    Id = id + "_texture",
                    Data = compile_result.FontImageData,
                    Width = compile_result.FontImageWidth,
                    Height = compile_result.FontImageHeight,
                },
                
            };

            FontDescrParser.ParseAndFillData(font_data, compile_result.FontDescrData);

            return font_data;
        }
    }
}
