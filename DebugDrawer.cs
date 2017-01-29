using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGameToolkit
{
    public enum DrawingSpace
    {
        World,
        Screen
    }

    public class DebugDrawer
    {
        private const int MaxVerticesPerCall = 2048;

        private int _circleSmoothness;
        public int CircleSmoothness
        {
            get { return _circleSmoothness; }
            set
            {
                if (_circleSmoothness != value)
                {
                    _circleSmoothness = value;
                    UpdateCosSinTables();
                }
            }
        }

        private SpriteFont _font;
        public SpriteFont Font { get { return _font; } }

        private float[] _circleSinTable = new float[0];
        private float[] _circleCosTable = new float[0];

        private GraphicsDevice _device;
        private VertexBuffer _vertexBuffer;
        private BasicEffect _basicEffect;
        
        private SpriteBatch _spriteBatch;

        public DebugDrawer(GraphicsDevice graphicsDevice)
        {
            _device = graphicsDevice;
            _spriteBatch = new SpriteBatch(_device);
            _font = MGTK.Instance.Content.Load<SpriteFont>(MGTK.Instance.DefaultFont); 
            _basicEffect = new BasicEffect(_device);
            _vertexBuffer = new VertexBuffer(_device, typeof(VertexPositionColor), MaxVerticesPerCall, BufferUsage.WriteOnly);
            _circleSmoothness = 64;
            UpdateCosSinTables();
        }

        private void UpdateCosSinTables()
        {
            float twicePi = (float)Math.PI * 2;
            _circleSinTable = new float[_circleSmoothness * 2 + 1];
            _circleCosTable = new float[_circleSmoothness * 2 + 1];
            for (int i = 0; i <= _circleSmoothness * 2; i++)
            {
                _circleSinTable[i] = (float)Math.Sin(i * twicePi / _circleSmoothness);
                _circleCosTable[i] = (float)Math.Cos(i * twicePi / _circleSmoothness);
            }
        }

        public void DrawVertices(VertexPositionColor[] vertices, PrimitiveType primType, int primCount, DrawingSpace space)
        {
            if(vertices.Length > MaxVerticesPerCall)
            {
                _vertexBuffer.Dispose();
                _vertexBuffer = new VertexBuffer(_device, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            }

            _vertexBuffer.SetData(vertices);

            Camera2D camera = MGTK.Instance.LoadedScene.ActiveCamera;
            Matrix matrix = camera != null ? camera.Matrix : Matrix.Identity;

            _basicEffect.View = space == DrawingSpace.World ? matrix : Matrix.Identity;
            _basicEffect.Projection = Matrix.CreateOrthographicOffCenter(
                0, MGTK.Instance.Graphics.GraphicsDevice.Viewport.Width,
                MGTK.Instance.Graphics.GraphicsDevice.Viewport.Height, 0, 0, 1);

            _basicEffect.VertexColorEnabled = true;
            
            _device.SetVertexBuffer(_vertexBuffer);
            
            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _device.DrawPrimitives(primType, 0, primCount);
            }
        }

        public void DrawText(Vector2 position, string text, Color color, float scale, DrawingSpace space = DrawingSpace.World)
        {
            Matrix matrix = Matrix.Identity;
            if(space == DrawingSpace.World)
                matrix = MGTK.Instance.LoadedScene.ActiveCamera != null ? MGTK.Instance.LoadedScene.ActiveCamera.Matrix : Matrix.Identity;

            _spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise,
                null, matrix);
            Vector2 size = _font.MeasureString(text);
            _spriteBatch.DrawString(_font, text, position, color, 0.0f, new Vector2(size.X / 2, size.Y / 2), scale, SpriteEffects.None, 0.0f);
            _spriteBatch.End();
        }

        public void DrawText(Vector2 position, string text, Color color, DrawingSpace space = DrawingSpace.World)
        {
            DrawText(position, text, color, 1.0f, space);
        }

        public void DrawLine(Vector2 from, Vector2 to, Color color, DrawingSpace space = DrawingSpace.World)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[2];
            vertices[0] = new VertexPositionColor(new Vector3(from.X, from.Y, 0), color);
            vertices[1] = new VertexPositionColor(new Vector3(to.X, to.Y, 0), color);
            DrawVertices(vertices, PrimitiveType.LineList, vertices.Length, space);
        }

        public void DrawCircle(Vector2 position, float radius, Color color, DrawingSpace space = DrawingSpace.World)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[CircleSmoothness + 1];
            for (int i = 0; i <= CircleSmoothness; i++)
            {
                float x = position.X + (radius * _circleCosTable[i]);
                float y = position.Y + (radius * _circleSinTable[i]);
                vertices[i] = new VertexPositionColor(new Vector3(x, y, 0.0f), color);
            }
            DrawVertices(vertices, PrimitiveType.LineStrip, CircleSmoothness, space);
        }

        public void DrawFilledCircle(Vector2 position, int radius, Color color, DrawingSpace space = DrawingSpace.World)
        {
            throw new NotImplementedException();
        }

        public void DrawRect(Vector2 position, Vector2 size, Color color, DrawingSpace space = DrawingSpace.World)
        {
            DrawRect(new Rectangle(position.ToPoint(), size.ToPoint()), color, space);
        }

        public void DrawRect(Rectangle rect, Color color, DrawingSpace space = DrawingSpace.World)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[8];

            Vector2 topLeft = new Vector2(rect.X, rect.Y);
            Vector2 topRight = new Vector2(rect.X + rect.Width, rect.Y);
            Vector2 bottomLeft = new Vector2(rect.X, rect.Y + rect.Height);
            Vector2 bottomRight = new Vector2(rect.X + rect.Width, rect.Y + rect.Height);

            vertices[0] = new VertexPositionColor(new Vector3(topLeft.X, topLeft.Y, 0), color);
            vertices[1] = new VertexPositionColor(new Vector3(topRight.X, topRight.Y, 0), color);

            vertices[2] = new VertexPositionColor(new Vector3(topRight.X, topRight.Y, 0), color);
            vertices[3] = new VertexPositionColor(new Vector3(bottomRight.X, bottomRight.Y, 0), color);

            vertices[4] = new VertexPositionColor(new Vector3(bottomRight.X, bottomRight.Y, 0), color);
            vertices[5] = new VertexPositionColor(new Vector3(bottomLeft.X, bottomLeft.Y, 0), color);

            vertices[6] = new VertexPositionColor(new Vector3(bottomLeft.X, bottomLeft.Y, 0), color);
            vertices[7] = new VertexPositionColor(new Vector3(topLeft.X, topLeft.Y, 0), color);

            DrawVertices(vertices, PrimitiveType.LineList, vertices.Length, space);
        }

        public void DrawFilledRect(Vector2 position, Vector2 size, Color color, DrawingSpace space = DrawingSpace.World)
        {
            throw new NotImplementedException();
        }

        public void DrawToSpriteBatch(Texture2D texture, Vector2 position, Rectangle sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects, float layer, DrawingSpace space = DrawingSpace.World)
        {
            Matrix matrix = Matrix.Identity;
            if(space == DrawingSpace.World)
                matrix = MGTK.Instance.LoadedScene.ActiveCamera != null ? MGTK.Instance.LoadedScene.ActiveCamera.Matrix : Matrix.Identity;

            _spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.Additive,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise,
                null, matrix);
            _spriteBatch.Draw(texture, position, sourceRect, color, rotation, origin, scale, spriteEffects, layer);
            _spriteBatch.End();
        }

        public void DrawTransformation(BaseObject obj)
        {
            float scaleX = obj.Scale.X > 0.0f ? obj.Scale.X : 0.1f;
            float scaleY = obj.Scale.Y > 0.0f ? obj.Scale.Y : 0.1f;
            DrawLine(obj.Position, obj.Position + obj.Right * scaleX * 32.0f, Color.Red);
            DrawLine(obj.Position, obj.Position + obj.Up * scaleY * 32.0f, Color.Green);
        }
    }
}
