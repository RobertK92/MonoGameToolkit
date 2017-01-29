using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameToolkit
{
    public class KeyFrame
    {
        public readonly Rectangle SourceRect;
        public readonly Vector2 Origin;

        public KeyFrame(Rectangle sourceRect, Vector2 origin)
        {
            this.SourceRect = sourceRect;
            this.Origin = origin;
        }

        public KeyFrame(TexturePackerSprite texturePackerSprite)
        {
            SourceRect = texturePackerSprite.Rect;
            Origin = texturePackerSprite.Origin;
        }
    }
}
