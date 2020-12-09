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
            ),
            top_left_col: Color.Red,
            top_right_col: Color.Orange,
            bottom_left_col: Color.PeachPuff,
            bottom_right_col: Color.GreenYellow);

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
            canvas.DrawQuad(in quad);
        }
    }
}
