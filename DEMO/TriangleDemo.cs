using OMEGA;

namespace DEMO
{
    public class TriangleDemo : Game
    {
        ShaderProgram shader_program;
        VertexStream<VertexPositionColor> vertex_stream;

        public override void Load()
        {
            shader_program = Engine.Content.Get<ShaderProgram>("base_shader_pc");

            vertex_stream = new VertexStream<VertexPositionColor>(new VertexPositionColor[] {

                new VertexPositionColor ( 0.0f, 0.5f, 0.0f, Color.RoyalBlue ),
                new VertexPositionColor ( 0.5f, -0.5f, 0.0f, Color.SpringGreen ),
                new VertexPositionColor ( -0.5f, -0.5f, 0.0f, Color.LightPink ),
            });


            Engine.DrawDevice.SetupDrawPass(draw_pass_index: 0, new DrawPass(
                clear_color: Color.Blue    
            ));

            Engine.DrawDevice.SetShader(shader_program);
        }

        public override void VariableUpdate(float dt)
        {
            if (Input.GamePadPressed(GamePadButtons.Back) || Input.KeyPressed(Keys.Escape))
            {
                Exit();
            }
        }

        public override void Draw(float dt)
        {
            var draw_device = Engine.DrawDevice;

            draw_device.SubmitVertexStream(draw_pass_index: 0, vertex_stream);
        }
    }
}
