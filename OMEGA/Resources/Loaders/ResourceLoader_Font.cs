
namespace OMEGA
{
    public static partial class ResourceLoader
    {
        public static TextureFont LoadFont(FontData fontData)
        {
            var texture = LoadTexture(fontData.FontSheet);

            var font = new TextureFont(texture, fontData)
            {
                Id = fontData.Id
            };

            return font;
        }
    }
}
