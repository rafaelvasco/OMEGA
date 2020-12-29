using OMEGA;

namespace DEMO
{
    public class TextDemo : Game
    {
        private Text text;
        private Text text2;
        private Text text3;

        public override void Load()
        {
            text = new Text
            {
                TextValue = "HELLO WORLD!",
                Font = Engine.Content.Get<Font>("nokia16")
            };

            text2 = new Text
            {
                TextValue = "OMEGA ENGINE!",
                Font = Engine.Content.Get<Font>("nokia60")
            };

            text3 = new Text
            {
                TextValue = "IS AWESOME! :)",
                Font = Engine.Content.Get<Font>("droid60")
            };

            text.SetPosition(10, 10);
            text2.SetPosition(100, 100);
            text3.SetPosition(300, 300);

        }

        public override void Draw(Canvas canvas, float dt)
        {
            canvas.Begin();

            text.Draw(canvas);
            text2.Draw(canvas);
            text3.Draw(canvas);

            canvas.End();
        }
    }
}
