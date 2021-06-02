namespace OMEGA.Framework.UI
{
    public class CheckBox : Container
    {
        public const int DEFAULT_WIDTH = 20;
        public const int DEFAULT_HEIGHT = 20;

        public string Label
        {
            get => _label.Text;
            set => _label.Text = value;
        }

        protected readonly Label _label;

        public CheckBox(string id, int width = DEFAULT_WIDTH, int height = DEFAULT_HEIGHT, string label = "Check") : base(id, width, height)
        {
            CanHaveInputFocus = true;

            Toggable = true;

            _label = new Label(id + "_label", label)
            {
                BubbleEventsToParent = true
            };

            Add(_label);

            _label.Height = _height;

            _label.X = _width + 10;
        }

        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            OffsetY = 1;
        }

        public override void OnMouseUp(MouseButton button, int x, int y)
        {
            OffsetY = 0;
        }

        public override void OnKeyUp(Keys key)
        {
            if (!HasInputFocus)
            {
                return;
            }

            if (key == Keys.Space)
            {
                OffsetY = 0;

                if (ToggleGroup == null)
                {
                    On = !On;
                }
                else
                {
                    On = true;
                    Gui.UpdateToggleGroup(this);
                }
            }
        }

        public override void OnKeyDown(Keys key)
        {
            if (!HasInputFocus)
            {
                return;
            }

            if (key == Keys.Space)
            {
                OffsetY = 1;
            }
        }

        public override void Draw(IGuiDrawer drawer)
        {
            drawer.DrawCheckbox(this);

            DrawChildren(drawer);
        }
    }
}