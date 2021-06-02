using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static Bgfx.Bgfx;

namespace OMEGA
{
    internal static unsafe class GraphicsContext
    {
        private static readonly List<GraphicsResource> AllocatedGraphicsResources = new(16);

        public static RendererType RendererBackend { get; private set; }

        public static void SetPlatformData(IntPtr windowHandle)
        {
            PlatformData platformData = new()
            {
                nwh = windowHandle.ToPointer()
            };
            set_platform_data(&platformData);
        }

        public static Init Initialize(int resolutionWidth, int resolutionHeight, RendererType renderer)
        {
            Init init = new();

            RendererBackend = renderer;
            
            init_ctor(&init);
            init.type = renderer;
            init.vendorId = (ushort)PciIdFlags.None;
            init.resolution.width = (uint)resolutionWidth;
            init.resolution.height = (uint)resolutionHeight;
            init.resolution.reset = (uint)ResetFlags.None;
            Bgfx.Bgfx.init(&init);


            return init;
        }

        public static IntPtr MakeRef(IntPtr data, uint size)
        {
            return new(make_ref(data.ToPointer(), size));
        }

        public static void Frame(bool capture = false)
        {
            frame(capture);
        }

        public static void Reset(int width, int height, ResetFlags resetFlags)
        {
            reset((uint)width, (uint)height, (uint)resetFlags, TextureFormat.BGRA8);
        }
        
        public static void SetDebug(DebugFlags flag)
        {
            set_debug((uint)flag);
        }

        public static void SetViewMode(ushort viewId, ViewMode sequential)
        {
            set_view_mode(viewId, sequential);
        }

        public static void SetViewClear(ushort viewId, ClearFlags flags, uint color, float depth = 0.0f, byte stencil = 1)
        {
            set_view_clear(viewId, (ushort)flags, color, depth, stencil);
        }

        public static void Touch(ushort i)
        {
            touch(i);
        }

        public static void SetViewRect(ushort viewId, int x, int y, int resolutionWidth, int resolutionHeight)
        {
            set_view_rect(viewId, (ushort)x, (ushort)y, (ushort)resolutionWidth, (ushort)resolutionHeight);
        }

        public static void SetViewProjection(ushort viewId, ref Mat4 view, ref Mat4 projection)
        {
            set_view_transform(
                viewId,
                Unsafe.AsPointer(ref view.M11),
                Unsafe.AsPointer(ref projection.M11)
            );
        }

        public static void SetProjection(ushort viewId, ref float projection)
        {
            set_view_transform(viewId, null, Unsafe.AsPointer(ref projection));
        }

        public static void SetViewTransform(ushort viewId, float[] view, float[] projection)
        {
            set_view_transform(viewId, view != null ? Unsafe.AsPointer(ref view[0]) : null, projection != null ? Unsafe.AsPointer(ref projection[0]) : null);
        }

        public static TransientVertexBuffer CreateTransientVertexBuffer(Span<VertexPositionColorTexture> vertices, VertexLayout layout)
        {
            var transient_vtx_buffer = new TransientVertexBuffer();

            alloc_transient_vertex_buffer(&transient_vtx_buffer, (uint)vertices.Length, &layout.InternalHandle);

            var data_size = (uint)(vertices.Length * Unsafe.SizeOf<VertexPositionColorTexture>());

            Unsafe.CopyBlock(transient_vtx_buffer.data, Unsafe.AsPointer(ref vertices[0]), data_size);

            return transient_vtx_buffer;
        }

        public static TransientIndexBuffer CreateTransientIndexBuffer(Span<ushort> indices)
        {
            var transient_idx_buffer = new TransientIndexBuffer();

            alloc_transient_index_buffer(&transient_idx_buffer, (uint)indices.Length, false);

            var data_size = (uint)(indices.Length * sizeof(ushort));

            Unsafe.CopyBlock(transient_idx_buffer.data, Unsafe.AsPointer(ref indices[0]), data_size);

            return transient_idx_buffer;
        }

        public static ShaderProgram CreateShaderProgram(byte[] vertexSrc, byte[] fragSrc, string[] samplers, string[] @params)
        {
            if (vertexSrc.Length == 0 || fragSrc.Length == 0)
            {
                throw new Exception("Cannot load ShaderProgram with empty shader sources");
            }

            var vertex_shader = CreateShader(vertexSrc);
            var frag_shader = CreateShader(fragSrc);

            if (!vertex_shader.Valid || !frag_shader.Valid)
            {
                throw new Exception("Could not load shader correctly.");
            }

            var program = new ShaderProgram(CreateProgram(vertex_shader, frag_shader, true), samplers, @params);

            if (!program.Program.Valid)
            {
                throw new Exception("Could not load shader correctly.");
            }

            return program;
        }

