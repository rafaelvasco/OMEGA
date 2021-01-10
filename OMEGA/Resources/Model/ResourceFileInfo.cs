
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

    public class ImageResInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

    }

    public class FontResInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("size")]
        public int Size { get;set; }

        [JsonPropertyName("range_level")]
        public int CharRangeLevel { get;set;}

        [JsonPropertyName("padding")]
        public int[] Padding { get;set;}

        [JsonPropertyName("dropshadow")]
        public bool DropShadow { get;set;}

        [JsonPropertyName("shadow_offset_x")]
        public int ShadowOffsetX { get;set;}

        [JsonPropertyName("shadow_offset_y")]
        public int ShadowOffsetY { get;set;}

        [JsonPropertyName("shadow_color")]
        public string ShadowColor { get;set;}

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
