using System;
using System.Collections.Generic;
using System.Text;
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
        public Tile(int iX, int iY) {
            X = iX;
            Y = iY;
        }

        public void Draw(SpriteBatch s, int gridSize) {
            s.DrawRectangle(new RectangleF(Utility.GameToScreen(X, gridSize), Utility.GameToScreen(Y, gridSize), gridSize, gridSize), Color.White, 0.02f * gridSize);
        }
    }
}