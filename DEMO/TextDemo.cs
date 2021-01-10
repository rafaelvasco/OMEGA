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

            text.SetColor(Color.White);

            text2 = new Text
            {
                TextValue = "OMEGA ENGINE!",
                Font = Engine.Content.Get<Font>("nokia60")
            };

            text2.SetColor(Color.DodgerBlue);

            text3 = new Text
            {
                TextValue = "IS AWESOME!\nNEW LINE\nNEW LINE :)",
                Font = Engine.Content.Get<Font>("droid60")
            };

            text3.SetColor(Color.LimeGreen);

            text.SetPosition(0, 10);
            text2.SetPosition(0, 100);
            text3.SetPosition(0, 300);


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
