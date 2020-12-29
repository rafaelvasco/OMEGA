using ProtoBuf;

namespace OMEGA
{
    [ProtoContract]
    public class TextFileData
    {
        [ProtoMember(1)]
        public string Id {get;set;}

        [ProtoMember(2)]
        public byte[][] TextData {get;set;}
    }
}
