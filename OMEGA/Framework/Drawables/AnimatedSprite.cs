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
        private const int DefaultFrameDelay = 5;

        internal int CurrentFrameIndex => _mFrameIndices[_mIndex];
        internal Vec2 CurrentFrameOrigin => _mFrameOrigins[_mIndex];

        public bool Paused { get; set; }

        private readonly int[] _mFrameIndices;
        private readonly float[] _mFrameDelays;
        private readonly Vec2[] _mFrameOrigins;
        private int _mAnimDirection = 1;
        private float _mTimer;
        private int _mIndex;

        internal SpriteAnimation(int[] frameIndices, SpriteAnimationMode mode)
        {
            Mode = mode;
            _mFrameIndices = frameIndices;

            _mFrameDelays = new float[frameIndices.Length];
            _mFrameOrigins = new Vec2[frameIndices.Length];

            for (int i = 0; i < frameIndices.Length; ++i)
            {
                _mFrameDelays[i] = DefaultFrameDelay;
                _mFrameOrigins[i] = new Vec2(0.5f, 0.5f);
            }
        }

        public SpriteAnimationMode Mode { get;set;}

        public void SetFrame(int index, bool pause = true)
        {
            index = Calc.Clamp(index, 0, _mFrameIndices.Length-1);

            _mIndex = index;

            Paused = pause;
        }

        public void Reset()
        {
            _mIndex = 0;
        }

        public void SetFrameDelay(float delay)
        {
            for (int i = 0; i < _mFrameDelays.Length; ++i)
            {
                _mFrameDelays[i] = delay;
            }
        }

        public void SetFrameDelay(int index, float delay)
        {
            index = ClampIndex(index);

            _mFrameDelays[index] = delay;
        }

        public void SetFrameOrigin(Vec2 origin)
        {
            for (int i = 0; i < _mFrameOrigins.Length; ++i)
            {
                _mFrameOrigins[i] = origin;
            }
        }

        public void SetFrameOrigin(int index, Vec2 origin)
        {
            index = ClampIndex(index);

            _mFrameOrigins[index] = origin;
        }

        private int ClampIndex(int index)
        {
            if (index > _mFrameIndices.Length - 1)
            {
                index = _mFrameIndices.Length - 1;
            }

            if (index < 0)
            {
                if (index == -1)
                {
                    index = _mFrameIndices.Length - 1;
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
            if (Paused || _mFrameIndices.Length == 1)
            {
                return;
            }

            _mTimer += 1;

            if (_mTimer < _mFrameDelays[_mIndex]) return;

            _mIndex += _mAnimDirection;

            int max_idx = _mFrameIndices.Length - 1;

            switch (Mode)
            {
                case SpriteAnimationMode.OneTime:

                    if (_mIndex > max_idx)
                    {
                        _mIndex = max_idx;
                        Paused = true;
                    }

                    break;
                case SpriteAnimationMode.Loop:

                    if (_mIndex > max_idx)
                    {
                        _mIndex = 0;
                    }

                    break;
                case SpriteAnimationMode.PingPong:

                    if (_mAnimDirection > 0 && _mIndex > max_idx)
                    {
                        _mIndex = max_idx;
                        _mAnimDirection = -1;
                    }
                    else if (_mAnimDirection < 0 && _mIndex < 0)
                    {
                        _mIndex = 0;
                        _mAnimDirection = 1;
                    }

                    break;
            }

            _mTimer = 0;
        }
    }

    public class AnimatedSprite<T> : Drawable where T : System.Enum
    {
        public SpriteAnimation this[T animation] => _mAnimations?[animation] ?? null;

        public SpriteAnimation CurrentAnimation => _mCurrentAnim;

        public bool Paused
        {
            get => _mCurrentAnim?.Paused ?? false;
            set
            {
                if (_mCurrentAnim != null)
                {
                    _mCurrentAnim.Paused = value;
                }
            }
        }

        public AnimatedSprite(TextureAtlas atlas)
        {
            _mAtlas = atlas;
            _mQuads = new Quad[atlas.Count];
            _mAnimations = new Dictionary<T, SpriteAnimation>();

            for (int i = 0; i < atlas.Count; ++i)
            {
                _mQuads[i] = new Quad(atlas.Texture, atlas[i]);
            }

            Width = _mQuads[0].Width;
            Height = _mQuads[0].Height;
            
        }

        public AnimatedSprite<T> AddAnimation(T animation, int[] indices, SpriteAnimationMode mode = SpriteAnimationMode.Loop)
        {
            _mAnimations.Add(animation, new SpriteAnimation(indices, mode));
            _mCurrentAnim = _mAnimations[animation];
            return this;
        }

        public void Clear()
        {
            _mAnimations.Clear();

            _mCurrentAnim = null;
        }

        public void SetAnimation(T animation, bool paused = false)
        {
            _mCurrentAnim = _mAnimations[animation];

            _mCurrentAnim.Paused = paused;
        }
            
        public void Update()
        {
            if (_mCurrentAnim == null)
            {
                return;
            }

            _mCurrentAnim.Update();
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
            if (color == _mColor)
            {
                return;
            }

            _mColor = color;
        }

        public void FlipHorizontal(bool flip)
        {
            SetFlip(flip, _mFlipY);
        }

        public void FlipVertical(bool flip)
        {
            SetFlip(_mFlipX, flip);
        }

        public void SetFlip(bool flipH, bool flipV)
        {
            if (_mFlipX == flipH && _mFlipY == flipV)
            {
                return;
            }

            _mFlipX = flipH;
            _mFlipY = flipV;
        }

        public void SetBlend(BlendMode mode)
        {
            _mBlendMode = mode;
        }

        public override void Draw(Canvas2D canvas)
        {
            if (_mCurrentAnim == null)
            {
                return;
            }

            canvas.BlendMode = _mBlendMode;

            Vec2 origin = _mCurrentAnim.CurrentFrameOrigin;

            var origin_dx = origin.X * Width;
            var origin_dy = origin.Y * Height;

            var draw_q = _mQuads[_mCurrentAnim.CurrentFrameIndex];

            draw_q.Set(X - origin_dx, Y - origin_dy, Width, Height);

            if (_mFlipX)
            {
                float tx = draw_q.TopLeft.Tx;
                draw_q.TopLeft.Tx = draw_q.TopRight.Tx;
                draw_q.TopRight.Tx = tx;

                tx = draw_q.BottomLeft.Tx;
                draw_q.BottomLeft.Tx = draw_q.BottomRight.Tx;
                draw_q.BottomRight.Tx = tx;
            }

            if (_mFlipY)
            {
                float ty = draw_q.TopLeft.Ty;
                draw_q.TopLeft.Ty = draw_q.BottomLeft.Ty;
                draw_q.BottomLeft.Ty = ty;

                ty = draw_q.TopRight.Ty;
                draw_q.TopRight.Ty = draw_q.BottomRight.Ty;
                draw_q.BottomRight.Ty = ty;
            }

            canvas.DrawQuad(in draw_q, _mAtlas.Texture);
        }

        private bool _mFlipX;
        private bool _mFlipY;
        private Color _mColor;
        private readonly TextureAtlas _mAtlas;
        private BlendMode _mBlendMode = BlendMode.Alpha;
        private readonly Quad[] _mQuads;
        private readonly Dictionary<T, SpriteAnimation> _mAnimations;
        private SpriteAnimation _mCurrentAnim;
        
    }
}
