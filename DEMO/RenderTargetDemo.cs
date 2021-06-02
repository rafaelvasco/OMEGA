using OMEGA;

namespace DEMO
{
    public class RenderTargetDemo : Game
    {
        
        private Sprite m_sprite;
        private RenderTarget m_render_target;
        private CanvasView m_render_target_view;
        private Sprite m_sprite_target;

        public override void Load()
        {
            m_render_target = RenderTarget.Create(320, 240, false);

            m_render_target_view = Engine.Canvas.CreateView(Color.Black);
            m_render_target_view.AbsoluteViewport = RectF.FromBox(0, 0, 320, 240);
            m_render_target_view.RenderTarget = m_render_target;

            m_sprite = new Sprite(Engine.Content.Get<Texture2D>("party"));
            m_sprite.SetPosition(Engine.Canvas.Width/2, Engine.Canvas.Height/2);

            m_sprite_target = new Sprite(m_render_target.Texture);

            m_sprite_target.SetPosition(Engine.Canvas.Width/2, Engine.Canvas.Height/2);
        }

        public override void Update(float dt)
        {
            m_sprite.Move(200.0f * dt, 200.0f * dt);

            if (m_sprite.X > Engine.Canvas.Width + m_sprite.Width)
            {
                m_sprite.SetPosition(0f, m_sprite.Y);
            }

            if (m_sprite.Y > Engine.Canvas.Height + m_sprite.Height)
            {
                m_sprite.SetPosition(m_sprite.X, 0f);
            }
        }

        public override void Draw(Canvas2D canvas, float dt)
        {
            canvas.Begin(m_render_target_view);

            m_sprite.Draw(canvas);

            canvas.End();

            canvas.Begin();

            m_sprite_target.Draw(canvas);

            m_sprite.Draw(canvas);

            canvas.End();
        }
    }
}
