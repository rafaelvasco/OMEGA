using MessagePack;

namespace OMEGA
{
    [MessagePackObject]
    public struct SVec3
    {
        [Key(0)]
        public float X { get;set;}

        [Key(1)]
        public float Y { get; set; }

        [Key(2)]
        public float Z { get; set; }

        public SVec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
