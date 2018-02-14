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
        protected bool disabled = false;
        public Utility.TowerType type;
        protected int damage;
        public string towerInfo;
        int x;
        int y;
        public Tower(int iX, int iY)
        {
            x = iX;
            y = iY;
            towerInfo = "undefined";
        }
        public virtual void Update(GameTime gameTime, List<Minion> iMinionList)
        {

        }
        public virtual void Draw(SpriteBatch s)
        {
            if (disabled)
                s.FillRectangle(new RectangleF(Utility.GameToScreen(x), Utility.GameToScreen(y), Utility.board.GridSize, Utility.board.GridSize), Color.Gray);
            else
                s.FillRectangle(new RectangleF(Utility.GameToScreen(x), Utility.GameToScreen(y), Utility.board.GridSize, Utility.board.GridSize), towerColor);
        }
        public void DrawSelectionHightlight(SpriteBatch s)
        {
            s.FillRectangle(new RectangleF(Utility.GameToScreen(x) - Utility.board.GridSize * highlightOffset / 2, Utility.GameToScreen(y) - Utility.board.GridSize * highlightOffset / 2, Utility.board.GridSize + highlightOffset * Utility.board.GridSize, Utility.board.GridSize + highlightOffset * Utility.board.GridSize), Color.Yellow);
        }
        protected bool IsWithinRange(int iX, int iY)
        {
            return !(Math.Abs(iX) > minRange || Math.Abs(iY) > minRange);
        }
        // currently doesnt work?
        protected bool OutsideMinimumRange(int iX, int iY)
        {
            float distanceSQ = (Math.Abs(iX - x) * Math.Abs(iX - x)) + (Math.Abs(iY - y) * Math.Abs(iY - y));
            return Math.Sqrt(distanceSQ) < minRange;
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
        public bool IsDisabled
        {
            get { return disabled; }
            set { disabled = value; }
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

