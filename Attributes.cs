using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameToolkit
{
    public enum LogColor
    {
        White,
        Black,
        Gray,
        Yellow,
        Red,
        Green,
        Blue,
        Cyan,
        Magenta
    }

    public class LogStaticValue : Attribute
    {
        private LogColor _color;
        public LogColor Color { get { return _color; } }

        public LogStaticValue()
            : base()
        {
            _color = LogColor.White;
        }

        public LogStaticValue(LogColor color)
            : base()
        {
            _color = color;
        }
    }
}
