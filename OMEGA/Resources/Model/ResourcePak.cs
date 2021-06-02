using MessagePack;
using System.Collections.Generic;

namespace OMEGA
{
    [MessagePackObject]
    public class ResourcePak
    {

        [Key(0)]
        public string Name {get;set;}
        
        [Key(1)]
        public Dictionary<string, ImageData> Images {get;set;}

        [Key(2)]
        public Dictionary<string, TextureAtlasData> Atlases { get; set;}

        [Key(3)]
        public Dictionary<string, ShaderProgramData> Shaders {get;set;}

        [Key(4)]
        public Dictionary<string, FontData> Fonts {get;set;}

        [Key(5)]
        public Dictionary<string, TextFileData> TextFiles {get;set;}

        [Key(6)]
        public int TotalResourcesCount { get;set;}

        //public ResourcePak() 
        //{
        //    Name = null;
            
        //}

        public ResourcePak(string name)
        {
            Name = name;
            Images = new Dictionary<string, ImageData>();
            Shaders = new Dictionary<string, ShaderProgramData>();
            Fonts = new Dictionary<string, FontData>();
            TextFiles = new Dictionary<string, TextFileData>();
            Atlases = new Dictionary<string, TextureAtlasData>();
            TotalResourcesCount = 0;
        }
        
    }
}
