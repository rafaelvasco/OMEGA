using System.Runtime.CompilerServices;

namespace OMEGA
{
    public class Sprite : Drawable
    {
        public Texture2D Texture => _mTexture;

        public Sprite(Texture2D texture) : this(texture, RectF.FromBox(0f, 0f, texture.Width, texture.Height))
        {
        }

        public Sprite(TextureAtlas atlas) : this(atlas.Texture, atlas[0])
        {

        }

        public Sprite(Texture2D texture, RectF srcRect)
        {
            _mTexture = texture;
            _mQuad = new Quad(texture, srcRect);
            Width = _mQuad.Width;
            Height = _mQuad.Height;

            _mOriginX = 0.5f;
            _mOriginY = 0.5f;

            _mFlipX = false;

            _mFlipY = false;
        }


        private bool _mFlipX;
        private bool _mFlipY;
        private float _mOriginX;
        private float _mOriginY;
        private Color _mColor = Color.White;

        private Texture2D _mTexture;
        private BlendMode _mBlendMode = BlendMode.Alpha;
        private Quad _mQuad;

        public void SetTextureRegion(RectF rect, bool resetSize)
        {
            if (resetSize)
            {
                Width = rect.Width;
                Height = rect.Height;
            }

            _mQuad = new Quad(_mTexture, rect, RectF.FromBox(0, 0, Width, Height));

        }

        public void SetTextureRegion(Texture2D texture, RectF region, bool resetSize)
        {
            _mTexture = texture;
            SetTextureRegion(region, resetSize);
        }

        public void SetAtlasFrame(TextureAtlas atlas, int frame)
        {
            SetTextureRegion(atlas.Texture, atlas[frame], false);
        }

        public void SetAtlasFrame(TextureAtlas atlas, string frame)
        {
            SetTextureRegion(atlas.Texture, atlas[frame], false);
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

            _mQuad.TopLeft.Col = color;
            _mQuad.TopRight.Col = color;
            _mQuad.BottomRight.Col = color;
            _mQuad.BottomLeft.Col = color;
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

            float tx, ty;

            if (flipH != _mFlipX)
            {
                tx = _mQuad.TopLeft.Tx;
                _mQuad.TopLeft.Tx = _mQuad.TopRight.Tx;
                _mQuad.TopRight.Tx = tx;

                tx = _mQuad.BottomLeft.Tx;
                _mQuad.BottomLeft.Tx = _mQuad.BottomRight.Tx;
                _mQuad.BottomRight.Tx = tx;
            }

            if (flipV != _mFlipY)
            {
                ty = _mQuad.TopLeft.Ty;
                _mQuad.TopLeft.Ty = _mQuad.BottomLeft.Ty;
                _mQuad.BottomLeft.Ty = ty;

                ty = _mQuad.TopRight.Ty;
                _mQuad.TopRight.Ty = _mQuad.BottomRight.Ty;
                _mQuad.BottomRight.Ty = ty;
            }
        }

        public void SetOrigin(float x, float y)
        {
            _mOriginX = x;
            _mOriginY = y;
        }

        public void SetBlend(BlendMode mode)
        {
            _mBlendMode = mode;
        }


        public override void Draw(Canvas2D canvas)
        {
            canvas.BlendMode = _mBlendMode;
            

            var origin_dx = _mOriginX * Width;
            var origin_dy = _mOriginY * Height;

            var draw_q = _mQuad;

            draw_q.Set(X - origin_dx, Y - origin_dy, Width, Height);

            canvas.DrawQuad(in draw_q, _mTexture);

        }

        
    }
}
