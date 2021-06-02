using MessagePack;

namespace OMEGA
{
    [MessagePackObject]
    public class ImageData
    {
        [Key(0)]
        public string Id {get;set;}

        [Key(1)]
        public byte[] Data {get;set;}

        [Key(2)]
        public int Width {get;set;}

        [Key(3)]
        public int Height {get;set;}

    }
}
