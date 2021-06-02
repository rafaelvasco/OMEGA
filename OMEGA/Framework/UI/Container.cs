using System.Collections.Generic;

namespace OMEGA.Framework.UI
{
    public enum ContainerAlignment
    {
        Start,
        Center,
        End,
        Stretch
    }

    public class Container : Widget
    {
        public List<Widget> Children { get; }

        public Container(string id, int width, int height) : base(id, width, height)
        {
            Children = new List<Widget>();
        }

        public void Add(Widget child)
        {
            child.Parent = this;

            Children.Add(child);
            Gui.Register(child);

            if (!this.Visible)
            {
                Gui.SetVisible(child, false);
            }
        }

        public void Layout(Orientation orientation, ContainerAlignment hAlign, ContainerAlignment vAlign, int padding, int itemSpacing)
        {
            if (Children.Count == 0)
            {
                return;
            }

            int CalcTotalItemWidth(int spacing)
            {
                int total = 0;

                foreach (var widget in Children)
                {
                    total += widget.Width;
                }

                total += (spacing * (Children.Count - 1));

                return total;
            }

            int CalcTotalItemHeight(int spacing)
            {
                int total = 0;

                foreach (var widget in Children)
                {
                    total += widget.Height;
                }

                total += (spacing * (Children.Count - 1));

                return total;
            }

            void LimitChildSize(int w, int h)
            {
                foreach (var widget in Children)
                {
                    if (w > 0 && widget.Width > w)
                    {
                        widget.Width = w;
                    }

                    if (h > 0 && widget.Height > h)
                    {
                        widget.Height = h;
                    }
                }
            }

            void DoLayoutHorizontal()
            {
                if (padding > (Width / 2 - 2))
                {
                    padding = Width / 2 - 2;
                }


                int available_width = (Width - 2 * padding) - (Children.Count - 1) * itemSpacing;

                int max_item_width = available_width / Children.Count;

                int total_item_width = CalcTotalItemWidth(0);

                int total_item_width_with_spacing = CalcTotalItemWidth(itemSpacing);

                if (total_item_width > available_width)
                {
                    LimitChildSize(max_item_width, Height - 2 * padding);
                    total_item_width_with_spacing = CalcTotalItemWidth(itemSpacing);
                }

                for (int i = 0; i < Children.Count; ++i)
                {
                    var child = Children[i];

                    switch (hAlign)
                    {
                        case ContainerAlignment.Start:
                            if (i == 0)
                            {
                                child.X = padding;
                            }
                            else
                            {
                                child.X = Children[i - 1].X + Children[i - 1].Width + itemSpacing;
                            }

                            break;
                        case ContainerAlignment.Center:

                            if (i == 0)
                            {
                                child.X = (Width / 2 - total_item_width_with_spacing / 2);
                            }
                            else
                            {
                                child.X = Children[i - 1].X + Children[i - 1].Width + itemSpacing;
                            }

                            break;
                        case ContainerAlignment.End:

                            if (i == 0)
                            {
                                child.X = Width - padding - total_item_width_with_spacing;
                            }
                            else
                            {
                                child.X = Children[i - 1].X + Children[i - 1].Width + itemSpacing;
                            }


                            break;
                        case ContainerAlignment.Stretch:

                            child.X = (i * max_item_width) + (i * itemSpacing) + padding;
                            child.Width = max_item_width;
                            break;
                    }

                    switch (vAlign)
                    {
                        case ContainerAlignment.Start:

                            child.Y = padding;

                            break;
                        case ContainerAlignment.Center:

                            child.Y = Height / 2 - child.Height / 2;

                            break;
                        case ContainerAlignment.End:

                            child.Y = Height - padding - child.Height;
                            break;
                        case ContainerAlignment.Stretch:

                            child.Y = padding;
                            child.Height = Height - padding * 2;

                            break;
                    }


                }
            }

            void DoLayoutVertical()
            {
                if (padding > (Height / 2 - 2))
                {
                    padding = Height / 2 - 2;
                }

                int available_height = (Height - 2 * padding) - (Children.Count - 1) * itemSpacing;

                int max_item_height = available_height / Children.Count;

                int total_item_height = CalcTotalItemHeight(0);

                int total_item_height_with_spacing = CalcTotalItemHeight(itemSpacing);

                if (total_item_height > available_height)
                {
                    LimitChildSize(Width - 2 * padding, max_item_height);
                    total_item_height_with_spacing = CalcTotalItemHeight(itemSpacing);
                }

                for (int i = 0; i < Children.Count; ++i)
                {
                    var child = Children[i];

                    switch (vAlign)
                    {
                        case ContainerAlignment.Start:
                            if (i == 0)
                            {
                                child.Y = padding;
                            }
                            else
                            {
                                child.Y += Children[i - 1].Y + Children[i - 1].Height + itemSpacing;
                            }

                            break;
                        case ContainerAlignment.Center:

                            if (i == 0)
                            {
                                child.Y = (Height / 2 - total_item_height_with_spacing / 2);
                            }
                            else
                            {
                                child.Y = Children[i - 1].Y + Children[i - 1].Height + itemSpacing;
                            }

                            break;
                        case ContainerAlignment.End:

                            if (i == 0)
                            {
                                child.Y = Height - padding - total_item_height_with_spacing;
                            }
                            else
                            {
                                child.Y = Children[i - 1].Y + Children[i - 1].Height + itemSpacing;
                            }


                            break;
                        case ContainerAlignment.Stretch:

                            child.Y = (i * max_item_height) + (i * itemSpacing) + padding;
                            child.Height = max_item_height;
                            break;
                    }

                    switch (hAlign)
                    {
                        case ContainerAlignment.Start:

                            child.X = padding;

                            break;
                        case ContainerAlignment.Center:

                            child.X = Width / 2 - child.Width / 2;

                            break;
                        case ContainerAlignment.End:

                            child.X = Width - padding - child.Width;

                            break;
                        case ContainerAlignment.Stretch:

                            child.X = padding;
                            child.Width = Width - padding * 2;

                            break;
                    }
                }
            }

            switch (orientation)
            {
                case Orientation.Horizontal:

                    DoLayoutHorizontal();

                    break;
                case Orientation.Vertical:

                    DoLayoutVertical();

                    break;
            }
        }

        protected void DrawChildren(IGuiDrawer drawer)
        {
            foreach (var widget in Children)
            {
                if (!widget.Visible)
                {
                    continue;
                }

                widget.Draw(drawer);
            }
        }

        public override void Draw(IGuiDrawer drawer)
        {
            DrawChildren(drawer);
        }
    }
}
