using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameToolkit
{
    public static class Time
    {
        internal static float _deltaTime;
        public static float DeltaTime { get { return _deltaTime; } }

        internal static float _timeInSeconds;
        public static float TimeInSeconds { get { return _timeInSeconds; } }
    }
}
