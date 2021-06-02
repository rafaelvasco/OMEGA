namespace OMEGA.Framework.UI
{
    public class Window : Container
    {
        public string Title
        {
            get => Label.Text;
            set => Label.Text = value;
        }

        protected readonly Button CloseButton;
        protected Label Label;

        public Window(string id, int width, int height, string title = "Window") : base(id, width, height)
        {
            CloseButton = new Button(id + "_close_button", "X", 30, 30)
            {
                CanHaveInputFocus = false
            };

            Label = new Label(id + "_label", title)
            {
                Width = _width,
                Height = 30,
                IgnoreInput = true
            };

            Draggable = true;

            Add(CloseButton);
            Add(Label);

            CloseButton.X = ((Widget) this).Width - CloseButton.Width;

            CloseButton.OnClick += OnCloseClick;
        }

        public override void OnKeyDown(Keys key)
        {
            if (key == Keys.Escape)
            {
                Gui.SetVisible(this, false);
            }
        }

        private void OnCloseClick()
        {
            Gui.SetVisible(this, false);
        }

        public override void Draw(IGuiDrawer drawer)
        {
            drawer.DrawWindow(this);

            DrawChildren(drawer);
        }
    }
}