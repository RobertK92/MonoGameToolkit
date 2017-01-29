using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using System;

namespace MonoGameToolkit
{
    public abstract class Scene
    {
        [LogStaticValue(MGTK.InternalLogColor)]
        public static int ObjectCount { get { return MGTK.Instance.LoadedScene.Objects.Count; } }

        [LogStaticValue(MGTK.InternalLogColor)]
        public static int Batches { get { return MGTK.Instance.LoadedScene.Drawables.Count; } }
        
        protected ContentManager Content { get { return MGTK.Instance.Content; } }
        protected GraphicsDevice GraphicsDevice { get { return MGTK.Instance.GraphicsDevice; } }
        protected GraphicsDeviceManager Graphics { get { return MGTK.Instance.Graphics; } }

        private List<BaseObject> _objects = new List<BaseObject>();
        public List<BaseObject> Objects { get { return _objects; } }

        private Dictionary<SpriteOptions, List<DrawableObject>> _drawables = new Dictionary<SpriteOptions, List<DrawableObject>>();
        public Dictionary<SpriteOptions, List<DrawableObject>> Drawables { get { return _drawables; } }

        private World _physicsWorld;
        public World PhysicsWorld { get { return _physicsWorld; } }

        private Camera2D _activeCamera;
        public Camera2D ActiveCamera
        {
            get { return _activeCamera; }
            set
            {
                if (value == null)
                    throw new Exception("Active camera cannot be null.");
                _activeCamera = value;
            }
        }

        private Coroutines _coroutines = new Coroutines();
        internal Coroutines Coroutines { get { return _coroutines; } }

        private PhysicsDebugView _physicsDebugView;
        private ContactListener _contactListener;
        private float _physicsTimeStep;
        private float _physicsAccumulator;

        public abstract void Load();
        public virtual void Unload() { }
        public virtual void Update(GameTime gameTime) { }
        public virtual void DebugDraw(DebugDrawer drawer) { }

        public IEnumerable<T> GetObjects<T>() where T : BaseObject
        {
            return Enumerable.Cast<T>(Objects.Where(x => x.GetType() == typeof(T)));
        }

        public T GetObject<T>() where T : BaseObject
        {
            return (T)Objects.FirstOrDefault(x => x.GetType() == typeof(T));
        }

        public BaseObject GetObject(string name)
        {
            return Objects.FirstOrDefault(x => x.Name == name);
        }

        public IEnumerable<BaseObject> GetObjects(string name)
        {
            return Objects.Where(x => x.Name == name);
        }
        
        internal void LoadInternal()
        {
            _activeCamera = new Camera2D();
            _physicsWorld = new World(Physics.Gravity);
            _physicsDebugView = new PhysicsDebugView(_physicsWorld);
            _physicsDebugView.LoadContent(GraphicsDevice, Content, MGTK.Instance.DefaultFont);
            _physicsDebugView.AppendFlags(
                FarseerPhysics.DebugViewFlags.PerformanceGraph | 
                FarseerPhysics.DebugViewFlags.DebugPanel | 
                FarseerPhysics.DebugViewFlags.Joint | 
                FarseerPhysics.DebugViewFlags.ContactPoints);

            _physicsDebugView.Enabled = true;
            _contactListener = new ContactListener(_physicsWorld.ContactManager);

            FarseerPhysics.Settings.VelocityIterations = 8;
            FarseerPhysics.Settings.PositionIterations = 3;
            _physicsTimeStep = 1.0f / 60.0f;
            
            Load();
        }

        internal void OrderDrawable(DrawableObject drawable)
        {
            bool contains = false;
            SpriteOptions? removeKey = null;
            foreach(KeyValuePair<SpriteOptions, List<DrawableObject>> kvp in _drawables)
            {
                if (kvp.Value.Contains(drawable))
                {
                    kvp.Value.Remove(drawable);
                    removeKey = kvp.Key;
                }
                if (kvp.Key == drawable.SpriteOptions)
                    contains = true;
            }
            
            if(!contains)
                _drawables.Add(drawable.SpriteOptions, new List<DrawableObject>());

            _drawables[drawable.SpriteOptions].Add(drawable);

            if (removeKey.HasValue)
            {
                if (_drawables[removeKey.Value].Count == 0)
                    _drawables.Remove(removeKey.Value);
            }

            OrderBatches();
        }

