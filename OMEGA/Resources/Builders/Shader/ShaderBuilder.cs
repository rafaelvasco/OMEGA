
using System;

namespace OMEGA
{
    public static class ShaderBuilder
    {
        public static ShaderProgramData Build(string id, string relative_vs_path, string relative_fs_path)
        {
            Console.WriteLine($"Compiling Shader: {id}");

            var result = ShaderCompiler.Compile(ResourceLoader.GetFullResourcePath(relative_vs_path), ResourceLoader.GetFullResourcePath(relative_fs_path));

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
