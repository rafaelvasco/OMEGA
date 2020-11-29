namespace OMEGA
{
    public struct DrawPass
    {
        public Rect Viewport;

        public Mat4 ProjectionMatrix;

        public Mat4 ViewMatrix;

        public Color ClearColor;

        public float ClearDepth;

        public RenderTarget RenderTarget;

        public DrawPass(
            Rect? view_port = null,
            Mat4? projection_matrix  = null,
            Mat4? view_matrix  = null,
            Color? clear_color = null,
            float clear_depth = 0.0f,
            RenderTarget render_target = null
        )
        {
            this.Viewport = view_port ?? Rect.FromBox(0, 0, Engine.DrawDevice.BackBufferWidth, Engine.DrawDevice.BackBufferHeight);
            this.ProjectionMatrix = projection_matrix ?? Mat4.Identity;
            this.ViewMatrix =  view_matrix ?? Mat4.Identity;
            this.ClearColor = clear_color ?? Color.Black;
            this.ClearDepth = clear_depth;
            this.RenderTarget = render_target;
            
        }
    }
}
