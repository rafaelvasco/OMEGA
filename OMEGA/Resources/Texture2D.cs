
using System;

namespace OMEGA
{
    public class Texture2D : Resource, IEquatable<Texture2D>
    {
        internal readonly TextureHandle Handle;

        internal TextureFlags TexFlags;

        public int Width { get; protected set; }

        public int Height { get; protected set; }

        public static uint PixelSizeInBytes => sizeof(byte) * 4;

        public bool Tiled
        {
            get => tiled;
            set
            {
                if (tiled == value) return;

                tiled = value;

                UpdateTexFlags();
            }
        }

        public bool Filtered
        {
            get => filtered;
            set
            {
                if (filtered == value) return;

                filtered = value;

                UpdateTexFlags();
            }
        }

        public bool RenderTarget
        {
            get => render_target;
            set
            {
                if (render_target == value)
                {
                    return;
                }

                render_target = value;

                UpdateTexFlags();
            }
        }


        private bool tiled;

        private bool filtered;

        private bool render_target = false;

        internal Texture2D(Pixmap pixmap, bool tiled, bool filtered, bool render_target = false)
        {
            Width = pixmap.Width;
            Height = pixmap.Height;
            Tiled = tiled;
            Filtered = filtered;
            RenderTarget = render_target;
            Handle = GraphicsContext.CreateTexture2D(pixmap.Width, pixmap.Height, false, 0, TextureFormat.BGRA8, TexFlags, pixmap.Data);
        }

        internal Texture2D(TextureHandle tex_handle, int width, int height, bool filtered, bool tiled)
        {
            this.Filtered = filtered;
            this.Tiled = tiled;
            this.Width = width;
            this.Height = height;
            this.RenderTarget = true;
            this.Handle = tex_handle;
        }

        public static Texture2D Create(Pixmap pixmap, bool tiled, bool filtered)
        {
            var texture = new Texture2D(pixmap, tiled, filtered);

            Engine.Content.RegisterRuntimeLoaded(texture);

            return texture;
        }

        //public void SetData(Pixmap pixmap)
        //{
        //    Game.Instance.GraphicsContext.UpdateTextureData(this, pixmap);
        //}

        //public Pixmap GetData()
        //{
        //    return new Pixmap(this);
        //}

        //public Pixmap GetData(int srcX, int srcY, int srcW, int srcH)
        //{
        //    return new Pixmap(this, srcX, srcY, srcW, srcH);
        //}

        //public void BlitTo(Texture2D texture, int srcX, int srcY, int srcW, int srcH)
        //{
        //    this.Texture.BlitTo(0, texture.Texture, 0, 0, srcX, srcY, srcW, srcH);
        //}


        private void UpdateTexFlags()
        {
            var flags = BuildTexFlags(Tiled, Filtered);

            this.TexFlags = flags;
        }

        private static TextureFlags BuildTexFlags(bool tiled, bool filtered)
        {
            var tex_flags = TextureFlags.None;

            if (!tiled) tex_flags = TextureFlags.ClampU | TextureFlags.ClampV;

            if (!filtered) tex_flags |= TextureFlags.FilterPoint;

            return tex_flags;
        }

        protected override void FreeUnmanaged()
        {
            GraphicsContext.DestroyTexture(Handle);
        }

        public bool Equals(Texture2D other)
        {
            return Handle.idx == other.Handle.idx;
        }
    }
}
