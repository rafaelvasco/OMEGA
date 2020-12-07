//using OMEGA;

//namespace DEMO
//{
//    /* Draws a Triangle and a Rect together in the same Vertex Stream by drawing with offsets in DrawDevice. Uses base Position Color shader. */
//    public class VertexStreamMixedWithOffsetDemo : Game
//    {
//        VertexStream<Vertex> vertex_stream;

//        public override void Load()
//        {
//            var win_w = Engine.DisplaySize.Width;
//            var win_h = Engine.DisplaySize.Height;

//            vertex_stream = new VertexStream<Vertex>(new Vertex[] {

//                /* Triangle */

//                new Vertex (win_w/2, 100f, 0f, Color.RoyalBlue), // TOP
//                new Vertex (100f, 200f, 0f, Color.SpringGreen), // LEFT
//                new Vertex (win_w-100f, 200f, 0f, Color.LightPink), // RIGHT

//                /* Quad */

//                new Vertex (100f, 250f, 0f, Color.RoyalBlue ), // TOP-LEFT
//                new Vertex (win_w-100, 250f, 0f, Color.SpringGreen ), // TOP-RIGHT
//                new Vertex (win_w-100, win_h-100f, 0f, Color.LightPink ), // BOTTOM-RIGHT
//                new Vertex (100, win_h-100f, 0f, Color.LightPink ), // BOTTOM-LEFT

//            }, new ushort[] {

//                0, 1, 2,
//                0, 1, 2, 0, 2, 3

//            });

//        }

//        public override void VariableUpdate(float dt)
//        {
//            if (Input.GamePadPressed(GamePadButtons.Back) || Input.KeyPressed(Keys.Escape))
//            {
//                Exit();
//            }
//        }

//        public override void Draw(float dt)
//        {
//            var draw_device = Engine.Renderer;

//            // Draw Triangle
//            draw_device.SubmitVertexStream(render_pass: 0, vertex_stream, 0, 3, 0, 3);

//            // Draw Quad
//            draw_device.SubmitVertexStream(render_pass: 0, vertex_stream, 3, 4, 3, 6);
//        }
//    }
//}
