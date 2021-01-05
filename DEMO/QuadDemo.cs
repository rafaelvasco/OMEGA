using OMEGA;

namespace DEMO
{
    public class QuadDemo : Game
    {
        Quad quad;

        public override void Load()
        {
            quad = new Quad(RectF.FromBox(
                100f,
                100f,
                Engine.Canvas.Width - 200f,
                Engine.Canvas.Height - 200f
            ));

            quad.SetColors(
                color_top_left: Color.Red,
                color_top_right: Color.Orange,
                color_bottom_left: Color.PeachPuff,
                color_bottom_right: Color.GreenYellow
            );

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

        public override void Draw(Canvas canvas, float dt)
        {
            canvas.Begin();

            canvas.DrawQuad(in quad);

            canvas.End();
        }
    }
}
