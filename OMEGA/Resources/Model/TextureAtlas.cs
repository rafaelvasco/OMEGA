using System.Collections.Generic;

namespace OMEGA
{
    public class TextureAtlas : Resource
    {
        public RectF this[int index] => m_regions[index];

        public RectF this[string name]
        {
            get
            {
                if (m_name_map.TryGetValue(name, out int index))
                {
                    return m_regions[index];
                }

                return default;
            }
        }

        public Texture2D Texture => m_texture;

        public int Count => m_regions.Length;


        private readonly RectF[] m_regions;
        private readonly Texture2D m_texture;
        private Dictionary<string, int> m_name_map;

        public static TextureAtlas FromGrid(Texture2D texture, int rows, int columns)
        {
            int tex_w = texture.Width;
            int tex_h = texture.Height;

            int tile_width = tex_w / columns;
            int tile_height = tex_h / rows;

            var regions = new RectF[rows * columns];

            int index = 0;

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < columns; ++j)
                {
                    int x = j * tile_width;
                    int y = i * tile_height;

                    regions[index++] = RectF.FromBox(x, y, tile_width, tile_height);
                }
            }

            return new TextureAtlas(texture, regions);
        }

        public static TextureAtlas FromAtlas(Texture2D texture, Dictionary<string, SRect> atlas)
        {
            var regions = new RectF[atlas.Count];
            var map = new Dictionary<string, int>();

            var idx = 0;

            foreach (var atlas_pair in atlas)
            {
                regions[idx] = RectF.FromBox(atlas_pair.Value.X, atlas_pair.Value.Y, atlas_pair.Value.W, atlas_pair.Value.H);

                map.Add(atlas_pair.Key, idx);

                idx++;
            }

            var tex_atlas = new TextureAtlas(texture, regions) { m_name_map = map };

            return tex_atlas;

        }

        private TextureAtlas(Texture2D texture, RectF[] regions)
        {
            m_texture = texture;

            m_regions = new RectF[regions.Length];

            for (int i = 0; i < regions.Length; ++i)
            {
                var region = regions[i];

                m_regions[i] = RectF.FromBox(region.X1, region.Y1, region.Width, region.Height);

            }
        }

        protected override void FreeUnmanaged()
        {
            m_texture.Dispose();
        }
    }
}
