using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using FontStashSharp;

namespace GameProject.Towers {
    /// <summary>
    /// Goal: Base class for the game's towers.
    /// </summary>
    abstract class Tower {
        public Tower(int x, int y, float attackCooldown, int hotkeyNumber) {
            _x = x;
            _y = y;
            _attackTimer = new CoolDownTimer(attackCooldown);
            _attackTimer.Reset();
            _attackTimer.SecondsElapsed = attackCooldown / 2;
            TowerInfo = "undefined";
            HotkeyNumber = hotkeyNumber;
        }

        public int X {
            get => _x;
            set { _x = value; }
        }
        public int Y {
            get => _y;
            set { _y = value; }
        }
        public bool IsDisabled {
            get => _disabled;
            set { _disabled = value; }
        }
        public int MinimumRange => _minRange;
        public int MaximumRange => _maxRange;

        public int HotkeyNumber { get; set; }

        public Color TowerColor;
        public Utility.TowerType Type;
        public int Damage { get; set; }
        public string TowerInfo;

        public virtual void Update(GameTime gameTime) {
            _attackTimer.Update(gameTime);
        }
        public virtual void Draw(SpriteBatch s) {
            if (_disabled)
                s.FillRectangle(new RectangleF(Utility.GameToScreen(_x), Utility.GameToScreen(_y), Utility.Board.GridSize, Utility.Board.GridSize), Color.Gray);
            else
                s.FillRectangle(new RectangleF(Utility.GameToScreen(_x), Utility.GameToScreen(_y), Utility.Board.GridSize, Utility.Board.GridSize), TowerColor);
            s.DrawString(Utility.AssetManager.GetFont(0.5f * Utility.Board.GridSize), HotkeyNumber.ToString(), new Vector2(Utility.GameToScreen(_x) + Utility.Board.GridSize / 2.5f, Utility.GameToScreen(_y) + Utility.Board.GridSize / 4), Color.Black);
            DrawCoolDownTimer(s);
        }

        private void DrawCoolDownTimer(SpriteBatch s) {
            Vector2 position = new Vector2(Utility.GameToScreen(X), Utility.GameToScreen(Y) + Utility.Board.GridSize / 8);
            RectangleF coolDownRectangle = new RectangleF(position.X, position.Y, 1 * ((_attackTimer.MaxTime - _attackTimer.SecondsElapsed) / _attackTimer.MaxTime) * Utility.Board.GridSize, Utility.Board.GridSize / 8);
            s.FillRectangle(coolDownRectangle, Color.Yellow);
            RectangleF outlineRectangle = new RectangleF(position.X, position.Y, Utility.Board.GridSize , Utility.Board.GridSize / 8);
            s.DrawRectangle(outlineRectangle, Color.Black, 2F);
        }

        public void DrawSelectionHighlight(SpriteBatch s) {
            s.FillRectangle(new RectangleF(Utility.GameToScreen(_x) - Utility.Board.GridSize * _highlightOffset / 2, Utility.GameToScreen(_y) - Utility.Board.GridSize * _highlightOffset / 2, Utility.Board.GridSize + _highlightOffset * Utility.Board.GridSize, Utility.Board.GridSize + _highlightOffset * Utility.Board.GridSize), Color.Yellow);
        }
        protected bool IsWithinRange(int x, int y) {
            return !(Math.Abs(x) > _minRange || Math.Abs(y) > _minRange);
        }

        protected bool RangeChecker(float x, float y, int range) {
            bool top = y < _y - range;
            bool right = x > _x + 1 + range;
            bool bottom = y > _y + 1 + range;
            bool left = x < _x - range;

            return !(top || right || bottom || left);
        }

        private const float _highlightOffset = 0.07F;
        protected int _minRange, _maxRange;
        protected bool _disabled = false;
        protected CoolDownTimer _attackTimer;
        int _x;
        int _y;
    }
}
