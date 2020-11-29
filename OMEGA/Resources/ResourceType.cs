using System.Text.Json.Serialization;

namespace OMEGA
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ResourceType
    {
        Image,
        Font,
        Shader,
        Sfx,
        Song,
        TextFile,
        Unknown
    }
}
