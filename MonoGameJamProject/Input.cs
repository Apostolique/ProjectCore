using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameJamProject
{
    /// <summary>
    /// Basic class for handling Input
    /// </summary>
    class Input
    {
        protected MouseState currentMouseState, previousMouseState;

        public void Update()
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }

        public Vector2 MousePosition
        {
            get { return new Vector2(currentMouseState.X, currentMouseState.Y); }
        }

        public bool MouseLeftButtonPressed()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released;
        }

        public bool MouseRightButtonPressed()
        {
            return currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released;
        }
    }
}
