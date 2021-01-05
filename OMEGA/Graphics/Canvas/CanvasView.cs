using System;

namespace OMEGA
{
    public class CanvasView
    {
        public Color ClearColor
        {
            get => m_clear_color;
            set
            {
                if (m_clear_color != value)
                {
                    m_clear_color = value;
                    Applied = false;
                }
            }
        }
        public Vec2 SizeFactor
        {
            get => m_size_factor;
            set
            {
                if (m_size_factor != value)
                {
                    m_size_factor = value;
                    NeedsUpdateTransform = true;
                    Applied = false;
                }
            }
        }
        public RectF Viewport
        {
            get => m_viewport;
            set
            {
                m_viewport = value;
                Applied = false;
            }
        }

        internal bool Applied {get;set;}

        internal bool NeedsUpdateTransform {get;set;} = true;

        internal ushort ViewId {get;set;}

        private Vec2 m_size_factor;
        private RectF m_viewport;
        private Transform m_transform;
        private Transform m_inverse_transform;
        private bool m_inv_transform_updated;
        private Color m_clear_color;

        internal CanvasView(float size_factor_x, float size_factor_y)
        {
            m_viewport = RectF.FromBox(0f, 0f, 1f, 1f);
            m_size_factor = new Vec2(size_factor_x, size_factor_y);

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
