using ProtoBuf;

namespace OMEGA
{
    [ProtoContract]
    public class TextureData
    {
        [ProtoMember(1)]
        public string Id {get;set;}

        [ProtoMember(2)]
        public byte[] Data {get;set;}

        [ProtoMember(3)]
        public int Width {get;set;}

        [ProtoMember(4)]
        public int Height {get;set;}

    }
}
