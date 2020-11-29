using OMEGA;

namespace DEMO
{
    public class RenderTargetDemo : Game
    {
        private Texture2D image;

        private RenderTarget render_target;

        ShaderProgram shader_program;

        VertexStream<VertexPositionColorTexture> vertex_stream;

        Mat4 display_projection;
        Mat4 render_target_projection;

        public override void Load()
        {
            image = Engine.Content.Get<Texture2D>("party");

            vertex_stream = new VertexStream<VertexPositionColorTexture>(
                new VertexPositionColorTexture[]
                {
                     // Texture Vertices
                    new VertexPositionColorTexture( 0, 0, 0, Color.White, 0, 0 ),
                    new VertexPositionColorTexture( 204, 0, 0, Color.White, 1, 0 ),
                    new VertexPositionColorTexture( 204, 184, 0, Color.White, 1, 1),
                    new VertexPositionColorTexture( 0, 184, 0, Color.White, 0, 1 ),

                    // Render Target Vertices
                    new VertexPositionColorTexture(0.0f, 0.0f, 0.0f, 0xFFFFFFFF, 0, 0),
                    new VertexPositionColorTexture(320.0f, 0.0f, 0.0f, 0xFFFFFFFF, 1, 0),
                    new VertexPositionColorTexture(320.0f, 240.0f, 0.0f, 0xFFFFFFFF, 1, 1),
                    new VertexPositionColorTexture(0.0f, 240.0f, 0.0f, 0xFFFFFFFF, 0, 1),
                },
                new ushort[]
                {
                    0, 1, 2, 0, 2, 3
                }
            );

            render_target = RenderTarget.Create(320, 240, false);

            Calc.MatOrtho(ref display_projection, 0, Engine.DisplaySize.Width, Engine.DisplaySize.Height, 0, 0.0f, 1.0f);

            Calc.MatOrtho(ref render_target_projection, 0, render_target.Width, render_target.Height, 0, 0.0f, 1000.0f);

            shader_program = Engine.Content.Get<ShaderProgram>("base_shader_pct");

            Engine.DrawDevice.SetShader(shader_program);

            Engine.DrawDevice.SetupDrawPass(draw_pass_index: 0, new DrawPass
            (
                clear_color: Color.Blue,
                projection_matrix: render_target_projection,
                view_port: Rect.FromBox(0, 0, 320, 240),
                render_target: render_target
            ));

            Engine.DrawDevice.SetupDrawPass(draw_pass_index: 1, new DrawPass (
                clear_color: Color.Red,
                projection_matrix: display_projection
            ));

        }

        public override void Draw(float dt)
        {
            var draw_device = Engine.DrawDevice;

            draw_device.SetTexture(shader_program, 0, image);

            draw_device.SubmitVertexStream(draw_pass_index: 0, vertex_stream, 0, 4, 0, 6);

            draw_device.SetTexture(shader_program, 0, render_target.Texture);

            draw_device.SubmitVertexStream(draw_pass_index: 1, vertex_stream, 4, 4, 0, 6);

        }
    }
}
