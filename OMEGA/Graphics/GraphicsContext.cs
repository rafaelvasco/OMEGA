using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OMEGA
{
    internal static unsafe partial class GraphicsContext
    {
        static readonly int TextureFormatCount = Enum.GetValues(typeof(TextureFormat)).Length;

        public static RendererType RendererBackend { get; private set; }

        private static readonly List<GraphicsResource> allocated_graphics_resources = new List<GraphicsResource>(16);

        /* ==================================================================*/
        /* INITIALIZATION */
        /* ==================================================================*/

        public static void SetPlatformData(IntPtr WindowHandle)
        {
            unsafe
            {
                PlatformPtrData platformData = new PlatformPtrData();
                platformData.nwh = WindowHandle.ToPointer();
                Bgfx.set_platform_data(&platformData);
            }
        }

        public static Init Initialize(int resolutionWidth, int resolutionHeight, RendererType renderer_backend)
        {
            RendererBackend = renderer_backend;

            var result = new Init();
            InitPtrData init_data = new InitPtrData();
            unsafe
            {
                Bgfx.init_ctor(&init_data);
                init_data.type = renderer_backend;
                init_data.vendorId = (ushort)PciIdFlags.None;
                init_data.resolution.width = (uint)resolutionWidth;
                init_data.resolution.height = (uint)resolutionHeight;
                init_data.resolution.reset = (uint)ResetFlags.None;
                Bgfx.init(&init_data);
            }

            result.init = init_data;
            return result;
        }

        public static void Shutdown()
        {
            foreach (var resource in allocated_graphics_resources)
            {
                resource.Dispose(unregister: false);
            }

            Bgfx.shutdown();
        }

        /* ==================================================================*/
        /* GRAPHICS RESOURCE CREATION */
        /* ==================================================================*/

        public static IndexBufferHandle CreateIndexBuffer(ushort[] indices, BufferFlags flags = BufferFlags.None)
        {
            unsafe
            {
                var memory = GetMemoryBufferReference(indices);
                var index_buffer = Bgfx.create_index_buffer(memory, (ushort)flags);
                return index_buffer;
            }
        }

        public static VertexBufferHandle CreateVertexBuffer<T>(T[] vertices, VertexLayout layout, BufferFlags flags = BufferFlags.None) where T : struct, IVertexType
        {
            var memory = GetMemoryBufferReference(vertices);
            var vertex_buffer = Bgfx.create_vertex_buffer(memory, &layout.InternalHandle, (ushort)flags);
            return vertex_buffer;
        }

        public static DynamicVertexBufferHandle CreateDynamicVertexBuffer(int vertex_count, VertexLayout layout, BufferFlags flags = BufferFlags.None)
        {
            var dyn_vertex_buffer = Bgfx.create_dynamic_vertex_buffer((uint)vertex_count, &layout.InternalHandle, (ushort)flags);
            return dyn_vertex_buffer;
        }

        public static DynamicVertexBufferHandle CreateDynamicVertexBuffer<T>(T[] vertices, VertexLayout layout, BufferFlags flags = BufferFlags.None) where T : struct, IVertexType
        {
            var memory = GetMemoryBufferReference(vertices);
            var dyn_vertex_buffer = Bgfx.create_dynamic_vertex_buffer_mem(memory, &layout.InternalHandle, (ushort)flags);
            return dyn_vertex_buffer;
        }

        public static void UpdateDynamicVertexBuffer<T>(DynamicVertexBufferHandle handle, int start_vertex, T[] vertices)
        {
            var memory = GetMemoryBufferReference(vertices);
            Bgfx.update_dynamic_vertex_buffer(handle, (uint)start_vertex, memory);
        }

        public static ShaderProgram CreateShaderProgram(byte[] vertex_src, byte[] frag_src, string[] samplers, string[] _params)
        {
            if (vertex_src.Length == 0 || frag_src.Length == 0)
            {
                throw new Exception("Cannot load ShaderProgram with empty shader sources");
            }

            var vertex_shader = CreateShader(vertex_src);
            var frag_shader = CreateShader(frag_src);

            if (!vertex_shader.Valid || !frag_shader.Valid)
            {
                throw new Exception("Could not load shader correctly.");
            }

            var program = new ShaderProgram(CreateProgram(vertex_shader, frag_shader, true), samplers, _params);

            if (!program.Program.Valid)
            {
                throw new Exception("Could not load shader correctly.");
            }

            return program;
        }

        public static ShaderHandle CreateShader(byte[] bytes)
        {
            ShaderHandle shader;
            unsafe
            {
                var data = AllocGraphicsMemoryBuffer(bytes);
                shader = Bgfx.create_shader(data);
            }
            return shader;
        }

        public static ProgramHandle CreateProgram(ShaderHandle vertex, ShaderHandle fragment, bool destroyShader)
        {
            var program = Bgfx.create_program(vertex, fragment, destroyShader);
            return program;
        }

        public static UniformHandle CreateUniform(string name, UniformType type, int num_elements)
        {
            var uniform = Bgfx.create_uniform(name, type, (ushort)num_elements);
            return uniform;
        }

        public static TextureHandle CreateTexture2D(int width, int height, bool has_mips, int num_layers, TextureFormat tex_format, TextureFlags flags, byte[] pixel_data)
        {
            unsafe
            {
                Memory* data = AllocGraphicsMemoryBuffer(pixel_data);
                return Bgfx.create_texture_2d((ushort)width, (ushort)height, has_mips, (ushort)num_layers, tex_format, (ulong)flags, data);
            }
        }

        public static FrameBufferHandle CreateFrameBuffer(int width, int height, TextureFormat tex_format, TextureFlags tex_flags)
        {
            return Bgfx.create_frame_buffer((ushort)width, (ushort)height, tex_format, (ulong)tex_flags);
        }

        public static void AllocTransientVertexBuffer(out TransientVertexBuffer buffer, int vertex_count, ref VertexLayout layout)
        {
            Bgfx.alloc_transient_vertex_buffer(out buffer, (uint)vertex_count, ref layout.InternalHandle);
        }

        public static void AllocTransientIndexBuffer(out TransientIndexBuffer buffer, int num_indices)
        {
            Bgfx.alloc_transient_index_buffer(out buffer, (uint)num_indices);
        }

        public static void RegisterAllocatedResource(GraphicsResource resource)
        {
            allocated_graphics_resources.Add(resource);
        }

        public static void UnregisterAllocatedResource(GraphicsResource resource)
        {
            allocated_graphics_resources.Remove(resource);
        }

        public static TextureHandle GetFrameBufferTexture(FrameBufferHandle frame_buffer, byte attachment)
        {
            return Bgfx.get_texture(frame_buffer, attachment);
        }

        public static IntPtr MakeRef(IntPtr data, uint size)
        {
            unsafe
            {
                return new IntPtr(Bgfx.make_ref(data.ToPointer(), size));
            }
        }

        /* ==================================================================*/
        /* GRAPHICS RESOURCE DISPOSING */
        /* ==================================================================*/

        public static void DestroyTexture(TextureHandle texture)
        {
            Bgfx.destroy_texture(texture);
        }

        public static void DestroyIndexBuffer(IndexBufferHandle index_buffer)
        {
            Bgfx.destroy_index_buffer(index_buffer);
        }

        public static void DestroyDynamicIndexBuffer(DynamicIndexBufferHandle index_buffer)
        {
            Bgfx.destroy_dynamic_index_buffer(index_buffer);
        }

        public static void DestroyFrameBuffer(FrameBufferHandle frame_buffer)
        {
            Bgfx.destroy_frame_buffer(frame_buffer);
        }

        public static void DestroyVertexBuffer(VertexBufferHandle vertex_buffer)
        {
            Bgfx.destroy_vertex_buffer(vertex_buffer);
        }

        public static void DestroyDynamicVertexBuffer(DynamicVertexBufferHandle vertex_buffer)
        {
            Bgfx.destroy_dynamic_vertex_buffer(vertex_buffer);
        }

        public static void DestroyProgram(ProgramHandle shader_program)
        {
            Bgfx.destroy_program(shader_program);
        }

        public static void DestroyShader(ShaderHandle shader)
        {
            Bgfx.destroy_shader(shader);
        }

        public static void DestroyUniform(UniformHandle uniform)
        {
            Bgfx.destroy_uniform(uniform);
        }

        /* ==================================================================*/
        /* RENDERING */
        /* ==================================================================*/

        /* BUFFERS */
        /* ==================================================================*/

        public static void SetDynamicIndexBuffer(DynamicIndexBufferHandle index_buffer, int first_index, int num_indices)
        {
            Bgfx.set_dynamic_index_buffer(index_buffer, (uint)first_index, (uint)num_indices);
        }

        public static void SetDynamicVertexBuffer(byte stream, DynamicVertexBufferHandle vertex_buffer, int start_vertex, int num_vertices)
        {
            Bgfx.set_dynamic_vertex_buffer(stream, vertex_buffer, (uint)start_vertex, (uint)num_vertices);
        }

        public static void SetTransientVertexBuffer(byte stream, TransientVertexBuffer vertex_buffer, int start_vertex, int num_vertices)
        {
            unsafe
            {
                Bgfx.set_transient_vertex_buffer(stream, &vertex_buffer, (uint)start_vertex, (uint)num_vertices);
            }
        }

        public static void SetTransientIndexBuffer(TransientIndexBuffer index_buffer, int first_index, int num_indices)
        {
            unsafe
            {
                Bgfx.set_transient_index_buffer(&index_buffer, (uint)first_index, (uint)num_indices);
            }
        }

        public static void SetIndexBuffer(IndexBufferHandle index_buffer, int first_index, int num_indices)
        {
            Bgfx.set_index_buffer(index_buffer, (uint)first_index, (uint)num_indices);
        }

        public static void SetVertexBuffer(byte stream, VertexBufferHandle vertex_buffer, int start_vertex, int num_vertices)
        {
            Bgfx.set_vertex_buffer(stream, vertex_buffer, (uint)start_vertex, (uint)num_vertices);
        }

        public static void SetFrameBuffer(ushort view_id, FrameBufferHandle handle)
        {
            Bgfx.set_view_frame_buffer(view_id, handle);
        }

        public static int GetAvailableTransientVertexBuffers(int required_vertex_count, VertexLayout layout)
        {
            return (int)Bgfx.get_avail_transient_vertex_buffer((uint)required_vertex_count, ref layout.InternalHandle);
        }

        public static int GetAvailableTransientIndexBuffers(int num_indices)
        {
            return (int)Bgfx.get_avail_transient_index_buffer((uint)num_indices);
        }

        /* TEXTURE AND FRAMEBUFFER */
        /* ==================================================================*/
        public static void UpdateTexture2D<T>(TextureHandle texture, int layer, byte mip, int x, int y, int width, int height, T[] pixel_data, int pitch) where T : struct
        {
            unsafe
            {
                var data = GetMemoryBufferReference(pixel_data);
                Bgfx.update_texture_2d(texture, (ushort)layer, mip, (ushort)x, (ushort)y, (ushort)width, (ushort)height, data, (ushort)pitch);
            }
        }

        public static void SetTexture(byte tex_stage, UniformHandle sampler_uniform, TextureHandle texture, TextureFlags flags = (TextureFlags)uint.MaxValue)
        {
            Bgfx.set_texture(tex_stage, sampler_uniform, texture, (uint)flags);
        }

        /* SHADERS */
        /* ==================================================================*/

        public static void SetUniform(UniformHandle uniform, float value)
        {
            Bgfx.set_uniform(uniform, &value, 1);
        }

        public static void SetUniform(UniformHandle uniform, ref Vec4 value)
        {
            Bgfx.set_uniform(uniform, Unsafe.AsPointer(ref value), 1);
        }

        public static void Frame(bool capture = false)
        {
            unsafe
            {
                _ = Bgfx.frame(capture);
            }
        }

        public static void Reset(int width, int height, ResetFlags resetFlags)
        {
            Reset(width, height, resetFlags, (TextureFormat)TextureFormatCount);
        }

        public static void Reset(int width, int height, ResetFlags resetFlags, TextureFormat format)
        {
            unsafe
            {
                Bgfx.reset((uint)width, (uint)height, (uint)resetFlags, format);
            }
        }

        public static void SetViewMode(ushort view_id, ViewMode view_mode)
        {
            Bgfx.set_view_mode(view_id, view_mode);
        }

        public static void SetViewClear(ushort view_id, ClearFlags flags, uint color, float depth = 0.0f, byte stencil = 1)
        {
            Bgfx.set_view_clear(view_id, (ushort)flags, color, depth, stencil);
        }

        public static void SetViewRect(ushort view_id, int x, int y, int resolutionWidth, int resolutionHeight)
        {
            Bgfx.set_view_rect(view_id, (ushort)x, (ushort)y, (ushort)resolutionWidth, (ushort)resolutionHeight);
        }

        public static void SetViewProjection(ushort view_id, ref Mat4 view, ref Mat4 projection)
        {
            Bgfx.set_view_transform(
                view_id,
                Unsafe.AsPointer(ref view.M11),
                Unsafe.AsPointer(ref projection.M11)
            );
        }

        public static void Touch(ushort view_id)
        {
            Bgfx.touch(view_id);
        }

        public static void SetModelTransform(ref Mat4 transform)
        {
            Bgfx.set_transform(
                Unsafe.AsPointer(ref transform.M11),
                1
            );
        }

        /* RENDER */
        /* ==================================================================*/

        public static void SetState(StateFlags state, uint rgba = 0)
        {
            Bgfx.set_state((ulong)state, rgba);
        }

        public static void SetScissor(int x, int y, int width, int height)
        {
            Bgfx.set_scissor((ushort)x, (ushort)y, (ushort)width, (ushort)height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StateFlags STATE_BLEND_FUNC(StateFlags src, StateFlags dst)
        {
            return STATE_BLEND_FUNC_SEPARATE(src, dst, src, dst);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StateFlags STATE_BLEND_FUNC_SEPARATE(StateFlags srcRGB, StateFlags dstRGB, StateFlags srcA, StateFlags dstA)
        {
            return (StateFlags)((((ulong)(srcRGB) | ((ulong)(dstRGB) << 4))) | (((ulong)(srcA) | ((ulong)(dstA) << 4)) << 8));
        }

        public static void Submit(ushort viewId, ProgramHandle program, uint depth = 0, bool preserve_state = false)
        {
            Bgfx.submit(viewId, program, depth, (byte)(preserve_state ? 1 : 0));
        }

        /* DEBUB */
        /* ==================================================================*/

        public static void SetDebug(DebugFlags flag)
        {
            Bgfx.set_debug((uint)flag);
        }

        /// <summary>
        /// Writes debug text to the screen.
        /// </summary>
        /// <param name="x">The X position, in cells.</param>
        /// <param name="y">The Y position, in cells.</param>
        /// <param name="foreColor">The foreground color of the text.</param>
        /// <param name="backColor">The background color of the text.</param>
        /// <param name="format">The format of the message.</param>
        /// <param name="args">The arguments with which to format the message.</param>
        public static void DebugTextClear(DebugColor color = DebugColor.Black, bool smallText = false)
        {
            var attr = (byte)((byte)color << 4);
            Bgfx.dbg_text_clear(attr, smallText);
        }


        /// <summary>
        /// Writes debug text to the screen.
        /// </summary>
        /// <param name="x">The X position, in cells.</param>
        /// <param name="y">The Y position, in cells.</param>
        /// <param name="foreColor">The foreground color of the text.</param>
        /// <param name="backColor">The background color of the text.</param>
        /// <param name="message">The message to write.</param>
        public static void DebugTextWrite(int x, int y, DebugColor foreColor, DebugColor backColor, string message)
        {
            var attr = (byte)(((byte)backColor << 4) | (byte)foreColor);
            Bgfx.dbg_text_printf((ushort)x, (ushort)y, attr, "%s", message);
        }

        /// <summary>
        /// Draws data directly into the debug text buffer.
        /// </summary>
        /// <param name="x">The X position, in cells.</param>
        /// <param name="y">The Y position, in cells.</param>
        /// <param name="width">The width of the image to draw.</param>
        /// <param name="height">The height of the image to draw.</param>
        /// <param name="data">The image data bytes.</param>
        /// <param name="pitch">The pitch of each line in the image data.</param>
        public static void DebugTextImage(int x, int y, int width, int height, IntPtr data, int pitch)
        {
            Bgfx.dbg_text_image((ushort)x, (ushort)y, (ushort)width, (ushort)height, data, (ushort)pitch);
        }

        /// <summary>
        /// Draws data directly into the debug text buffer.
        /// </summary>
        /// <param name="x">The X position, in cells.</param>
        /// <param name="y">The Y position, in cells.</param>
        /// <param name="width">The width of the image to draw.</param>
        /// <param name="height">The height of the image to draw.</param>
        /// <param name="data">The image data bytes.</param>
        /// <param name="pitch">The pitch of each line in the image data.</param>
        public static void DebugTextImage(int x, int y, int width, int height, byte[] data, int pitch)
        {
            fixed (byte* ptr = data)
                Bgfx.dbg_text_image((ushort)x, (ushort)y, (ushort)width, (ushort)height, new IntPtr(ptr), (ushort)pitch);
        }

        /* UTILS */
        /* ==================================================================*/
        private static unsafe Memory* AllocGraphicsMemoryBuffer<T>(T[] array)
        {
            var size = (uint)(array.Length * Unsafe.SizeOf<T>());
            var data = Bgfx.alloc(size);
            Unsafe.CopyBlock(data->data, Unsafe.AsPointer(ref array[0]), size);
            return data;
        }

        private static unsafe Memory* GetMemoryBufferReference<T>(T[] array)
        {
            var size = (uint)(array.Length * Unsafe.SizeOf<T>());
            var data = Bgfx.make_ref(Unsafe.AsPointer(ref array[0]), size);
            return data;
        }
    }
}
