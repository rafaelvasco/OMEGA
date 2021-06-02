using System.Runtime.CompilerServices;
using static Bgfx.Bgfx;

namespace OMEGA
{
    public unsafe struct VertexLayout
    {
        internal Bgfx.Bgfx.VertexLayout InternalHandle;

        public void Begin()
        {
            vertex_layout_begin((Bgfx.Bgfx.VertexLayout*)Unsafe.AsPointer(ref InternalHandle), GraphicsContext.RendererBackend);
        }

        public void Add(Attrib attrib, AttribType attribType, byte num, bool normalized, bool asInt)
        {
            vertex_layout_add((Bgfx.Bgfx.VertexLayout*)Unsafe.AsPointer(ref InternalHandle), attrib, num, attribType, normalized, asInt);
        }

        public void End()
        {
            vertex_layout_end((Bgfx.Bgfx.VertexLayout*)Unsafe.AsPointer(ref InternalHandle));
        }
    }
}
