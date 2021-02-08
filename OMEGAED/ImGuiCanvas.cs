using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ImGuiNET;
using OMEGA;

namespace OMEGAED
{
    public class ImGuiCanvas : IDisposable
    {
        private Texture2D m_font_atlas;

        private readonly Dictionary<string, ImFontPtr> m_fonts = new Dictionary<string, ImFontPtr>();
        private readonly Dictionary<IntPtr, Texture2D> m_textures = new Dictionary<IntPtr, Texture2D>();

        private ShaderProgram m_imgui_program;
        private ShaderProgram m_imgui_tex_program;

        private VertexLayout m_vertex_layout;
        
        private IntPtr m_imgui_context;

        private IntPtr m_font_atlas_tex_id;

        private CanvasView m_canvas_view;

        private VertexStream m_vertex_stream;

        private List<int> m_mapped_keys = new List<int>();

        private int m_scroll_wheel_value;


        public ImGuiCanvas()
        {
            m_imgui_context = ImGui.CreateContext();

            InitGraphics();

            InitInput();
        }

        private void InitGraphics()
        {
            var io = ImGui.GetIO();

            io.DisplaySize = new System.Numerics.Vector2(Engine.Canvas.Width, Engine.Canvas.Height);

            m_imgui_program = Engine.Content.Get<ShaderProgram>("imgui_shader");
            m_imgui_tex_program = Engine.Content.Get<ShaderProgram>("imgui_shader_image");

            m_vertex_layout = new VertexLayout();
            m_vertex_layout.Begin();
            m_vertex_layout.Add(Attrib.Position, AttribType.Float, 2, false, false);
            m_vertex_layout.Add(Attrib.TexCoord0, AttribType.Float, 2, false, false);
            m_vertex_layout.Add(Attrib.Color0, AttribType.Uint8, 4, true, false);
            m_vertex_layout.End();

            m_fonts.Add("default", io.Fonts.AddFontDefault());

            unsafe
            {
                io.Fonts.GetTexDataAsRGBA32(out IntPtr data, out var width, out var height, out var bytes_per_pixel);

                m_font_atlas = Texture2D.Create(data, width, height, bytes_per_pixel);

                m_textures.Add((IntPtr)m_font_atlas.GetHashCode(), m_font_atlas);
            }

            m_font_atlas_tex_id = (IntPtr)m_font_atlas.GetHashCode();

            io.Fonts.SetTexID(m_font_atlas_tex_id);

            m_canvas_view = Engine.Canvas.CreateView(Color.Transparent);

            m_vertex_stream = new VertexStream(VertexStreamMode.Stream);

            m_vertex_stream.SetVertexLayout(m_vertex_layout);
        }

        private void InitInput()
        {
            var io = ImGui.GetIO();

            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Back);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.Space] = (int)Keys.Space);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y);
            m_mapped_keys.Add(io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z);

        }
        public void Begin()
        {
            ImGui.NewFrame();
        }

        public void Update()
        {
            var io = ImGui.GetIO();

            for (int i = 0; i < m_mapped_keys.Count; ++i)
            {
                io.KeysDown[m_mapped_keys[i]] = Input.KeyDown((Keys)m_mapped_keys[i]);
            }

            io.KeyShift = Input.KeyDown(Keys.LeftShift) || Input.KeyDown(Keys.RightShift);
            io.KeyCtrl = Input.KeyDown(Keys.LeftControl) || Input.KeyDown(Keys.RightControl);
            io.KeyAlt = Input.KeyDown(Keys.LeftAlt) || Input.KeyDown(Keys.RightAlt);

            io.MousePos = new System.Numerics.Vector2(Input.MousePos.X, Input.MousePos.Y);
            
            io.MouseDown[0] = Input.MouseLeftDown();
            io.MouseDown[1] = Input.MouseRightDown();
            io.MouseDown[2] = Input.MouseMiddleDown();

            var scroll_delta = Input.MouseWheel - m_scroll_wheel_value;

            io.MouseWheel = scroll_delta > 0 ? 1 : scroll_delta < 0 ? -1 : 0;

            m_scroll_wheel_value = Input.MouseWheel;
        }

        public void RefreshSize()
        {
            var io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(Engine.Canvas.Width, Engine.Canvas.Height);
            var imgui_width = io.DisplaySize.X;
            var imgui_height = io.DisplaySize.Y;
            m_canvas_view.Viewport = Rect.FromBox(0, 0, (int)imgui_width, (int)imgui_height);
        }

        public void End(float elapsed)
        {
            var io = ImGui.GetIO();
            io.DeltaTime = elapsed;
            
            ImGui.EndFrame();
            ImGui.Render();

            Draw(ImGui.GetDrawData());
        }

        private unsafe void Draw(ImDrawDataPtr draw_data)
        {
            var canvas = Engine.Canvas;

            canvas.Begin(m_canvas_view);

            for (int i = 0; i < draw_data.CmdListsCount; ++i)
            {
                var draw_list = draw_data.CmdListsRange[i];
                var num_vertices = draw_list.VtxBuffer.Size;
                var num_indices = draw_list.IdxBuffer.Size;
                
                m_vertex_stream.Reset();
                m_vertex_stream.PushVertices(draw_list.VtxBuffer.Data, draw_list.IdxBuffer.Data, num_vertices * Unsafe.SizeOf<ImDrawVert>(), num_indices * sizeof(short));

                var offset = 0;

                for (int cmd_index = 0; cmd_index < draw_list.CmdBuffer.Size; ++cmd_index)
                {
                    var cmd = draw_list.CmdBuffer[cmd_index];
                    var current_tex = m_font_atlas;
                    var current_program = m_imgui_program;

                    if (cmd.UserCallback == IntPtr.Zero && cmd.ElemCount != 0)
                    {

                        if (cmd.TextureId != m_font_atlas_tex_id)
                        {
                            current_tex = m_textures[cmd.TextureId];
                        }

                        var clip_x = (int)Math.Max(cmd.ClipRect.X, 0.0f);
                        var clip_y = (int)Math.Max(cmd.ClipRect.Y, 0.0f);
                        var clip_x2 = (int)Math.Min(cmd.ClipRect.Z, 65535.0f);
                        var clip_y2 = (int)Math.Min(cmd.ClipRect.W, 65535.0f);

                        canvas.SetShaderProgram(current_program);
                        canvas.SetScissor(clip_x, clip_y, clip_x2 - clip_x, clip_y2 - clip_y);
                        canvas.SetTexture(0, current_tex);
                        

                        canvas.SubmitVertexStream(
                            m_canvas_view.ViewId,
                            m_vertex_stream,
                            0,
                            num_vertices,
                            offset,
                            (int)cmd.ElemCount
                        );
                    }

                    offset += (int) cmd.ElemCount;
                }
            }

            canvas.End();

        }

        public void Dispose()
        {
            ImGui.DestroyContext(m_imgui_context);
        }
    }
}
