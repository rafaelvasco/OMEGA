using ProtoBuf;

namespace OMEGA
{
    [ProtoContract]
    public struct SVec3
    {
        [ProtoMember(1)]
        public float X { get;set;}

        [ProtoMember(2)]
        public float Y { get; set; }

        [ProtoMember(3)]
        public float Z { get; set; }

        public SVec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
