using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameProject {
    /// <summary>
    /// Goal: Base class for the game's minions.
    /// </summary>
    class Tile {
        public Tile(int x, int y) {
            X = x;
            Y = y;
        }

        public int X;
        public int Y;

        public void Draw(SpriteBatch s, Color c) {
            s.DrawRectangle(new RectangleF(Utility.GameToScreen(X), Utility.GameToScreen(Y), Utility.Board.GridSize, Utility.Board.GridSize), c, 1);
        }
    }
}