        private static ShaderHandle CreateShader(byte[] bytes)
        {
            ShaderHandle shader;
            var data = AllocGraphicsMemoryBuffer(bytes);
            shader = create_shader(data);
            return shader;
        }

        private static ProgramHandle CreateProgram(ShaderHandle vertex, ShaderHandle fragment, bool destroyShader)
        {
            var program = create_program(vertex, fragment, destroyShader);
            return program;
        }

        public static UniformHandle CreateUniform(string name, UniformType type, int numElements)
        {
            var uniform = create_uniform(name, type, (ushort)numElements);
            return uniform;
        }

        public static TextureHandle CreateTexture2D(ushort width, ushort height, bool hasmips, ushort layers, TextureFormat bgra8, SamplerFlags flags, IntPtr makeRef)
        {
            return create_texture_2d(width, height, hasmips, layers, bgra8, (ulong)flags, (Memory*)makeRef.ToPointer());
        }

        public static void DestroyTexture2D(Texture2D texture)
        {
            destroy_texture(texture.Handle);
        }

        public static void UpdateTexture2D(Texture2D texture, int layer, byte mip, int x, int y, int width, int height, byte[] pixelData, int pitch)
        {
            var data = GetMemoryBufferReference(pixelData);
            update_texture_2d(texture.Handle, (ushort)layer, mip, (ushort)x, (ushort)y, (ushort)width, (ushort)height, data, (ushort)pitch);
        }

        public static IndexBufferHandle CreateIndexBuffer(ushort[] indices, BufferFlags flags = BufferFlags.None)
        {
            var memory = GetMemoryBufferReference(indices);
            var index_buffer = create_index_buffer(memory, (ushort)flags);
            return index_buffer;
        }

        public static void DestroyIndexBuffer(IndexBufferHandle indexBuffer)
        {
            destroy_index_buffer(indexBuffer);
        }

        public static VertexBufferHandle CreateVertexBuffer(VertexPositionColorTexture[] vertices, VertexLayout layout, BufferFlags flags = BufferFlags.None)
        {
            var memory = GetMemoryBufferReference(vertices);
            var vertex_buffer = create_vertex_buffer(memory, &layout.InternalHandle, (ushort)flags);
            return vertex_buffer;
        }

        public static void DestroyVertexBuffer(VertexBufferHandle vertexBuffer)
        {
            destroy_vertex_buffer(vertexBuffer);
        }

        public static DynamicIndexBufferHandle CreateDynamicIndexBuffer(int indexCount, BufferFlags flags = BufferFlags.None)
        {
            var dyn_index_buffer = create_dynamic_index_buffer((uint)indexCount, (ushort)flags);
            return dyn_index_buffer;
        }

        public static DynamicIndexBufferHandle CreateDynamicIndexBuffer(ushort[] indices, BufferFlags flags = BufferFlags.None)
        {
            var memory = GetMemoryBufferReference(indices);
            var dyn_index_buffer = create_dynamic_index_buffer_mem(memory, (ushort)flags);
            return dyn_index_buffer;
        }

        public static void DestroyDynamicIndexBuffer(DynamicIndexBufferHandle indexBuffer)
        {
            destroy_dynamic_index_buffer(indexBuffer);
        }

        public static void UpdateDynamicIndexBuffer(DynamicIndexBufferHandle handle, int startIndex, ushort[] indices)
        {
            var memory = GetMemoryBufferReference(indices);
            update_dynamic_index_buffer(handle, (uint)startIndex, memory);
        }

        public static DynamicVertexBufferHandle CreateDynamicVertexBuffer(int vertexCount, VertexLayout layout, BufferFlags flags = BufferFlags.None)
        {
            var dyn_vertex_buffer = create_dynamic_vertex_buffer((uint)vertexCount, &layout.InternalHandle, (ushort)flags);
            return dyn_vertex_buffer;
        }

        public static DynamicVertexBufferHandle CreateDynamicVertexBuffer(VertexPositionColorTexture[] vertices, VertexLayout layout, BufferFlags flags = BufferFlags.None)
        {
            var memory = GetMemoryBufferReference(vertices);
            var dyn_vertex_buffer = create_dynamic_vertex_buffer_mem(memory, &layout.InternalHandle, (ushort)flags);
            return dyn_vertex_buffer;
        }

