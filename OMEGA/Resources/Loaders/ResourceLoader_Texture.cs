
namespace OMEGA
{
    public static partial class ResourceLoader
    {
        public static Texture2D LoadTexture(TextureData texture_data)
        {
            var pixmap = new Pixmap(texture_data.Data, texture_data.Width, texture_data.Height);

            var texture = new Texture2D(pixmap, false, false);
        
            texture.Id = texture_data.Id;

            return texture;
        }

       
    }
}
