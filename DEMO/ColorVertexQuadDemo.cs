using OMEGA;

namespace DEMO
{
    public class ColorVertexQuadDemo : Game
    {
        ShaderProgram shader_program;
        VertexStream<VertexPositionColor> vertex_stream;
        Mat4 projection_matrix;

        public override void Load()
        {
            Calc.MatOrtho(ref projection_matrix, 0, Engine.DisplaySize.Width, Engine.DisplaySize.Height, 0, 0.0f, 1.0f);

            shader_program = Engine.Content.Get<ShaderProgram>("base_shader_pc");

            var win_w = Engine.DisplaySize.Width;
            var win_h = Engine.DisplaySize.Height;


            vertex_stream = new VertexStream<VertexPositionColor>(new VertexPositionColor[] {
                new VertexPositionColor ( 100, 100, 0, Color.RoyalBlue ), // TOP-LEFT
                new VertexPositionColor ( win_w - 100, 100, 0, Color.SpringGreen ), // TOP-RIGHT
                new VertexPositionColor (  win_w - 100, win_h - 100, 0, Color.LightPink ), // BOTTOM-RIGHT
                new VertexPositionColor ( 100,  win_h - 100, 0, Color.SpringGreen ), // BOTTOM-LEFT

            }, new ushort[] { 
                0, 1, 2, 0, 2, 3
            });


            Engine.DrawDevice.SetupDrawPass(draw_pass_index: 0, new DrawPass
            (
                clear_color: Color.Blue,
                projection_matrix: projection_matrix
            ));

            Engine.DrawDevice.SetShader(shader_program);
        }

        public override void VariableUpdate(float dt)
        {
            if (GamePad.GetState(GamePadIndex.One).IsButtonDown(Buttons.Back) || Input.KeyPressed(Keys.Escape))
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
