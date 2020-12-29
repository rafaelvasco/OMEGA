using ProtoBuf;

namespace OMEGA
{
    [ProtoContract]
    public class FontData
    {
        [ProtoMember(1)]
        public string Id { get; set; }

        [ProtoMember(2)]
        public int Size { get;set;}

        [ProtoMember(3)]
        public int LineHeight { get;set;}

        [ProtoMember(4)]
        public TextureData FontSheet { get; set; }

        [ProtoMember(5)]
        public char[] Chars { get;set;}

        [ProtoMember(6)]
        public SRect[] GlyphRects { get; set; }

        [ProtoMember(7)]
        public SVec2[] GlyphOffsets { get;set; }

        [ProtoMember(8)]
        public float[] GlyphXAdvances { get;set;}
    }
}
