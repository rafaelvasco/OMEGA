
using System;

namespace OMEGA
{
    public static class ShaderBuilder
    {
        public static ShaderProgramData Build(string id, string relativeVsPath, string relativeFsPath)
        {
            Console.WriteLine($"Compiling Shader: {id}");

            var result = ShaderCompiler.Compile(ResourceLoader.GetFullResourcePath(relativeVsPath), ResourceLoader.GetFullResourcePath(relativeFsPath));

            var shader_program_data = new ShaderProgramData()
            {
                Id = id,
                VertexShader = result.VsBytes,
                FragmentShader = result.FsBytes,
                Samplers = result.Samplers,
                Params = result.Params
            };

            return shader_program_data;
        }
    }
}
