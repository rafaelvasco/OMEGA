using System.Collections.Generic;

namespace OMEGA
{
    public class TextureAtlas : Resource
    {
        public RectF this[int index] => _mRegions[index];

        public RectF this[string name]
        {
            get
            {
                if (_mNameMap.TryGetValue(name, out int index))
                {
                    return _mRegions[index];
                }

                return default;
            }
        }

        public Texture2D Texture => _mTexture;

        public int Count => _mRegions.Length;


        private readonly RectF[] _mRegions;
        private readonly Texture2D _mTexture;
        private Dictionary<string, int> _mNameMap;

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

            var tex_atlas = new TextureAtlas(texture, regions) { _mNameMap = map };

            return tex_atlas;

        }

        private TextureAtlas(Texture2D texture, RectF[] regions)
        {
            _mTexture = texture;

            _mRegions = new RectF[regions.Length];

            for (int i = 0; i < regions.Length; ++i)
            {
                var region = regions[i];

                _mRegions[i] = RectF.FromBox(region.X1, region.Y1, region.Width, region.Height);

            }
        }

        protected override void FreeUnmanaged()
        {
            _mTexture.Dispose();
        }
    }
}
