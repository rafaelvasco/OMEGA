
namespace OMEGA
{
    public static partial class ResourceLoader
    {
        public static Texture2D LoadTexture(ImageData image_data)
        {
            var pixmap = new Pixmap(image_data.Data, image_data.Width, image_data.Height);

            Texture2D texture = new Texture2D(pixmap, false, false);

            texture.Id = image_data.Id;

            return texture;
        }

       
    }
}
