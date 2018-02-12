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
    abstract class Tower
    {
        private const float highlightOffset = 0.07F;
        protected Color towerColor;
        protected int minRange, maxRange;
        public Utility.TowerType type;
        int x;
        int y;
        public Tower(int iX, int iY)
        {
            x = iX;
            y = iY;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch s, int gridSize)
        {
            s.FillRectangle(new RectangleF(Utility.GameToScreen(x, gridSize), Utility.GameToScreen(y, gridSize), gridSize, gridSize), towerColor);
        }

        public void DrawSelectionHightlight(SpriteBatch s, int gridSize)
        {
            s.FillRectangle(new RectangleF(Utility.GameToScreen(x, gridSize) - gridSize * highlightOffset / 2, Utility.GameToScreen(y, gridSize) - gridSize * highlightOffset / 2, gridSize + highlightOffset * gridSize, gridSize + highlightOffset * gridSize), Color.Yellow);
        }

        protected bool IsWithinRange(int iX, int iY)
        {
            if (Math.Abs(iX) > minRange || Math.Abs(iY) > minRange)
                return false;
            return true;
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

        public int MinimumRange
        {
            get { return minRange; }
        }

        public int MaximumRange
        {
            get { return maxRange; }
        }
    }
}

