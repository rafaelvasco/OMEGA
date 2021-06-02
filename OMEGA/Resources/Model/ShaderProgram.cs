using System.Collections.Generic;
using static Bgfx.Bgfx;

namespace OMEGA
{
    public class ShaderParameter
    {
        internal UniformHandle Uniform;

        public bool Constant { get; set; } = false;

        internal bool SubmitedOnce;

        public Vec4 Value => _value;

        private Vec4 _value;


        internal ShaderParameter(string name)
        {
            this.Uniform = GraphicsContext.CreateUniform(name, UniformType.Vec4, 4);
        }

        public void SetValue(float v)
        {
            _value.X = v;
        }

        public void SetValue(Vec2 v)
        {
            _value.X = v.X;
            _value.Y = v.Y;
        }

        public void SetValue(Vec3 v)
        {
            _value.X = v.X;
            _value.Y = v.Y;
            _value.Z = v.Z;
        }

        public void SetValue(Vec4 v)
        {
            _value = v;
        }

        public void SetValue(Color color)
        {
            _value.X = color.Rf;
            _value.Y = color.Gf;
            _value.Z = color.Bf;
            _value.W = color.Af;
        }
    }

    public class ShaderProgram : Resource
    {
        internal ProgramHandle Program;

        private ShaderParameter[] _parameters;

        private Dictionary<string, int> _parametersMap;

        private readonly Texture2D[] _textures;

        private int _textureIndex;

        private UniformHandle[] _samplers;

        internal ShaderProgram(ProgramHandle program, IReadOnlyList<string> samplers, IReadOnlyList<string> @params)
        {
            Program = program;

            _textures = new Texture2D[samplers.Count];

            BuildSamplersList(samplers);

            BuildParametersList(@params);
        }

        internal void SetTexture(int slot, Texture2D texture)
        {
            slot = Calc.Clamp(slot, 0, 2);

            _textures[slot] = texture;

            if (slot > _textureIndex)
            {
                _textureIndex = slot;
            }
        }

        public ShaderParameter GetParameter(string name)
        {
            return _parametersMap.TryGetValue(name, out var index) ? _parameters[index] : null;
        }

        internal void Submit()
        {
            if (_textureIndex == 0)
            {
                GraphicsContext.SetTexture(0, _samplers[0], _textures[0].Handle);
            }
            else
            {
                for (int i = 0; i <= _textureIndex; ++i)
                {
                    GraphicsContext.SetTexture((byte)i, _samplers[i], _textures[i].Handle);
                }
            }

            if (_parameters == null)
            {
                return;
            }

            for (int i = 0; i < _parameters.Length; ++i)
            {
                var p = _parameters[i];

                if (p.Constant)
                {
                    if (p.SubmitedOnce)
                    {
                        continue;
                    }

                    p.SubmitedOnce = true;

                }

                var val = p.Value;

                GraphicsContext.SetUniform(p.Uniform, ref val);
            }
        }

        private void BuildSamplersList(IReadOnlyList<string> samplers)
        {
            if (samplers == null)
            {
                return;
            }

            _samplers = new UniformHandle[samplers.Count];

            for (int i = 0; i < samplers.Count; ++i)
            {
                _samplers[i] = GraphicsContext.CreateUniform(samplers[i], UniformType.Sampler, 1);
            }
        }

        private void BuildParametersList(IReadOnlyList<string> @params)
        {
            if (@params == null)
            {
                return;
            }

            _parameters = new ShaderParameter[@params.Count];
            _parametersMap = new Dictionary<string, int>();

            for (int i = 0; i < @params.Count; ++i)
            {
                _parameters[i] = new ShaderParameter(@params[i]);
                _parametersMap.Add(@params[i], i);
            }
        }

        protected override void FreeUnmanaged()
        {
            if (!Program.Valid)
            {
                return;
            }

            if (_samplers != null)
            {
                for (int i = 0; i < _samplers.Length; ++i)
                {
                    GraphicsContext.DestroyUniform(_samplers[i]);
                }
            }

            if (_parameters != null)
            {
                for (int i = 0; i < _parameters.Length; ++i)
                {
                    GraphicsContext.DestroyUniform(_parameters[i].Uniform);
                }
            }

            GraphicsContext.DestroyProgram(Program);

        }
    }
}
