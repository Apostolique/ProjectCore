using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameJamProject
{
    /// <summary>
    /// Goal: Base class for the game's minions.
    /// </summary>
    class Tile
    {
        public int X;
        public int Y;
        public Tile(int iX, int iY)
        {
            X = iX;
            Y = iY;
        }

        public void Draw(SpriteBatch s, Color c)
        {
            s.DrawRectangle(new RectangleF(Utility.GameToScreen(X), Utility.GameToScreen(Y), Utility.board.GridSize, Utility.board.GridSize), c, 0.02f * Utility.board.GridSize);
        }
    }
}