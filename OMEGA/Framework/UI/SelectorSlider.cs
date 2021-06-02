namespace OMEGA.Framework.UI
{
    public class SelectorOption
    {
        public int Value { get; }

        public Rect Rect { get; set; }

        public SelectorOption(int value)
        {
            Value = value;
        }
    }

    public class SelectorSlider : Widget
    {
        public delegate void SelectorEvent(int value);

        public event SelectorEvent OnChange;

        public SelectorOption[] Options { get; }

        public Orientation Orientation { get; }

        internal int SelectedIndex { get; private set; }

        internal int ThumbSize { get; }

        private bool _mouseDown;

        private const int StepMargin = 2;

        public SelectorSlider(string id, int thumbSize, int[] options, Orientation orientation = Orientation.Horizontal) : base(id, 0, 0)
        {
            ThumbSize = thumbSize;

            Options = new SelectorOption[options.Length];

            for (int i = 0; i < options.Length; ++i)
            {
                Options[i] = new SelectorOption(options[i]);
            }

            Orientation = orientation;

            if (orientation == Orientation.Horizontal)
            {
                _height = thumbSize;
                _width = options.Length * thumbSize;
            }
            else
            {
                _width = thumbSize;
                _height = options.Length * thumbSize;
            }

            UpdateGeometry();
        }

        public void SetValue(int value)
        {
            for (int i = 0; i < Options.Length; ++i)
            {
                if (Options[i].Value == value)
                {
                    SelectedIndex = i;
                    return;
                }
            }
        }

        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            _mouseDown = true;
            UpdateIndexFromCursor(x, y);
        }

        public override void OnMouseUp(MouseButton button, int x, int y)
        {
            _mouseDown = false;
        }

        public override void OnMouseMove(int x, int y)
        {
            if (!_mouseDown)
            {
                return;
            }

            UpdateIndexFromCursor(x, y);
        }

        private void UpdateGeometry()
        {
            var steps = Options.Length;

            switch (Orientation)
            {
                case Orientation.Horizontal:

                    for (int i = 0; i < steps; ++i)
                    {
                        Options[i].Rect = Rect.FromBox(
                            (i * ThumbSize) + (i * StepMargin),
                            0,
                            ThumbSize,
                            ThumbSize
                        );
                    }

                    break;
                case Orientation.Vertical:

                    for (int i = 0; i < steps; ++i)
                    {
                        Options[i].Rect = Rect.FromBox(
                            0,
                            (i * ThumbSize) + (i * StepMargin),
                            ThumbSize,
                            ThumbSize
                        );
                    }

                    break;
            }


        }

        public override void Draw(IGuiDrawer drawer)
        {
            drawer.DrawSelectorSlider(this);
        }

        private void UpdateIndexFromCursor(int x, int y)
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    for (int i = 0; i < Options.Length; ++i)
                    {
                        var option = Options[i];
                        if (!option.Rect.Contains(x, 0)) continue;
                        SelectedIndex = i;
                        OnChange?.Invoke(Options[SelectedIndex].Value);
                        break;
                    }
                    break;
                case Orientation.Vertical:
                    for (int i = 0; i < Options.Length; ++i)
                    {
                        var option = Options[i];
                        if (!option.Rect.Contains(0, y)) continue;
                        SelectedIndex = i;
                        OnChange?.Invoke(Options[SelectedIndex].Value);
                        break;
                    }
                    break;
            }
        }
    }
}