        private void OrderBatches()
        {
            IEnumerable<KeyValuePair<SpriteOptions, List<DrawableObject>>> ordered = _drawables.OrderBy(x => x.Key.DrawOrder).ToList();
            _drawables.Clear();
            foreach(KeyValuePair<SpriteOptions, List<DrawableObject>> item in ordered)
                _drawables.Add(item.Key, item.Value);
        }

        internal void UpdatePhysics(float dt)
        {
            _physicsAccumulator += dt;
            while (_physicsAccumulator >= _physicsTimeStep)
            {
                for (int i = _objects.Count - 1; i >= 0; i--)
                {
                    if (_objects[i].Destroying)
                    {
                        _objects[i].OnDestroyInternal();
                        _objects[i] = null;
                        _objects.RemoveAt(i);
                    }
                    else
                    {
                        _objects[i].FixedUpdateInternal();
                    }
                }
                PhysicsWorld.Step(_physicsTimeStep);
                _contactListener.NotifyObjects();
                _physicsAccumulator -= _physicsTimeStep;
            }
        }

        internal void UpdateInternal(GameTime gameTime)
        {
            UpdatePhysics((float)gameTime.ElapsedGameTime.TotalSeconds);
            for (int i = _objects.Count - 1; i >= 0; i--)
            {
                if (_objects[i].Destroying)
                {
                    _objects[i].OnDestroyInternal();
                    _objects[i] = null;
                    _objects.RemoveAt(i);
                }
                else {
                    _objects[i].UpdateInternal(gameTime);
                }
            }
            _coroutines.Update();
            Update(gameTime);
        }
        
        internal void DebugDrawInternal(DebugDrawer drawer)
        {
            for (int i = _objects.Count - 1; i >= 0; i--)
                _objects[i].DebugDrawInternal(drawer);
            DebugDraw(drawer);
        }

        internal void PhysicsDebugDrawInternal()
        {
            _physicsDebugView.RenderDebugData(MGTK.Instance.Projection, ActiveCamera.PhysicsMatrix);
        }

        internal void DrawInternal(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteOptions? removeKey = null;
            foreach (KeyValuePair<SpriteOptions, List<DrawableObject>> kvp in _drawables)
            {
                SpriteBatchBegin(spriteBatch, kvp.Key);
                for (int i = kvp.Value.Count - 1; i >= 0; i--)
                {
                    if (kvp.Value[i].Destroying)
                    {
                        kvp.Value[i] = null;
                        kvp.Value.RemoveAt(i);
                        if(kvp.Value.Count == 0)
                            removeKey = kvp.Key;
                    }
                    else
                    {
                        if(kvp.Value[i].Visible)
                            kvp.Value[i].DrawInternal(gameTime, spriteBatch);
                    }
                }
                SpriteBatchEnd(spriteBatch);
            }
            
            if (removeKey.HasValue)
                _drawables.Remove(removeKey.Value);
        }

        private void SpriteBatchBegin(SpriteBatch spriteBatch, SpriteOptions options)
        {
            Matrix camMat = ActiveCamera.Matrix;
            
            if (options.DrawingSpace == DrawingSpace.Screen)
                camMat = Matrix.Identity;

            spriteBatch.Begin(
                options.SpriteSortMode, 
                options.BlendState, 
                options.SamplerState, 
                options.DepthStencilState, 
                options.RasterizerState, 
                options.Effect, 
                camMat * options.TransformMatrix);   
        }

        private void SpriteBatchEnd(SpriteBatch spriteBatch)
        {
            spriteBatch.End();
        }
    }
}

