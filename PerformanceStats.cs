using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MonoGameToolkit
{
    public class PerformanceStats
    {
        private bool _visible;
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        private List<LogData> _internalLogData = new List<LogData>();
        private List<LogData> _logData = new List<LogData>();

        private Vector2 _textOffset;
        private Vector2 _boxSize;
        private Vector2 _boxOffset;
        private float _textScale;
        
        internal PerformanceStats()
        {
            _boxSize = new Vector2(200, 300);
            _boxOffset = new Vector2(MGTK.Instance.Graphics.PreferredBackBufferWidth - _boxSize.X, 10);
            _textOffset = new Vector2(10, 10);
            _textScale = 1.0f;
            
            FindFieldsAndProperties(Assembly.GetExecutingAssembly(), ref _internalLogData);
            FindFieldsAndProperties(Assembly.GetEntryAssembly(), ref _logData);
        }

        private void FindFieldsAndProperties(Assembly ass, ref List<LogData> data)
        {
            foreach (Type classType in ass.GetTypes())
            {
                /* static fields */
                foreach (FieldInfo field in classType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    
                    if (field.GetCustomAttributes(typeof(LogStaticValue), false).Length > 0)
                    {
                        LogStaticValue attribute = field.GetCustomAttribute<LogStaticValue>();
                        StringBuilder fieldName = new StringBuilder(field.Name);
                        fieldName[0] = char.ToUpper(fieldName[0]);
                        data.Add(new LogData(fieldName.ToString(), field, null, attribute.Color));
                    }
                }

                /* static properties */
                foreach (PropertyInfo prop in classType.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (prop.GetCustomAttributes(typeof(LogStaticValue), false).Length > 0)
                    {
                        LogStaticValue attribute = prop.GetCustomAttribute<LogStaticValue>();
                        StringBuilder propName = new StringBuilder(prop.Name);
                        propName[0] = char.ToUpper(propName[0]);
                        data.Add(new LogData(propName.ToString(), null, prop, attribute.Color));
                    }
                }
            }
        }

        internal void Draw(DebugDrawer drawer)
        {
            Vector2 boxPos = new Vector2(_boxOffset.X, _boxOffset.Y);
            Vector2 offset = _textOffset;
            Vector2 biggestTextField = Vector2.Zero;
            int count = 0;
            
            foreach(LogData data in _internalLogData)
            {
                DrawToLogBox(drawer, data, boxPos, ref offset, ref biggestTextField);
                count++;
            }

            foreach(LogData data in _logData)
            {
                DrawToLogBox(drawer, data, boxPos, ref offset, ref biggestTextField);
                count++;
            }

            if (biggestTextField.X > _boxSize.X)
                _boxSize = new Vector2(biggestTextField.X + _textOffset.X * 2, biggestTextField.Y * count + (_textOffset.Y * 2));
            //drawer.DrawRect(boxPos, _boxSize, Color.Gray, DrawingSpace.Screen);
        }

        private void DrawToLogBox(DebugDrawer drawer, LogData data, Vector2 boxPos, ref Vector2 offset, ref Vector2 biggestTextField)
        {
            string value = string.Empty;
            if (data.fieldInfo != null)
                value = data.fieldInfo.GetValue(null).ToString();
            else if (data.propertyInfo != null)
                value = data.propertyInfo.GetValue(null).ToString();

            string text = string.Format("{0} : {1}", data.name, value);
            Vector2 size = drawer.Font.MeasureString(text) * _textScale;
            if (size.X > biggestTextField.X)
                biggestTextField = size;
            Color color = GetColor(data.logColor);
            drawer.DrawText(boxPos + offset + (size / 2), text, color, _textScale, DrawingSpace.Screen);
            offset += new Vector2(0, (size.Y));
        }
        
        private Color GetColor(LogColor logColor)
        {
            switch (logColor)
            {
                case LogColor.White:
                    return Color.White;
                case LogColor.Black:
                    return Color.Black;
                case LogColor.Gray:
                    return Color.Gray;
                case LogColor.Yellow:
                    return Color.Yellow;
                case LogColor.Red:
                    return Color.Red;
                case LogColor.Green:
                    return Color.Green;
                case LogColor.Blue:
                    return Color.Blue;
                case LogColor.Cyan:
                    return Color.Cyan;
                case LogColor.Magenta:
                    return Color.Magenta;
                default:
                    break;
            }
            return Color.White;
        }
    }
}
