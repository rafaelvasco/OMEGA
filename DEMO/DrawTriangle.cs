using OMEGA;

namespace DEMO
{
    public class DrawTriangle : Game
    {
        Vertex[] vertices;

        public override void Load()
        {
            var display_size = Engine.DisplaySize;

            vertices = new Vertex[] {

                new Vertex ( 0f, 0f, 0.0f, Color.RoyalBlue, 0f, 0f),
                new Vertex ( display_size.Width, display_size.Height/2, 0.0f, Color.SpringGreen, 0f, 0f ),
                new Vertex ( 0f, display_size.Height, 0.0f, Color.LightPink, 0f, 0f )
            };
        }

        public override void VariableUpdate(float dt)
        {
            if (Input.GamePadPressed(GamePadButtons.Back) || Input.KeyPressed(Keys.Escape))
            {
                Exit();
            }

            if (Input.KeyPressed(Keys.F11))
            {
                Engine.ToggleFullscreen();
            }
        }

        public override void Draw(Canvas canvas, float dt)
        {
            canvas.DrawVertices(vertices);
        }
    }
}
