using OMEGA;

namespace DEMO
{
    public class TextDemo : Game
    {

        public override void Load()
        {

        }



        public override void Draw(Canvas2D canvas, float dt)
        {
            canvas.Begin();

            canvas.DrawString("HELLO WORLD ! hello world ! 0123456789", new Vec2(0, 0), Color.Green);
            canvas.DrawString("This is OMEGA ENGINE", new Vec2(5, 15), Color.White);
            canvas.DrawString($"Draw Calls: {Engine.Canvas.MaxDrawCalls}", new Vec2(5, 25), Color.OrangeRed);

            canvas.End();
        }
    }
}
