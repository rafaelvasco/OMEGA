using System.Runtime.InteropServices;

namespace OMEGA
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionColor : IVertexType
    {
        public float X;

        public float Y;

        public float Z;

        public uint Col;

        VertexLayout IVertexType.Layout => VertexLayout;

        public static readonly VertexLayout VertexLayout;

        static VertexPositionColor()
        {
            VertexLayout = new VertexLayout();

            VertexLayout.Begin();
            VertexLayout.Add(Attrib.Position, AttribType.Float, 3, false, false);
            VertexLayout.Add(Attrib.Color0, AttribType.Uint8, 4, true, false);
            VertexLayout.End();
        }

        public VertexPositionColor(float x, float y, float z, uint abgr)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Col = abgr;
        }

        public static int Stride => 16;
    }
}
