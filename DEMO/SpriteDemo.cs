using OMEGA;

namespace DEMO
{
    public class SpriteDemo : Game
    {
        public enum Animations
        {
            WalkDown
        }

        private Sprite sprite;
        private AnimatedSprite<Animations> character;

        public override void Load()
        {
            sprite = new Sprite(Engine.Content.Get<Texture2D>("party"));

            character = new AnimatedSprite<Animations>(Engine.Content.Get<TextureAtlas>("character"));

            character
                .AddAnimation(Animations.WalkDown, new int[] { 0, 1, 2, 3 }, SpriteAnimationMode.OneTime);

            character.CurrentAnimation.Mode = SpriteAnimationMode.Loop;

            sprite.SetPosition(Engine.Canvas.Width/2, Engine.Canvas.Height/2);

            character.SetPosition(sprite.X + 100 , sprite.Y + 100);

            sprite.SetSize(sprite.Width * 2f, sprite.Height * 2f);

            character.SetSize(character.Width * 4f, character.Height * 4f);

        }

        public override void FixedUpdate(float dt)
        {
            character.Update();

            if (Input.KeyPressed(Keys.Left))
            {
                character.FlipHorizontal(true);
            }

            if (Input.KeyPressed(Keys.Right))
            {
                character.FlipHorizontal(false);
            }

            if (Input.KeyDown(Keys.Left))
            {
                character.Move(-5f, 0f);
            }

            if (Input.KeyDown(Keys.Right))
            {
                character.Move(5f, 0f);
            }

            if (Input.KeyDown(Keys.Up))
            {
                character.Move(0f, -5f);
            }

            if (Input.KeyDown(Keys.Down))
            {
                character.Move(0f, 5f);
            }
        }

        public override void Draw(Canvas canvas, float dt)
        {
            canvas.Begin();

            sprite.Draw(canvas);
            character.Draw(canvas);

            canvas.End();
        }
    }
}
