
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameToolkit
{
    public class NineSlicedSprite : Sprite
    {
        private NineSliceData _data;
        private Rectangle[] _sourcePatches;
        private Rectangle[] _destPatches;

        public NineSlicedSprite(string texture, NineSliceData data)
            : base(texture)
        {
            _data = data;
            Init();
        }

        public NineSlicedSprite(Texture2D texture, NineSliceData data)
            : base(texture)
        {
            _data = data;
            Init();
        }

        private void Init()
        {
            _sourcePatches = CreatePatches(Texture.Bounds);
            OnTransformed += UpdateDestPatches;
            UpdateDestPatches();
        }

        private void UpdateDestPatches()
        {
            _destPatches = CreatePatches(new Rectangle(
                    (int)(Bounds.X + (Origin.X * Scale.X)),
                    (int)(Bounds.Y + (Origin.Y * Scale.Y)),
                    Bounds.Width / 2, Bounds.Height / 2));
        }

        protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < _destPatches.Length; i++)
            {
                if (Effect != null)
                {
                    foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        spriteBatch.Draw(Texture, _destPatches[i], _sourcePatches[i], Color, MathHelper.ToRadians(Rotation), Vector2.Zero, SpriteEffects, 0.0f);
                    }
                }
                else
                {
                    spriteBatch.Draw(Texture, _destPatches[i], _sourcePatches[i], Color, MathHelper.ToRadians(Rotation), Vector2.Zero, SpriteEffects, 0.0f);
                }
            }
        }
        
        private Rectangle[] CreatePatches(Rectangle rectangle)
        {
            int x = rectangle.X;
            int y = rectangle.Y;
            int w = rectangle.Width;
            int h = rectangle.Height;
            int middleWidth = w - _data.LeftPadding - _data.RightPadding;
            int middleHeight = h - _data.TopPadding - _data.BottomPadding;
            int bottomY = y + h - _data.BottomPadding;
            int rightX = x + w - _data.RightPadding;
            int leftX = x + _data.LeftPadding;
            int topY = y + _data.TopPadding;

            Rectangle[] patches = new[]
            {
                new Rectangle(x,      y,        _data.LeftPadding,  _data.TopPadding),      // top left
                new Rectangle(leftX,  y,        middleWidth,        _data.TopPadding),      // top middle
                new Rectangle(rightX, y,        _data.RightPadding, _data.TopPadding),      // top right
                new Rectangle(x,      topY,     _data.LeftPadding,  middleHeight),          // left middle
                new Rectangle(leftX,  topY,     middleWidth,        middleHeight),          // middle
                new Rectangle(rightX, topY,     _data.RightPadding, middleHeight),          // right middle
                new Rectangle(x,      bottomY,  _data.LeftPadding,  _data.BottomPadding),   // bottom left
                new Rectangle(leftX,  bottomY,  middleWidth,        _data.BottomPadding),   // bottom middle
                new Rectangle(rightX, bottomY,  _data.RightPadding, _data.BottomPadding)    // bottom right
            };
            return patches;
        }
    }
}
