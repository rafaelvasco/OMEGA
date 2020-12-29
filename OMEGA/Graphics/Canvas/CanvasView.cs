using System;

namespace OMEGA
{
    public class CanvasView
    {
        public Color ClearColor {get;set;}

        internal bool Applied {get;set;}

        internal bool NeedsUpdateTransform {get;set;} = true;

        internal ushort ViewId {get;set;}

        public Vec2 SizeFactor => m_size_factor;

        public RectF Viewport => m_viewport;

        private Vec2 m_size_factor;
        private RectF m_viewport;
        private Transform m_transform;
        private Transform m_inverse_transform;
        private bool m_inv_transform_updated;

        internal CanvasView(float size_factor_x, float size_factor_y)
        {
            m_viewport = RectF.FromBox(0f, 0f, 1f, 1f);
            m_size_factor = new Vec2(size_factor_x, size_factor_y);

        }

        public void SetSize(float width_factor, float height_factor)
        {
            m_size_factor = new Vec2(width_factor, height_factor);
            NeedsUpdateTransform = true;
            Applied = false;
        }

        public void SetSize(Vec2 size_factor)
        {
            SetSize(size_factor.X, size_factor.Y);
        }

        public void SetViewport(RectF viewport)
        {
            m_viewport = viewport;
            Applied = false;
        }

        internal ref readonly Transform GetTransform(int canvas_width, int canvas_height)
        {
            if (NeedsUpdateTransform)
            {
                float size_w = canvas_width * m_size_factor.X;
                float size_h = canvas_height * m_size_factor.Y;

                float center_x = canvas_width / 2.0f;
                float center_y = canvas_height / 2.0f;

                // Projection Components

                float a = 2f / size_w;
                float b = -2f / size_h;
                float c = -a * center_x;
                float d = -b * center_y;

                m_transform = new Transform(
                    a,  0f, c,
                    0f, b,  d,
                    0f, 0f, 1f
                );

                NeedsUpdateTransform = false;
                m_inv_transform_updated = false;

                Console.WriteLine("Update View Transform");
                    
            }

            return ref m_transform;
        }

        public ref readonly Transform GetInverseTransform()
        {
            if (!m_inv_transform_updated)
            {
                m_inverse_transform = GetTransform(Engine.Canvas.Width, Engine.Canvas.Height).GetInverse();
                m_inv_transform_updated = true;
            }

            return ref m_inverse_transform;
        }
    }
}
