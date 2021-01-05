using OMEGA;
using System;

namespace DEMO
{
    public class PixmapDemo : Game
    {
        private Pixmap pixmap;
        private Pixmap paste;
        private Sprite sprite;

        public override void Load()
        {
            pixmap = new Pixmap(512, 512, Color.Black);
            paste = new Pixmap(16, 16, Color.Orange);
            
            Blitter.Begin(paste);

            Blitter.Rect(0, 0, 8, 8, Color.Blue);
            Blitter.Rect(8, 8, 8, 8, Color.Blue);

            Blitter.End();

            sprite = new Sprite(Texture2D.Create(pixmap));
            sprite.SetOrigin(0, 0);
            sprite.SetPosition(Engine.Canvas.Width/2 - sprite.Width/2, Engine.Canvas.Height/2 - sprite.Height/2);


        }

        public override void Update(float dt)
        {
            Blitter.Begin(pixmap);

            if (Input.MouseLeftPressed())
            {
                int local_x = (int)(Input.MousePos.X - sprite.X);
                int local_y = (int)(Input.MousePos.Y - sprite.Y);

                Console.WriteLine($"X: {local_x}, Y: {local_y}");

                Blitter.Rect(local_x - 16, local_y - 16, 32, 32, Color.White);
                sprite.Texture.UpdatePixels(pixmap);
            }

            if (Input.MouseRightPressed())
            {
                int local_x = (int)(Input.MousePos.X - sprite.X);
                int local_y = (int)(Input.MousePos.Y - sprite.Y);

                Blitter.Blit(paste, local_x - 32, local_y - 32, Rect.Empty, 64, 64);
                
                sprite.Texture.UpdatePixels(pixmap);
            }

            if (Input.KeyPressed(Keys.S))
            {
                Blitter.ColorAdd(255, 255, 255, 0);
                sprite.Texture.UpdatePixels(pixmap);
            }

            if (Input.KeyPressed(Keys.Left))
            {
                Blitter.PixelShift(-16, 0);
                sprite.Texture.UpdatePixels(pixmap);
            }

            if (Input.KeyPressed(Keys.Right))
            {
                Blitter.PixelShift(16, 0);
                sprite.Texture.UpdatePixels(pixmap);
            }

            if (Input.KeyPressed(Keys.Up))
            {
                Blitter.PixelShift(0, -16);
                sprite.Texture.UpdatePixels(pixmap);
            }

            if (Input.KeyPressed(Keys.Down))
            {
                Blitter.PixelShift(0, 16);
                sprite.Texture.UpdatePixels(pixmap);
            }

            Blitter.End();
        }

        public override void Draw(Canvas canvas, float dt)
        {
            canvas.Begin();

            sprite.Draw(canvas);

            canvas.End();
        }

        
    }
}
