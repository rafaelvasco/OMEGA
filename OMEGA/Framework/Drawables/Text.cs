using System.Text;

namespace OMEGA
{
    public class Text : Drawable
    {
        public string TextValue
        {
            get => _text.ToString();
            set
            {
                _text.Clear();
                _text.Append(value);

                MeasureText();
            }
        }

        public TextureFont Font
        {
            get => _font;
            set
            {
                _font = value;

                MeasureText();
            }
        }

        public Vec2 Scale
        {
            get => _scale;
            set => _scale = value;
        }

        private readonly StringBuilder _text = new();
        private TextureFont _font;
        private Color _color = Color.White;
        private Vec2 _size;
        private Vec2 _scale = Vec2.One;

        public override void Move(float dx, float dy)
        {
            X += dx;
            Y += dy;
        }

        public override void SetColor(Color color)
        {
            _color = color;
        }

        public override void SetPosition(float x, float y)
        {
            X = x;
            Y = y;
        }
        public override void SetSize(float w, float h)
        {
            _scale = new Vec2(w/_size.X, h/_size.Y);
        }

        private void MeasureText()
        {
            if (_font == null || _text.Length == 0)
            {
                _size = Vec2.Zero;
            }
            else
            {
                _size = _font.MeasureString(_text);
            }
        }

        public override void Draw(Canvas2D canvas)
        {
            if (_font == null || _text.Length == 0)
            {
                return;
            }

            canvas.SetFont(_font);

            canvas.DrawString(_text, new Vec2(X, Y), _color, _scale);
        }

        
    }
}
