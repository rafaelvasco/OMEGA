using ProtoBuf;
using System.Collections.Generic;

namespace OMEGA
{
    [ProtoContract]
    public class TextureAtlasData
    {
        [ProtoMember(1)]
        public string Id { get; set; }

        [ProtoMember(2)]
        public byte[] Data { get; set; }

        [ProtoMember(3)]
        public int Width { get; set; }

        [ProtoMember(4)]
        public int Height { get; set; }

        [ProtoMember(5)]
        public Dictionary<string, SRect> Atlas { get;set; }

        [ProtoMember(6)]
        public bool RuntimeUpdatable { get;set;}
    }
}
