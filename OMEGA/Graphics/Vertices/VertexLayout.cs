using System.Runtime.CompilerServices;

namespace OMEGA
{
    public struct VertexLayout
    {
        internal VertexLayoutPtrData InternalHandle;

        public void Begin()
        {
            unsafe
            {
                Bgfx.vertex_layout_begin((VertexLayoutPtrData*)Unsafe.AsPointer(ref InternalHandle), GraphicsContext.RendererBackend);
            }
        }

        public void Add(Attrib attrib, AttribType attribType, byte num, bool normalized, bool asInt)
        {
            unsafe
            {
                Bgfx.vertex_layout_add((VertexLayoutPtrData*)Unsafe.AsPointer(ref InternalHandle), attrib, num, attribType, normalized, asInt);
            }
        }

        public void End()
        {
            unsafe
            {
                Bgfx.vertex_layout_end((VertexLayoutPtrData*)Unsafe.AsPointer(ref InternalHandle));
            }
        }
    }
}
