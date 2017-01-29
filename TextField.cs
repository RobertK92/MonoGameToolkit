using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameToolkit
{
    public class TextField : DrawableObject
    {
        private SpriteFont _font;
        public SpriteFont Font
        {
            get { return _font; }
            set { _font = value; }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if(_text != value)
                {
                    Vector2 stringSize = Font.MeasureString(Text);
                    SourceRect = new Rectangle(0, 0, (int)stringSize.X, (int)stringSize.Y);
                    Origin = stringSize / 2;
                }
                _text = value;
            }
        }
        
        public TextField(string text, string font)
            : base()
        {
            if (!string.IsNullOrEmpty(font))
                _font = Content.Load<SpriteFont>(font);
            else
                _font = Content.Load<SpriteFont>(MGTK.Instance.DefaultFont);

            _text = text;
            Vector2 stringSize = Font.MeasureString(Text);
            SourceRect = new Rectangle(0, 0, (int)stringSize.X, (int)stringSize.Y);
            Origin = stringSize / 2;
        }

        public TextField()
            : this("Text", string.Empty)
        { }

        public TextField(string text)
            : this(text, string.Empty)
        { }

        protected override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Effect != null)
            {
                foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    spriteBatch.DrawString(Font, Text, Position, Color, MathHelper.ToRadians(Rotation), Origin, Scale, SpriteEffects, 0.0f);
                }
            }
            else {
                spriteBatch.DrawString(Font, Text, Position, Color, MathHelper.ToRadians(Rotation), Origin, Scale, SpriteEffects, 0.0f);
            }
        }
    }
}
