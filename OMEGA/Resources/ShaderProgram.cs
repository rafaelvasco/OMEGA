using System.Collections.Generic;

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

        private ShaderParameter[] Parameters;

        private Dictionary<string, int> ParametersMap;

        internal UniformHandle[] Samplers;

        internal ShaderProgram(ProgramHandle program, IReadOnlyList<string> samplers, IReadOnlyList<string> _params)
        {
            this.Program = program;

            BuildSamplersList(samplers);

            BuildParametersList(_params);
        }

        public ShaderParameter GetParameter(string name)
        {
            if (ParametersMap.TryGetValue(name, out var index))
            {
                return Parameters[index];
            }

            return null;
        }

        internal unsafe void Submit()
        {
            if (Parameters == null)
            {
                return;
            }

            for (int i = 0; i < Parameters.Length; ++i)
            {
                var p = Parameters[i];

                if (p.Constant)
                {
                    if (p.SubmitedOnce)
                    {
                        continue;
                    }
                    else
                    {
                        p.SubmitedOnce = true;
                    }

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

            Samplers = new UniformHandle[samplers.Count];

            for (int i = 0; i < samplers.Count; ++i)
            {
                Samplers[i] = GraphicsContext.CreateUniform(samplers[i], UniformType.Sampler, 1);
            }
        }

        private void BuildParametersList(IReadOnlyList<string> _params)
        {
            if (_params == null)
            {
                return;
            }

            Parameters = new ShaderParameter[_params.Count];
            ParametersMap = new Dictionary<string, int>();

            for (int i = 0; i < _params.Count; ++i)
            {
                Parameters[i] = new ShaderParameter(_params[i]);
                ParametersMap.Add(_params[i], i);
            }
        }

        protected override void FreeUnmanaged()
        {
            if (!Program.Valid)
            {
                return;
            }

            if (Samplers != null)
            {
                for (int i = 0; i < Samplers.Length; ++i)
                {
                    GraphicsContext.DestroyUniform(Samplers[i]);
                }
            }

            if (Parameters != null)
            {
                for (int i = 0; i < Parameters.Length; ++i)
                {
                    GraphicsContext.DestroyUniform(Parameters[i].Uniform);
                }
            }

            GraphicsContext.DestroyProgram(Program);

        }
    }
}
