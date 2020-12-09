using OMEGA;

namespace DEMO
{
    public class CanvasDemo : Game
    {
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

        private CanvasView left_view;
        private CanvasView right_view;

        public override void Load()
        {

            Engine.Canvas.StretchMode = CanvasStretchMode.Resize;

            left_view = Engine.Canvas.CreateView();
            right_view = Engine.Canvas.CreateView();

            left_view.SetViewport(RectF.FromBox(0f, 0f, 0.495f, 1f));
            right_view.SetViewport(RectF.FromBox(0.5f, 0f, 0.5f, 1f));

            left_view.SetSize(new Vec2(0.5f, 1.0f));
            right_view.SetSize(new Vec2(0.5f, 1.0f));

            texture = Engine.Content.Get<Texture2D>("party");
            bg_tile = Engine.Content.Get<Texture2D>("purple_tile");

            bg_tile.Tiled = true;

            int canvas_w = Engine.Canvas.Width;
            int canvas_h = Engine.Canvas.Height;

            quad = new Quad(texture);

            quad.V0.X = canvas_w / 2 - texture.Width / 2;
            quad.V0.Y = canvas_h / 2 - texture.Height / 2;
            quad.V1.X = quad.V0.X + texture.Width;
            quad.V1.Y = quad.V0.Y;
            quad.V2.X = quad.V1.X;
            quad.V2.Y = quad.V0.Y + texture.Height;
            quad.V3.X = quad.V0.X;
            quad.V3.Y = quad.V2.Y;

            bg_quad = new Quad();

            bg_quad.V0.X = 0;
            bg_quad.V0.Y = 0;
            bg_quad.V0.Col = 0xFFFFFFFF;
            bg_quad.V1.X = canvas_w;
            bg_quad.V1.Y = 0;
            bg_quad.V1.Col = 0xFFFFFFFF;
            bg_quad.V2.X = canvas_w;
            bg_quad.V2.Y = canvas_h;
            bg_quad.V2.Col = 0xFFFFFFFF;
            bg_quad.V3.X = 0;
            bg_quad.V3.Y = canvas_h;
            bg_quad.V3.Col = 0xFFFFFFFF;

            bg_quad.V0.Tx = 0;
            bg_quad.V0.Ty = 0;
            bg_quad.V1.Tx = canvas_w / 64f;
            bg_quad.V1.Ty = 0;
            bg_quad.V2.Tx = canvas_w / 64f;
            bg_quad.V2.Ty = canvas_h / 64f;
            bg_quad.V3.Tx = 0;
            bg_quad.V3.Ty = canvas_h / 64f;

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

            int canvas_w = Engine.Canvas.Width;
            int canvas_h = Engine.Canvas.Height;

            bg_quad.V0.Tx = bg_scroll_value;
            bg_quad.V0.Ty = bg_scroll_value;
            bg_quad.V1.Tx = bg_scroll_value + canvas_w / 64f;
            bg_quad.V1.Ty = bg_scroll_value;
            bg_quad.V2.Tx = bg_scroll_value + canvas_w / 64f;
            bg_quad.V2.Ty = bg_scroll_value + canvas_h / 64f;
            bg_quad.V3.Tx = bg_scroll_value;
            bg_quad.V3.Ty = bg_scroll_value + canvas_h / 64f;

            if (bg_scroll_value > 1.0f)
            {
                bg_scroll_value = 0.0f;
            }
        }

        public override void Draw(Canvas canvas, float dt)
        {
            canvas.Begin(left_view);
            canvas.DrawQuad(in bg_quad, bg_tile);
            canvas.DrawQuad(in quad, texture);
            canvas.End();

            canvas.Begin(right_view);
            canvas.DrawQuad(in bg_quad, bg_tile);
            canvas.DrawQuad(in quad, texture);
            canvas.End();

            //canvas.Begin();
            //canvas.DrawQuad(in bg_quad, bg_tile);
            //canvas.DrawQuad(in quad, texture);
            //canvas.End();
        }

        public override void OnDisplayResize()
        {
            int canvas_w = Engine.Canvas.Width;
            int canvas_h = Engine.Canvas.Height;

            bg_quad.V0.X = 0;
            bg_quad.V0.Y = 0;
            bg_quad.V1.X = canvas_w;
            bg_quad.V1.Y = 0;
            bg_quad.V2.X = canvas_w;
            bg_quad.V2.Y = canvas_h;
            bg_quad.V3.X = 0;
            bg_quad.V3.Y = canvas_h;

            quad.V0.X = canvas_w / 2 - texture.Width / 2;
            quad.V0.Y = canvas_h / 2 - texture.Height / 2;
            quad.V1.X = quad.V0.X + texture.Width;
            quad.V1.Y = quad.V0.Y;
            quad.V2.X = quad.V1.X;
            quad.V2.Y = quad.V0.Y + texture.Height;
            quad.V3.X = quad.V0.X;
            quad.V3.Y = quad.V2.Y;
        }
    }
}
