using OMEGA;

namespace DEMO
{
    public class DrawQuad : Game
    {
        Quad quad;

        public override void Load()
        {
            var display_size = Engine.GameResolution;

            quad = new Quad(RectF.FromBox(
                100f,
                100f,
                display_size.Width - 200f,
                display_size.Height - 200f
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
        }

        public override void Draw(Canvas canvas, float dt)
        {
            canvas.DrawQuad(in quad);
        }
    }
}
