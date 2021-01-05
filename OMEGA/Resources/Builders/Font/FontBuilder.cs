using System;

namespace OMEGA
{
    public struct FontBuildParams
    {
        public string Id;
        public string Path;
        public int Size;
        public int PaddingUp;
        public int PaddingDown;
        public int PaddingLeft;
        public int PaddingRight;
        public int CharRangeLevel;
    }

    public static class FontBuilder
    {
        public static FontData Build(FontBuildParams @params)
        {
            var compile_params = new FontCompileParams() {
                CharRangeLevel = @params.CharRangeLevel,
                FontFilePath = ResourceLoader.GetFullResourcePath(@params.Path),
                FontName = @params.Id,
                FontSize = @params.Size,
                SpacingH = 1,
                SpacingV = 1,
                PaddingUp = @params.PaddingUp,
                PaddingDown = @params.PaddingDown,
                PaddingLeft = @params.PaddingLeft,
                PaddingRight = @params.PaddingRight
            };     

            Console.WriteLine($"Compiling Font: {@params.Id}, Size: {@params.Size}");

            var compile_result = FontCompiler.Compile(compile_params);

            var font_data = new FontData()
            {
                Id = @params.Id,
                FontSheet = new TextureData()
                {
                    Id = @params.Id + "_texture",
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
