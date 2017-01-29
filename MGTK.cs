using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameToolkit
{
    public abstract class MGTK : Game
    {
        public const LogColor InternalLogColor = LogColor.White;
        
        public abstract string DefaultTexture { get; }
        public abstract string DefaultFont { get; }
        
        private static MGTK _instance;
        public static MGTK Instance
        {
            get { return _instance; }
        }

        private GraphicsDeviceManager _graphics;
        public GraphicsDeviceManager Graphics { get { return _graphics; } }

        private Scene _loadedScene;
        public Scene LoadedScene { get { return _loadedScene; } }

        private KeyboardState _prevKeyboardState;
        public KeyboardState PrevKeyboardState { get { return _prevKeyboardState; } }

        private GamePadState _prevGamePadState;
        public GamePadState PrevGamePadState { get { return _prevGamePadState; } }

        private Effect _frameBufferEffect;
        public Effect FrameBufferEffect
        {
            get { return _frameBufferEffect; }
            set { _frameBufferEffect = value; }
        }

        private bool _debugDrawEnabled;
        public bool DebugDrawEnabled
        {
            get { return _debugDrawEnabled; }
            set { _debugDrawEnabled = value; }
        }

        private Color _clearColor;
        public Color ClearColor
        {
            get { return _clearColor; }
            set { _clearColor = value; }
        }

        private SpriteOptions _defaultSpriteOptions;
        public SpriteOptions DefaultSpriteOptions
        {
            get { return _defaultSpriteOptions; }
            set { _defaultSpriteOptions = value; }
        }

        private bool _loggerEnabled;
        public bool LoggerEnabled
        {
            get { return _loggerEnabled; }
            set { _loggerEnabled = value; }
        }

        private bool _debugPhysicsViewEnabled;
        public bool DebugPhysicsViewEnabled
        {
            get { return _debugPhysicsViewEnabled; }
            set { _debugPhysicsViewEnabled = value; }
        }

        private Texture2D _pixel;
        /// <summary>
        /// A white 1x1 texture.
        /// </summary>
        public Texture2D Pixel { get { return _pixel; } }

        private Matrix _projection;
        public Matrix Projection { get { return _projection; } }

        public event Action OnBackBufferSizeChanged = delegate { };
        
        private RenderTarget2D _renderTarget;
        private SpriteBatch _spriteBatch;
        private DebugDrawer _debugDrawer;
        private PerformanceStats _performanceStats;

        private int _prevBackBufferWidth;
        private int _prevBackBufferHeight;

        public MGTK()
            : base()
        {
            _instance = this;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _debugDrawEnabled = true;

            _defaultSpriteOptions = new SpriteOptions(
                    SpriteSortMode.Deferred,
                    BlendState.NonPremultiplied,
                    SamplerState.PointClamp,
                    DepthStencilState.None,
                    RasterizerState.CullCounterClockwise,
                    null, DrawingSpace.World, Matrix.Identity);

            
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _prevBackBufferWidth = _graphics.PreferredBackBufferWidth;
            _prevBackBufferHeight = _graphics.PreferredBackBufferHeight;
            _projection = Matrix.CreateOrthographicOffCenter(0f, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, 0f, 0f, 1f);
            
            _clearColor = Color.Black;

            DisplayMode display = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

            Window.Title = "MonoGameToolkit";
            Window.AllowUserResizing = false;
            Window.AllowAltF4 = true;
            
            Window.ClientSizeChanged += OnWindowResized;

            IsMouseVisible = true;

            _graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;

            OnBackBufferSizeChanged += UpdateRenderTarget;
        }

        private void OnWindowResized(object sender, EventArgs e)
        {
            _projection = Matrix.CreateOrthographicOffCenter(0f, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight, 0f, 0f, 1f);
        }

        private void UpdateRenderTarget()
        {
            _renderTarget = new RenderTarget2D(
                GraphicsDevice,
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight,
                false,                  // No mipmap. 
                SurfaceFormat.Color,    // (Unsigned format) 32-bit ARGB pixel format with alpha, using 8 bits per channel.
                DepthFormat.None);
        }

        public void LoadScene<T>() where T : Scene, new()
        {
            if (_loadedScene != null)
                _loadedScene.Unload();
            _loadedScene = new T();
            _loadedScene.LoadInternal();
        }

        protected override void Initialize()
        {
            base.Initialize();
            
        }

        protected override void LoadContent()
        {
            UpdateRenderTarget();
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new Color[] { Color.White, Color.White });
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _debugDrawer = new DebugDrawer(GraphicsDevice);
            _performanceStats = new PerformanceStats();
            Log.Init();
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            Time._deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Time._timeInSeconds += Time.DeltaTime;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_loadedScene != null)
                _loadedScene.UpdateInternal(gameTime);

            if (_graphics.PreferredBackBufferWidth != _prevBackBufferWidth || _graphics.PreferredBackBufferHeight != _prevBackBufferHeight)
            {
                _graphics.ApplyChanges();
                OnBackBufferSizeChanged.Invoke();
            }

            _prevBackBufferWidth = _graphics.PreferredBackBufferWidth;
            _prevBackBufferHeight = _graphics.PreferredBackBufferHeight;
            _prevKeyboardState = Keyboard.GetState();
            _prevGamePadState = GamePad.GetState(PlayerIndex.One);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(ClearColor);

            if (_loadedScene != null)
            {
                _loadedScene.DrawInternal(gameTime, _spriteBatch);
                if (_debugPhysicsViewEnabled)
                {
                    _loadedScene.PhysicsDebugDrawInternal();
                }
                if (_debugDrawEnabled)
                {
                    _loadedScene.DebugDrawInternal(_debugDrawer);
                    Log.UpdateScreenLogs();
                }
                if (_loggerEnabled)
                {
                    _performanceStats.Draw(_debugDrawer);
                }
            }

            // Renders the render target to the frame buffer.
            GraphicsDevice.SetRenderTarget(null);
            
            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise,
                _frameBufferEffect, Matrix.Identity);

            if (_frameBufferEffect != null)
            {
                foreach (EffectPass pass in _frameBufferEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _spriteBatch.Draw(_renderTarget, Vector2.Zero, Color.White);
                }
            }
            else
            {
                _spriteBatch.Draw(_renderTarget, Vector2.Zero, Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
