using System;

namespace OMEGA.Framework.UI
{
    public abstract class Widget : IComparable<Widget>
    {
        public delegate void WidgetMouseEventHandler();

        public event WidgetMouseEventHandler OnClick;

        public event WidgetMouseEventHandler OnPress;

        public event WidgetMouseEventHandler OnRelease;

        public static Gui Gui { get; internal set; }

        public Container Parent { get; internal set; }

        private static int _globalUid;

        public int Uid { get; }

        public int ZIndex { get; set; }

        public string Id { get; }

        public GuiState State
        {
            get
            {
                if (Disabled)
                {
                    return GuiState.Disabled;
                }

                if (Hovered)
                {
                    return GuiState.Hover;
                }

                if (Active)
                {
                    return GuiState.Active;
                }

                if (On)
                {
                    return GuiState.On;
                }

                return GuiState.Idle;
            }
        }

        public int X { get; set; }
        public int Y { get; set; }

        public int OffsetX { get; set; }

        public int OffsetY { get; set; }

        public virtual int Width
        {
            get => _width;
            set => _width = value;
        }

        public virtual int Height
        {
            get => _height;
            set => _height = value;
        }

        public bool Hovered { get; internal set; }

        public bool Active { get; internal set; }

        public bool On { get; internal set; }

        public bool Visible { get; internal set; } = true;

        public bool Disabled { get; set; } = false;

        public bool IgnoreInput { get; set; } = false;

        public bool Draggable { get; set; }

        public bool Toggable { get; set; } = false;

        public bool Dragging { get; internal set; }

        public bool HasInputFocus { get; internal set; }

        public bool CanHaveInputFocus { get; set; } = false;

        public bool BubbleEventsToParent { get; set; } = false;

        public string ToggleGroup
        {
            get => _toggleGroup;
            set
            {
                _toggleGroup = value;

                if (value != null)
                {
                    Gui.AssignToggleGroup(this, _toggleGroup);
                }
            }
        }

        public int DrawX => Parent?.DrawX + X + OffsetX ?? X + OffsetX;
        public int DrawY => Parent?.DrawY + Y + OffsetY ?? Y + OffsetY;

        public Rect GlobalGeometry => Rect.FromBox(DrawX, DrawY, Width, Height);

        protected int _width;

        protected int _height;

        private string _toggleGroup;

        public void ShowAndFocus()
        {
            if (Visible)
            {
                return;
            }

            Align(Alignment.Center);
            Gui.SetVisible(this, true);
            Gui.SetInputFocus(this, true);
        }

        protected Widget(string id, int width, int height)
        {
            Id = id;
            Uid = ++_globalUid;
            X = 0;
            Y = 0;
            _width = width;
            _height = height;
        }

        internal void ProcessMouseDown(MouseButton button, int x, int y)
        {
            OnMouseDown(button, x, y);

            if (State == GuiState.Hover)
            {
                OnPress?.Invoke();
            }
        }

        internal void ProcessMouseUp(MouseButton button, int x, int y)
        {
            OnMouseUp(button, x, y);
            if (State == GuiState.Hover)
            {
                OnClick?.Invoke();
                OnRelease?.Invoke();
            }
        }

        internal void ProcessMouseMove(int x, int y)
        {
            OnMouseMove(x, y);
        }

        internal void ProcessKeyDown(Keys key)
        {
            OnKeyDown(key);

            Parent?.ProcessKeyDown(key);
        }

        internal void ProcessKeyUp(Keys key)
        {
            OnKeyUp(key);

            Parent?.ProcessKeyUp(key);
        }

        internal void ProcessMouseEnter()
        {
            OnMouserEnter();
        }

        internal void ProcessMouseLeave()
        {
            OnMouseExit();
        }

        public virtual void OnMouseDown(MouseButton button, int x, int y)
        {
        }

        public virtual void OnMouseUp(MouseButton button, int x, int y)
        {
        }

        public virtual void OnMouseMove(int x, int y)
        {
        }

        public virtual void OnKeyDown(Keys key)
        {
        }

        public virtual void OnKeyUp(Keys key)
        {
        }

        public virtual void OnMouserEnter()
        {
        }

        public virtual void OnMouseExit()
        {
        }

        public override string ToString()
        {
            return Id;
        }

        public void Align(Alignment alignment, int marginTop = 0, int marginLeft = 0, int marginRight = 0, int marginBottom = 0)
        {
            if (Parent == null)
            {
                return;
            }

            switch (alignment)
            {
                case Alignment.TopLeft:

                    X = marginLeft;
                    Y = marginTop;

                    break;

                case Alignment.Top:

                    X = Parent.Width / 2 - Width / 2;
                    Y = marginTop;

                    break;

                case Alignment.TopRight:

                    X = Parent.Width - Width - marginRight;
                    Y = marginTop;

                    break;

                case Alignment.Left:

                    X = marginLeft;
                    Y = Parent.Height / 2 - Height / 2;

                    break;

                case Alignment.Center:

                    X = Parent.Width / 2 - Width / 2;
                    Y = Parent.Height / 2 - Height / 2;

                    break;

                case Alignment.Right:

                    X = Parent.Width - Width - marginRight;
                    Y = Parent.Height / 2 - Height / 2;

                    break;

                case Alignment.BottomLeft:

                    X = marginLeft;
                    Y = Parent.Height - Height - marginBottom;

                    break;

                case Alignment.Bottom:

                    X = Parent.Width / 2 - Width / 2;
                    Y = Parent.Height - Height - marginBottom;

                    break;

                case Alignment.BottomRight:

                    X = Parent.Width - Width - marginRight;
                    Y = Parent.Height - Height - marginBottom;

                    break;
            }
        }

        public virtual void Update()
        {
        }

        public abstract void Draw(IGuiDrawer drawer);

        public int CompareTo(Widget other)
        {
            if (ReferenceEquals(this, other)) return 0;
            return other is null ? 1 : Uid.CompareTo(other.Uid);
        }
    }
}