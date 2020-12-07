using System;
using System.Runtime.CompilerServices;

namespace OMEGA
{
#pragma warning disable CS0649
    internal struct Init
    {
        public InitPtrData init;

        public TextureFormat Format => init.resolution.format;
    }

    internal unsafe struct RangeAccessor<T> where T : struct
    {
        private static readonly int SizeOfT = Unsafe.SizeOf<T>();

        public readonly void* data;
        public readonly int count;
        public IntPtr Ptr => new IntPtr(data);
        public RangeAccessor(IntPtr data, int count) : this(data.ToPointer(), count) { }
        public RangeAccessor(void* data, int count)
        {
            this.data = data;
            this.count = count;
        }

        public ref T this[int index]
        {
            get
            {
                if (index < 0 || index >= count)
                {
                    throw new IndexOutOfRangeException();
                }

                return ref Unsafe.AsRef<T>((byte*)data + SizeOfT * index);
            }
        }
    }

    internal unsafe struct RangePtrAccessor<T> where T : struct
    {
        public readonly void* data;
        public readonly int count;

        public RangePtrAccessor(IntPtr data, int count) : this(data.ToPointer(), count) { }
        public RangePtrAccessor(void* data, int count)
        {
            this.data = data;
            this.count = count;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= count)
                {
                    throw new IndexOutOfRangeException();
                }

