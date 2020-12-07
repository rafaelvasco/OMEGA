namespace OMEGA
{
    public class CanvasView
    {
        public Color ClearColor {get;set;}

        internal bool Applied {get;set;}

        internal ushort ViewId {get;set;}

        public Vec2 Center => m_center;
        public Vec2 Size => m_size;

        public float Rotation => m_rotation;

        public RectF Viewport => m_viewport;

        private Vec2 m_center;
        private Vec2 m_size;
        private float m_rotation;
        private RectF m_viewport;
        private Transform m_transform;
        private Transform m_inverse_transform;
        private bool m_transform_updated;
        private bool m_inv_transform_updated;

        internal CanvasView()
        {
            m_viewport = RectF.FromBox(0f, 0f, 1f, 1f);
            Reset(RectF.FromBox(0, 0, Engine.DisplaySize.Width, Engine.DisplaySize.Height));
        }

        public void SetCenter(float x, float y)
        {
            m_center.X = x;
            m_center.Y = y;

            m_transform_updated = false;
            m_inv_transform_updated = false;

            Applied = false;
        }

        public void SetCenter(Vec2 center)
        {
            SetCenter(center.X, center.Y);
        }

        public void SetSize(float width, float height)
        {
            m_size.X = width;
            m_size.Y = height;
            m_transform_updated = false;
            m_inv_transform_updated = false;
            Applied = false;
        }

        public void SetSize(Vec2 size)
        {
            SetSize(size.X, size.Y);
        }

        public void SetRotation(float angle)
        {
            m_rotation = angle % 360.0f;

            if (m_rotation < 0)
            {
                m_rotation += 360.0f;
            }

            m_transform_updated = false;
            m_inv_transform_updated = false;
            Applied = false;
        }

        public void SetViewport(RectF viewport)
        {
            m_viewport = viewport;
            Applied = false;
        }

        public void Move(float offset_x, float offset_y)
        {
            SetCenter(m_center.X + offset_x, m_center.Y + offset_y);
        }

        public void Move(Vec2 offset)
        {
            SetCenter(m_center + offset);
        }

        public void Rotate(float angle)
        {
            SetRotation(m_rotation + angle);
        }

        public void Zoom(float factor)
        {
            SetSize(m_size.X * factor, m_size.Y * factor);
        }

        public ref readonly Transform GetTransform()
        {
            if (!m_transform_updated)
            {
                float angle = Calc.ToRadians(m_rotation);
                float cos = Calc.Cos(angle);
                float sin = Calc.Sin(angle);
                float tx = -m_center.X * cos - m_center.Y * sin + m_center.X;
                float ty = m_center.Y * sin - m_center.Y * cos + m_center.Y;

                // Projection Components

                float a = 2f / m_size.X;
                float b = -2f / m_size.Y;
                float c = -a * m_center.X;
                float d = -b * m_center.Y;

                m_transform = new Transform(
                    a * cos, a * sin, a * tx + c,
                   -b * sin, b * cos, b * ty + d,
                    0f,      0f,      1f
                );

                m_transform_updated = true;
            }

            return ref m_transform;
        }

        public ref readonly Transform GetInverseTransform()
        {
            if (!m_inv_transform_updated)
            {
                m_inverse_transform = GetTransform().GetInverse();
                m_inv_transform_updated = true;
            }

            return ref m_inverse_transform;
        }

        public void Reset(RectF rect)
        {
            m_center = new Vec2(rect.X1 + rect.Width/2f, rect.Y1 + rect.Height / 2f);
            m_size = new Vec2(rect.Width, rect.Height);
            m_rotation = 0f;

            m_transform_updated = false;
            m_inv_transform_updated = false;
        }

    }
}
