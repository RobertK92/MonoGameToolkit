using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameToolkit
{
    public class Sprite : DrawableObject
    {
        private Texture2D _texture;
        public Texture2D Texture { get { return _texture; } }


        public Sprite(Texture2D texture)
            : base()
        {
            _texture = texture;
            SourceRect = new Rectangle(0, 0, _texture.Width, _texture.Height);
            Origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);
        }

        public Sprite(string texture)
            : base()
        {
            _texture = Content.Load<Texture2D>(texture);
            SourceRect = new Rectangle(0, 0, _texture.Width, _texture.Height);
            Origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);
        }

        public Sprite() 
            : this(MGTK.Instance.DefaultTexture) { }
        
        public void ChangeTexture(string texture, bool updateSourceRect, bool updateOrigin)
        {
            ChangeTexture(Content.Load<Texture2D>(texture), updateSourceRect, updateOrigin);
        }

        public void ChangeTexture(string texture, bool updateSourceRect)
        {
            ChangeTexture(Content.Load<Texture2D>(texture), updateSourceRect, false);
        }

        public void ChangeTexture(Texture2D texture, bool updateSourceRect)
        {
            ChangeTexture(texture, updateSourceRect, false);
        }

        public void ChangeTexture(Texture2D texture, bool updateSourceRect, bool updateOrigin)
        {
            _texture = texture;
            if(updateSourceRect)
                SourceRect = new Rectangle(0, 0, _texture.Width, _texture.Height);
            if(updateOrigin)
                Origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);
        }

        protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Effect != null)
            {
                foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    spriteBatch.Draw(Texture, Position, SourceRect, Color, MathHelper.ToRadians(Rotation), Origin, Scale, SpriteEffects, 0.0f);
                }
            }
            else {
                spriteBatch.Draw(Texture, Position, SourceRect, Color, MathHelper.ToRadians(Rotation), Origin, Scale, SpriteEffects, 0.0f);
            }
        }
    }
}
