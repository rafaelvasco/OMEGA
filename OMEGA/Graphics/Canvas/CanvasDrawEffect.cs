using static Bgfx.Bgfx;

namespace OMEGA.Graphics.Canvas
{
    public class CanvasDrawEffect
    {
        public BlendMode BlendMode
        {
            get => _blendMode;
            set
            {
                if (_blendMode == value)
                {
                    return;
                }

                _blendMode = value;

                _blendMode = value;

                _blendState = _blendMode switch
                {
                    BlendMode.Solid => 0x0,
                    BlendMode.Mask => StateFlags.BlendAlphaToCoverage,
                    BlendMode.Alpha => GraphicsContext.STATE_BLEND_FUNC_SEPARATE(StateFlags.BlendSrcAlpha,
                        StateFlags.BlendInvSrcAlpha, StateFlags.BlendOne, StateFlags.BlendInvSrcAlpha),
                    BlendMode.AlphaPre => GraphicsContext.STATE_BLEND_FUNC(StateFlags.BlendOne,
                        StateFlags.BlendInvSrcAlpha),
                    BlendMode.Add => GraphicsContext.STATE_BLEND_FUNC_SEPARATE(StateFlags.BlendSrcAlpha,
                        StateFlags.BlendOne, StateFlags.BlendOne, StateFlags.BlendOne),
                    BlendMode.Light => GraphicsContext.STATE_BLEND_FUNC_SEPARATE(StateFlags.BlendDstColor,
                        StateFlags.BlendOne, StateFlags.BlendZero, StateFlags.BlendOne),
                    BlendMode.Multiply => GraphicsContext.STATE_BLEND_FUNC(StateFlags.BlendDstColor,
                        StateFlags.BlendZero),
                    BlendMode.Invert => GraphicsContext.STATE_BLEND_FUNC(StateFlags.BlendInvDstColor,
                        StateFlags.BlendInvSrcColor),
                    _ => _blendState
                };

                UpdateRenderState();
            }
        }

       

        public CullMode CullMode
        {
            get => _cullMode;

            set
            {
                if (_cullMode == value)
                {
                    return;
                }

                _cullMode = value;

                _cullState = _cullMode switch
                {
                    CullMode.ClockWise => StateFlags.CullCw,
                    CullMode.CounterClockWise => StateFlags.CullCcw,
                    CullMode.None => StateFlags.None,
                    _ => _cullState
                };
            }
        }

        public DepthTest DepthTest
        {
            get => _depthTest;
            set
            {
                if (_depthTest == value)
                {
                    return;
                }

                _depthTest = value;

                _depthState = _depthTest switch
                {
                    DepthTest.Always => StateFlags.WriteZ | StateFlags.DepthTestAlways,
                    DepthTest.Equal => StateFlags.WriteZ | StateFlags.DepthTestEqual,
                    DepthTest.GreaterOrEqual => StateFlags.WriteZ | StateFlags.DepthTestGequal,
                    DepthTest.Greater => StateFlags.WriteZ | StateFlags.DepthTestGreater,
                    DepthTest.LessOrEqual => StateFlags.WriteZ | StateFlags.DepthTestLequal,
                    DepthTest.Less => StateFlags.WriteZ | StateFlags.DepthTestLess,
                    DepthTest.Never => StateFlags.DepthTestNever,
                    DepthTest.NotEqual => StateFlags.WriteZ | StateFlags.DepthTestNotequal,
                    _ => _depthState
                };
            }
        }

        private void UpdateRenderState()
        {
        }

        private BlendMode _blendMode;
        private CullMode _cullMode;
        private DepthTest _depthTest;
        private StateFlags _blendState;
        private StateFlags _depthState;
        private StateFlags _cullState;
        private StateFlags _renderState;
        private ShaderProgram _shader;
    }
}
