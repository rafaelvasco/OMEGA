using ProtoBuf;
using System.Collections.Generic;

namespace OMEGA
{
    [ProtoContract]
    public class ResourcePak
    {

        [ProtoMember(1)]
        public string Name {get;set;}
        
        [ProtoMember(2)]
        public Dictionary<string, ImageData> Images {get;set;}

        [ProtoMember(3)]
        public Dictionary<string, TextureAtlasData> Atlases { get; set;}

        [ProtoMember(4)]
        public Dictionary<string, ShaderProgramData> Shaders {get;set;}

        [ProtoMember(5)]
        public Dictionary<string, FontData> Fonts {get;set;}

        [ProtoMember(6)]
        public Dictionary<string, TextFileData> TextFiles {get;set;}

        [ProtoMember(7)]
        public int TotalResourcesCount { get;set;}

        public ResourcePak() {}

        public ResourcePak(string name)
        {
            Name = name;
            Images = new Dictionary<string, ImageData>();
            Shaders = new Dictionary<string, ShaderProgramData>();
            Fonts = new Dictionary<string, FontData>();
            TextFiles = new Dictionary<string, TextFileData>();
            Atlases = new Dictionary<string, TextureAtlasData>();
        }
        
    }
}
