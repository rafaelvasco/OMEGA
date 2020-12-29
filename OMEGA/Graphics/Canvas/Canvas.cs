using System;
using System.Collections.Generic;

namespace OMEGA
{
    public class Canvas
    {
        internal bool NeedsResetDisplay { get; set; } = false;

        private static ushort StaticViewId = 0;

        public CanvasView DefaultView => base_view;

        public int Width { get; private set; }

        public int Height { get; private set; }

        public (int Width, int Height) Size => (Width, Height);

        public CanvasStretchMode StretchMode { get; set; } = CanvasStretchMode.LetterBox;

        public int MaxDrawCalls => max_draw_calls;

        public BlendMode BlendMode
        {
            get => blend_mode;
            set
            {
                if (blend_mode == value)
                {
                    return;
                }

                Flush();

                blend_mode = value;

                blend_mode = value;

                switch (blend_mode)
                {
                    case BlendMode.Solid:
                        blend_state = 0x0;
                        break;
                    case BlendMode.Mask:

                        blend_state =
                            StateFlags.BlendAlphaToCoverage;

                        break;
                    case BlendMode.Alpha:

                        blend_state = GraphicsContext
                           .STATE_BLEND_FUNC_SEPARATE(
                                StateFlags.BlendSrcAlpha,
                                StateFlags.BlendInvSrcAlpha,
                                StateFlags.BlendOne,
                                StateFlags.BlendInvSrcAlpha
                       );

                        break;

                    case BlendMode.AlphaPre:

                        blend_state = GraphicsContext
                           .STATE_BLEND_FUNC(
                               StateFlags.BlendOne,
                               StateFlags.BlendInvSrcAlpha
                       );

                        break;

                    case BlendMode.Add:

                        blend_state = GraphicsContext
                           .STATE_BLEND_FUNC_SEPARATE(
                               StateFlags.BlendSrcAlpha,
                               StateFlags.BlendOne,
                               StateFlags.BlendOne,
                               StateFlags.BlendOne
                       );

                        break;

                    case BlendMode.Light:

                        blend_state = GraphicsContext
                           .STATE_BLEND_FUNC_SEPARATE(
                               StateFlags.BlendDstColor,
                               StateFlags.BlendOne,
                               StateFlags.BlendZero,
                               StateFlags.BlendOne
                       );

                        break;

                    case BlendMode.Multiply:
                        blend_state = GraphicsContext.STATE_BLEND_FUNC(StateFlags.BlendDstColor, StateFlags.BlendZero);
                        break;

                    case BlendMode.Invert:
                        blend_state = GraphicsContext.STATE_BLEND_FUNC(StateFlags.BlendInvDstColor, StateFlags.BlendInvSrcColor);
                        break;
                }

                UpdateRenderState();
            }
        }

        public CullMode CullMode
        {
            get => cull_mode;

            set
            {
                if (cull_mode == value)
                {
                    return;
                }

                Flush();

                cull_mode = value;

                switch (cull_mode)
                {

                    case CullMode.ClockWise:
                        cull_state = StateFlags.CullCw;
                        break;
                    case CullMode.CounterClockWise:
                        cull_state = StateFlags.CullCcw;
                        break;
                    case CullMode.None:
                        cull_state = StateFlags.None;
                        break;
                }
            }
        }

