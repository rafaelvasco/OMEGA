
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

    public class FontResFaceInfo
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("char_ranges")]
        public string[] CharRanges { get; set; }
    }

    public class FontResInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("faces")]
        public FontResFaceInfo[] Faces { get; set; }

        [JsonPropertyName("line_spacing")]
        public int LineSpacing { get; set; }

        [JsonPropertyName("spacing")]
        public int Spacing { get; set; }

        [JsonPropertyName("default_char")]
        public char DefaultChar { get; set; }
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