        public static void UpdateDynamicVertexBuffer(DynamicVertexBufferHandle handle, int startVertex, VertexPositionColorTexture[] vertices)
        {
            var memory = GetMemoryBufferReference(vertices);
            update_dynamic_vertex_buffer(handle, (uint)startVertex, memory);
        }

        public static void DestroyDynamicVertexBuffer(DynamicVertexBufferHandle vertexBuffer)
        {
            destroy_dynamic_vertex_buffer(vertexBuffer);
        }

        public static TextureHandle CreateTexture2D(int width, int height, bool hasMips, int numLayers, TextureFormat texFormat, SamplerFlags flags, byte[] pixelData)
        {
            Memory* data = AllocGraphicsMemoryBuffer(pixelData);

            TextureHandle tex = create_texture_2d((ushort)width, (ushort)height, hasMips, (ushort)numLayers, texFormat, (ulong)flags, data);
            return tex;
        }

        public static TextureHandle CreateTexture2D(int width, int height, int bytesPerPixel, bool hasMips, int numLayers, TextureFormat texFormat, SamplerFlags flags, IntPtr pixelData)
        {
            var data = AllocGraphicsMemoryBuffer(pixelData, width * height * bytesPerPixel);
            TextureHandle tex = create_texture_2d((ushort)width, (ushort)height, hasMips, (ushort)numLayers, texFormat, (ulong)flags, data);
            return tex;
        }

        public static TextureHandle CreateTexture2D<T>(int width, int height, bool hasMips, int numLayers, TextureFormat texFormat, SamplerFlags flags, T[] pixelBytes) where T : struct
        {
            var data = AllocGraphicsMemoryBuffer(pixelBytes);
            TextureHandle tex = create_texture_2d((ushort)width, (ushort)height, hasMips, (ushort)numLayers, texFormat, (ulong)flags, data);
            return tex;
        }

        public static TextureHandle CreateDynamicTexture2D(int width, int height, bool hasMips, int numLayers, TextureFormat texFormat, SamplerFlags flags, byte[] pixelData)
        {
            Memory* data = AllocGraphicsMemoryBuffer(pixelData);
            TextureHandle tex = create_texture_2d((ushort)width, (ushort)height, hasMips, (ushort)numLayers, texFormat, (ulong)flags, null);
            update_texture_2d(tex, 0, 0, 0, 0, (ushort)width, (ushort)height, data, ushort.MaxValue);
            return tex;
        }

        public static FrameBufferHandle CreateFrameBuffer(int width, int height, TextureFormat texFormat, SamplerFlags texFlags)
        {
            return create_frame_buffer((ushort)width, (ushort)height, texFormat, (ulong)texFlags);
        }

        public static FrameBufferHandle CreateFrameBuffer(Texture2D texture, TextureFormat texFormat, SamplerFlags texFlags)
        {
            var attachment = stackalloc Attachment[1];

            attachment->handle = texture.Handle;
            attachment->access = Access.ReadWrite;
            attachment->layer = 0;
            attachment->mip = 0;
            attachment->resolve = 0;

            return create_frame_buffer_from_attachment(1, attachment, false);
        }

        public static void DestroyFrameBuffer(FrameBufferHandle frameBuffer)
        {
            destroy_frame_buffer(frameBuffer);
        }

        public static void SetDynamicIndexBuffer(DynamicIndexBuffer indexBuffer, int firstIndex, int numIndices)
        {
            set_dynamic_index_buffer(indexBuffer.Handle, (uint)firstIndex, (uint)numIndices);
        }

        public static void SetDynamicVertexBuffer(byte stream, DynamicVertexBuffer vertexBuffer, int startVertex, int numVertices)
        {
            set_dynamic_vertex_buffer(stream, vertexBuffer.Handle, (uint)startVertex, (uint)numVertices);
        }

        public static void SetTransientVertexBuffer(byte stream, TransientVertexBuffer vertexBuffer, int startVertex, int numVertices)
        {
            set_transient_vertex_buffer(stream, &vertexBuffer, (uint)startVertex, (uint)numVertices);
        }

        public static void SetTransientIndexBuffer(TransientIndexBuffer indexBuffer, int firstIndex, int numIndices)
        {
            set_transient_index_buffer(&indexBuffer, (uint)firstIndex, (uint)numIndices);
        }

        public static void SetIndexBuffer(IndexBuffer indexBuffer, int firstIndex, int numIndices)
        {
            set_index_buffer(indexBuffer.Handle, (uint)firstIndex, (uint)numIndices);
        }

        public static void SetVertexBuffer(byte stream, VertexBuffer vertexBuffer, int startVertex, int numVertices)
        {
            set_vertex_buffer(stream, vertexBuffer.Handle, (uint)startVertex, (uint)numVertices);
        }

