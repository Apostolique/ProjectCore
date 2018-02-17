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
        public Color towerColor;
        protected int minRange, maxRange;
        protected bool disabled = false;
        protected CoolDownTimer attackTimer;
        public Utility.TowerType type;
        protected int damage;
        public string towerInfo;
        int _x;
        int _y;
        public Tower(int iX, int iY, float iAttackCooldown)
        {
            _x = iX;
            _y = iY;
            attackTimer = new CoolDownTimer(iAttackCooldown);
            attackTimer.Reset();
            attackTimer.SecondsElapsed = iAttackCooldown / 2;
            towerInfo = "undefined";
        }
        public virtual void Update(GameTime gameTime)
        {
            attackTimer.Update(gameTime);
        }
        public virtual void Draw(SpriteBatch s)
        {
            if (disabled)
                s.FillRectangle(new RectangleF(Utility.GameToScreen(_x), Utility.GameToScreen(_y), Utility.board.GridSize, Utility.board.GridSize), Color.Gray);
            else
                s.FillRectangle(new RectangleF(Utility.GameToScreen(_x), Utility.GameToScreen(_y), Utility.board.GridSize, Utility.board.GridSize), towerColor);
        }
        public void DrawSelectionHightlight(SpriteBatch s)
        {
            s.FillRectangle(new RectangleF(Utility.GameToScreen(_x) - Utility.board.GridSize * highlightOffset / 2, Utility.GameToScreen(_y) - Utility.board.GridSize * highlightOffset / 2, Utility.board.GridSize + highlightOffset * Utility.board.GridSize, Utility.board.GridSize + highlightOffset * Utility.board.GridSize), Color.Yellow);
        }
        protected bool IsWithinRange(int iX, int iY)
        {
            return !(Math.Abs(iX) > minRange || Math.Abs(iY) > minRange);
        }

        protected bool RangeChecker(float iX, float iY, int range)
        {
            bool top = iY < _y - range;
            bool right = iX > _x + 1 + range;
            bool bottom = iY > _y + 1 + range;
            bool left = iX < _x - range;

            return !(top || right || bottom || left);
        }
        public int X
        {
            get { return _x; }
            set { _x = value; }
        }
        public int Y
        {
            get { return _y; }
            set { _y = value; }
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

        public int HotKeyNumber
        {
            get; set;
        }
    }
}

