using ProtoBuf;

namespace OMEGA
{
    [ProtoContract]
    public struct SVec2
    {
        [ProtoMember(1)]
        public float X { get; set; }

        [ProtoMember(2)]
        public float Y { get; set; }


        public SVec2(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
