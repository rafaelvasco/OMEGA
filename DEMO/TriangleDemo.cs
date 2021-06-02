using OMEGA;

namespace DEMO
{
    public class TriangleDemo : Game
    {
        VertexPositionColorTexture[] vertices;

        public override void Load()
        {
            var canvas_size = Engine.Canvas.Size;

            vertices = new VertexPositionColorTexture[] {

                new VertexPositionColorTexture ( 0f, 0f, 0.0f, Color.RoyalBlue, 0f, 0f),
                new VertexPositionColorTexture ( canvas_size.Width, canvas_size.Height/2, 0.0f, Color.SpringGreen, 0f, 0f ),
                new VertexPositionColorTexture ( 0f, canvas_size.Height, 0.0f, Color.LightPink, 0f, 0f )
            };
        }

        public override void Update(float dt)
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

        public override void Draw(Canvas2D canvas, float dt)
        {
            canvas.Begin();

            canvas.DrawTriangle(vertices[0], vertices[1], vertices[2]);

            canvas.End();
        }
    }
}