        public DepthTest DepthTest
        {
            get => depth_test;
            set
            {
                if (depth_test == value)
                {
                    return;
                }

                //if (blend_mode != BlendMode.Solid && blend_mode != BlendMode.Mask)
                //{
                //    return;
                //}

                Flush();

                depth_test = value;

                switch (depth_test)
                {
                    case DepthTest.Always:
                        depth_state = StateFlags.WriteZ | StateFlags.DepthTestAlways;
                        break;
                    case DepthTest.Equal:
                        depth_state = StateFlags.WriteZ | StateFlags.DepthTestEqual;
                        break;
                    case DepthTest.GreaterOrEqual:
                        depth_state = StateFlags.WriteZ | StateFlags.DepthTestGequal;
                        break;
                    case DepthTest.Greater:
                        depth_state = StateFlags.WriteZ | StateFlags.DepthTestGreater;
                        break;
                    case DepthTest.LessOrEqual:
                        depth_state = StateFlags.WriteZ | StateFlags.DepthTestLequal;
                        break;
                    case DepthTest.Less:
                        depth_state = StateFlags.WriteZ | StateFlags.DepthTestLess;
                        break;
                    case DepthTest.Never:
                        depth_state = StateFlags.DepthTestNever;
                        break;
                    case DepthTest.NotEqual:
                        depth_state = StateFlags.WriteZ | StateFlags.DepthTestNotequal;
                        break;
                    default:
                        break;
                }
            }
        }

        private const int MAX_BATCH_VERTICES = 2048;

        private VertexStream m_free_vertex_stream;
        private VertexStream m_quad_vertex_stream;

        private bool inside_begin_block = false;
        private int draw_calls;
        private int max_draw_calls;
        private BlendMode blend_mode;
        private CullMode cull_mode;
        private DepthTest depth_test;
        private StateFlags blend_state;
        private StateFlags depth_state;
        private StateFlags cull_state;
        private StateFlags render_state;
        private ShaderProgram current_shader;
        private ShaderProgram base_shader;
        private Texture2D base_texture;
        private Texture2D current_quads_texture;
        private Texture2D current_free_vertices_texture;
        private CanvasView base_view;
        private List<CanvasView> additional_views = new List<CanvasView>();
        private CanvasView current_view;

        internal Canvas(int width, int height)
        {
            Width = width;
            Height = height;

            InitVertexStreams();

            InitDefaultResources();

            InitDefaultView();

            ApplyViewProperties(base_view);

            BlendMode = BlendMode.Alpha;

            DepthTest = DepthTest.LessOrEqual;

            CullMode = CullMode.None;

        }

        public CanvasView CreateView()
        {
            var view = new CanvasView(1f, 1f)
            {
                ViewId = StaticViewId++,
                ClearColor = base_view?.ClearColor ?? Color.CornflowerBlue,
            };

            if (view.ViewId > 0)
            {
                additional_views.Add(view);
            }

            return view;
        }

        public void Begin(CanvasView view = null)
        {
            inside_begin_block = true;

            current_view = view ?? base_view;

            if (!current_view.Applied)
            {
                ApplyViewProperties(current_view);
            }
        }

        private void Flush()
        {
            RenderFreeVerticesBatch();
            RenderQuadsBatch();

        }

        public void End()
        {
            if (!inside_begin_block)
            {
                return;
            }

            Flush();
            ResetRenderState();
            inside_begin_block = false;
        }

        public void DrawVertices(Span<Vertex> vertices, Texture2D texture = null)
        {
            if (!inside_begin_block)
            {
                throw new Exception("Cannot call drawing methods in Canvas outside of Begin End Block");
            }

            texture ??= base_texture;

            if (current_free_vertices_texture != texture)
            {
                RenderFreeVerticesBatch();
                current_free_vertices_texture = texture;
            }

            if (!m_free_vertex_stream.PutVertices(vertices))
            {
                RenderFreeVerticesBatch();
                m_free_vertex_stream.Reset();

                if (!m_free_vertex_stream.PutVertices(vertices))
                {
                    throw new Exception("Free Vertices Batch Overflow");
                }
            }
        }

        public void DrawTextureQuad(in Quad quad, Texture2D texture = null)
        {
            if (!inside_begin_block)
            {
                throw new Exception("Cannot call drawing methods in Canvas outside of Begin End Block");
            }

            texture ??= base_texture;

            if (current_quads_texture != texture)
            {
                RenderQuadsBatch();
                current_quads_texture = texture;
            }

            if (!m_quad_vertex_stream.PutQuad(in quad))
            {
                RenderQuadsBatch();
                m_quad_vertex_stream.Reset();

                if (!m_quad_vertex_stream.PutQuad(in quad))
                {
                    throw new Exception("Quad Batch Overflow");
                }
            }
        }


