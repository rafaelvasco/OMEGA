using System.IO;

namespace OMEGA
{
    public static partial class ResourceLoader
    {
        public static ShaderProgram LoadShader(ShaderProgramData shader_data)
        {
            var shader_program =
                GraphicsContext.CreateShaderProgram(
                    shader_data.VertexShader,
            shader_data.FragmentShader,
                    shader_data.Samplers,
                    shader_data.Params);

            shader_program.Id = shader_data.Id;

            return shader_program;
        }

        public static ShaderProgram LoadShader(string id, string relative_vs_path, string relative_fs_path)
        {
            var shader_prog_data = LoadShaderProgramData(id, relative_vs_path, relative_fs_path);

            return LoadShader(shader_prog_data);
        }

        public static ShaderProgramData LoadShaderProgramData(string id, string relative_vs_path, string relative_fs_path)
        {
            var result = ShaderBuilder.Build(GetFullResourcePath(relative_vs_path), GetFullResourcePath(relative_fs_path));

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
