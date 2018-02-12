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
        private const float highlightOffset = 0.1F;
        int x;
        int y;
        public Tower(int iX, int iY) {
            x = iX;
            y = iY;
        }

        public void Draw(SpriteBatch s, int gridSize) {
            s.FillRectangle(new Rectangle(x * gridSize - gridSize, y * gridSize - gridSize, gridSize, gridSize), Color.Red);
        }

        public void DrawHighlight(SpriteBatch s, int gridsize)
        {
            s.FillRectangle(new RectangleF(x * gridsize - gridsize * highlightOffset / 2 - gridsize, y * gridsize - gridsize * highlightOffset / 2 - gridsize, gridsize + highlightOffset * gridsize, gridsize + highlightOffset * gridsize), Color.Yellow);
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }
    }
}

