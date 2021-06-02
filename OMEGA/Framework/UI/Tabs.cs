using System.Collections.Generic;

namespace OMEGA.Framework.UI
{
    public class TabButton : Button
    {
        public int Index { get; }

        public delegate void TabButtonEventHandler(int index);

        public event TabButtonEventHandler OnSelect;

        public TabButton(int index, string id, string label, int width, int height) : base(id, label, width, height)
        {
            Index = index;
            OnClick += OnClickTab;
            CanHaveInputFocus = false;
        }

        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            _label.OffsetY = 1;
        }

        public override void OnMouseUp(MouseButton button, int x, int y)
        {
            _label.OffsetY = 0;
        }

        private void OnClickTab()
        {
            OnSelect?.Invoke(this.Index);
        }
    }

    public class TabHeader : Container
    {
        private int _tabCount;
        private int _tabPosIndex;
        private readonly int _tabWidth;


        public TabHeader(string id, int width, int height, int tabWidth) : base(id, width, height)
        {
            _tabWidth = tabWidth;
        }

        internal TabButton AddTab(string name)
        {
            var tab_id = _tabCount++;
            var tab_header = new TabButton(tab_id, Id + "_" + name, name, _tabWidth, ((Widget) this).Height)
            {
                ToggleGroup = Id + "_toggle_group",
                Toggable = true
            };

            if (Children.Count == 0)
            {
                tab_header.On = true;
            }

            Add(tab_header);

            tab_header.X = _tabPosIndex;

            _tabPosIndex = tab_header.X + tab_header.Width + 2;

            return tab_header;
        }

        public override void Draw(IGuiDrawer drawer)
        {
            DrawChildren(drawer);
        }
    }

    public class Tabs : Container
    {
        private int _tabIndex;

        private readonly TabHeader _header;

        private readonly List<Panel> _tabPanels;

        private readonly int _tabHeaderHeight;

        public Tabs(string id, int width, int height, int tabWidth, int tabHeight) : base(id, width, height)
        {
            _tabHeaderHeight = tabHeight;

            _tabPanels = new List<Panel>();

            _header = new TabHeader(id + "header", _width, height: _tabHeaderHeight, tabWidth: tabWidth);

            Add(_header);
        }

        public Panel AddTab(string title)
        {
            var tab = _header.AddTab(title);
            tab.OnSelect += TabOnSelect;

            var panel = new Panel(Id + "_panel", ((Widget) this).Width, ((Widget) this).Height - _tabHeaderHeight)
            {
                Y = _tabHeaderHeight,
                IgnoreInput = true,
                CanHaveInputFocus = false
            };

            Add(panel);
            _tabPanels.Add(panel);
            Gui.SetVisible(panel, _tabPanels.Count == 1);

            return panel;
        }

        private void TabOnSelect(int index)
        {
            Gui.SetVisible(_tabPanels[_tabIndex], false);

            _tabIndex = index;

            Gui.SetVisible(_tabPanels[index], true);
        }
    }
}
