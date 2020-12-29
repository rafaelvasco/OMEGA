namespace OMEGA
{
    public abstract class Drawable
    {
        public float X { get; protected set;}

        public float Y { get; protected set;}

        public float Width { get; protected set;}

        public float Height { get; protected set;}

        public abstract void SetPosition(float x, float y);

        public abstract void SetSize(float w, float h);

        public abstract void Move(float dx, float dy);

        public abstract void SetColor(Color color);

        public bool Visible { get;set;} = true;

        public abstract void Draw(Canvas canvas);
    }
}
