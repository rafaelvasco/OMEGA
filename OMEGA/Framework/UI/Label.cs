namespace OMEGA.Framework.UI
{
    public class Label : Widget
    {
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                if (!string.IsNullOrEmpty(_text))
                {
                    var text_measure = Gui.Drawer.DrawFont.MeasureString(_text);

                    TextMeasure = ((int)text_measure.X, (int)text_measure.Y);

                    if (Width < TextMeasure.W)
                    {
                        Width = TextMeasure.W;
                    }

                    if (Height < TextMeasure.H)
                    {
                        Height = TextMeasure.H;
                    }
                }
            }
        }

        protected string _text;

        public (int W, int H) TextMeasure { get; private set; }

        public Label(string id, string text) : base(id, 0, 0)
        {
            Text = text;
        }

        public override void Draw(IGuiDrawer drawer)
        {
            drawer.DrawLabel(this);
        }
    }
}