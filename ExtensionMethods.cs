using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameToolkit
{
    public static class ExtensionMethods
    {
        public static bool IsKeyPressedOnce(this KeyboardState state, Keys key)
        {
            return state.IsKeyDown(key) && !MGTK.Instance.PrevKeyboardState.IsKeyDown(key);
        }

        public static bool IsButtonPressedOnce(this GamePadState state, Buttons button)
        {
            return state.IsButtonDown(button) && !MGTK.Instance.PrevGamePadState.IsButtonDown(button);
        }

        public static T[] To1DArray<T>(this T[,] TwoDimArray)
        {
            int width = TwoDimArray.GetLength(0);
            int height = TwoDimArray.GetLength(1);
            T[] array = new T[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    T value = TwoDimArray[x, y];
                    array[y * width + x] = value;
                }
            }

            return array;
        }
    }
}
