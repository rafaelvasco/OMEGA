using ImGuiNET;
using OMEGA;

namespace OMEGAED
{
    public class Editor : Game
    {
        private ImGuiCanvas m_gui_canvas;
        private Sprite sprite;

        public override void Load()
        {
            sprite = new Sprite(Engine.Content.Get<Texture2D>("logo"));

            sprite.PutOnCenter();

            m_gui_canvas = new ImGuiCanvas();
        }

        public override void Unload()
        {
            m_gui_canvas.Dispose();
        }

        public override void OnDisplayResize()
        {
            m_gui_canvas.RefreshSize();
        }

        public override void Update(float dt)
        {
            m_gui_canvas.Update();
        }

        public override void Draw(Canvas canvas, float dt)
        {
            canvas.Begin();

            sprite.Draw(canvas);

            canvas.End();

            m_gui_canvas.Begin();

            ImGui.ShowDemoWindow();

            m_gui_canvas.End(dt);
        }
    }
}
