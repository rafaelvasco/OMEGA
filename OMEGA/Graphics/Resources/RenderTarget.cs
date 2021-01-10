using System;

namespace OMEGA
{
    public class RenderTarget : GraphicsResource
    {
        public Texture2D Texture => internal_texture;

        public int Width => Texture.Width;

        public int Height => Texture.Height;

        internal readonly FrameBufferHandle handle;
        private Texture2D internal_texture;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget"/> struct.
        /// </summary>
        /// <param name="width">The width of the render target.</param>
        /// <param name="height">The height of the render target.</param>
        /// <param name="format">The format of the new surface.</param>
        /// <param name="flags">Texture sampling flags.</param>
        private RenderTarget(int width, int height, TextureFormat format, TextureFlags flags)
        {
            handle = GraphicsContext.CreateFrameBuffer(width, height, format, flags);
            internal_texture = new Texture2D(GraphicsContext.GetFrameBufferTexture(handle, 0), width, height, false, false);
        }

        private RenderTarget(Texture2D texture, TextureFormat format, TextureFlags flags)
        {
            handle = GraphicsContext.CreateFrameBuffer(texture, format, flags);
            internal_texture = texture;
        }

        public static RenderTarget Create(Texture2D texture)
        {
            var tex_flags = TextureFlags.ClampU | TextureFlags.ClampV;

            if (!texture.Filtered)
            {
                tex_flags |= TextureFlags.FilterPoint;
            }

            var render_target = new RenderTarget(texture, TextureFormat.BGRA8, tex_flags);

            GraphicsContext.RegisterAllocatedResource(render_target);

            return render_target;
        }

        public static RenderTarget Create(int width, int height, bool filtered)
        {
            var tex_flags = TextureFlags.ClampU | TextureFlags.ClampV | TextureFlags.RenderTarget ;

            if (!filtered)
            {
                tex_flags |= TextureFlags.FilterPoint;
            }

            var render_target = new RenderTarget(width, height, TextureFormat.BGRA8, tex_flags);

            GraphicsContext.RegisterAllocatedResource(render_target);

            return render_target;
        }

        ~RenderTarget()
        {
            if (handle.Valid)
            {
                GraphicsContext.DestroyFrameBuffer(handle);
            }

            throw new Exception("Graphics Resource Leak: FrameBuffer");
        }


        /// <summary>
        /// Releases the frame buffer.
        /// </summary>
        protected override void Dispose()
        {
            if (handle.Valid)
            {
                GC.SuppressFinalize(this);
                GraphicsContext.DestroyFrameBuffer(handle);
            }
        }
    }
}
