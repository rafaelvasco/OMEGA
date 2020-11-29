
using STB;
using System.IO;

namespace OMEGA
{
    public static partial class ResourceLoader
    {
        public static Texture2D LoadTexture(ImageData image_data)
        {
            var pixmap = new Pixmap(image_data.Data, image_data.Width, image_data.Height);

            var texture = new Texture2D(pixmap, false, false);
        
            texture.Id = image_data.Id;

            return texture;
        }

        public static ImageData LoadImageData(string id, string relative_path)
        {
            using var file = File.OpenRead(GetFullResourcePath(relative_path));

            var image = ImageResult.FromStream(file, ColorComponents.RedGreenBlueAlpha);

            var image_data = new ImageData()
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
