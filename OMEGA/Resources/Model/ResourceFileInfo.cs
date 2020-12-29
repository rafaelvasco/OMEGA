
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OMEGA
{
    public class CommonResInfo
    {
        [JsonPropertyName("id")]
        public string Id { get;set;}

        [JsonPropertyName("path")]
        public string Path { get;set;}
    }

    public class FontResInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("sizes")]
        public int[] Sizes { get;set; }

        [JsonPropertyName("ranges")]
        public string[] Ranges { get;set;}

    }

    public class AtlasResInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("regions")]
        public Dictionary<string, SRect> Regions { get;set;}
    }

    public class ShaderResInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("vs_path")]
        public string VsPath { get;set; }

        [JsonPropertyName("fs_path")]
        public string FsPath { get;set; }
    }
}
