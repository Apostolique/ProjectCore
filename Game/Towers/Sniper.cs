using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameProject.Towers {
    class Sniper : Tower {
        public Sniper(int x, int y, int hotkeyNumber) : base(x, y, 9f, hotkeyNumber) {
            TowerColor = Color.LimeGreen;
            Type = Utility.TowerType.Sniper;
            _minRange = 2;
            _maxRange = Math.Max(Utility.Board.FullWidth, Utility.Board.FullHeight);
            Damage = 250;
            TowerInfo = "Sniper Tower\nMin. Range: " + _minRange + "\nMax. Range: " + _maxRange + "\nDamage: " + Damage + "\nSingle target\nhigh damage tower";
        }

        public override void Update(GameTime gameTime) {
            if (!_disabled) {
                _attackTimer.Update(gameTime);
                TargetMinion();
                if (_targetedMinion != null) {
                    if (_attackTimer.IsExpired) {
                        _targetedMinion.TakeDamage(Damage);
                        Utility.AssetManager.PlaySFX("sniper_shot", 0.75f);
                        _attackTimer.Reset();
                    }
                }
            }
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch s) {
            base.Draw(s);
            if (!_disabled && !(_targetedMinion == null))
                s.DrawLine(Utility.GameToScreen(X) + Utility.Board.GridSize / 2, Utility.GameToScreen(Y) + Utility.Board.GridSize / 2, Utility.GameToScreen(_targetedMinion.Position.X), Utility.GameToScreen(_targetedMinion.Position.Y), Color.Red, 2f);
        }
        private void TargetMinion() {
            _targetedMinion = null;
            foreach (Path p in Utility.Board.Paths) {
                foreach (Minion m in p.MinionList) {
                    if (RangeChecker(m.Position.X, m.Position.Y, MinimumRange))
                        continue;
                    else if (_targetedMinion == null)
                        _targetedMinion = m;
                    else if (m.MaxHP > _targetedMinion.MaxHP) {
                        if (m.MaxHP == _targetedMinion.MaxHP && m.DistanceTraveled > _targetedMinion.DistanceTraveled)
                            _targetedMinion = m;
                        else if (m.MaxHP != _targetedMinion.MaxHP)
                            _targetedMinion = m;
                    }
                }
            }
        }

        Minion? _targetedMinion;
    }
}
