using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OMEGA
{
    public class GameAssetsManifest
    {
        [JsonPropertyName("resources")]
        public Dictionary<string, ResourceFileInfo[]> Resources { get; set; }
    }
}
