
using System;
using static Bgfx.Bgfx;

namespace OMEGA
{
    public class Texture2D : Resource, IEquatable<Texture2D>
    {
        internal readonly TextureHandle Handle;

        internal SamplerFlags SamplerFlags;

        public Pixmap Pixmap { get; }

        public int Width { get; protected set; }

        public int Height { get; protected set; }

        public static uint PixelSizeInBytes => sizeof(byte) * 4;

        public bool Tiled
        {
            get => _tiled;
            set
            {
                if (_tiled == value) return;

                _tiled = value;

                UpdateTexFlags();
            }
        }

        public bool Filtered
        {
            get => _filtered;
            set
            {
                if (_filtered == value) return;

                _filtered = value;

                UpdateTexFlags();
            }
        }

        private bool _tiled;

        private bool _filtered;

        internal Texture2D(IntPtr data, int width, int height, int bytesPerPixel)
        {
            Pixmap = null;
            Width = width;
            Height = height;
            Tiled = false;
            Filtered = false;
            UpdateTexFlags();
            Handle = GraphicsContext.CreateTexture2D(width, height, bytesPerPixel, false, 0, TextureFormat.BGRA8, SamplerFlags, data);
        }

        internal Texture2D(Pixmap pixmap, bool tiled, bool filtered)
        {
            Pixmap = pixmap;
            Width = pixmap.Width;
            Height = pixmap.Height;
            Tiled = tiled;
            Filtered = filtered;
            UpdateTexFlags();
            Handle = GraphicsContext.CreateDynamicTexture2D(pixmap.Width, pixmap.Height, false, 0, TextureFormat.BGRA8, SamplerFlags, pixmap.Data);
        }

        internal Texture2D(TextureHandle texHandle, int width, int height, bool filtered, bool tiled)
        {
            Pixmap = null;
            this.Filtered = filtered;
            this.Tiled = tiled;
            this.Width = width;
            this.Height = height;
            this.Handle = texHandle;
            UpdateTexFlags();
        }

        public static Texture2D Create(int width, int height, Color fillColor, bool tiled = false, bool filtered = false)
        {
            var pixmap = new Pixmap(width, height, fillColor);

            return Create(pixmap, tiled, filtered);
        }

        public static Texture2D Create(Pixmap pixmap, bool tiled = false, bool filtered = false)
        {
            var texture = new Texture2D(pixmap, tiled, filtered);

            Engine.Content.RegisterRuntimeLoaded(texture);

            return texture;
        }

        public void SaveToFile(string path)
        {
            Pixmap.SaveToFile(path);
        }

        internal void ReloadPixels()
        {
            if (Pixmap == null)
            {
                return;
            }

            GraphicsContext.UpdateTexture2D(this, 0, 0, 0, 0, Width, Height, Pixmap.Data, Pixmap.Stride);
        }

        private void UpdateTexFlags()
        {
            var flags = BuildTexFlags(Tiled, Filtered);

            this.SamplerFlags = flags;
        }

        private static SamplerFlags BuildTexFlags(bool tiled, bool filtered)
        {
            var tex_flags = SamplerFlags.None;

            if (!tiled) tex_flags = SamplerFlags.UClamp | SamplerFlags.VClamp;

            if (!filtered) tex_flags |= SamplerFlags.Point;

            return tex_flags;
        }

        protected override void FreeUnmanaged()
        {
            GraphicsContext.DestroyTexture2D(this);
        }

        protected override void FreeManaged()
        {
            Pixmap?.Dispose();
        }

        public bool Equals(Texture2D other)
        {
            return other != null && Handle.idx == other.Handle.idx;
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
