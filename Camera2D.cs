using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Collections;

namespace MonoGameToolkit
{
    public class Camera2D : BaseObject
    {
        private Matrix _physicsMatrix;
        internal Matrix PhysicsMatrix { get { return _physicsMatrix; } }

        private Matrix _matrix;
        public Matrix Matrix { get { return _matrix; } }

        private float _zoom;
        public float Zoom
        {
            get { return _zoom; }
            set
            {
                if (_zoom != value)
                {
                    _zoom = MathHelper.Clamp(value, 0.0f, float.MaxValue);
                    UpdateMatrix();
                }
            }
        }

        private Point _size;
        public Point Size
        {
            get { return _size; }
            private set
            {
                if (_size != value)
                {
                    _size = value;
                    UpdateMatrix();
                }
            }
        }

        private Rectangle _visibleArea;
        public Rectangle VisibleArea { get { return _visibleArea; } }

        public int Width
        {
            get { return Size.X; }
            set { Size = new Point(value, Size.Y); }
        }

        public int Height
        {
            get { return Size.Y; }
            set { Size = new Point(Size.X, value); }
        }

        private Vector2 _prevPosition;
        private float _prevRotation;
        
        public Camera2D(int width, int height)
            : base()
        {
            Name = "Camera2D";
            _zoom = 1.0f;
            _size = new Point(width, height);
            UpdateMatrix();
        }

        public Camera2D()
            : this(MGTK.Instance.Graphics.PreferredBackBufferWidth, MGTK.Instance.Graphics.PreferredBackBufferHeight)
        { }

        public void UpdateMatrix()
        {
            float w = Size.X;
            float h = Size.Y;
            float radians = MathHelper.ToRadians(Rotation);
            float cos0 = (float)Math.Cos(radians);
            float sin0 = (float)Math.Sin(radians);
            float ppu = Physics.UPP;
            Vector2 localPosition = new Vector2((Position.X * cos0 + Position.Y * sin0), (-Position.X * sin0 + Position.Y * cos0));

            _matrix =
                Matrix.CreateTranslation(-(w * 0.5f), -(h * 0.5f), 0.0f) *
                Matrix.CreateTranslation(-localPosition.X, -localPosition.Y, 0.0f) *
                Matrix.CreateScale(ppu) *
                Matrix.CreateScale(Zoom / ppu, Zoom / ppu, 1.0f) *
                Matrix.CreateRotationZ(radians) *
                Matrix.CreateTranslation((w * 0.5f), (h * 0.5f), 0.0f);

            _physicsMatrix =
                Matrix.CreateTranslation(-(w * 0.5f) * ppu, -(h * 0.5f) * ppu, 0.0f) *
                Matrix.CreateTranslation(-localPosition.X * ppu, -localPosition.Y * ppu, 0.0f) *
                Matrix.CreateScale(Zoom / ppu, Zoom / ppu, 1.0f) *
                Matrix.CreateRotationZ(radians) *
                Matrix.CreateTranslation((w * 0.5f), (h * 0.5f), 0.0f);
            
            UpdateVisibleArea();
        }

        private void UpdateVisibleArea()
        {
            Matrix inverseViewMatrix = Matrix.Invert(Matrix);
            Vector2 tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
            Vector2 tr = Vector2.Transform(new Vector2(Size.X, 0), inverseViewMatrix);
            Vector2 bl = Vector2.Transform(new Vector2(0, Size.Y), inverseViewMatrix);
            Vector2 br = Vector2.Transform(Size.ToVector2(), inverseViewMatrix);
            Vector2 min = new Vector2(
                MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
            Vector2 max = new Vector2(
                MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
            _visibleArea = new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
        }

        public void Shake(Vector2 axis, float distance, float duration, float intensity)
        {
            Coroutines.Run(ShakeRoutine(axis, distance, intensity, duration), "CameraShake");
        }

        public void StopAllShakes()
        {
            Coroutines.Stop("CameraShake");
        }

        private IEnumerator ShakeRoutine(Vector2 axis, float distance, float intensity, float duration)
        {
            Vector2 offset = Vector2.Zero;
            Vector2 totalOffset = Vector2.Zero;
            bool back = false;

            while(duration > 0.0f)
            {
                yield return null;
                duration -= Time.DeltaTime;

                if (totalOffset.X >= axis.X * distance && totalOffset.Y >= axis.Y * distance && !back)
                    back = true;
                else if (totalOffset.X <= -axis.X * distance && totalOffset.Y <= -axis.Y * distance && back)
                    back = false;
                
                if(back)
                    offset = -axis * intensity; 
                else
                    offset = axis * intensity;

                Position += offset;
                totalOffset += offset;
            }
            
            Position -= totalOffset;
        }

        protected override void Update(GameTime gameTime)
        {
            if(_prevPosition != Position || _prevRotation != Rotation)
            {
                UpdateMatrix();
            }

            _prevPosition = Position;
            _prevRotation = Rotation;
        }

        protected override void OnDestroy()
        {
            StopAllShakes();
        }
    }
}
