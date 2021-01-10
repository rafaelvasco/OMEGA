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
        public bool DropShadow;
        public int ShadowOffsetX;
        public int ShadowOffsetY;
        public uint ShadowColor;
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

            if (@params.DropShadow)
            {
                if (Calc.Abs(@params.ShadowOffsetX) > @params.PaddingLeft + @params.PaddingRight)
                {
                    @params.PaddingLeft = Calc.Abs(@params.ShadowOffsetX);
                    @params.PaddingRight = Calc.Abs(@params.ShadowOffsetX);
                }

                if (Calc.Abs(@params.ShadowOffsetY) > @params.PaddingUp + @params.PaddingDown)
                {
                    @params.PaddingUp = Calc.Abs(@params.ShadowOffsetY);
                    @params.PaddingDown = Calc.Abs(@params.ShadowOffsetY);
                }

                compile_params.PaddingUp = @params.PaddingUp;
                compile_params.PaddingDown = @params.PaddingDown;
                compile_params.PaddingLeft = @params.PaddingLeft;
                compile_params.PaddingRight = @params.PaddingRight;
            }

            var compile_result = FontCompiler.Compile(compile_params);

            if (@params.DropShadow)
            {
                Blitter.Begin(compile_result.FontImageData, compile_result.FontImageWidth, compile_result.FontImageHeight);

                Blitter.DropShadow(@params.ShadowOffsetX, @params.ShadowOffsetY, new Color(@params.ShadowColor));

                Blitter.End();
            }

            var font_data = new FontData()
            {
                Id = @params.Id,
                FontSheet = new ImageData()
                {
                    Id = @params.Id + "_texture",
                    Data = compile_result.FontImageData,
                    Width = compile_result.FontImageWidth,
                    Height = compile_result.FontImageHeight
                },
            };

            FontDescrParser.ParseAndFillData(font_data, compile_result.FontDescrData);

            return font_data;
        }
    }
}
