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

      
    }
}