                return Unsafe.Read<T>((byte*)data + sizeof(void*) * index);
            }
        }
    }

    internal unsafe struct Caps
    {
        public struct GPU
        {
            public ushort vendorId;
            public ushort deviceId;
        }

        public struct Limits
        {
            public uint maxDrawCalls;
            public uint maxBlits;
            public uint maxTextureSize;
            public uint maxTextureLayers;
            public uint maxViews;
            public uint maxFrameBuffers;
            public uint maxFBAttachments;
            public uint maxPrograms;
            public uint maxShaders;
            public uint maxTextures;
            public uint maxTextureSamplers;
            public uint maxComputeBindings;
            public uint maxVertexLayouts;
            public uint maxVertexStreams;
            public uint maxIndexBuffers;
            public uint maxVertexBuffers;
            public uint maxDynamicIndexBuffers;
            public uint maxDynamicVertexBuffers;
            public uint maxUniforms;
            public uint maxOcclusionQueries;
            public uint maxEncoders;
            public uint minResourceCbSize;
            public uint transientVbSize;
            public uint transientIbSize;
        }

        public RendererType rendererType;
        public ulong supported;
        public ushort vendorId;
        public ushort deviceId;
        public byte homogeneousDepth;
        public byte originBottomLeft;
        public byte numGPUs;
        public fixed uint gpu[4];
        public Limits limits;
        public fixed ushort formats[85];
    }

    internal unsafe struct PlatformPtrData
    {
        public void* ndt;
        public void* nwh;
        public void* context;
        public void* backBuffer;
        public void* backBufferDS;
    }

    internal struct Resolution
    {
        public TextureFormat format;
        public uint width;
        public uint height;
        public uint reset;
        public byte numBackBuffers;
        public byte maxFrameLatency;
    }

    internal struct InitPtrData
    {
        public struct Limits
        {
            public ushort maxEncoders;
            public uint minResourceCbSize;
            public uint transientVbSize;
            public uint transientIbSize;
        }

        public RendererType type;
        public ushort vendorId;
        public ushort deviceId;
        public byte debug;
        public byte profile;
        public PlatformPtrData platformData;
        public Resolution resolution;
        public Limits limits;
        public IntPtr callback;
        public IntPtr allocator;
    }

    internal unsafe struct Memory
    {
        public byte* data;
        public uint size;
    }

    internal unsafe struct InstanceDataBuffer
    {
        public byte* data;
        public uint size;
        public uint offset;
        public uint num;
        public ushort stride;
        public VertexBufferHandle handle;
    }

    internal struct TextureInfo
    {
        public TextureFormat format;
        public uint storageSize;
        public ushort width;
        public ushort height;
        public ushort depth;
        public ushort numLayers;
        public byte numMips;
        public byte bitsPerPixel;
        public byte cubeMap;
    }

    internal unsafe struct UniformInfo
    {
        public fixed byte name[256];
        public UniformType type;
        public ushort num;
    }

    internal struct Attachment
    {
        public Access access;
        public TextureHandle handle;
        public ushort mip;
        public ushort layer;
        public byte resolve;
    }

    internal unsafe struct TransformPtrData
    {
        public float* data;
        public ushort num;
    }

    internal unsafe struct ViewStats
    {
        public fixed byte name[256];
        public ushort view;
        public long cpuTimeBegin;
        public long cpuTimeEnd;
        public long gpuTimeBegin;
        public long gpuTimeEnd;
    }

    internal struct EncoderStats
    {
        public long cpuTimeBegin;
        public long cpuTimeEnd;
    }

    internal unsafe struct Stats
    {
        public long cpuTimeFrame;
        public long cpuTimeBegin;
        public long cpuTimeEnd;
        public long cpuTimerFreq;
        public long gpuTimeBegin;
        public long gpuTimeEnd;
        public long gpuTimerFreq;
        public long waitRender;
        public long waitSubmit;
        public uint numDraw;
        public uint numCompute;
        public uint numBlit;
        public uint maxGpuLatency;
        public ushort numDynamicIndexBuffers;
        public ushort numDynamicVertexBuffers;
        public ushort numFrameBuffers;
        public ushort numIndexBuffers;
        public ushort numOcclusionQueries;
        public ushort numPrograms;
        public ushort numShaders;
        public ushort numTextures;
        public ushort numUniforms;
        public ushort numVertexBuffers;
        public ushort numVertexLayouts;
        public long textureMemoryUsed;
        public long rtMemoryUsed;
        public int transientVbUsed;
        public int transientIbUsed;
        public fixed uint numPrims[5];
        public long gpuMemoryMax;
        public long gpuMemoryUsed;
        public ushort width;
        public ushort height;
        public ushort textWidth;
        public ushort textHeight;
        public ushort numViews;
        public ViewStats* viewStats;
        public byte numEncoders;
        public EncoderStats* encoderStats;
    }

    internal unsafe struct VertexLayoutPtrData
    {
        public uint hash;
        public ushort stride;
        public fixed ushort offset[18];
        public fixed ushort attributes[18];
    }

    internal struct DynamicIndexBufferHandle
    {
        public ushort idx;
        public bool Valid => idx != UInt16.MaxValue;
    }

    internal struct DynamicVertexBufferHandle
    {
        public ushort idx;
        public bool Valid => idx != UInt16.MaxValue;
    }

    internal struct FrameBufferHandle
    {
        public ushort idx;
        public bool Valid => idx != UInt16.MaxValue;

        public static FrameBufferHandle Null => new FrameBufferHandle();
    }

    internal struct IndexBufferHandle
    {
        public ushort idx;
        public bool Valid => idx != UInt16.MaxValue;
    }

    internal struct IndirectBufferHandle
    {
        public ushort idx;
        public bool Valid => idx != UInt16.MaxValue;
    }

    internal struct OcclusionQueryHandle
    {
        public ushort idx;
        public bool Valid => idx != UInt16.MaxValue;
    }

    internal struct ProgramHandle
    {
        public ushort idx;
        public bool Valid => idx != UInt16.MaxValue;
    }

    internal struct ShaderHandle
    {
        public ushort idx;
        public bool Valid => idx != UInt16.MaxValue;
    }

    internal struct TextureHandle
    {
        public ushort idx;
        public bool Valid => idx != UInt16.MaxValue;
    }

    internal struct UniformHandle
    {
        public ushort idx;
        public bool Valid => idx != UInt16.MaxValue;
    }

    internal struct VertexBufferHandle
    {
        public ushort idx;
        public bool Valid => idx != UInt16.MaxValue;
    }

    internal struct VertexLayoutHandle
    {
        public ushort idx;
        public bool Valid => idx != UInt16.MaxValue;
    }
#pragma warning restore CS0649
}
