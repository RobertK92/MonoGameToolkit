using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace MonoGameToolkit
{
    public struct SpriteOptions
    {
        public static SpriteOptions Default
        {
            get
            {
                return MGTK.Instance.DefaultSpriteOptions;
            }
        }

        private DrawingSpace _drawingSpace;
        public DrawingSpace DrawingSpace
        {
            get { return _drawingSpace; }
            internal set { _drawingSpace = value; }
        }

        private SpriteSortMode _spriteSortMode;
        public SpriteSortMode SpriteSortMode
        {
            get { return _spriteSortMode; }
            internal set { _spriteSortMode = value; }
        }

        private BlendState _blendState;
        public BlendState BlendState
        {
            get { return _blendState; }
            internal set { _blendState = value; }
        }

        private SamplerState _samplerState;
        public SamplerState SamplerState
        {
            get { return _samplerState; }
            internal set { _samplerState = value; }
        }

        private DepthStencilState _depthStencilState;
        public DepthStencilState DepthStencilState
        {
            get { return _depthStencilState; }
            internal set { _depthStencilState = value; }
        }

        private RasterizerState _rasterizerState;
        public RasterizerState RasterizerState
        {
            get { return _rasterizerState; }
            internal set { _rasterizerState = value; }
        }

        private Effect _effect;
        public Effect Effect
        {
            get { return _effect; }
            internal set { _effect = value; }
        }

        private Matrix _transformMatrix;
        public Matrix TransformMatrix
        {
            get { return _transformMatrix; }
            internal set { _transformMatrix = value; }
        }

        private int _drawOrder;
        public int DrawOrder
        {
            get { return _drawOrder; }
            internal set { _drawOrder = value; }
        }

        public SpriteOptions(
            SpriteSortMode spriteSortMode,
            BlendState blendState,
            SamplerState samplerState,
            DepthStencilState depthStencilState,
            RasterizerState rasterizerState,
            Effect effect, DrawingSpace space,
            Matrix transformMatrix)
        {
            _spriteSortMode     = spriteSortMode;
            _blendState         = blendState;
            _drawingSpace       = space;
            _samplerState       = samplerState;
            _depthStencilState  = depthStencilState;
            _rasterizerState    = rasterizerState;
            _effect             = effect;
            _transformMatrix    = transformMatrix;
            _drawOrder          = 0;
        }

        public bool Equals(SpriteOptions b)
        {
            if (b == null)
                return false;
            return (
                this.SpriteSortMode == b.SpriteSortMode &&
                this.BlendState == b.BlendState &&
                this.SamplerState == b.SamplerState &&
                this.DepthStencilState == b.DepthStencilState &&
                this.RasterizerState == b.RasterizerState &&
                this.Effect == b.Effect &&
                this.TransformMatrix == b.TransformMatrix &&
                this.DrawOrder == b.DrawOrder &&
                this.DrawingSpace == b.DrawingSpace);
        }

        public override bool Equals(object obj)
        {
            return Equals((SpriteOptions)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(SpriteOptions a, SpriteOptions b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if ((object)a == null || (object)b == null)
                return false;
            return a.Equals(b);
        }

        public static bool operator !=(SpriteOptions a, SpriteOptions b)
        {
            return !(a == b);
        }
    }
}
