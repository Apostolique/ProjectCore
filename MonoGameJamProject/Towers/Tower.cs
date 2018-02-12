using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameJamProject.Towers
{
    /// <summary>
    /// Goal: Base class for the game's towers.
    /// </summary>
    class Tower
    {
        enum TowerType { Sniper, Shotgun, FlameThrower };
        private const float highlightOffset = 0.1F;
        protected Color towerColor;
        int x;
        int y;
        public Tower(int iX, int iY) {
            x = iX;
            y = iY;
        }

        public void Draw(SpriteBatch s, int gridSize) {
            s.FillRectangle(new RectangleF(Utility.GameToScreen(x, gridSize), Utility.GameToScreen(y, gridSize), gridSize, gridSize), Color.Red);
        }

        public void DrawHighlight(SpriteBatch s, int gridsize)
        {
            s.FillRectangle(new RectangleF(Utility.GameToScreen(x, gridsize) - gridsize * highlightOffset / 2, Utility.GameToScreen(y, gridsize) - gridsize * highlightOffset / 2, gridsize + highlightOffset * gridsize, gridsize + highlightOffset * gridsize), Color.Yellow);
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

