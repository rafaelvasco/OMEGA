using OMEGA;
using System;

namespace DEMO
{
    public class CanvasDemo : Game
    {
        private Canvas canvas;
        private Texture2D texture;
        private Texture2D bg_tile;
        private Quad quad;
        private Quad bg_quad;
        private float vx;
        private float vy;
        private float speed_x = 100.0f;
        private float speed_y = 100.0f;
        private float v_limit = 10.0f;
        private float friction = 0.90f;
        private float bg_scroll_speed = 2.0f;
        private float bg_scroll_value = 0.0f;

        public override void Load()
        {
            canvas = new Canvas();

            texture = Engine.Content.Get<Texture2D>("party");
            bg_tile = Engine.Content.Get<Texture2D>("purple_tile");

            quad = new Quad(texture);
            bg_quad = new Quad();

            bg_quad.V0.X = 0;
            bg_quad.V0.Y = 0;
            bg_quad.V0.Col = 0xFFFFFFFF;
            bg_quad.V1.X = canvas.Width;
            bg_quad.V1.Y = 0;
            bg_quad.V1.Col = 0xFFFFFFFF;
            bg_quad.V2.X = canvas.Width;
            bg_quad.V2.Y = canvas.Height;
            bg_quad.V2.Col = 0xFFFFFFFF;
            bg_quad.V3.X = 0;
            bg_quad.V3.Y = canvas.Height;
            bg_quad.V3.Col = 0xFFFFFFFF;

            bg_quad.V0.Tx = 0;
            bg_quad.V0.Ty = 0;
            bg_quad.V1.Tx = canvas.Width / 64f;
            bg_quad.V1.Ty = 0;
            bg_quad.V2.Tx = canvas.Width / 64f;
            bg_quad.V2.Ty = canvas.Height / 64f;
            bg_quad.V3.Tx = 0;
            bg_quad.V3.Ty = canvas.Height / 64f;

        }

        public override void VariableUpdate(float dt)
        {

            var input_vector = Input.GetGamePadState().ThumbSticks.Left;

            if (Input.KeyPressed(Keys.F11))
            {
                Engine.ToggleFullscreen();
            }

            if (Input.GamePadPressed(GamePadButtons.Back) || Input.KeyPressed(Keys.Escape))
            {
                Exit();
            }

            vx += input_vector.X * speed_x * dt;
            vy -= input_vector.Y * speed_y * dt;

            vx = Calc.Clamp(vx, -v_limit, v_limit);
            vy = Calc.Clamp(vy, -v_limit, v_limit);

            vx *= friction;
            vy *= friction;

            quad.V0.X += vx;
            quad.V0.Y += vy;
            quad.V1.X += vx;
            quad.V1.Y += vy;
            quad.V2.X += vx;
            quad.V2.Y += vy;
            quad.V3.X += vx;
            quad.V3.Y += vy;

            bg_scroll_value += bg_scroll_speed * dt;

            bg_quad.V0.Tx = bg_scroll_value;
            bg_quad.V0.Ty = bg_scroll_value;
            bg_quad.V1.Tx = bg_scroll_value + canvas.Width / 64f;
            bg_quad.V1.Ty = bg_scroll_value;
            bg_quad.V2.Tx = bg_scroll_value + canvas.Width / 64f;
            bg_quad.V2.Ty = bg_scroll_value + canvas.Height / 64f;
            bg_quad.V3.Tx = bg_scroll_value;
            bg_quad.V3.Ty = bg_scroll_value + canvas.Height / 64f;

            if (bg_scroll_value > 1.0f)
            {
                bg_scroll_value = 0.0f;
            }
        }

        public override void Draw(float dt)
        {
            canvas.Begin();

            canvas.DrawQuad(bg_tile, ref bg_quad);
            canvas.DrawQuad(texture, ref quad);
            canvas.End();
        }

        public override void OnResize()
        {
            Console.WriteLine("Resize");

            canvas.OnScreenResized(Engine.DisplaySize.Width, Engine.DisplaySize.Height);
        }
    }
}
