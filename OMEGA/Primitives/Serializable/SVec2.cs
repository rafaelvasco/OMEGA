using MessagePack;

namespace OMEGA
{
    [MessagePackObject]
    public struct SVec2
    {
        [Key(0)]
        public float X { get; set; }

        [Key(1)]
        public float Y { get; set; }

        public SVec2(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
