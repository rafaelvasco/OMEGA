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
        public Dictionary<string, ShaderProgramData> Shaders {get;set;}

        [ProtoMember(4)]
        public Dictionary<string, FontData> Fonts {get;set;}

        [ProtoMember(5)]
        public Dictionary<string, TextFileData> TextFiles {get;set;}

        //public readonly Dictionary<string, SfxData> Sfx;
        //public readonly Dictionary<string, SongData> Songs;
        

        public ResourcePak() {}

        public ResourcePak(string name)
        {
            Name = name;
            Images = new Dictionary<string, ImageData>();
            Shaders = new Dictionary<string, ShaderProgramData>();
            Fonts = new Dictionary<string, FontData>();
            TextFiles = new Dictionary<string, TextFileData>();
            //Sfx = new Dictionary<string, SfxData>();
            //Songs = new Dictionary<string, SongData>();
            
        }
        
    }
}
