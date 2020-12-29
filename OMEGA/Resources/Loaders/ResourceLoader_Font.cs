
namespace OMEGA
{
    public static partial class ResourceLoader
    {
        public static Font LoadFont(FontData font_data)
        {
            var texture = LoadTexture(font_data.FontSheet);

            var font = new Font(texture, font_data);

            font.Id = font_data.Id;

            return font;
        }
    }
}
