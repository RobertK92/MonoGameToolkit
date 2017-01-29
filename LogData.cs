using System.Reflection;

namespace MonoGameToolkit
{
    internal struct LogData
    {
        internal string name;
        internal FieldInfo fieldInfo;
        internal PropertyInfo propertyInfo;
        internal LogColor logColor;

        internal LogData(string name, FieldInfo fieldInfo, PropertyInfo propertyInfo, LogColor logColor)
        {
            this.name = name;
            this.fieldInfo = fieldInfo;
            this.propertyInfo = propertyInfo;
            this.logColor = logColor;
        }
    }
}
