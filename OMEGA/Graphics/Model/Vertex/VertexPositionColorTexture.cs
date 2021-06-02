﻿using System.Runtime.InteropServices;
using static Bgfx.Bgfx;

namespace OMEGA
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionColorTexture
    {
        public float X;

        public float Y;

        public float Z;

        public uint Col;

        public float Tx;

        public float Ty;


        public static readonly VertexLayout VertexLayout;

        public VertexPositionColorTexture(float x, float y, float z = 0f, uint abgr = 0xffffffff, float tx = 0f, float ty= 0f)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Col = abgr;
            this.Tx = tx;
            this.Ty = ty;
        }

        static VertexPositionColorTexture()
        {
            VertexLayout = new VertexLayout();
            VertexLayout.Begin();
            VertexLayout.Add(Attrib.Position, AttribType.Float, 3, false, false);
            VertexLayout.Add(Attrib.Color0, AttribType.Uint8, 4, true, false);
            VertexLayout.Add(Attrib.TexCoord0, AttribType.Float, 2, false, false);
            VertexLayout.End();
        }

        public override string ToString()
        {
            return $"{X},{Y},{Z},{Col},{Tx},{Ty}";
        }

        public static int Stride => 24;

    }
}

