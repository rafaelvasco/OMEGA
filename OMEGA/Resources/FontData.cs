using ProtoBuf;

namespace OMEGA
{
    [ProtoContract]
    public class FontData
    {
        [ProtoMember(1)]
        public string Id { get; set; }

        [ProtoMember(2)]
        public ImageData FontSheet { get; set; }

        [ProtoMember(3)]
        public Rect[] GlyphRects { get; set; }

        [ProtoMember(4)]
        public float[] PreSpacings { get; set; }

        [ProtoMember(5)]
        public float[] PostSpacings { get; set; }
    }
}
