using ProtoBuf;

namespace OMEGA
{
    [ProtoContract]
    public struct SRect
    {
        [ProtoMember(1)]
        public int X { get;set;}

        [ProtoMember(2)]
        public int Y { get;set;}

        [ProtoMember(3)]
        public int W { get;set;}

        [ProtoMember(4)]
        public int H { get;set;}

        public SRect(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }
    }
}
