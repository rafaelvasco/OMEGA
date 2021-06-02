﻿using MessagePack;

namespace OMEGA
{
    [MessagePackObject]
    public class TextFileData
    {
        [Key(0)]
        public string Id {get;set;}

        [Key(1)]
        public byte[][] TextData {get;set;}
    }
}
