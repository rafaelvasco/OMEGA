using OMEGA;
using System;

namespace DEMO
{
    public class PixmapDemo : Game
    {
        private Sprite surface;
        private Pixmap paste;

        public override void Load()
        {
            paste = Pixmap.Create(16, 16, Color.Orange);
            
            Blitter.Begin(paste);

            Blitter.Rect(0, 0, 8, 8, Color.Blue);
            Blitter.Rect(8, 8, 8, 8, Color.Blue);

            Blitter.End();

            surface = new Sprite(Texture2D.Create(512, 512, Color.White));
            surface.SetOrigin(0, 0);
            surface.SetPosition(Engine.Canvas.Width/2 - surface.Width/2, Engine.Canvas.Height/2 - surface.Height/2);

            Input.OnKeyPress += Input_OnKeyPress;
        }

        private void Input_OnKeyPress(Keys key)
        {
        }

        public override void Update(float dt)
        {
            Blitter.Begin(surface.Texture);

            if (Input.MouseLeftPressed())
            {
                Console.WriteLine("Mouse Pressed");

                int local_x = (int)(Input.MousePos.X - surface.X);
                int local_y = (int)(Input.MousePos.Y - surface.Y);

                Blitter.Rect(local_x - 16, local_y - 16, 32, 32, Color.Black);
            }

            if (Input.MouseRightPressed())
            {
                Console.WriteLine("Mouse Right Pressed");

                int local_x = (int)(Input.MousePos.X - surface.X);
                int local_y = (int)(Input.MousePos.Y - surface.Y);

                Blitter.Blit(paste, local_x - 32, local_y - 32, Rect.Empty, 64, 64);
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

        public override void Draw(Canvas canvas, float dt)
        {
            canvas.Begin();
            surface.Draw(canvas);
            canvas.End();
        }

    }
}
