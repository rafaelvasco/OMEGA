using System.Collections.Generic;

namespace OMEGA
{
    public enum SpriteAnimationMode
    {
        OneTime,
        Loop,
        PingPong
    }

    public class SpriteAnimation
    {
        private const int DEFAULT_FRAME_DELAY = 5;

        internal int CurrentFrameIndex => m_frame_indices[m_index];
        internal Vec2 CurrentFrameOrigin => m_frame_origins[m_index];

        public bool Paused { get; set; }

        private int[] m_frame_indices;
        private float[] m_frame_delays;
        private Vec2[] m_frame_origins;
        private int m_anim_direction = 1;
        private float m_timer;
        private int m_index;

        internal SpriteAnimation(int[] frame_indices, SpriteAnimationMode mode)
        {
            Mode = mode;
            m_frame_indices = frame_indices;

            m_frame_delays = new float[frame_indices.Length];
            m_frame_origins = new Vec2[frame_indices.Length];

            for (int i = 0; i < frame_indices.Length; ++i)
            {
                m_frame_delays[i] = DEFAULT_FRAME_DELAY;
                m_frame_origins[i] = new Vec2(0.5f, 0.5f);
            }
        }

        public SpriteAnimationMode Mode { get;set;}

        public void SetFrame(int index, bool pause = true)
        {
            index = Calc.Clamp(index, 0, m_frame_indices.Length-1);

            m_index = index;

            Paused = pause;
        }

        public void Reset()
        {
            m_index = 0;
        }

        public void SetFrameDelay(float delay)
        {
            for (int i = 0; i < m_frame_delays.Length; ++i)
            {
                m_frame_delays[i] = delay;
            }
        }

        public void SetFrameDelay(int index, float delay)
        {
            index = ClampIndex(index);

            m_frame_delays[index] = delay;
        }

        public void SetFrameOrigin(Vec2 origin)
        {
            for (int i = 0; i < m_frame_origins.Length; ++i)
            {
                m_frame_origins[i] = origin;
            }
        }

        public void SetFrameOrigin(int index, Vec2 origin)
        {
            index = ClampIndex(index);

            m_frame_origins[index] = origin;
        }

        private int ClampIndex(int index)
        {
            if (index > m_frame_indices.Length - 1)
            {
                index = m_frame_indices.Length - 1;
            }

            if (index < 0)
            {
                if (index == -1)
                {
                    index = m_frame_indices.Length - 1;
                }
                else
                {
                    index = 0;
                }
            }

            return index;
        }

        internal void Update()
        {
            if (Paused || m_frame_indices.Length == 1)
            {
                return;
            }

            m_timer += 1;

            if (m_timer < m_frame_delays[m_index]) return;

            m_index += m_anim_direction;

            int max_idx = m_frame_indices.Length - 1;

            switch (Mode)
            {
                case SpriteAnimationMode.OneTime:

                    if (m_index > max_idx)
                    {
                        m_index = max_idx;
                        Paused = true;
                    }

                    break;
                case SpriteAnimationMode.Loop:

                    if (m_index > max_idx)
                    {
                        m_index = 0;
                    }

                    break;
                case SpriteAnimationMode.PingPong:

                    if (m_anim_direction > 0 && m_index > max_idx)
                    {
                        m_index = max_idx;
                        m_anim_direction = -1;
                    }
                    else if (m_anim_direction < 0 && m_index < 0)
                    {
                        m_index = 0;
                        m_anim_direction = 1;
                    }

                    break;
            }

            m_timer = 0;
        }
    }

    public class AnimatedSprite<T> : Drawable where T : System.Enum
    {
        public SpriteAnimation this[T animation] => m_animations?[animation] ?? null;

        public SpriteAnimation CurrentAnimation => m_current_anim;

        public bool Paused
        {
            get => m_current_anim?.Paused ?? false;
            set
            {
                if (m_current_anim != null)
                {
                    m_current_anim.Paused = value;
                }
            }
        }

        public AnimatedSprite(TextureAtlas atlas)
        {
            m_atlas = atlas;
            m_quads = new Quad[atlas.Count];
            m_animations = new Dictionary<T, SpriteAnimation>();

            for (int i = 0; i < atlas.Count; ++i)
            {
                m_quads[i] = new Quad(atlas.Texture, atlas[i]);
            }

            Width = m_quads[0].Width;
            Height = m_quads[0].Height;
            
        }

        public AnimatedSprite<T> AddAnimation(T animation, int[] indices, SpriteAnimationMode mode = SpriteAnimationMode.Loop)
        {
            m_animations.Add(animation, new SpriteAnimation(indices, mode));
            m_current_anim = m_animations[animation];
            return this;
        }

        public void Clear()
        {
            m_animations.Clear();

            m_current_anim = null;
        }

        public void SetAnimation(T animation, bool paused = false)
        {
            m_current_anim = m_animations[animation];

            m_current_anim.Paused = paused;
        }
            
        public void Update()
        {
            if (m_current_anim == null)
            {
                return;
            }

            m_current_anim.Update();
        }

        public override void SetPosition(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override void Move(float dx, float dy)
        {
            X += dx;
            Y += dy;
        }

        public override void SetSize(float w, float h)
        {
            Width = w;
            Height = h;
        }

        public override void SetColor(Color color)
        {
            if (color == m_color)
            {
                return;
            }

            m_color = color;
        }

        public void FlipHorizontal(bool flip)
        {
            SetFlip(flip, m_flip_y);
        }

        public void FlipVertical(bool flip)
        {
            SetFlip(m_flip_x, flip);
        }

        public void SetFlip(bool flip_h, bool flip_v)
        {
            if (m_flip_x == flip_h && m_flip_y == flip_v)
            {
                return;
            }

            m_flip_x = flip_h;
            m_flip_y = flip_v;
        }

        public void SetBlend(BlendMode mode)
        {
            m_blend_mode = mode;
        }

        public override void Draw(Canvas canvas)
        {
            if (m_current_anim == null)
            {
                return;
            }

            canvas.BlendMode = m_blend_mode;

            Vec2 origin = m_current_anim.CurrentFrameOrigin;

            var origin_dx = origin.X * Width;
            var origin_dy = origin.Y * Height;

            var draw_q = m_quads[m_current_anim.CurrentFrameIndex];

            draw_q.Set(X - origin_dx, Y - origin_dy, Width, Height);

            if (m_flip_x)
            {
                float tx = draw_q.V0.Tx;
                draw_q.V0.Tx = draw_q.V1.Tx;
                draw_q.V1.Tx = tx;

                tx = draw_q.V3.Tx;
                draw_q.V3.Tx = draw_q.V2.Tx;
                draw_q.V2.Tx = tx;
            }

            if (m_flip_y)
            {
                float ty = draw_q.V0.Ty;
                draw_q.V0.Ty = draw_q.V3.Ty;
                draw_q.V3.Ty = ty;

                ty = draw_q.V1.Ty;
                draw_q.V1.Ty = draw_q.V2.Ty;
                draw_q.V2.Ty = ty;
            }

            canvas.DrawTextureQuad(in draw_q, m_atlas.Texture);
        }

        private bool m_flip_x;
        private bool m_flip_y;
        private Color m_color;
        private TextureAtlas m_atlas;
        private BlendMode m_blend_mode = BlendMode.Alpha;
        private readonly Quad[] m_quads;
        private Dictionary<T, SpriteAnimation> m_animations;
        private SpriteAnimation m_current_anim;
        
    }
}
