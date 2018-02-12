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
        float x;
        float y;
        public Tile(float iX, float iY) {
            x = iX;
            y = iY;
        }

        public void Draw(SpriteBatch s, int gridSize) {
            s.DrawRectangle(new RectangleF(x * gridSize, y * gridSize, gridSize, gridSize), Color.White, 0.02f * gridSize);
        }
    }
}