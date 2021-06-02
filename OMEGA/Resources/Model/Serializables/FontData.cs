using MessagePack;

namespace OMEGA
{
    [MessagePackObject]
    public class FontData
    {
        [Key(0)]
        public string Id { get; set; }

        [Key(1)]
        public ImageData FontSheet { get; set; }

        [Key(2)]
        public char[] Chars { get;set;}

        [Key(3)]
        public SRect[] GlyphRects { get; set; }

        [Key(4)]
        public SRect[] GlyphCroppings { get;set; }

        [Key(5)]
        public SVec3[] GlyphKernings { get;set;}

        [Key(6)]
        public int LineSpacing { get; set; }

        [Key(7)]
        public int Spacing { get; set; }

        [Key(8)]
        public char DefaultChar { get; set; }
    }
}
