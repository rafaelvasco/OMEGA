//using OMEGA;

//namespace DEMO
//{
//    /* Drawing a Textured Cube using VertexStream. */
//    public class VertexStreamTexturedCubeDemo : Game
//    {

//        Texture2D texture;
//        float rotate_x, rotate_y;
//        Mat4 projection_matrix;
//        Mat4 view_matrix;
//        VertexStream<Vertex> vertex_stream;

//        public override void Load()
//        {
//            var pixmap = new Pixmap(4, 4, Color.Black);

//            pixmap.BlitColors(new Color[] {
//                0xFFFFFFFF, 0x000000FF, 0xFFFFFFFF, 0x000000FF,
//                0x000000FF, 0xFFFFFFFF, 0x000000FF, 0xFFFFFFFF,
//                0xFFFFFFFF, 0x000000FF, 0xFFFFFFFF, 0x000000FF,
//                0x000000FF, 0xFFFFFFFF, 0x000000FF, 0xFFFFFFFF
//            });

//            texture = Texture2D.Create(pixmap, true, false);

//            Calc.MatProj(ref projection_matrix, 60.0f, (float)Engine.DisplaySize.Width / Engine.DisplaySize.Height, 0.01f, 10.0f);

//            Calc.MatLookAt(ref view_matrix, new Vec3(0.0f, 1.5f, 6.0f), new Vec3(0f, 0f, 0f));

//            vertex_stream = new VertexStream<Vertex>(new Vertex[] {
//                new Vertex(-1,-1,-1, Color.Red, 0, 0),
//                new Vertex(1,-1,-1, Color.Red, 5, 0),
//                new Vertex(1,1,-1, Color.Red, 5, 5),
//                new Vertex(-1,1,-1, Color.Red, 0, 5),

//                new Vertex(-1,-1,1, Color.Green, 0, 0),
//                new Vertex(1,-1,1, Color.Green, 5, 0),
//                new Vertex(1,1,1, Color.Green, 5, 5),
//                new Vertex(-1,1,1, Color.Green, 0, 5),

//                new Vertex(-1,-1,-1, Color.Blue, 0, 0),
//                new Vertex(-1,1,-1, Color.Blue, 5, 0),
//                new Vertex(-1,1,1, Color.Blue, 5, 5),
//                new Vertex(-1,-1,1, Color.Blue, 0, 5),

//                new Vertex(1,-1,-1, new Color(1.0f, 0.5f, 0.0f), 0, 0),
//                new Vertex(1,1,-1, new Color(1.0f, 0.5f, 0.0f), 5, 0),
//                new Vertex(1,1,1, new Color(1.0f, 0.5f, 0.0f), 5, 5),
//                new Vertex(1,-1,1, new Color(1.0f, 0.5f, 0.0f), 0, 5),

//                new Vertex(-1,-1,-1, new Color(0.0f, 0.5f, 1.0f), 0, 0),
//                new Vertex(-1,-1,1, new Color(0.0f, 0.5f, 1.0f), 5, 0),
//                new Vertex(1,-1,1, new Color(0.0f, 0.5f, 1.0f), 5, 5),
//                new Vertex(1,-1,-1, new Color(0.0f, 0.5f, 1.0f), 0, 5),

//                new Vertex(-1,1,-1, new Color(1.0f, 0.0f, 0.5f), 0, 0),
//                new Vertex(-1,1,1, new Color(1.0f, 0.0f, 0.5f), 5, 0),
//                new Vertex(1,1,1, new Color(1.0f, 0.0f, 0.5f), 5, 5),
//                new Vertex(1,1,-1, new Color(1.0f, 0.0f, 0.5f), 0, 5)
//            }, new ushort[] {

//                0, 1, 2,  0, 2, 3,
//                6, 5, 4,  7, 6, 4,
//                8, 9, 10,  8, 10, 11,
//                14, 13, 12,  15, 14, 12,
//                16, 17, 18,  16, 18, 19,
//                22, 21, 20,  23, 22, 20
//            });

//            Engine.Renderer.CullMode = CullMode.CounterClockWise;

//            var render_pass = new RenderPass(projection_matrix: projection_matrix, view_matrix: view_matrix);
            
//            Engine.Renderer.UpdateRenderPass(0, render_pass);

//            Engine.Renderer.SetTexture(0, texture);
//        }

//        public override void Draw(float dt)
//        {
//            var draw_device = Engine.Renderer;

//            Mat4 model_mat = new Mat4();

//            Calc.MatRotateXY(ref model_mat, rotate_x, rotate_y);

//            draw_device.SetModelTransform(ref model_mat);

//            draw_device.SubmitVertexStream(render_pass: 0, vertex_stream);
//        }

//        public override void VariableUpdate(float dt)
//        {
//            if (Input.KeyPressed(Keys.F11))
//            {
//                Engine.ToggleFullscreen();
//            }

//            rotate_x += 2f * dt;
//            rotate_y += 2.5f * dt;
//        }
//    }
//}
