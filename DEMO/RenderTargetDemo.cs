//using OMEGA;

//namespace DEMO
//{
//    /* Drawing a Texture using a VertexStream to a Render Target. Then drawing that Render Target Texture to the screen; Uses base PositionColorTexture Shader. */
//    public class RenderTargetDemo : Game
//    {
//        private Texture2D image;

//        private RenderTarget render_target;

//        VertexStream<Vertex> vertex_stream;

//        Mat4 display_projection;
//        Mat4 render_target_projection;

//        public override void Load()
//        {
//            image = Engine.Content.Get<Texture2D>("party");

//            vertex_stream = new VertexStream<Vertex>(
//                new Vertex[]
//                {
//                     // Texture Vertices
//                    new Vertex( 0, 0, 0, Color.White, 0, 0 ),
//                    new Vertex( 204, 0, 0, Color.White, 1, 0 ),
//                    new Vertex( 204, 184, 0, Color.White, 1, 1),
//                    new Vertex( 0, 184, 0, Color.White, 0, 1 ),

//                    // Render Target Vertices
//                    new Vertex(0.0f, 0.0f, 0.0f, 0xFFFFFFFF, 0, 0),
//                    new Vertex(320.0f, 0.0f, 0.0f, 0xFFFFFFFF, 1, 0),
//                    new Vertex(320.0f, 240.0f, 0.0f, 0xFFFFFFFF, 1, 1),
//                    new Vertex(0.0f, 240.0f, 0.0f, 0xFFFFFFFF, 0, 1),
//                },
//                new ushort[]
//                {
//                    0, 1, 2, 0, 2, 3
//                }
//            );

//            render_target = RenderTarget.Create(320, 240, false);

//            Calc.MatOrtho(ref display_projection, 0, Engine.DisplaySize.Width, Engine.DisplaySize.Height, 0, 0.0f, 1.0f);

//            Calc.MatOrtho(ref render_target_projection, 0, render_target.Width, render_target.Height, 0, 0.0f, 1000.0f);

//            Engine.Renderer.SetupDrawPass(draw_pass_index: 0, new RenderPass
//            (
//                clear_color: Color.Blue,
//                projection_matrix: render_target_projection,
//                view_port: Rect.FromBox(0, 0, 320, 240),
//                render_target: render_target
//            ));

//            Engine.Renderer.SetupDrawPass(draw_pass_index: 1, new RenderPass (
//                clear_color: Color.Red,
//                projection_matrix: display_projection
//            ));

//        }

//        public override void Draw(float dt)
//        {
//            var draw_device = Engine.Renderer;

//            draw_device.SetTexture(0, image);

//            draw_device.SubmitVertexStream(render_pass: 0, vertex_stream, 0, 4, 0, 6);

//            draw_device.SetTexture(0, render_target.Texture);

//            draw_device.SubmitVertexStream(render_pass: 1, vertex_stream, 4, 4, 0, 6);

//        }
//    }
//}
