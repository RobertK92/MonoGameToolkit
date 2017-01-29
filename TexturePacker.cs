using Microsoft.Xna.Framework;
using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

namespace MonoGameToolkit
{
    public static class TexturePacker
    {
        private static Dictionary<string, TexturePackerAtlas> _atlasses = new Dictionary<string, TexturePackerAtlas>();

        public static TexturePackerAtlas LoadAtlas(string xmlFile)
        {
            if (_atlasses.ContainsKey(xmlFile))
                return _atlasses[xmlFile];
            XmlSerializer reader = new XmlSerializer(typeof(TexturePackerAtlas));
            TexturePackerAtlas atlas = null;
            using (FileStream stream = new FileStream(string.Format("Content/{0}.xml", xmlFile), FileMode.Open))
            {
                 atlas = (TexturePackerAtlas)reader.Deserialize(stream);
            }
            
            foreach(TexturePackerSprite s in atlas.Sprites)
            {
                atlas.spriteDict.Add(Path.GetFileNameWithoutExtension(s.Name), s);
            }

            string img = Path.ChangeExtension(atlas.ImagePath, null);
            atlas.ImagePath = Path.GetDirectoryName(xmlFile) + "/" + img;
            if(!_atlasses.ContainsKey(xmlFile))
                _atlasses.Add(xmlFile, atlas);
            return atlas;
        }
    }

    [XmlRoot("TextureAtlas")]
    public class TexturePackerAtlas
    {
        [XmlAttribute("imagePath")]
        public string ImagePath { get; set; }

        [XmlAttribute("width")]
        public int Width { get; set; }

        [XmlAttribute("height")]
        public int Height { get; set; }

        [XmlElement("sprite")]
        public List<TexturePackerSprite> Sprites { get; set; }

        /// <summary>
        /// Name of the TexturePacker sprite (extension not required).
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TexturePackerSprite this[string name]
        {
            get
            {
                return spriteDict[name];
                /*foreach(TexturePackerSprite sprite in Sprites)
                {
                    string checkString = string.Empty;
                    if (!Path.HasExtension(name))
                        checkString = Path.GetFileNameWithoutExtension(sprite.Name);
                    else
                        checkString = sprite.Name;

                    if (checkString == name)
                        return sprite;
                }
                return null;*/
            }
        }

        [XmlIgnore]
        internal Dictionary<string, TexturePackerSprite> spriteDict = new Dictionary<string, TexturePackerSprite>();

    }

    public class TexturePackerSprite
    {
        [XmlAttribute("n")]
        public string Name
        {
            get;set;
        }

        [XmlAttribute("x")]
        public int X { get; set; }

        [XmlAttribute("y")]
        public int Y { get; set; }

        [XmlAttribute("w")]
        public int Width { get; set; }

        [XmlAttribute("h")]
        public int Height { get; set; }

        [XmlAttribute("pX")]
        public float PivotX { get; set; }

        [XmlAttribute("pY")]
        public float PivotY { get; set; }

        [XmlAttribute("r")]
        public string R { get; set; }

        public bool Rotated { get { return (R == "y"); } }

        private Rectangle _rect = Rectangle.Empty;
        public Rectangle Rect
        {
            get
            {
                if(_rect == Rectangle.Empty)
                {
                    _rect = new Rectangle(X, Y, Width, Height);
                }
                return _rect;
            }
        }

        private Vector2 _origin;
        public Vector2 Origin
        {
            get
            {
                if(_origin == Vector2.Zero)
                {
                    _origin = new Vector2(Rect.Width * PivotX, Rect.Height * PivotY);
                }
                return _origin;
            }
        }
    }
}
