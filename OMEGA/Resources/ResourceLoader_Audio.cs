namespace OMEGA
{
    public static partial class ResourceLoader
    {
        //public unsafe Effect LoadEffect(SfxData sfx_data)
        //{
        //    var wav = new Wav();

        //    fixed (byte* p = sfx_data.Data)
        //    {
        //        var ptr = (IntPtr)p;

        //        wav.loadMem(ptr, (uint) sfx_data.Data.Length, aCopy: true);
        //    }

        //    var effect = new Effect(wav)
        //    {
        //        Id = sfx_data.Id
        //    };

        //    return effect;
        //}

        //public Effect LoadEffect(string path)
        //{
        //    var sfx_data = LoadSfxData(path);

        //    return LoadEffect(sfx_data);
        //}

        //public unsafe Song LoadSong(SongData song_data)
        //{
        //    var wav_stream = new WavStream();

        //    fixed (byte* p = song_data.Data)
        //    {
        //        var ptr = (IntPtr)p;

        //        wav_stream.loadMem(ptr, (uint) song_data.Data.Length, aCopy: true);
        //    }

        //    var song = new Song(wav_stream)
        //    {
        //        Id = song_data.Id
        //    };

        //    return song;
        //}

        //public Song LoadSong(string path)
        //{
        //    var song_data = LoadSongData(path);

        //    return LoadSong(song_data);
        //}

        //public SfxData LoadSfxData(string path)
        //{
        //    var bytes = File.ReadAllBytes(path);

        //    var id = Path.GetFileNameWithoutExtension(path);

        //    var sfx_data = new SfxData()
        //    {
        //        Id = id,
        //        Data = bytes
        //    };

        //    return sfx_data;
        //}

        //public SongData LoadSongData(string path)
        //{
        //    var bytes = File.ReadAllBytes(path);

        //    var id = Path.GetFileNameWithoutExtension(path);

        //    var song_data = new SongData()
        //    {
        //        Id = id,
        //        Data = bytes
        //    };

        //    return song_data;
        //}
    }
}
