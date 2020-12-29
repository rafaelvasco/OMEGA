namespace OMEGA
{
    public static partial class ResourceLoader
    {
        public static TextureAtlas LoadAtlas(TextureAtlasData data)
        {
            var pixmap = new Pixmap(data.Data, data.Width, data.Height);

            var texture = new Texture2D(pixmap, false, false);

            var atlas = TextureAtlas.FromAtlas(texture, data.Atlas);

            atlas.Id = data.Id;

            return atlas;
        }
    }
}
