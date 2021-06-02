using System;
using static Bgfx.Bgfx;

namespace OMEGA
{
    public class RenderTarget : GraphicsResource
    {
        public Texture2D Texture => _internalTexture;

        public int Width => Texture.Width;

        public int Height => Texture.Height;

        internal readonly FrameBufferHandle Handle;

        private readonly Texture2D _internalTexture;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderTarget"/> struct.
        /// </summary>
        /// <param name="width">The width of the render target.</param>
        /// <param name="height">The height of the render target.</param>
        /// <param name="format">The format of the new surface.</param>
        /// <param name="flags">Texture sampling flags.</param>
        private RenderTarget(int width, int height, TextureFormat format, SamplerFlags flags)
        {
            Handle = GraphicsContext.CreateFrameBuffer(width, height, format, flags);
            _internalTexture = new Texture2D(GraphicsContext.GetFrameBufferTexture(Handle, 0), width, height, false, false);
        }

        private RenderTarget(Texture2D texture, TextureFormat format, SamplerFlags flags)
        {
            Handle = GraphicsContext.CreateFrameBuffer(texture, format, flags);
            _internalTexture = texture;
        }

        public static RenderTarget Create(Texture2D texture)
        {
            var tex_flags = SamplerFlags.UClamp | SamplerFlags.VClamp;

            if (!texture.Filtered)
            {
                tex_flags |= SamplerFlags.Point;
            }

            var render_target = new RenderTarget(texture, TextureFormat.BGRA8, tex_flags);

            GraphicsContext.RegisterAllocatedResource(render_target);

            return render_target;
        }

        public static RenderTarget Create(int width, int height, bool filtered)
        {
            var tex_flags = SamplerFlags.UClamp | SamplerFlags.VClamp;

            if (!filtered)
            {
                tex_flags |= SamplerFlags.Point;
            }

            var render_target = new RenderTarget(width, height, TextureFormat.BGRA8, tex_flags);

            GraphicsContext.RegisterAllocatedResource(render_target);

            return render_target;
        }

        ~RenderTarget()
        {
            if (Handle.Valid)
            {
                GraphicsContext.DestroyFrameBuffer(Handle);
            }

            throw new Exception("Graphics Resource Leak: FrameBuffer");
        }


        /// <summary>
        /// Releases the frame buffer.
        /// </summary>
        protected override void Free()
        {
            base.Free();

            if (Handle.Valid)
            {
                GraphicsContext.DestroyFrameBuffer(Handle);
            }
        }
    }
}
