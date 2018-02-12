using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGameJamProject
{
    /// <summary>
    /// Basic class for handling Input
    /// </summary>
    class Input
    {
        protected MouseState currentMouseState, previousMouseState;
        protected KeyboardState currentKeyboardState, previousKeyboardState;

        public void Update()
        {
            previousMouseState = currentMouseState;
            previousKeyboardState = currentKeyboardState;
            currentMouseState = Mouse.GetState();
            currentKeyboardState = Keyboard.GetState();
        }

        public Vector2 MousePosition
        {
            get { return new Vector2(currentMouseState.X, currentMouseState.Y); }
        }
        public Point MouseToGameGrid(int gridSize)
        {
            return new Point((int)Utility.ScreenToGame(MousePosition.X, gridSize), (int)(Utility.ScreenToGame(MousePosition.Y, gridSize)));
        }

        public Point MouseGridPosition(int gridSize)
        {
            return new Point((int)(MousePosition.X / gridSize), (int)(MousePosition.Y / gridSize));
        }

        public bool MouseLeftButtonPressed
        {
            get { return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released; }
        }

        public bool MouseRightButtonPressed
        {
            get { return currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Released; }
        }

        public bool KeyPressed(Keys k)
        {
            return currentKeyboardState.IsKeyDown(k) && previousKeyboardState.IsKeyUp(k);
        }

        public bool IsKeyDown(Keys k)
        {
            return currentKeyboardState.IsKeyDown(k);
        }
    }
}
