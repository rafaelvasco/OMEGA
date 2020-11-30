
using System;

namespace OMEGA
{
    public class DrawDevice : IDisposable
    {
        public int BackBufferWidth { get; private set; }
        public int BackBufferHeight { get; private set; }

        internal StateFlags RenderState => render_state;

        public BlendMode BlendMode
        {
            get => blend_mode;
            set
            {
                if (blend_mode == value)
                {
                    return;
                }

                blend_mode = value;

                state_flags_changed = true;

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
                           .STATE_BLEND_FUNC_SEPARATE(
                               StateFlags.BlendOne,
                               StateFlags.BlendInvSrcAlpha,
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

                cull_mode = value;

                state_flags_changed = true;

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

                UpdateRenderState();
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

                if (blend_mode != BlendMode.Solid && blend_mode != BlendMode.Mask)
                {
                    return;
                }

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

                UpdateRenderState();
            }
        }

        private bool state_flags_changed = true;

        private ShaderProgram current_shader;

        private BlendMode blend_mode;

        private CullMode cull_mode;

        private DepthTest depth_test;

        private StateFlags blend_state;

        private StateFlags depth_state;

        private StateFlags cull_state;

        private StateFlags render_state;

        private DebugFlags debug_flags = DebugFlags.None;

        internal DrawDevice(IntPtr window_surface_handle, int back_buffer_width, int back_buffer_height)
        {
            try
            {
                BackBufferWidth = back_buffer_width;

                BackBufferHeight = back_buffer_height;

                GraphicsContext.SetPlatformData(window_surface_handle);

                GraphicsContext.Initialize(back_buffer_width, back_buffer_height, RendererType.Direct3D11);

                GraphicsContext.SetViewClear(view_id: 0, flags: ClearFlags.Color, color: Color.Black);
                GraphicsContext.SetViewMode(view_id: 0, ViewMode.Sequential);

                GraphicsContext.Touch(0);

                GraphicsContext.Reset(back_buffer_width, back_buffer_height, ResetFlags.Vsync);

                GraphicsContext.SetViewRect(view_id: 0, 0, 0, back_buffer_width, back_buffer_height);

                BlendMode = BlendMode.Alpha;
                CullMode = CullMode.None;

                UpdateRenderState();

            }
            catch (Exception e)
            {
                throw new GraphicsDeviceException(e.Message);
            }
        }

        public void SetupDrawPass(ushort draw_pass_index, DrawPass draw_pass)
        {
            GraphicsContext.SetViewMode(draw_pass_index, ViewMode.Sequential);

            if (draw_pass.RenderTarget != null)
            {
                GraphicsContext.SetFrameBuffer(draw_pass_index, draw_pass.RenderTarget.handle);
            }

            GraphicsContext.SetViewClear(draw_pass_index, ClearFlags.Color | ClearFlags.Depth, draw_pass.ClearColor.RGBA, draw_pass.ClearDepth);
            GraphicsContext.Touch(draw_pass_index);
            GraphicsContext.SetViewRect(draw_pass_index, draw_pass.Viewport.X1, draw_pass.Viewport.Y1, draw_pass.Viewport.Width, draw_pass.Viewport.Height);
            GraphicsContext.SetViewProjection(draw_pass_index, ref draw_pass.ViewMatrix, ref draw_pass.ProjectionMatrix);

        }

        public void SetShader(ShaderProgram shader)
        {
            current_shader = shader;
        }

        public void SetScissor(int x, int y, int w, int h)
        {
            GraphicsContext.SetScissor(x, y, w, h);
        }

        public void SetModelTransform(ref Mat4 transform)
        {
            GraphicsContext.SetModelTransform(ref transform);
        }

        public void SubmitVertexStream<T>(
            ushort draw_pass_index, 
            VertexStream<T> vertex_stream, 
            int start_vertex_index, 
            int vertex_count, 
            int start_indice_index = 0, 
            int index_count = 0) where T : struct, IVertexType
        {
            if (current_shader == null)
            {
                return;
            }

            if (state_flags_changed)
            {
                GraphicsContext.SetState(render_state);
                state_flags_changed = false;
            }

            current_shader.Submit();

            vertex_stream.SubmitSpan(start_vertex_index, vertex_count, start_indice_index, index_count);

            GraphicsContext.Submit(draw_pass_index, current_shader.Program);
        }

        public void SubmitVertexStream<T>(ushort draw_pass_index, VertexStream<T> vertex_stream) where T : struct, IVertexType
        {
            if (current_shader == null)
            {
                return;
            }

            if (state_flags_changed)
            {
                GraphicsContext.SetState(render_state);
                state_flags_changed = false;
            }

            current_shader.Submit();
            vertex_stream.Submit();
            GraphicsContext.Submit(draw_pass_index, current_shader.Program);
        }

        public void SetTexture(ShaderProgram program, int slot, Texture2D texture)
        {
            GraphicsContext.SetTexture(0, program.Samplers[slot], texture.Handle, texture.TexFlags);
        }

        public void ApplyVideoMode(int backbuffer_width, int backbuffer_height, bool fullscreen)
        {
            BackBufferWidth = backbuffer_width;
            BackBufferHeight = backbuffer_height;

            GraphicsContext.Reset(backbuffer_width, backbuffer_height, ResetFlags.Vsync);
            GraphicsContext.SetViewRect(view_id: 0, 0, 0, backbuffer_width, backbuffer_height);

            Platform.SetVideoMode(backbuffer_width, backbuffer_height, fullscreen);
        }

        public void EnableDebugText(bool enable)
        {
            if (enable)
            {
                debug_flags |= DebugFlags.Text;
            }
            else
            {
                debug_flags &= ~DebugFlags.Text;
            }

            GraphicsContext.SetDebug(debug_flags);
        }

        public void EnableDebugWireFrame(bool enable)
        {
            if (enable)
            {
                debug_flags |= DebugFlags.Wireframe;
            }
            else
            {
                debug_flags &= ~DebugFlags.Wireframe;
            }

            GraphicsContext.SetDebug(debug_flags);
        }

        public void DebugTextClear(DebugColor bg_color, bool small_text = false)
        {
            GraphicsContext.DebugTextClear(bg_color, small_text);
        }

        public void DebugTextWrite(int x, int y, DebugColor fore_color, DebugColor back_color, string message)
        {
            GraphicsContext.DebugTextWrite(x, y, fore_color, back_color, message);
        }

        public void DebugTextImage(int x, int y, int width, int height, byte[] data, int pitch)
        {
            GraphicsContext.DebugTextImage(x, y, width, height, data, pitch);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            GraphicsContext.Shutdown();
        }

        public void Frame()
        {
            GraphicsContext.Frame();
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
    }
}
