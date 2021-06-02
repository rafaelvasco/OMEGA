using OMEGA;
using System;

namespace DEMO
{
    public class PixmapDemo : Game
    {
        private Sprite _surface;
        private Pixmap _paste;

        public override void Load()
        {
            _paste = Pixmap.Create(16, 16, Color.Orange);
            
            Blitter.Begin(_paste);

            Blitter.SetColor(Color.Blue);

            Blitter.FillRect(0, 0, 8, 8);
            Blitter.FillRect(8, 8, 8, 8);

            Blitter.End();

            _surface = new Sprite(Texture2D.Create(512, 512, Color.White));
            _surface.SetOrigin(0, 0);
            _surface.SetPosition(Engine.Canvas.Width/2.0f - _surface.Width/2, Engine.Canvas.Height/2.0f - _surface.Height/2);

            Input.OnKeyPress += Input_OnKeyPress;
        }

        private void Input_OnKeyPress(Keys key)
        {
        }

        public override void Update(float dt)
        {
            Blitter.Begin(_surface.Texture);

            if (Input.MouseLeftPressed())
            {
                Console.WriteLine("Mouse Pressed");

                int local_x = (int)(Input.MousePos.X - _surface.X);
                int local_y = (int)(Input.MousePos.Y - _surface.Y);

                Blitter.SetColor(Color.Black);

                Blitter.FillRect(local_x - 16, local_y - 16, 32, 32);
            }

            if (Input.MouseRightPressed())
            {
                Console.WriteLine("Mouse Right Pressed");

                int local_x = (int)(Input.MousePos.X - _surface.X);
                int local_y = (int)(Input.MousePos.Y - _surface.Y);

                Blitter.Blit(_paste, local_x - 32, local_y - 32, Rect.Empty, 64, 64);
            }

            if (Input.KeyPressed(Keys.A))
            {
                Blitter.ColorAdd(255, 255, 255, 0);
            }

            if (Input.KeyPressed(Keys.Left))
            {
                Blitter.PixelShift(-16, 0);
            }

            if (Input.KeyPressed(Keys.Right))
            {
                Blitter.PixelShift(16, 0);
            }

            if (Input.KeyPressed(Keys.Up))
            {
                Blitter.PixelShift(0, -16);
            }

            if (Input.KeyPressed(Keys.Down))
            {
                Blitter.PixelShift(0, 16);
            }

            Blitter.End();
        }

        public override void Draw(Canvas2D canvas, float dt)
        {
            canvas.Begin();
            _surface.Draw(canvas);
            canvas.End();
        }

    }
}
