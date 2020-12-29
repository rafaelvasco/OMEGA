using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OMEGA
{
    public class ResourceFileGroup
    {
        [JsonPropertyName("images")]
        public List<CommonResInfo> Images { get; set;}

        [JsonPropertyName("shaders")]
        public List<ShaderResInfo> Shaders { get;set; }

        [JsonPropertyName("text_files")]
        public List<CommonResInfo> TextFiles { get;set; }

        [JsonPropertyName("fonts")]
        public List<FontResInfo> Fonts { get;set; }

        [JsonPropertyName("atlases")]
        public List<AtlasResInfo> Atlases { get;set; }
    }
}
