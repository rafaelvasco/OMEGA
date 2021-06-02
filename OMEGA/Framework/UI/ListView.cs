using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMEGA.Framework.UI
{
    public class ListItem
    {
        public string Label { get; set; }

        public object Value { get; set; }
    }

    public class ListView : Widget
    {
        public delegate void ListViewSelectHandler(ListItem item);

        public event ListViewSelectHandler OnSelect;

        public List<ListItem> Items { get; }

        public ListItem SelectedItem { get; private set; }

        public int RowHeight { get; set; } = 20;

        public int ScrollThumbWidth { get; set; } = 20;

        internal int HoveredIndex { get; private set; } = -1;

        internal int SelectedIndex { get; private set; } = -1;

        internal Rect ScrollThumbRect { get; private set; } = Rect.Empty;

        internal int TranslateY { get; private set; }

        private int _maxTranslateY;

        private readonly int _maxVisibleRows;

        private bool _scrolling;

        private int _lastMouseY;


        public ListView(string id, int width, int height) : base(id, width, height)
        {
            Items = new List<ListItem>();

            _maxVisibleRows = _height / RowHeight;
        }

        public void AddItem(string label, object value)
        {
            var item = new ListItem()
            {
                Label = label,
                Value = value
            };

            Items.Add(item);

            UpdateScroll();
        }

        public override void OnMouseMove(int x, int y)
        {
            if (_scrolling)
            {
                HoveredIndex = -1;

                var delta_y = y - _lastMouseY;

                TranslateY += delta_y;

                if (TranslateY < 0)
                {
                    TranslateY = 0;
                }

                if (TranslateY > _maxTranslateY)
                {
                    TranslateY = _maxTranslateY;
                }

                _lastMouseY = y;
            }
            else
            {
                if (ScrollThumbRect.Translated(0, TranslateY).Contains(x, y))
                {
                    HoveredIndex = -1;
                }
                else
                {
                    HoveredIndex = (y + TranslateY) / RowHeight;

                    if (HoveredIndex > Items.Count - 1 || HoveredIndex < 0)
                    {
                        HoveredIndex = -1;
                    }
                }
            }

        }

        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            if (ScrollThumbRect.Translated(0, TranslateY).Contains(x, y))
            {
                _scrolling = true;

                _lastMouseY = y;
            }
            else
            {
                if (HoveredIndex > -1)
                {
                    if (HoveredIndex == SelectedIndex)
                    {
                        SelectedIndex = -1;
                    }
                    else
                    {
                        SelectedIndex = HoveredIndex;

                        SelectedItem = Items[SelectedIndex];
                        OnSelect?.Invoke(SelectedItem);
                    }
                }
            }
        }


        public override void OnMouseUp(MouseButton button, int x, int y)
        {
            if (_scrolling)
            {
                _scrolling = false;
            }
        }


        public override void OnMouseExit()
        {
            HoveredIndex = -1;
        }

        public override void Draw(IGuiDrawer drawer)
        {
            drawer.DrawListView(this);
        }

        private void UpdateScroll()
        {
            if (Items.Count <= _maxVisibleRows)
            {
                ScrollThumbRect = Rect.Empty;
            }
            else
            {
                var thumb_height = Height - ((Items.Count - _maxVisibleRows)) * RowHeight;

                _maxTranslateY = Height - thumb_height;

                ScrollThumbRect = Rect.FromBox(Width - ScrollThumbWidth, 0, ScrollThumbWidth, thumb_height);
            }
        }
    }
}
