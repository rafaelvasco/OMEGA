namespace OMEGA
{
    public class TextFile : Resource
    {
        public string[] Text {get; private set;}

        public string JoinedText => string.Join("\n", Text);

        internal TextFile(string[] text)
        {
            Text = text;
        }

        protected override void FreeManaged()
        {
            Text = null;
        }
    }
}
