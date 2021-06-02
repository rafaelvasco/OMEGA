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

            Engine.Canvas.StretchMode = CanvasStretchMode.LetterBox;

            left_view = Engine.Canvas.CreateView();
            right_view = Engine.Canvas.CreateView();

            left_view.Viewport = RectF.FromBox(0f, 0f, 0.495f, 1f);
            right_view.Viewport = RectF.FromBox(0.5f, 0f, 0.5f, 1f);

            left_view.SizeFactor = new Vec2(0.5f, 1.0f);
            right_view.SizeFactor = new Vec2(0.5f, 1.0f);

            texture = Engine.Content.Get<Texture2D>("party");
            bg_tile = Engine.Content.Get<Texture2D>("purple_tile");

            bg_tile.Tiled = true;

            int canvas_w = Engine.Canvas.Width;
            int canvas_h = Engine.Canvas.Height;

            quad = new Quad(texture);

            quad.TopLeft.X = canvas_w / 2 - texture.Width / 2;
            quad.TopLeft.Y = canvas_h / 2 - texture.Height / 2;
            quad.TopRight.X = quad.TopLeft.X + texture.Width;
            quad.TopRight.Y = quad.TopLeft.Y;
            quad.BottomRight.X = quad.TopRight.X;
            quad.BottomRight.Y = quad.TopLeft.Y + texture.Height;
            quad.BottomLeft.X = quad.TopLeft.X;
            quad.BottomLeft.Y = quad.BottomRight.Y;

            bg_quad = new Quad();

            bg_quad.TopLeft.X = 0;
            bg_quad.TopLeft.Y = 0;
            bg_quad.TopLeft.Col = 0xFFFFFFFF;
            bg_quad.TopRight.X = canvas_w;
            bg_quad.TopRight.Y = 0;
            bg_quad.TopRight.Col = 0xFFFFFFFF;
            bg_quad.BottomRight.X = canvas_w;
            bg_quad.BottomRight.Y = canvas_h;
            bg_quad.BottomRight.Col = 0xFFFFFFFF;
            bg_quad.BottomLeft.X = 0;
            bg_quad.BottomLeft.Y = canvas_h;
            bg_quad.BottomLeft.Col = 0xFFFFFFFF;

            bg_quad.TopLeft.Tx = 0;
            bg_quad.TopLeft.Ty = 0;
            bg_quad.TopRight.Tx = canvas_w / 64f;
            bg_quad.TopRight.Ty = 0;
            bg_quad.BottomRight.Tx = canvas_w / 64f;
            bg_quad.BottomRight.Ty = canvas_h / 64f;
            bg_quad.BottomLeft.Tx = 0;
            bg_quad.BottomLeft.Ty = canvas_h / 64f;

        }

        public override void Update(float dt)
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

            quad.TopLeft.X += vx;
            quad.TopLeft.Y += vy;
            quad.TopRight.X += vx;
            quad.TopRight.Y += vy;
            quad.BottomRight.X += vx;
            quad.BottomRight.Y += vy;
            quad.BottomLeft.X += vx;
            quad.BottomLeft.Y += vy;

            bg_scroll_value += bg_scroll_speed * dt;

            int canvas_w = Engine.Canvas.Width;
            int canvas_h = Engine.Canvas.Height;

            bg_quad.TopLeft.Tx = bg_scroll_value;
            bg_quad.TopLeft.Ty = bg_scroll_value;
            bg_quad.TopRight.Tx = bg_scroll_value + canvas_w / 64f;
            bg_quad.TopRight.Ty = bg_scroll_value;
            bg_quad.BottomRight.Tx = bg_scroll_value + canvas_w / 64f;
            bg_quad.BottomRight.Ty = bg_scroll_value + canvas_h / 64f;
            bg_quad.BottomLeft.Tx = bg_scroll_value;
            bg_quad.BottomLeft.Ty = bg_scroll_value + canvas_h / 64f;

            if (bg_scroll_value > 1.0f)
            {
                bg_scroll_value = 0.0f;
            }
        }

        public override void Draw(Canvas2D canvas, float dt)
        {
            canvas.Begin(left_view);
            canvas.DrawQuad(in bg_quad, bg_tile);
            canvas.DrawQuad(in quad, texture);
            canvas.End();

            canvas.Begin(right_view);
            canvas.DrawQuad(in bg_quad, bg_tile);
            canvas.DrawQuad(in quad, texture);
            canvas.End();
        }

        public override void OnDisplayResize()
        {
            int canvas_w = Engine.Canvas.Width;
            int canvas_h = Engine.Canvas.Height;

            bg_quad.TopLeft.X = 0;
            bg_quad.TopLeft.Y = 0;
            bg_quad.TopRight.X = canvas_w;
            bg_quad.TopRight.Y = 0;
            bg_quad.BottomRight.X = canvas_w;
            bg_quad.BottomRight.Y = canvas_h;
            bg_quad.BottomLeft.X = 0;
            bg_quad.BottomLeft.Y = canvas_h;

            quad.TopLeft.X = canvas_w / 2 - texture.Width / 2;
            quad.TopLeft.Y = canvas_h / 2 - texture.Height / 2;
            quad.TopRight.X = quad.TopLeft.X + texture.Width;
            quad.TopRight.Y = quad.TopLeft.Y;
            quad.BottomRight.X = quad.TopRight.X;
            quad.BottomRight.Y = quad.TopLeft.Y + texture.Height;
            quad.BottomLeft.X = quad.TopLeft.X;
            quad.BottomLeft.Y = quad.BottomRight.Y;
        }
    }
}
