using OMEGA;

namespace DEMO
{
    public class TriangleQuadDemo : Game
    {
        ShaderProgram shader_program;
        Mat4 projection_matrix;
        VertexStream<VertexPositionColor> vertex_stream;

        public override void Load()
        {
            Calc.MatOrtho(ref projection_matrix, 0, Engine.DisplaySize.Width, Engine.DisplaySize.Height, 0, 0.0f, 1.0f); ;

            shader_program = Engine.Content.Get<ShaderProgram>("base_shader_pc");

            var win_w = Engine.DisplaySize.Width;
            var win_h = Engine.DisplaySize.Height;

            vertex_stream = new VertexStream<VertexPositionColor>(new VertexPositionColor[] {

                /* Triangle */

                new VertexPositionColor { X = win_w/2, Y = 100, Col = Color.RoyalBlue }, // TOP
                new VertexPositionColor { X = 100, Y = 200, Col = Color.SpringGreen }, // LEFT
                new VertexPositionColor { X = win_w-100, Y = 200, Col = Color.LightPink }, // RIGHT

                /* Quad */

                new VertexPositionColor { X = 100, Y = 250, Col = Color.RoyalBlue }, // TOP-LEFT
                new VertexPositionColor { X = win_w-100, Y = 250, Col = Color.SpringGreen }, // TOP-RIGHT
                new VertexPositionColor { X = win_w-100, Y = win_h-100, Col = Color.LightPink }, // BOTTOM-RIGHT
                new VertexPositionColor { X = 100, Y = win_h-100, Col = Color.LightPink }, // BOTTOM-LEFT

            }, new ushort[] {

                0, 1, 2,
                0, 1, 2, 0, 2, 3

            });

            Engine.DrawDevice.SetupDrawPass(draw_pass_index: 0, 
            new DrawPass
            (
                clear_color: Color.Blue,
                projection_matrix: projection_matrix
            ));

            Engine.DrawDevice.SetShader(shader_program);

        }

        public override void VariableUpdate(float dt)
        {
            if (GamePad.GetState(GamePadIndex.One).IsButtonDown(Buttons.Back) || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
        }

        public override void Draw(float dt)
        {
            var draw_device = Engine.DrawDevice;

            // Draw Triangle
            draw_device.SubmitVertexStream(draw_pass_index: 0, vertex_stream, 0, 3, 0, 3);

            // Draw Quad
            draw_device.SubmitVertexStream(draw_pass_index: 0, vertex_stream, 3, 4, 3, 6);
        }
    }
}
