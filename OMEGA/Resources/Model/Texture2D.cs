
using System;

namespace OMEGA
{
    public class Texture2D : Resource, IEquatable<Texture2D>
    {
        internal readonly TextureHandle Handle;

        internal TextureFlags TexFlags;

        internal Pixmap Pixmap { get; private set; }

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

        internal Texture2D(IntPtr data, int width, int height, int bytes_per_pixel)
        {
            Pixmap = null;
            Width = width;
            Height = height;
            Tiled = false;
            Filtered = false;
            RenderTarget = false;
            UpdateTexFlags();
            Handle = GraphicsContext.CreateTexture2D(width, height, bytes_per_pixel, false, 0, TextureFormat.BGRA8, TexFlags, data);
        }

        internal Texture2D(Pixmap pixmap, bool tiled, bool filtered, bool render_target = false)
        {
            Pixmap = pixmap;
            Width = pixmap.Width;
            Height = pixmap.Height;
            Tiled = tiled;
            Filtered = filtered;
            RenderTarget = render_target;
            UpdateTexFlags();
            Handle = GraphicsContext.CreateDynamicTexture2D(pixmap.Width, pixmap.Height, false, 0, TextureFormat.BGRA8, TexFlags, pixmap.Data);
        }

        internal Texture2D(TextureHandle tex_handle, int width, int height, bool filtered, bool tiled)
        {
            Pixmap = null;
            this.Filtered = filtered;
            this.Tiled = tiled;
            this.Width = width;
            this.Height = height;
            this.RenderTarget = true;
            this.Handle = tex_handle;
            UpdateTexFlags();
        }

        public static Texture2D Create(int width, int height, Color fill_color, bool tiled = false, bool filtered = false)
        {
            var pixmap = new Pixmap(width, height, fill_color);

            return Create(pixmap, tiled, filtered);
        }

        public static Texture2D Create(Pixmap pixmap, bool tiled = false, bool filtered = false)
        {
            var texture = new Texture2D(pixmap, tiled, filtered);

            int id = Engine.Content.RegisterRuntimeLoaded(texture);

            texture.Id = $"Texture({id}) [{pixmap.Width},{pixmap.Height}]";

            return texture;
        }

        public static Texture2D Create(IntPtr data, int width, int height, int bytes_per_pixel)
        {
            var texture = new Texture2D(data, width, height, bytes_per_pixel);

            int id = Engine.Content.RegisterRuntimeLoaded(texture);

            texture.Id = $"Texture({id}) [{width},{height}]";

            return texture;
        }

        internal void ReloadPixels()
        {
            if (Pixmap == null)
            {
                return;
            }

            GraphicsContext.UpdateTexture2D(Handle, 0, 0, 0, 0, Width, Height, Pixmap.Data, Pixmap.Stride);
        }

        private void UpdateTexFlags()
        {
            var flags = BuildTexFlags(Tiled, Filtered, RenderTarget);

            this.TexFlags = flags;
        }

        private static TextureFlags BuildTexFlags(bool tiled, bool filtered, bool render_target)
        {
            var tex_flags = TextureFlags.None;

            if (!tiled) tex_flags = TextureFlags.ClampU | TextureFlags.ClampV;

            if (!filtered) tex_flags |= TextureFlags.FilterPoint;

            if (render_target)
            {
                tex_flags |= TextureFlags.RenderTarget;
            }

            return tex_flags;
        }

        protected override void FreeUnmanaged()
        {
            GraphicsContext.DestroyTexture(Handle);
        }

        protected override void FreeManaged()
        {
            if (Pixmap != null)
            {
                Pixmap.Dispose();
            }
        }

        public bool Equals(Texture2D other)
        {
            return Handle.idx == other.Handle.idx;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Texture2D);
        }

        public override int GetHashCode()
        {
            return Handle.idx.GetHashCode();
        }
    }
}
