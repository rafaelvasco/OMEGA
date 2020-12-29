using OMEGA;

namespace DEMO
{
    public class ShapesDemo : Game
    {
        private RectShape rect;
        private TriangleShape triangle;
        private Texture2D texture;

        public override void Load()
        {
            texture = Engine.Content.Get<Texture2D>("checker");

            rect = new RectShape(100, 100, 100, 100);

            triangle = new TriangleShape(400, 100, 300, 200, 500, 200);

            triangle.SetSize(400, 200);

            triangle.SetTexture(texture);
            rect.SetTexture(texture);
            rect.SetColor(Color.Red);
        }

        public override void FixedUpdate(float dt)
        {
            if (Input.KeyPressed(Keys.R))
            {
                triangle.SetPosition(0, 0);
            }

            if (Input.KeyDown(Keys.Left))
            {
                triangle.Move(-5f, 0f);
            }

            if (Input.KeyDown(Keys.Right))
            {
                triangle.Move(5f, 0f);
            }

            if (Input.KeyDown(Keys.Up))
            {
                triangle.Move(0f, -5f);
            }

            if (Input.KeyDown(Keys.Down))
            {
                triangle.Move(0f, 5f);
            }
        }

        public override void Draw(Canvas canvas, float dt)
        {
            canvas.Begin();
            rect.SetPosition(100, 100);
            rect.Draw(canvas);
            rect.Move(100, 100);
            rect.Draw(canvas);
            rect.Move(100, 100);
            rect.Draw(canvas);

            triangle.Draw(canvas);

            canvas.End();
        }
       
    }
}
