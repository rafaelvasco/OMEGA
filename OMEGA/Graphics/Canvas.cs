using System;
using System.Runtime.InteropServices;

namespace OMEGA
{
    public unsafe class Canvas
    {
        public int MaxDrawCalls => max_draw_calls;

        public int Width => render_surface.Width;

        public int Height => render_surface.Height;

        public CanvasStretchMode StretchMode { get; set; } = CanvasStretchMode.PixelPerfect;

        public float RenderScaleX { get; private set; }

        public float RenderScaleY { get; private set; }

        private ShaderProgram canvas_shader;
        private ShaderProgram current_shader;
        private RenderTarget render_surface;
        private VertexStream<VertexPositionColorTexture> vertex_stream;
        private VertexStream<VertexPositionColorTexture> render_surface_vertex_stream;
        private Texture2D current_texture;

        private DrawPass render_surface_draw_pass;
        private DrawPass surface_to_screen_draw_pass;

        private Quad render_surface_quad;
        private Mat4 screen_proj_matrix;
        private int draw_calls;
        private int max_draw_calls;
        private bool render_area_changed = true;

        //private StateFlags prev_render_state;

        private const int MAX_DEFAULT_VERTICES = 2048;


        public Canvas(int width = 0, int height = 0)
        {
            InitRenderSurface(width > 0 ? width : Engine.DrawDevice.BackBufferWidth, height > 0 ? height : Engine.DrawDevice.BackBufferHeight);

            InitVertexStream();

            InitDefaultShader();

            OnScreenResized(Engine.DisplaySize.Width, Engine.DisplaySize.Height);

            InitDrawPasses();
        }

        private void InitRenderSurface(int width, int height)
        {
            render_surface = RenderTarget.Create(width, height, false);
        }

        private void InitDrawPasses()
        {
            // Render Surface Pass
            var render_surface_proj_matrix = new Mat4();

            Calc.MatOrtho(ref render_surface_proj_matrix, 0f, render_surface.Width, render_surface.Height, 0f, 0.0f, 1000.0f);

            render_surface_draw_pass = new DrawPass(
                view_port: Rect.FromBox(0, 0, render_surface.Width, render_surface.Height),
                projection_matrix: render_surface_proj_matrix,
                clear_color: Color.Orange,
                render_target: render_surface
            );

            Engine.DrawDevice.SetupDrawPass(0, render_surface_draw_pass);

            // Render Surface to Screen Pass

            surface_to_screen_draw_pass = new DrawPass(
                view_port: Rect.FromBox(0, 0, Engine.DrawDevice.BackBufferWidth, Engine.DrawDevice.BackBufferHeight),
                projection_matrix: screen_proj_matrix,
                clear_color: Color.Black
            );

            Engine.DrawDevice.SetupDrawPass(1, surface_to_screen_draw_pass);

        }

        private void InitDefaultShader()
        {
            canvas_shader = Engine.Content.Get<ShaderProgram>("canvas_shader");

            current_shader = canvas_shader;
        }

        private void InitVertexStream()
        {
            var index_array = new ushort[MAX_DEFAULT_VERTICES / 4 * 6];

            ushort indice_i = 0;

            for (var i = 0; i < index_array.Length; i += 6, indice_i += 4)
            {
                index_array[i + 0] = (ushort)(indice_i + 0);
                index_array[i + 1] = (ushort)(indice_i + 1);
                index_array[i + 2] = (ushort)(indice_i + 2);
                index_array[i + 3] = (ushort)(indice_i + 0);
                index_array[i + 4] = (ushort)(indice_i + 2);
                index_array[i + 5] = (ushort)(indice_i + 3);
            }

            vertex_stream = new VertexStream<VertexPositionColorTexture>(MAX_DEFAULT_VERTICES, index_array);

            render_surface_quad = new Quad(render_surface.Texture);

            render_surface_vertex_stream = VertexStream<VertexPositionColorTexture>.FromQuad(render_surface_quad, stream_mode: VertexStreamMode.Dynamic);
        }


        public void Begin()
        {
            draw_calls = 0;
            Engine.DrawDevice.SetShader(current_shader);
        }

        public void End()
        {
            RenderBatch();

            RenderSurface();
        }

        private void RenderBatch()
        {
            if (vertex_stream.VertexCount == 0)
            {
                return;
            }

            draw_calls++;

            if (draw_calls > max_draw_calls)
            {
                max_draw_calls = draw_calls;
            }

            var draw_device = Engine.DrawDevice;

            draw_device.SetTexture(current_shader, 0, current_texture);

            draw_device.SubmitVertexStream(draw_pass_index: 0, vertex_stream);

            vertex_stream.Reset();
        }

