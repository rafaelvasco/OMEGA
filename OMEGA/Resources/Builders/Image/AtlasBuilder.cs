using System.Collections.Generic;

namespace OMEGA
{
    public static class AtlasBuilder
    {
        public static TextureAtlasData Build(string id, string relative_path, Dictionary<string, SRect> atlas)
        {
            var texture_data = TextureBuilder.Build(id, relative_path);

            var atlas_data = new TextureAtlasData()
            {
                Data = texture_data.Data,
                Id = id,
                Width = texture_data.Width,
                Height = texture_data.Height,
                Atlas = atlas
            };

            return atlas_data;
        }
    }
}