        public static void SetFrameBuffer(ushort viewId, FrameBufferHandle handle)
        {
            set_view_frame_buffer(viewId, handle);
        }

        public static int GetAvailableTransientVertexBuffers(int requiredVertexCount, VertexLayout layout)
        {
            return (int)get_avail_transient_vertex_buffer((uint)requiredVertexCount, (Bgfx.Bgfx.VertexLayout*)Unsafe.AsPointer(ref layout.InternalHandle));
        }

        public static int GetAvailableTransientIndexBuffers(int numIndices)
        {
            return (int)get_avail_transient_index_buffer((uint)numIndices, false);
        }

        private static Memory* AllocGraphicsMemoryBuffer<T>(T[] array)
        {
            var size = (uint)(array.Length * Unsafe.SizeOf<T>());
            var data = alloc(size);
            Unsafe.CopyBlockUnaligned(data->data, Unsafe.AsPointer(ref array[0]), size);
            return data;
        }

        private static Memory* AllocGraphicsMemoryBuffer(IntPtr dataPtr, int dataSize)
        {
            var data = alloc((uint)dataSize);
            Unsafe.CopyBlockUnaligned(data->data, dataPtr.ToPointer(), (uint)dataSize);
            return data;
        }

        private static Memory* GetMemoryBufferReference<T>(T[] array)
        {
            var size = (uint)(array.Length * Unsafe.SizeOf<T>());
            var data = make_ref(Unsafe.AsPointer(ref array[0]), size);
            return data;
        }
     
        public static TextureHandle GetFrameBufferTexture(FrameBufferHandle frameBuffer, byte attachment)
        {
            return get_texture(frameBuffer, attachment);
        }

        public static void SetState(StateFlags state)
        {
            set_state((ulong)state, 0);
        }

        public static void SetTexture(byte textureUnit, UniformHandle uniform, TextureHandle texture, TextureFlags flags = (TextureFlags)uint.MaxValue)
        {
            set_texture(textureUnit, uniform, texture, (uint)flags);
        }

        public static void SetTransientVertexBuffer(byte stream, ref TransientVertexBuffer tvb, uint startVertex,
            uint numVertices)
        {
            set_transient_vertex_buffer(stream, (TransientVertexBuffer*)Unsafe.AsPointer(ref tvb), startVertex, numVertices);
        }

        public static void SetTransientIndexBuffer(ref TransientIndexBuffer tib, uint firstIndex, uint numIndices)
        {
            set_transient_index_buffer((TransientIndexBuffer*)Unsafe.AsPointer(ref tib), firstIndex, numIndices);
        }

        public static void SetScissor(int x, int y, int width, int height)
        {
            set_scissor((ushort)x, (ushort)y, (ushort)width, (ushort)height);
        }

        public static void Submit(ushort viewId, ProgramHandle program, uint depth = 0, bool preserveState = false)
        {
            submit(viewId, program, depth, preserveState ? (byte)1 : (byte)0);
        }

        public static void RegisterAllocatedResource(GraphicsResource resource)
        {
            AllocatedGraphicsResources.Add(resource);
        }

        public static void UnregisterAllocatedResource(GraphicsResource resource)
        {
            AllocatedGraphicsResources.Remove(resource);
        }

        public static void SetUniform(UniformHandle uniform, float value)
        {
            set_uniform(uniform, &value, 1);
        }

        public static void SetUniform(UniformHandle uniform, ref Vec4 value)
        {
            set_uniform(uniform, Unsafe.AsPointer(ref value), 1);
        }

        public static void DestroyProgram(ProgramHandle shaderProgram)
        {
            destroy_program(shaderProgram);
        }

        public static void DestroyShader(ShaderHandle shader)
        {
            destroy_shader(shader);
        }

        public static void DestroyUniform(UniformHandle uniform)
        {
            destroy_uniform(uniform);
        }

        public static void Shutdown()
        {
            foreach (var resource in AllocatedGraphicsResources)
            {
                resource.Dispose();
            }

            shutdown();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StateFlags STATE_BLEND_FUNC(StateFlags src, StateFlags dst)
        {
            return STATE_BLEND_FUNC_SEPARATE(src, dst, src, dst);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StateFlags STATE_BLEND_FUNC_SEPARATE(StateFlags srcRgb, StateFlags dstRgb, StateFlags srcA, StateFlags dstA)
        {
            return (StateFlags)((((ulong)(srcRgb) | ((ulong)(dstRgb) << 4))) | (((ulong)(srcA) | ((ulong)(dstA) << 4)) << 8));
        }
    }
}
