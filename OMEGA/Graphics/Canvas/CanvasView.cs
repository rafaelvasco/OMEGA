using System;

namespace OMEGA
{
    public class CanvasView
    {
        public Color ClearColor
        {
            get => _mClearColor;
            set
            {
                if (_mClearColor != value)
                {
                    _mClearColor = value;
                    Applied = false;
                }
            }
        }
        public Vec2 SizeFactor
        {
            get => _mSizeFactor;
            set
            {
                if (_mSizeFactor != value)
                {
                    _mSizeFactor = value;
                    NeedsUpdateTransform = true;
                    Applied = false;
                }
            }
        }
        public RectF Viewport
        {
            get => _mViewport;
            set
            {
                _mViewport = value;
                Applied = false;
            }
        }

        public RectF AbsoluteViewport
        {
            get => Engine.Canvas.GetAbsoluteViewport(this);
            set {
                _mViewport = RectF.FromBox(value.X1 / Engine.Canvas.Width, value.Y1 / Engine.Canvas.Height, value.Width / Engine.Canvas.Width, value.Height / Engine.Canvas.Height);
                Applied = false;
            }
        }

        public RenderTarget RenderTarget
        {
            get => _mRenderTarget;
            set
            {
                _mRenderTarget = value;
                Applied = false;
            }
        }

        internal bool Applied {get;set;}

        internal bool NeedsUpdateTransform {get;set;} = true;

        public ushort ViewId {get;internal set;}

        private Vec2 _mSizeFactor;
        private RectF _mViewport;
        private Transform _mTransform;
        private Transform _mInverseTransform;
        private bool _mInvTransformUpdated;
        private Color _mClearColor;
        private RenderTarget _mRenderTarget;

        internal CanvasView(float sizeFactorX, float sizeFactorY)
        {
            _mViewport = RectF.FromBox(0f, 0f, 1f, 1f);
            _mSizeFactor = new Vec2(sizeFactorX, sizeFactorY);

        }

        internal ref readonly Transform GetTransform(int canvasWidth, int canvasHeight)
        {
            if (NeedsUpdateTransform)
            {
                float size_w = canvasWidth * _mSizeFactor.X;
                float size_h = canvasHeight * _mSizeFactor.Y;

                float center_x = canvasWidth / 2.0f;
                float center_y = canvasHeight / 2.0f;

                // Projection Components

                float a = 2f / size_w;
                float b = -2f / size_h;
                float c = -a * center_x;
                float d = -b * center_y;

                _mTransform = new Transform(
                    a,  0f, c,
                    0f, b,  d,
                    0f, 0f, 1f
                );

                NeedsUpdateTransform = false;
                _mInvTransformUpdated = false;

                Console.WriteLine("Update View Transform");
                    
            }

            return ref _mTransform;
        }

        public ref readonly Transform GetInverseTransform()
        {
            if (!_mInvTransformUpdated)
            {
                _mInverseTransform = GetTransform(Engine.Canvas.Width, Engine.Canvas.Height).GetInverse();
                _mInvTransformUpdated = true;
            }

            return ref _mInverseTransform;
        }
    }
}