        internal void Frame()
        {
            GraphicsContext.Frame();
        }

        private void ApplyCanvasStretchModeForBaseView()
        {
            var canvas_size = Size;
            var display_size = Platform.GetDisplaySize();

            switch (StretchMode)
            {
                case CanvasStretchMode.LetterBox:
                    {
                        float display_ratio = (float)display_size.Width / display_size.Height;
                        float view_ratio = (float)canvas_size.Width / canvas_size.Height;

                        float new_view_port_w = 1;
                        float new_view_port_h = 1;
                        float new_view_port_x = 0;
                        float new_view_port_y = 0;

                        bool horizontal_spacing = true;

                        if (display_ratio < view_ratio)
                        {
                            horizontal_spacing = false;
                        }

                        // If horizontal_spacing is true, black bars will appear on the left and right.
                        // Otherwise, they will appear on top and bottom

                        if (horizontal_spacing)
                        {
                            new_view_port_w = view_ratio / display_ratio;
                            new_view_port_x = (1 - new_view_port_w) / 2f;
                        }
                        else
                        {
                            new_view_port_h = display_ratio / view_ratio;
                            new_view_port_y = (1 - new_view_port_h) / 2f;
                        }

                        base_view.SetViewport(RectF.FromBox(new_view_port_x, new_view_port_y, new_view_port_w, new_view_port_h));
                    }
                    break;
                case CanvasStretchMode.Stretch:
                case CanvasStretchMode.Resize:
                    base_view.NeedsUpdateTransform = true;
                    break;
                default:
                    break;
            }

            base_view.Applied = false;
        }

        internal void HandleDisplayChange()
        {
            var display_size = Platform.GetDisplaySize();

            GraphicsContext.Reset(display_size.Width, display_size.Height, ResetFlags.Vsync);

            if (StretchMode == CanvasStretchMode.Resize)
            {
                Width = display_size.Width;
                Height = display_size.Height;
            }
            else
            {
                Width = Engine.RunningGame.GameInfo.ResolutionWidth;
                Height = Engine.RunningGame.GameInfo.ResolutionHeight;
            }

            ApplyCanvasStretchModeForBaseView();

            ApplyViewProperties(base_view);

            if (additional_views.Count > 0)
            {
                foreach (var view in additional_views)
                {
                    view.NeedsUpdateTransform = true;
                    ApplyViewProperties(view);
                }
            }

            NeedsResetDisplay = false;
        }

        private void InitDefaultView()
        {
            base_view = CreateView();

            base_view.ClearColor = Color.CornflowerBlue;

            current_view = base_view;
        }



        private void ApplyViewProperties(CanvasView view)
        {
            Console.WriteLine("Apply View");

            Rect viewport = GetViewPort(view);

            Console.WriteLine($"Viewport: {viewport}");

            GraphicsContext.SetViewRect(view.ViewId, viewport.X1, viewport.Y1, viewport.Width, viewport.Height);
            GraphicsContext.SetViewClear(view.ViewId, ClearFlags.Color | ClearFlags.Depth, view.ClearColor.RGBA);

            var projection = view.GetTransform(Width, Height);

            GraphicsContext.SetProjection(view.ViewId, ref projection.M0);

            view.Applied = true;
        }

