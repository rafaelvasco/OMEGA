using System.IO;

namespace OMEGA
{
    public static partial class ResourceLoader
    {
        public static ShaderProgram LoadShader(ShaderProgramData shaderData)
        {
            var shader_program =
                GraphicsContext.CreateShaderProgram(
                    shaderData.VertexShader,
            shaderData.FragmentShader,
                    shaderData.Samplers,
                    shaderData.Params);

            shader_program.Id = shaderData.Id;

            return shader_program;
        }

      
    }
}
