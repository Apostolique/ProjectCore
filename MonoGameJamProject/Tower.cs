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

        public void Draw(SpriteBatch s, int size) {
            s.FillRectangle(new Rectangle(x * size, y * size, size, size), Color.Red);
        }

        public void DrawHighlight(SpriteBatch s, int Gridsize)
        {
            s.FillRectangle(new Rectangle((int)(x * Gridsize - Gridsize * highlightOffset / 2), (int)(y * Gridsize - Gridsize * highlightOffset / 2), (int)(Gridsize + highlightOffset * Gridsize), (int)(Gridsize + highlightOffset * Gridsize)), Color.Yellow);
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

