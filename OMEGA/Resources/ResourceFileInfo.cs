
using System.Text.Json.Serialization;

namespace OMEGA
{
    public class ResourceFileInfo
    {
        [JsonPropertyName("resource_id")]
        public string ResourceId { get; set; }

        [JsonPropertyName("resource_type")]
        public ResourceType ResourceType { get; set; }

        [JsonPropertyName("resource_path")]
        public string ResourcePath { get; set; }

        [JsonPropertyName("secondary_resource_path")]
        public string SecondaryResourcePath { get; set; }
    }
}