        private void RenderSurface()
        {
            if (render_area_changed)
            {
                render_area_changed = false;
                render_surface_vertex_stream.Reset();

                var vtx_fragment = render_surface_vertex_stream.GetNextVertexFragment(4);

                fixed (VertexPositionColorTexture* vtx_ptr = &MemoryMarshal.GetReference(vtx_fragment))
                {
                    *(vtx_ptr) = render_surface_quad.V0;
                    *(vtx_ptr+1) = render_surface_quad.V1;
                    *(vtx_ptr+2) = render_surface_quad.V2;
                    *(vtx_ptr+3) = render_surface_quad.V3;
                }
            }

            var draw_device = Engine.DrawDevice;

            draw_device.SetTexture(current_shader, 0, render_surface.Texture);

            draw_device.SubmitVertexStream(draw_pass_index: 1, render_surface_vertex_stream);
        }


        public void DrawQuad(Texture2D texture, ref Quad quad)
        {
            if (current_texture != texture)
            {
                RenderBatch();
                current_texture = texture;
            }

            var current_vertex_fragment = vertex_stream.GetNextVertexFragment(4);

            if (current_vertex_fragment == null)
            {
                RenderBatch();

                vertex_stream.Reset();
                current_vertex_fragment = vertex_stream.GetNextVertexFragment(4);
            }

            ref var v0 = ref quad.V0;
            ref var v1 = ref quad.V1;
            ref var v2 = ref quad.V2;
            ref var v3 = ref quad.V3;

            fixed(VertexPositionColorTexture* vertex_ptr = &MemoryMarshal.GetReference(current_vertex_fragment))
            {
                *(vertex_ptr) = new VertexPositionColorTexture(v0.X, v0.Y, v0.Z, v0.Col, v0.Tx, v0.Ty);
                *(vertex_ptr + 1) = new VertexPositionColorTexture(v1.X, v1.Y, v1.Z, v1.Col, v1.Tx, v1.Ty);
                *(vertex_ptr + 2) = new VertexPositionColorTexture(v2.X, v2.Y, v2.Z, v2.Col, v2.Tx, v2.Ty); ;
                *(vertex_ptr + 3) = new VertexPositionColorTexture(v3.X, v3.Y, v3.Z, v3.Col, v3.Tx, v3.Ty);
            }
        }

        internal void OnScreenResized(int width, int height)
        {
            var canvas_w = this.Width;
            var canvas_h = this.Height;

            Rect render_area = Rect.Empty;

            switch (StretchMode)
            {
                case CanvasStretchMode.PixelPerfect:

                    if (width > canvas_w || height > canvas_h)
                    {
                        var ar_origin = (float)canvas_w / canvas_h;
                        var ar_new = (float)width / height;

                        var scale_w = Calc.FloorToInt((float)width / canvas_w);
                        var scale_h = Calc.FloorToInt((float)height / canvas_h);

                        if (ar_new > ar_origin)
                            scale_w = scale_h;
                        else
                            scale_h = scale_w;

                        var margin_x = (width - canvas_w * scale_w) / 2;
                        var margin_y = (height - canvas_h * scale_h) / 2;

                        RenderScaleX = scale_w;
                        RenderScaleY = scale_h;

                        render_area = Rect.FromBox(margin_x, margin_y, canvas_w * scale_w, canvas_h * scale_h);
                    }
                    else
                    {
                        render_area = Rect.FromBox(0, 0, canvas_w, canvas_h);
                    }

                    break;
                case CanvasStretchMode.LetterBox:

                    if (width > canvas_w || height > canvas_h)
                    {
                        var ar_origin = (float)canvas_w / canvas_h;
                        var ar_new = (float)width / height;

                        var scale_w = (float)width / canvas_w;
                        var scale_h = (float)height / canvas_h;

                        if (ar_new > ar_origin)
                            scale_w = scale_h;
                        else
                            scale_h = scale_w;

                        var margin_x = (int)((width - canvas_w * scale_w) / 2);
                        var margin_y = (int)((height - canvas_h * scale_h) / 2);

                        RenderScaleX = scale_w;
                        RenderScaleY = scale_h;

                        render_area = Rect.FromBox(margin_x, margin_y, (int)(canvas_w * scale_w),
                            (int)(canvas_h * scale_h));
                    }
                    else
                    {
                        RenderScaleX = 1.0f;
                        RenderScaleY = 1.0f;
                        render_area = Rect.FromBox(0, 0, canvas_w, canvas_h);
                    }

                    break;
                case CanvasStretchMode.Stretch:

                    RenderScaleX = (float)width / canvas_w;
                    RenderScaleY = (float)height / canvas_h;
                    render_area = Rect.FromBox(0, 0, width, height);

                    break;
                case CanvasStretchMode.Resize:

                    //if (width == canvas_w && height == canvas_h)
                    //{
                    //    break;
                    //}

                    //RenderScaleX = 1.0f;
                    //RenderScaleY = 1.0f;

                    //render_area = Rect.FromBox(0, 0, width, height);

                    //main_surface.ResizeSurface(width, height);

                    break;
            }

            Console.WriteLine(
                $"Render Area: {render_area.X1}, {render_area.Y1}, {render_area.Width}, {render_area.Height}");

            Calc.MatOrtho(ref screen_proj_matrix, 0, width, height, 0, 0.0f, 1.0f);

            render_surface_quad.SetRect(render_area);
        }
    }
}