        private Rect GetViewPort(CanvasView view)
        {
            var screen_size = Platform.GetDisplaySize();

            var view_viewport = view.Viewport;
            var base_view_viewport = base_view.Viewport;

            var default_view_viewport = Rect.FromBox
            (
                (int)(0.5f + screen_size.Width * base_view_viewport.X1),
                (int)(0.5f + screen_size.Height * base_view_viewport.Y1),
                (int)(0.5f + screen_size.Width * base_view_viewport.Width),
                (int)(0.5f + screen_size.Height * base_view_viewport.Height)
            );

            if (view.ViewId == 0)
            {
                return default_view_viewport;
            }
            else
            {
                return Rect.FromBox
                (
                    (int)(0.5f + default_view_viewport.Width * view_viewport.X1 + default_view_viewport.X1),
                    (int)(0.5f + default_view_viewport.Height * view_viewport.Y1 + default_view_viewport.Y1),
                    (int)(0.5f + default_view_viewport.Width * view_viewport.Width),
                    (int)(0.5f + default_view_viewport.Height * view_viewport.Height)
                );
            }
        }

        private void InitVertexStreams()
        {
            /* Quad Vertex Stream */

            var index_array = new ushort[MAX_BATCH_VERTICES / 4 * 6];

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

            m_quad_vertex_stream = new VertexStream(MAX_BATCH_VERTICES, index_array);

            /* Free Layout Vertex Stream */

            m_free_vertex_stream = new VertexStream(MAX_BATCH_VERTICES);
        }

        private void InitDefaultResources()
        {
            base_shader = Engine.Content.Get<ShaderProgram>("canvas_shader");

            current_shader = base_shader;

            base_texture = Texture2D.Create(Pixmap.OnePixel(Color.White));

            current_quads_texture = base_texture;
        }

        private void RenderFreeVerticesBatch()
        {
            if (m_free_vertex_stream.VertexCount == 0)
            {
                return;
            }

            draw_calls++;

            if (draw_calls > max_draw_calls)
            {
                max_draw_calls = draw_calls;
            }

            SetTexture(0, current_free_vertices_texture);

            GraphicsContext.SetState(render_state);

            SubmitVertexStream(render_pass: current_view.ViewId, m_free_vertex_stream);

            m_free_vertex_stream.Reset();
        }

        private void RenderQuadsBatch()
        {
            if (m_quad_vertex_stream.VertexCount == 0)
            {
                return;
            }

            draw_calls++;

            if (draw_calls > max_draw_calls)
            {
                max_draw_calls = draw_calls;
            }

            SetTexture(0, current_quads_texture);

            GraphicsContext.SetState(render_state);

            SubmitVertexStream(render_pass: current_view.ViewId, m_quad_vertex_stream);

            m_quad_vertex_stream.Reset();
        }

        private void SubmitVertexStream(
          ushort render_pass,
          VertexStream vertex_stream,
          int start_vertex_index,
          int vertex_count,
          int start_indice_index = 0,
          int index_count = 0)
        {
            if (current_shader == null)
            {
                return;
            }

            current_shader.Submit();

            vertex_stream.SubmitSpan(start_vertex_index, vertex_count, start_indice_index, index_count);

            GraphicsContext.Submit(render_pass, current_shader.Program);
        }

        private void SubmitVertexStream(ushort render_pass, VertexStream vertex_stream)
        {
            if (current_shader == null)
            {
                return;
            }

            current_shader.Submit();

            vertex_stream.Submit();
            GraphicsContext.Submit(render_pass, current_shader.Program);
        }

        private void SetTexture(int slot, Texture2D texture)
        {
            current_quads_texture = texture;
            GraphicsContext.SetTexture(0, current_shader.Samplers[slot], texture.Handle, texture.TexFlags);
        }
        private void UpdateRenderState()
        {
            render_state =
                StateFlags.WriteRgb |
                StateFlags.WriteA |
                StateFlags.WriteZ |
                blend_state |
                depth_state |
                cull_state;
        }

        private void ResetRenderState()
        {
            BlendMode = BlendMode.Alpha;
            CullMode = CullMode.None;
            DepthTest = DepthTest.LessOrEqual;
        }
    }
}
