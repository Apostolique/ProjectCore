using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameProject.UI {
    class HealthBar {
        public HealthBar(Minion owner) {
            _owner = owner;
            switch (_owner.Type) {
                case Utility.MinionType.Fast:
                    _healthBarHeight = 0.10f;
                    _healthBarWidth = 0.25f;
                    break;
                case Utility.MinionType.Slow:
                    _healthBarHeight = 0.12f;
                    _healthBarWidth = 0.35f;
                    break;
                case Utility.MinionType.Boss:
                    _healthBarHeight= 0.20f;
                    _healthBarWidth = 0.50f;
                    break;
            }
            _fillColor = Color.DarkRed;
            _outlineColor = Color.Black;
        }

        public void Draw(SpriteBatch s) {
            Vector2 position = new Vector2(Utility.GameToScreen(_owner.Position.X - _healthBarWidth / 2), Utility.GameToScreen(_owner.Position.Y - _owner.Radius - _healthBarHeight));
            RectangleF healthRectangle = new RectangleF(position.X, position.Y, _healthBarWidth * (_owner.HP / _owner.MaxHP) * Utility.Board.GridSize, _healthBarHeight * Utility.Board.GridSize);
            s.FillRectangle(healthRectangle, _fillColor);
            DrawOutline(s, position);
            s.DrawString(Utility.AssetManager.GetFont(0.25f * Utility.Board.GridSize), _owner.FireStacks.ToString(), new Vector2(healthRectangle.X + _healthBarWidth * Utility.Board.GridSize, healthRectangle.Y - healthRectangle.Height), Color.Orange);
        }

        private void DrawOutline(SpriteBatch s, Vector2 position) {
            RectangleF healthRectangle = new RectangleF(position.X, position.Y, _healthBarWidth * Utility.Board.GridSize, _healthBarHeight * Utility.Board.GridSize);
            s.DrawRectangle(healthRectangle, _outlineColor, _outlineThickness);
        }

        Color _fillColor, _outlineColor;
        private readonly float _healthBarWidth, _healthBarHeight;
        private const float _outlineThickness = 1f;
        readonly Minion _owner;
    }
}
