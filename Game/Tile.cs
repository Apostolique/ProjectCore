using Apos.Shapes;
using Microsoft.Xna.Framework;

namespace GameProject {
    /// <summary>
    /// Goal: Base class for the game's minions.
    /// </summary>
    class Tile(int x, int y) {
        public int X = x;
        public int Y = y;

        public void Draw(ShapeBatch sb, Color c) {
            sb.BorderRectangle(new Vector2(Utility.GameToScreen(X), Utility.GameToScreen(Y)), new Vector2(Utility.Board.GridSize, Utility.Board.GridSize), c, 1f);
        }
    }
}
