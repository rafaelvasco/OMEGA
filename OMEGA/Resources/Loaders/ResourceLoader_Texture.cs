
namespace OMEGA
{
    public static partial class ResourceLoader
    {
        public static Texture2D LoadTexture(ImageData imageData)
        {
            var pixmap = new Pixmap(imageData.Data, imageData.Width, imageData.Height);

            var texture = new Texture2D(pixmap, false, false)
            {
                Id = imageData.Id
            };

            return texture;
        }

       
    }
}
