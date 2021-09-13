using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameProject {
    /// <summary>
    /// Basic class for handling Input
    /// </summary>
    class Input {
        public void Update() {
            _previousMouseState = _currentMouseState;
            _previousKeyboardState = _currentKeyboardState;
            _currentMouseState = Mouse.GetState();
            _currentKeyboardState = Keyboard.GetState();
        }
        public Vector2 MousePosition {
            get { return new Vector2(_currentMouseState.X, _currentMouseState.Y); }
        }
        public Point MouseToGameGrid() {
            return new Point((int)Utility.ScreenToGame(MousePosition.X), (int)(Utility.ScreenToGame(MousePosition.Y)));
        }
        public Point MouseGridPosition() {
            return new Point((int)(MousePosition.X / Utility.Board.GridSize), (int)(MousePosition.Y / Utility.Board.GridSize));
        }
        public bool MouseLeftButtonPressed {
            get { return _currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released; }
        }
        public bool MouseRightButtonPressed {
            get { return _currentMouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released; }
        }
        public bool MouseMiddleButtonPressed {
            get { return _currentMouseState.MiddleButton == ButtonState.Pressed && _previousMouseState.MiddleButton == ButtonState.Released; }
        }
        public bool KeyPressed(Keys k) {
            return _currentKeyboardState.IsKeyDown(k) && _previousKeyboardState.IsKeyUp(k);
        }
        public bool IsKeyDown(Keys k) {
            return _currentKeyboardState.IsKeyDown(k);
        }

        protected MouseState _currentMouseState, _previousMouseState;
        protected KeyboardState _currentKeyboardState, _previousKeyboardState;
    }
}
