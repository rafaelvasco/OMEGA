using OMEGA;

namespace DEMO
{
    public class CanvasDemo : Game
    {
        private Canvas canvas;
        private Texture2D texture;
        private Quad quad;

        public override void Load()
        {
            canvas = new Canvas();

            texture = Engine.Content.Get<Texture2D>("party");

            quad = new Quad(texture);
        }

        public override void Draw(float dt)
        {
            canvas.Begin();
            canvas.DrawQuad(texture, ref quad);
            canvas.End();
        }
    }
}
