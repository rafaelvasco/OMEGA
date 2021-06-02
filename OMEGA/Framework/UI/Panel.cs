namespace OMEGA.Framework.UI
{
    public class Panel : Container
    {
        public Panel(string id, int width, int height) : base(id, width, height)
        {
        }

        public override void Draw(IGuiDrawer drawer)
        {
            drawer.DrawPanel(this);

            DrawChildren(drawer);
        }
    }
}