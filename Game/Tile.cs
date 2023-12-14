using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameProject {
    /// <summary>
    /// Goal: Base class for the game's minions.
    /// </summary>
    class Tile(int x, int y) {
        public int X = x;
        public int Y = y;

        public void Draw(SpriteBatch s, Color c) {
            s.DrawRectangle(new RectangleF(Utility.GameToScreen(X), Utility.GameToScreen(Y), Utility.Board.GridSize, Utility.Board.GridSize), c, 1);
        }
    }
}
