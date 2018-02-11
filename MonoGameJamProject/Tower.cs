using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameJamProject
{
    /// <summary>
    /// Goal: Base class for the game's towers.
    /// </summary>
    class Tower
    {
        int x;
        int y;
        public Tower(int iX, int iY) {
            x = iX;
            y = iY;
        }

        public void Draw(SpriteBatch s, int gridSize) {
            s.FillRectangle(new Rectangle(x * gridSize, y * gridSize, gridSize, gridSize), Color.Red);
        }
    }
}

