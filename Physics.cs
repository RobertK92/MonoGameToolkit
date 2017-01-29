
using Microsoft.Xna.Framework;

namespace MonoGameToolkit
{
    public static class Physics
    {
        /// <summary>
        /// Scale of the overal physics world in units per pixel (default = 1.0f / 64.0f -> (64 pixels = 1 metre)).
        /// </summary>
        //public static float UPP = 1.0f / 64.0f;
        public static float UPP = 1.0f / 64.0f;

        private static Vector2 _gravity = new Vector2(0.0f, 9.8f);
        public static Vector2 Gravity
        {
            get { return _gravity; }
            set
            {
                if(_gravity != value)
                {
                    Scene scene = MGTK.Instance.LoadedScene;
                    if(scene != null)
                    {
                        scene.PhysicsWorld.Gravity = value;
                    }
                }
                _gravity = value;
            }
        }


    }
}
