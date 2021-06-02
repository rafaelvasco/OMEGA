namespace OMEGA.Framework.UI
{
    public interface IGuiDrawer
    {
        public Gui Gui { get; init; }

        public TextureFont DrawFont { get; set;}

        public void DrawButton(Button button);

        public void DrawCheckbox(CheckBox checkBox);

        public void DrawLabel(Label label);

        public void DrawListView(ListView listView);

        public void DrawPanel(Panel panel);

        public void DrawSelectorSlider(SelectorSlider selectorSlider);

        public void DrawTabs(Tabs tabs);

        public void DrawWindow(Window window);
    }
}