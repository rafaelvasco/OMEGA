using OMEGA;
using OMEGA.Framework.UI;

namespace DEMO
{
    public class GuiDemo : Game
    {
        private Gui _ui;

        public override void Load()
        {
            _ui = new Gui()
            {
                DebugMode = false
            };

            var container = new Container("container", _ui.Width, _ui.Height);

            var button = new Button("button", "BUTTON1\nSUBTEXT");

            var button2 = new Button("button2", "BUTTON2");

            container.Add(button);
            container.Add(button2);

            container.Layout(Orientation.Vertical, ContainerAlignment.Center, ContainerAlignment.Center, 10, 10);

            _ui.Add(container);
        }

        public override void Update(float dt)
        {
            if (Input.KeyPressed(Keys.F11))
            {
                Engine.ToggleFullscreen();
            }
        }

        public override void Draw(Canvas2D canvas, float dt)
        {
            _ui.Draw(canvas);
        }
    }
}