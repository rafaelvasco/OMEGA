using System.Collections.Generic;

namespace OMEGA
{
    public class FontFace
    {
        public int Size { get;set;}

        public string Path { get; set; }

        public List<CharRange> CharRanges;
    }
}
