using ProtoBuf;

namespace OMEGA
{
    [ProtoContract]
    public class ShaderProgramData
    {
        [ProtoMember(1)]
        public string Id {get;set;}

        [ProtoMember(2)]
        public byte[] VertexShader {get;set;}

        [ProtoMember(3)]
        public byte[] FragmentShader {get;set;}

        [ProtoMember(4)]
        public string[] Samplers {get;set;}

        [ProtoMember(5)]
        public string[] Params {get;set;}
    }
}
