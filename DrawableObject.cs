using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameToolkit
{
    public abstract class DrawableObject : BaseObject
    {
        public SpriteSortMode SpriteSortMode
        {
            get { return _spriteOptions.SpriteSortMode; }
            set
            {
                if (_spriteOptions.SpriteSortMode != value)
                {
                    _spriteOptions.SpriteSortMode = value;
                    LoadedScene.OrderDrawable(this);
                }
            }
        }
        public BlendState BlendState
        {
            get { return _spriteOptions.BlendState; }
            set
            {
                if (_spriteOptions.BlendState != value)
                {
                    _spriteOptions.BlendState = value;
                    LoadedScene.OrderDrawable(this);
                }
            }
        }
        public SamplerState SamplerState
        {
            get { return _spriteOptions.SamplerState; }
            set
            {
                if(_spriteOptions.SamplerState != value)
                {
                    _spriteOptions.SamplerState = value;
                    LoadedScene.OrderDrawable(this);
                }
            }
        }
        public DepthStencilState DepthStencilState
        {
            get { return _spriteOptions.DepthStencilState; }
            set
            {
                if(_spriteOptions.DepthStencilState != value)
                {
                    _spriteOptions.DepthStencilState = value;
                    LoadedScene.OrderDrawable(this);
                }
            }
        }
        public RasterizerState RasterizerState
        {
            get { return _spriteOptions.RasterizerState; }
            set
            {
                if(_spriteOptions.RasterizerState != value)
                {
                    _spriteOptions.RasterizerState = value;
                    LoadedScene.OrderDrawable(this);
                }
            }
        }
        public Effect Effect
        {
            get { return _spriteOptions.Effect; }
            set
            {
                if(_spriteOptions.Effect != value)
                {
                    _spriteOptions.Effect = value;
                    LoadedScene.OrderDrawable(this);
                }
            }
        }
        public Matrix SpriteBatchMatrix
        {
            get { return _spriteOptions.TransformMatrix; }
            set
            {
                if(_spriteOptions.TransformMatrix != value)
                {
                    _spriteOptions.TransformMatrix = value;
                    LoadedScene.OrderDrawable(this);
                }
            }
        }
        
        public int DrawOrder
        {
            get { return _spriteOptions.DrawOrder; }
            set
            {
                if (_spriteOptions.DrawOrder != value)
                {
                    _spriteOptions.DrawOrder = value;
                    LoadedScene.OrderDrawable(this);
                }
            }
        }
        
        public DrawingSpace DrawingSpace
        {
            get { return _spriteOptions.DrawingSpace; }
            set
            {
                if(_spriteOptions.DrawingSpace != value)
                {
                    _spriteOptions.DrawingSpace = value;
                    LoadedScene.OrderDrawable(this);
                }
            }
        }
        
        private Color _color;
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        private bool _visible;
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        private float _opacity;
        public float Opacity
        {
            get { return _opacity; }
            set
            {
                if(_opacity != value)
                {
                    Color temp = Color;
                    Color = new Color(temp, value);
                }
                _opacity = value;
            }
        }

        private Rectangle _sourceRect;
        public Rectangle SourceRect
        {
            get { return _sourceRect; }
            set
            {
                if (_sourceRect != value)
                {
                    _sourceRect = value;
                    Origin = new Vector2(SourceRect.Width / 2, SourceRect.Height / 2);
                }
            }
        }

        private Vector2 _origin;
        public Vector2 Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }

        private SpriteEffects _spriteEffects;
        public SpriteEffects SpriteEffects
        {
            get { return _spriteEffects; }
            set { _spriteEffects = value; }
        }

        private SpriteOptions _spriteOptions;
        public SpriteOptions SpriteOptions { get { return _spriteOptions; } }

        public Vector2 Size
        {
            get { return new Vector2(SourceRect.Width * Scale.X, SourceRect.Height * Scale.Y); }
        }

        public Rectangle Bounds
        {
            get
            {
                Vector2 currentSize = this.Size;
                return new Rectangle(
                    (int)(Position.X - (currentSize.X / 2)), 
                    (int)(Position.Y - (currentSize.Y / 2)), 
                    (int)(SourceRect.Width * Scale.X), 
                    (int)(SourceRect.Height * Scale.Y));
            }
        }

        public DrawableObject() 
            : base()
        {
            _color          = Color.White;
            _spriteEffects  = SpriteEffects.None;
            _spriteOptions  = SpriteOptions.Default;
            _visible        = true;
            _opacity        = 1.0f;

            LoadedScene.OrderDrawable(this);
        }
        
        internal void DrawInternal(GameTime gameTime, SpriteBatch spriteBatch) { Draw(gameTime, spriteBatch); }
        protected abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
