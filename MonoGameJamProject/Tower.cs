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
        int size;
        public Tower(int iX, int iY, int iSize) {
            x = iX;
            y = iY;
            size = iSize;
        }

        public void Draw(SpriteBatch s) {
            s.FillRectangle(BoundingBox, Color.Red);
        }

        public Rectangle BoundingBox
        {
            get { return new Rectangle(x * size, y * size, size, size); }
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

