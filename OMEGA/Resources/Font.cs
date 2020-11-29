namespace OMEGA
{
    public class Font : Resource
    {
        public Texture2D Texture => font_tex;

        internal readonly Texture2D font_tex;
        internal readonly Quad[] letters;
        internal readonly float[] pre_spacings;
        internal readonly float[] post_spacings;

        internal Font(Texture2D tex, Quad[] glyphs, float[] pre, float[] post)
        {
            this.font_tex = tex;
            this.letters = glyphs;
            this.pre_spacings = pre;
            this.post_spacings = post;
        }

        protected override void FreeUnmanaged()
        {
            this.font_tex.Dispose();
        }
    }
}
