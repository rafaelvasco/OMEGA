using STB;
using System.IO;

namespace OMEGA
{
    public static class TextureBuilder
    {
        public static TextureData Build(string id, string relative_path)
        {
            using var file = File.OpenRead(ResourceLoader.GetFullResourcePath(relative_path));

            var image = ImageResult.FromStream(file, ColorComponents.RedGreenBlueAlpha);

            var image_data = new TextureData()
            {
                Id = id,
                Data = image.Data,
                Width = image.Width,
                Height = image.Height
            };

            return image_data;
        }
    }
}
