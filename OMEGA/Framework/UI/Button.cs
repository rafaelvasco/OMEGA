namespace OMEGA.Framework.UI
{
    public class Button : Container
    {
        public const int DEFAULT_WIDTH = 80;
        public const int DEFAULT_HEIGHT = 30;

        public string Label
        {
            get => _label.Text;
            set
            {
                _label.Text = value;
                RecalculateSizeBasedOnLabel();
            }
        }

        protected readonly Label _label;

        public override int Width
        {
            get => _width;
            set
            {
                if (_width != value)
                {
                    _width = value;

                    if (_label != null)
                    {
                        _label.Width = value;
                    }
                }
            }
        }

        public override int Height
        {
            get => _height;
            set
            {
                if (_height != value)
                {
                    _height = value;

                    if (_label != null)
                    {
                        _label.Height = value;
                    }
                }
            }
        }

        public Button(string id, string label, int width = DEFAULT_WIDTH, int height = DEFAULT_HEIGHT) : base(id, width, height)
        {
            CanHaveInputFocus = true;

            _label = new Label(id + "_label", label)
            {
                Width = width,
                Height = height,
                IgnoreInput = true,
            };

            RecalculateSizeBasedOnLabel();

            Add(_label);
        }

        private void RecalculateSizeBasedOnLabel()
        {
            if (!string.IsNullOrEmpty(Label))
            {
                if (_label.TextMeasure.W > Width)
                {
                    Width = _label.TextMeasure.W + 10;

                    _label.Width = Width;
                }

                if (_label.TextMeasure.H > Height)
                {
                    Height = _label.TextMeasure.H + 10;
                    _label.Height = Height;
                }
            }
        }

        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            OffsetY = 1;
            _label.OffsetY = OffsetY;
        }

        public override void OnMouseUp(MouseButton button, int x, int y)
        {
            OffsetY = 0;
            _label.OffsetY = OffsetY;
        }

        public override void Draw(IGuiDrawer drawer)
        {
            drawer.DrawButton(this);

            DrawChildren(drawer);
        }
    }
}